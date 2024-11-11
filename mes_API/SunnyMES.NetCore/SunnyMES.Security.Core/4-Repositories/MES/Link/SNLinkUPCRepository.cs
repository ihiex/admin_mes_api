#define IsSql
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Enums.DynamicItemName;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.ToolExtensions;
using SunnyMES.Commons.Enums;
using MySqlX.XDevAPI.Common;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Log;

namespace SunnyMES.Security.Repositories;

public class SNLinkUPCRepository : MesBaseRepository, ISNLinkUPCRepository
{

    public SNLinkUPCRepository(IDbContextCore contextCore) : base(contextCore)
    {

    }
    public new async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        var mConfirmPoOutput = await base.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);

        if (!string.IsNullOrEmpty(mConfirmPoOutput.ErrorMsg))
            return mConfirmPoOutput;

        #region 构建动态类型
        var tmpPoDetails = await Public_Repository.GetmesPoDetailConfigAsync(S_POID);
        var poDetails = tmpPoDetails.Where(x => x.IsCheckList).OrderBy(x => x.Sequence).ToList();
        var dynamicSNMat = new SortedList<int, DynamicItemsDto>();

        for (int i = 0; i < poDetails.Count; i++)
        {
            var item = poDetails[i];
            var defaultDetails = tmpPoDetails.Where(x => x.PoDefID == item.ParentValueID)
                .Select(x => x.Content).ToArray();
            dynamicSNMat.Add(i, new DynamicItemsDto()
            {
                PoDefID = item.PoDefID,
                Name = item.Description,
                Description = item.Description.ToEnum<SnLinkUpcInputNames>().GetDescriptionByEnum(),
                IsEnable = item.Content.Trim() == "1",
                DefaultSpec = defaultDetails,
                CheckType = item.CheckType,
                Parameters = item.Parameters,
                ParentCheckID = item.ParentCheckID,
            });
        }

        mConfirmPoOutput.CurrentInitPageInfo.DataList = dynamicSNMat;


        if (dynamicSNMat.Values.Count( x => x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPCSN))) != 1)
        {
            mConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_6035;
        }

        string tmpCommand;
        if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC == "1" && mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanJAN == "1")
        {
            tmpCommand = "D";
        }
        else if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC != "1" && mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanJAN != "1")
        {
            tmpCommand = "B";
        }
        else
        {
            tmpCommand = "C";
        }

        var tmpDataList = (SortedList<int, DynamicItemsDto>)mConfirmPoOutput.CurrentInitPageInfo.DataList;

        #region cmd
        //tmpCommand = (tmpDataList.Values.Count) switch
        //{
        //    4 => "E",
        //    3 => "D",
        //    2 => "C",
        //    1 => "B",
        //    _ => "C"
        //};
        #endregion

        tmpCommand = (tmpDataList) switch
        {
            { } when tmpDataList.Values.Count(x => x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC)) || x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanJAN))) == 2 => "D",
            { } when tmpDataList.Values.Count(x => x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC)) || x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanJAN))) == 0 => "B",
            _ => "C"
        };


        mConfirmPoOutput.DynamicCommand = tmpCommand;
        #endregion

        return mSetConfirmPoOutput = mConfirmPoOutput;
    }
    /// <summary>
    /// 主条码校验
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<MesOutputDto> MainSnVerifyAsync(SNLinkUPCInput input)
    {
        MesOutputDto mMesOutputDto = new MesOutputDto();
        mSetConfirmPoOutput = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID,
            input.S_POID, input.S_UnitStatus, input.S_URL);

        var dynamicMat = mSetConfirmPoOutput.CurrentInitPageInfo.DataList;
        string res = await Public_Repository.uspFGLinkUPCFGSNCheck(input.S_SN, input.S_PartID, List_Login);
        if (res != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(res));

        mainSnDynamicSnKey = GetDynamicSnKey(input.S_SN);
        cacheHelper.Add(mainSnDynamicSnKey, dynamicMat, new TimeSpan(2, 0, 0, 0));
        return mMesOutputDto;
    }
    private async Task<string> ScanSubmit(SNLinkUPCInput input)
    {
        string result = "0";

        string nextUnitStateId = await Public_Repository.GetMesUnitState(input.S_PartID, input.S_PartFamilyID,
            List_Login.LineID.ToString(), List_Login.StationTypeID.ToInt(), input.S_POID, input.S_UnitStatus);
        if (string.IsNullOrEmpty(nextUnitStateId))
            return "20203";

        #region FG info
        string FGSN = input.S_SN;
        var fgUnit = await Public_Repository.GetmesUnitSNAsync(FGSN);
        if (fgUnit is null || fgUnit.ID <= 0)
            return P_MSG_Public.MSG_Public_6031;

        var fgUnitDetail = await Public_Repository.GetmesUnitDetailAsync(fgUnit?.ID.ToString());
        if (fgUnitDetail is null || fgUnitDetail.ID <= 0)
            return P_MSG_Public.MSG_Public_6031;

        #endregion

        #region UPC info
        var upcDynamicItems = input.DataList.Where(x => x.Value.Name == SnLinkUpcInputNames.IsScanUPCSN.ToString())?.ToList();
        var upcDynamicItem = upcDynamicItems?[0];
        string UPCSN = upcDynamicItem?.Value.Value;
        if (string.IsNullOrEmpty(UPCSN))
            return msgSys.MSG_Sys_20012;

        var upcUnit = await Public_Repository.GetmesUnitSNAsync(UPCSN);
        if (upcUnit is null || upcUnit.ID <= 0)
            return msgSys.MSG_Sys_20125;

        #endregion

        fgUnit.UnitStateID = nextUnitStateId.ToInt();
        fgUnit.StatusID = input.S_UnitStatus.ToInt();
        fgUnit.EmployeeID = List_Login.EmployeeID;
        fgUnit.StationID = List_Login.StationID;
        fgUnit.ProductionOrderID = input.S_POID.ToInt();

        fgUnitDetail.KitSerialNumber = UPCSN;

        string sql = DataCommit_Repository.SubmitData_Unit_Detail_History_Mod(fgUnit, fgUnitDetail);

        upcUnit.UnitStateID = nextUnitStateId.ToInt();
        upcUnit.StatusID = input.S_UnitStatus.ToInt();
        upcUnit.EmployeeID = List_Login.EmployeeID;
        upcUnit.StationID = List_Login.StationID;
        upcUnit.ProductionOrderID = input.S_POID.ToInt();

        sql += "\r\n" + DataCommit_Repository.SubmitData_Unit_Detail_History_Mod(upcUnit, null);

        result = await ExecuteTransactionSqlAsync(sql);
        return result;
    }
    private string GetDynamicSnKey(string sn) => $"DynamicSnMat_" + List_Login.CurrentLoginIP + "_userID_" + List_Login.EmployeeID + "_stationID_" + List_Login.StationID + "_SN_" + sn;

    public delegate Task<(string, DynamicItemsDto)> CheckFunc(DynamicItemsDto currentItemsDto, SNLinkUPCInput input);
    public async Task<MesOutputDto> DynamicSnVerifyAsync(SNLinkUPCInput input)
    {
        MesOutputDto mMesOutputDto = new MesOutputDto();

        string tmpRes = string.Empty;

        mSetConfirmPoOutput = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID,
            input.S_POID, input.S_UnitStatus, input.S_URL);

        mMesOutputDto.CurrentInitPageInfo.DataList = input.DataList;
        if (string.IsNullOrEmpty(input.S_SN))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20007);

        mainSnDynamicSnKey = GetDynamicSnKey(input.S_SN);
        object dynamicMat = cacheHelper.Get(mainSnDynamicSnKey);
        if (dynamicMat is null)
            return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6005);

        var cacheDynamicData = JsonSerializer.Deserialize<SortedList<SnLinkUpcInputNames, DynamicItemsDto>>(dynamicMat.ToString());

        var dynamicItems = input.DataList;

        if (dynamicItems == null)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20077);

        var dynamicItemsCopyDtos = input.DataList;

        var currentItems = dynamicItems?.Where(x => x.Value.IsEnable && x.Value.IsCurrentItem && !x.Value.IsScanFinished && !string.IsNullOrEmpty(x.Value.Value))?.ToList();
        if (currentItems?.Count != 1)
            return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6014);

        var currentItem = currentItems[0].Value;
        var itemIndex = currentItems[0].Key;
        var currentFuncKey = currentItem.Name.ToEnum<SnLinkUpcInputNames>().GetFuncDescription().FuncName;

        (string, DynamicItemsDto) checkVal;
        #region A套餐
        //try
        //{
        //    var methods = typeof(SNLinkUPCRepository).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.Name == currentFuncKey).ToList();
        //    var deleFunc = Delegate.CreateDelegate(typeof(CheckFunc), this, methods[0]) as CheckFunc;
        //    checkVal = await MakeCheckSN(deleFunc, currentItem, input);
        //}
        //catch (Exception e)
        //{
        //    Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error...", e);
        //    throw;
        //}

        #region A-套餐
        //var vfun = assemblyFuncs()?.Where(x => x.Method.Name == currentFuncKey).ToList();
        //checkVal = await MakeCheckSN(vfun[0], currentItem, input);
        #endregion

        #endregion

        #region B套餐
        //checkVal = (currentFuncKey) switch
        //{
        //    "UPC_Code" => await UPC_Code(currentItem, input),
        //    "JAN_Code" => await JAN_Code(currentItem, input),
        //    "UPC_SN" => await UPC_SN(currentItem, input),
        //    "Check_SN" => await Check_SN(currentItem, input),
        //    _ => throw new Exception("cosmo not found check item."),
        //};
        #endregion

        #region C套餐
        checkVal = await CheckInputsAsync(currentItem, input);
        #endregion

        //检查结果
        if (checkVal.Item1 != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(checkVal.Item1));

        if (OutputExtensions.VerifyEndResult(currentItem, input))
        {
            checkVal.Item1 = await ScanSubmit(input);
            //提交结果
            if (checkVal.Item1 != "1")
                return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(checkVal.Item1));
        }

        checkVal.Item2 ??= currentItem;
        checkVal.Item2.IsScanFinished = true;
        dynamicItemsCopyDtos[itemIndex] = checkVal.Item2;
        mMesOutputDto.CurrentInitPageInfo.DataList = dynamicItemsCopyDtos;

        if (string.IsNullOrEmpty(mMesOutputDto.ErrorMsg))
        {
            mainSnDynamicSnKey = GetDynamicSnKey(input.S_SN);
            cacheHelper.Add(mainSnDynamicSnKey, mMesOutputDto.CurrentInitPageInfo.DataList, new TimeSpan(2, 0, 0, 0));
        }
        return mMesOutputDto;
    }

    private (string, DynamicItemsDto) CheckParentValue(int parentCheckId, SortedList<int, DynamicItemsDto> dataList,ref string parentSN)
    {
        (string, DynamicItemsDto) res = (null, null);
        if (parentCheckId == 0)
            return res;

        var parentItem = dataList.Values.Where(x => x.PoDefID == parentCheckId).ToList()?[0];
        if (parentItem is null)
            return res;

        if (string.IsNullOrEmpty(parentItem.Value) || !parentItem.IsScanFinished)
            return (P_MSG_Public.MSG_Public_6033, null);

        if (string.IsNullOrEmpty(parentSN))
            parentSN = parentItem.Value;

        return parentItem.ParentCheckID == 0 ? res : CheckParentValue(parentItem.ParentCheckID, dataList, ref parentSN);
    }

    private async Task<(string, DynamicItemsDto)> CheckInputsAsync(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos)
    {
        (string, DynamicItemsDto) resValueTuple = ("Error : no setup the check type.", currentItem);
        if (string.IsNullOrEmpty(currentItem.Value))
            return (P_MSG_Public.MSG_Public_016, null);

        var poDefCheckType = (PoDefCheckType)currentItem.CheckType;
        var parentSn = string.Empty;

        if (currentItem.ParentCheckID != 0)
        {
            var parRes = CheckParentValue(currentItem.ParentCheckID, inputDtos.DataList, ref parentSn);
            if (!string.IsNullOrEmpty(parRes.Item1))
                return parRes;
        }

        switch (poDefCheckType)  
        {
            case PoDefCheckType.NoSet:
                break;
            case PoDefCheckType.ValueCompare:
                var compareRes = currentItem.DefaultSpec.Where(x => x == currentItem.Value)?.ToList();
                resValueTuple.Item1 = compareRes.Count > 0 ? "1" : "20119";
                break;
            case PoDefCheckType.Procedures:
                if (string.IsNullOrEmpty(xmlStation))
                    xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
                if (string.IsNullOrEmpty(xmlExtraData))
                    xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

                xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + inputDtos.S_POID + "\"> </ProdOrder>");
                xmlPart = "<Part PartID=\"" + inputDtos.S_PartID + "\"> </Part>";

                if (string.IsNullOrEmpty(currentItem.Parameters))
                    return (P_MSG_Public.MSG_Public_6034, null);

                var procOutput = await Public_Repository.uspCallProcedureAsync(currentItem.Parameters, parentSn,
                    xmlProdOrder, xmlPart, xmlStation, xmlExtraData, currentItem.Value);
                resValueTuple.Item1 = procOutput;
                break;
            case PoDefCheckType.Interfaces:
                break;
            case PoDefCheckType.UPCSNCheck:
                resValueTuple.Item1 = await Public_Repository.uspFGLinkUPCUPCSNCheck(currentItem.Value, inputDtos.S_PartID, inputDtos.S_POID,
                    List_Login);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return resValueTuple;
    }

    private List<CheckFunc> assemblyFuncs() => new List<CheckFunc>()
    {
        UPC_Code,
        JAN_Code,
        UPC_SN,
        Check_SN,
    };
    private async Task<(string, DynamicItemsDto)> MakeCheckSN(CheckFunc func, DynamicItemsDto currentItem,
        SNLinkUPCInput inputDtos) => await func(currentItem, inputDtos);

    private async Task<(string, DynamicItemsDto)> UPC_Code(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos) => (
        currentItem.DefaultSpec.Where(x => x == currentItem.Value)?.ToList()?.Count <= 0 ? "20119" : "1", currentItem);

    private async Task<(string, DynamicItemsDto)> JAN_Code(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos) => (
        currentItem.DefaultSpec.Where(x => x == currentItem.Value)?.ToList()?.Count <= 0 ? "20119" : "1", currentItem);

    private async Task<(string, DynamicItemsDto)> UPC_SN(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos) => (
        await Public_Repository.uspFGLinkUPCUPCSNCheck(currentItem.Value, inputDtos.S_PartID, inputDtos.S_POID,
            List_Login), currentItem);

    private async Task<(string, DynamicItemsDto)> Check_SN(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos)
    {
        string result = "1";
        var allDynamicItemsDtos = inputDtos.DataList;
        var tmpUpcItems = allDynamicItemsDtos.Where(x => x.Value.Name == SnLinkUpcInputNames.IsScanUPCSN.ToString())?.ToList();
        var tmpUpcItem = tmpUpcItems?[0].Value;

        if (string.IsNullOrEmpty(tmpUpcItem.Value) || !tmpUpcItem.IsScanFinished)
            return (P_MSG_Public.MSG_Public_6033, null);

        if (string.IsNullOrEmpty(currentItem?.Value))
            return (P_MSG_Public.MSG_Public_016, null);

        //result = Public_Repository.uspKitBoxFGSNCheck(currentItem.Value, tmpUpcItem.Value);

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + inputDtos.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + inputDtos.S_PartID + "\"> </Part>";

        result = await Public_Repository.uspKitBoxFGSNCheck(tmpUpcItem.Value, currentItem.Value, xmlProdOrder, xmlPart,
            xmlStation, xmlExtraData);

        if (result != "1")
            return (result, null);


        return (result, currentItem);
    }
}