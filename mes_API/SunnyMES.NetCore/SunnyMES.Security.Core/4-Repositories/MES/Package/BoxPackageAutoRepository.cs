#define IsSql
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using NPOI.OpenXmlFormats.Dml;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES.MES_Output;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxPackageAuto;
using SunnyMES.Security.Models;
using Dapper;
using SunnyMES.Commons.Core.PublicFun;
using StackExchange.Profiling.Internal;

namespace SunnyMES.Security.Repositories;

public class BoxPackageAutoRepository : MesBaseRepository, IBoxPackageAutoRepository
{
    private int FullBoxQty = 0;
    private int ActiveBoxQty = 0;
    private List<CartonBoxConfirmed> cartonBoxConfirmeds;
    bool IsGenerateBoxSN = false;
    private string BandingBoxSn = string.Empty;
    private bool IsScanOnlyFGSN = false;
    private string UPCSN = string.Empty;
    private string BoxSNFormatName = string.Empty;
    private string newBoxSn = string.Empty;
    public BoxPackageAutoRepository(IDbContextCore contextCore) : base(contextCore)
    {

    }
    public new async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        var mConfirmPoOutput = await base.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);

        if (!string.IsNullOrEmpty(mConfirmPoOutput.ErrorMsg))
            return mConfirmPoOutput;

        if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.BoxQty.ToInt() == 0)
            return mConfirmPoOutput.SetErrorCode(P_MSG_Public.MSG_Public_6045);

        #region 查询模板
        if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsGenerateBoxSN is "1")
        {
            mConfirmPoOutput.PrinterParams = new PrinterParams();
            mConfirmPoOutput.PrinterParams.IsPrint = true;

            BoxSNFormatName = await Public_Repository.mesGetSNFormatIDByListAsync(S_PartID, S_PartFamilyID, List_Login.LineID.ToString(), S_POID, List_Login.StationTypeID.ToString());
            if (string.IsNullOrEmpty(BoxSNFormatName))
                return mConfirmPoOutput.SetErrorCode(msgSys.GetLanguage("20075"));
            
            mConfirmPoOutput.PrinterParams.SNFormatName = BoxSNFormatName;

            mConfirmPoOutput.PrinterParams.PrintIPPort =
                mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.PrintIPPort;

            //当打印IPPort为空时，前端使用客户端本机IPPort
            //if (string.IsNullOrEmpty(mConfirmPoOutput.PrinterParams.PrintIPPort))
            //    return mConfirmPoOutput.SetErrorCode(P_MSG_Public.MSG_Public_6042);

            string S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
            if (string.IsNullOrEmpty(S_LabelPath))
                return mConfirmPoOutput.SetErrorCode(msgSys.GetLanguage("20076"));
            else
            {
                if (S_LabelPath.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                    return mSetConfirmPoOutput.SetErrorCode(S_LabelPath);
            }
            string pathList = string.Empty;
            string[] ListTemplate = S_LabelPath.Split(';');
            foreach (string str in ListTemplate)
            {
                string[] listStr = str.Split(',');
                pathList = (string.IsNullOrEmpty(pathList) ? "" : pathList + ";") + listStr[1].ToString();
            }
            mConfirmPoOutput.PrinterParams.LabelPath = pathList.Replace(@"\\", @"\");
            mConfirmPoOutput.PrinterParams.LabelCommand = S_LabelPath;
        }
        #endregion

        #region 构建动态类型
        var tmpPoDetails = await Public_Repository.GetmesPoDetailConfigAsync(S_POID);
        var poDetails = tmpPoDetails.Where(x => x.IsCheckList).OrderBy(x => x.Sequence).ToList();
        var dynamicSNMat = new SortedList<int, DynamicItemsDto>();

        for (int i = 0; i < poDetails.Count; i++)
        {
            var item = poDetails[i];
            //只筛选启用的项目
            //if (item.Content.Trim() != "1")
            //    continue;
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
                Required = item.Required,
            });
        }

        mConfirmPoOutput.CurrentInitPageInfo.DataList = dynamicSNMat;

        if (dynamicSNMat.Values.Count(x => !x.IsEnable && x.Required) > 0)
        {
            mConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_6044;
        }
        if (dynamicSNMat.Values.Count(x => x.IsEnable) <= 0)
        {
            mConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_6044;
        }

        //if (dynamicSNMat.Values.Count( x => x.Name == ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPCSN))) != 1)
        //{
        //    mConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_6035;
        //}

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
    public async Task<BoxPackageMesOutputDtos> MainSnVerifyAsync(BoxPackageAutoInput input)
    {
        var mMesOutputDto = new BoxPackageMesOutputDtos();
        mSetConfirmPoOutput = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID,
            input.S_POID, input.S_UnitStatus, input.S_URL);

        string BoxSN_Pattern = mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.BoxSN_Pattern;

        if (!Regex.IsMatch(input.S_SN, BoxSN_Pattern))
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage("20027"));

        var dynamicMat = mSetConfirmPoOutput.CurrentInitPageInfo.DataList;
        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";
        var res = await Public_Repository.uspKitBoxCheckAsync(input.S_SN, xmlProdOrder,xmlPart,xmlStation,xmlExtraData,"BOX");
        if (res?.strOutput != "1")
        {
            if (res is null)
                return mMesOutputDto.ErrorMsg = "sql exception : uspKitBoxCheckAsync";
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(res.strOutput));
        }

        mMesOutputDto.BoxSN = res.BoxSN;
        mMesOutputDto.CartonBoxConfirmeds = await  Public_Repository.GetBoxPackageDataAsync(mMesOutputDto.BoxSN);

        var mMesPackage = await Public_Repository.GetMesPackageBySNAsync(mMesOutputDto.BoxSN);
        if (mMesPackage.ParentID.ToInt() > 0)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20045);

        mMesOutputDto.IsPackingFinish = mMesPackage is { StatusID: 1, Stage: 1 }; 

        mainSnDynamicSnKey = GetDynamicSnKey(mMesOutputDto.BoxSN);
        cacheHelper.Add(mainSnDynamicSnKey, dynamicMat, new TimeSpan(2, 0, 0, 0));
        return mMesOutputDto;
    }
    private string GetDynamicSnKey(string sn) => $"DynamicSnMat_" + List_Login.CurrentLoginIP + "_PageID_"+ baseCommonHeader?.PageId + "_userID_" + List_Login.EmployeeID + "_stationID_" + List_Login.StationID + "_SN_" + sn;

    public delegate Task<(string, DynamicItemsDto)> CheckFunc(DynamicItemsDto currentItemsDto, BoxPackageAutoInput input);


    public async Task<BoxPackageMesOutputDtos> DynamicSnVerifyAsync(BoxPackageAutoInput input)
    {
        var mMesOutputDto = new BoxPackageMesOutputDtos();

        string tmpRes = string.Empty;

        mSetConfirmPoOutput = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID,
            input.S_POID, input.S_UnitStatus, input.S_URL);

        mMesOutputDto.CurrentInitPageInfo.DataList = input.DataList;
        IsScanOnlyFGSN = mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.MultipackScanOnlyFGSN == "1";
        IsGenerateBoxSN = mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsGenerateBoxSN == "1";
        FullBoxQty = mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.BoxQty.ToInt();

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";


        var dynamicItems = input.DataList;
        if (dynamicItems is null)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20077);

        var dynamicItemsCopyDtos = input.DataList;
        var currentItems = dynamicItems?.Where(x => x.Value.IsEnable && x.Value.IsCurrentItem && !x.Value.IsScanFinished && !string.IsNullOrEmpty(x.Value.Value))?.ToList();
        if (currentItems?.Count != 1)
            return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6014);

        var v = dynamicItems.GroupBy(x => x.Value).Where(g => g.Count() > 1).Count() > 0;
        if (v)
        {
            Log4NetHelper.Warn("条码存在重复提交， -->" + dynamicItems.ToJson());
        }
        var currentItem = currentItems[0].Value;
        var itemIndex = currentItems[0].Key;
        var currentFuncKey = currentItem.Name.ToEnum<SnLinkUpcInputNames>().GetFuncDescription().FuncName;

        if (!string.IsNullOrEmpty(input.S_SN))
        {
            #region 获取实际数量
            cartonBoxConfirmeds = await Public_Repository.GetBoxPackageDataAsync(input.S_SN);
            mMesOutputDto.CartonBoxConfirmeds = cartonBoxConfirmeds;
            var existHadScans = cartonBoxConfirmeds.Where(x => x.SN == currentItem.Value || x.UPCSN == currentItem.Value).ToList();
            if (existHadScans != null && existHadScans.Count > 0)
                return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6004);

            ActiveBoxQty = cartonBoxConfirmeds is null || cartonBoxConfirmeds.Count <= 0 ? 0 : cartonBoxConfirmeds.Count;
            //if (currentItem.Name == SnLinkUpcInputNames.IsScanUPCSN.ToString())
            //{
            //    if (cartonBoxConfirmeds is not null && cartonBoxConfirmeds.Exists(x => x.UPCSN == currentItem.Value))
            //    {
            //        return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20059);
            //    }
            //}

            if (cartonBoxConfirmeds.Count >= FullBoxQty)
                return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20086);
            #endregion
        }

        (string, DynamicItemsDto) checkVal;
        BandingBoxSn = string.Empty;
        #region C套餐
        checkVal = await CheckInputsAsync(currentItem, input);
        #endregion

        if (string.IsNullOrEmpty(input.S_SN) && IsGenerateBoxSN && !string.IsNullOrEmpty(newBoxSn))
            input.S_SN = mMesOutputDto.BoxSN = newBoxSn;

        if (string.IsNullOrEmpty(input.S_SN) && !string.IsNullOrEmpty(BandingBoxSn))
            input.S_SN = mMesOutputDto.BoxSN = BandingBoxSn;

        if (!string.IsNullOrEmpty(input.S_SN) && string.IsNullOrEmpty(mMesOutputDto.BoxSN))
            mMesOutputDto.BoxSN = input.S_SN;

        //检查结果
        if (checkVal.Item1 != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(checkVal.Item1));

        if (OutputExtensions.VerifyEndResult(currentItem, input))
        {
            (string, DynamicItemsDto) singleRes = ("0",null);
            List<string> sns = new List<string>();
            foreach (var item in input.DataList.Values.Where(x => x.IsEnable))
            {
                if (sns.Contains(item.Value))
                    return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_041);

                sns.Add(item.Value);
                singleRes = await CheckInputsAsync(item, input);
                if (singleRes.Item1 != "1")
                    break;
            }
            if (singleRes.Item1 != "1")
            {
                checkVal.Item1 = singleRes.Item1;
                return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(checkVal.Item1));
            }

            var tmpSubmitRes  = await ScanSubmit(input);
            
            if (tmpSubmitRes != "1")
            {
                checkVal.Item1 = tmpSubmitRes;
                
                return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6039);
            }
        }

        checkVal.Item2 ??= currentItem;
        checkVal.Item2.IsScanFinished = true;
        dynamicItemsCopyDtos[itemIndex] = checkVal.Item2;
        mMesOutputDto.CurrentInitPageInfo.DataList = dynamicItemsCopyDtos;

        cartonBoxConfirmeds = await Public_Repository.GetBoxPackageDataAsync(input.S_SN);
        ActiveBoxQty = cartonBoxConfirmeds is null || cartonBoxConfirmeds.Count <= 0 ? 0 : cartonBoxConfirmeds.Count;
        if (FullBoxQty == ActiveBoxQty)
        {
            mMesOutputDto.IsPackingFinish = true;
            if (IsGenerateBoxSN)
            {
                //打印
            }
        }
        mMesOutputDto.CartonBoxConfirmeds = cartonBoxConfirmeds;
        if (string.IsNullOrEmpty(mMesOutputDto.ErrorMsg))
        {
            mainSnDynamicSnKey = GetDynamicSnKey(input.S_SN);
            cacheHelper.Add(mainSnDynamicSnKey, mMesOutputDto.CurrentInitPageInfo.DataList, new TimeSpan(2, 0, 0, 0));
        }
        return mMesOutputDto;
    }

    public async Task<BoxPackageMesOutputDtos> RemoveSingleAsync(BoxPackageRemove input)
    {
        BoxPackageMesOutputDtos mesOutputDtos = new BoxPackageMesOutputDtos();
        if (string.IsNullOrEmpty(input.InnerSN))
            return mesOutputDtos.SetErrorCode(P_MSG_Public.MSG_Public_016);
        var tmpBoxInfo = await Public_Repository.GetMesPackageByUnitSNAsync(input.InnerSN);
        if (tmpBoxInfo == null)
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20119);
        if (tmpBoxInfo is not null and { Stage : 1,StatusID :1})
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20119);


        if (tmpBoxInfo.SerialNumber != input.S_SN)
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20119);

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");

        var removeRes = await Public_Repository.uspPackageRemoveSingleAsync(input.InnerSN, xmlProdOrder,xmlStation, xmlExtraData, "1");
        if (removeRes.strOutput != "1")
            return mesOutputDtos.SetErrorCode(msgSys.GetLanguage(removeRes.strOutput));

        if (string.IsNullOrEmpty(removeRes.BoxSN))
            return mesOutputDtos.SetErrorCode(P_MSG_Public.MSG_Public_6041);

        cartonBoxConfirmeds = await Public_Repository.GetBoxPackageDataAsync(string.IsNullOrEmpty(input.S_SN)?removeRes.BoxSN: input.S_SN);
        ActiveBoxQty = cartonBoxConfirmeds is null || cartonBoxConfirmeds.Count <= 0 ? 0 : cartonBoxConfirmeds.Count;
        mesOutputDtos.CartonBoxConfirmeds = cartonBoxConfirmeds;
        return mesOutputDtos;
    }

    public async Task<BoxPackageMesOutputDtos> ReprintBoxSnAsync(MesSnInputDto input)
    {
        BoxPackageMesOutputDtos mesOutputDtos = new BoxPackageMesOutputDtos();
        if (string.IsNullOrEmpty(input.S_SN))
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20007);

        var tmpBoxInfo = await Public_Repository.GetMesPackageBySNAsync(input.S_SN);
        tmpBoxInfo ??= await Public_Repository.GetMesPackageByUnitSNAsync(input.S_SN);

        if (tmpBoxInfo is null or { StatusID : not 1})
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20190);

        cartonBoxConfirmeds = await Public_Repository.GetBoxPackageDataAsync(tmpBoxInfo.SerialNumber);
        ActiveBoxQty = cartonBoxConfirmeds is null || cartonBoxConfirmeds.Count <= 0 ? 0 : cartonBoxConfirmeds.Count;
        mesOutputDtos.CartonBoxConfirmeds = cartonBoxConfirmeds;
        await DapperConn.ExecuteAsync(Public_Repository.InsertMesPackageHistory(tmpBoxInfo.ID, 6, List_Login),null,null,I_DBTimeout,null);
        return mesOutputDtos;
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

    private async Task<(string, DynamicItemsDto)> CheckInputsAsync(DynamicItemsDto currentItem, BoxPackageAutoInput inputDtos)
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
                return (parRes.Item1,parRes.Item2);
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
                //查询是否已经有绑定关系
                var res = await Public_Repository.uspKitBoxCheckAsync(currentItem.Value, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "SN");
                if (res is null)
                {
                    resValueTuple.Item1 = P_MSG_Public.MSG_Public_6036;
                    break;
                }

                if (res.strOutput == "0")
                {
                    //校验主条码
                    if (!string.IsNullOrEmpty(inputDtos.S_SN) && inputDtos.S_SN != res.BoxSN)
                    {
                        resValueTuple.Item1 = P_MSG_Public.MSG_Public_6037;
                        break;
                    }
                    inputDtos.S_SN = BandingBoxSn = res.BoxSN;
                    var mainSnVerify = await MainSnVerifyAsync(inputDtos);
                    resValueTuple.Item1 = mainSnVerify.ErrorMsg ?? "1";
                }
                else if(res.strOutput == "1")
                {
                    var tmpFGSN = currentItem.Value;
                    if (!IsScanOnlyFGSN)
                    {
                        UPCSN = currentItem.Value;
                        tmpFGSN = await Public_Repository.MesGetFGSNByUPCSNAsync(UPCSN);
                    }

                    var fgUnitEntity = await Public_Repository.GetMesUnitSerialAsync(tmpFGSN);

                    if (IsScanOnlyFGSN)
                    {
                        fgUnitEntity.ProductionOrderID = inputDtos.S_POID.ToInt();
                    }

                    var resRouteCheck = await Public_Repository.GetRouteCheckAsync(List_Login.StationTypeID.ToInt(), List_Login.StationID,
                        List_Login.LineID.ToString(), fgUnitEntity, tmpFGSN);

                    if (resRouteCheck != "1")
                    {
                        resValueTuple.Item1 = resRouteCheck;
                        break;
                    }

                    if (string.IsNullOrEmpty(inputDtos.S_SN))
                    {
                        if (IsGenerateBoxSN)
                        {
                            mesUnit mesUnit = new mesUnit
                            {
                                StationID = List_Login.StationID,
                                EmployeeID = List_Login.EmployeeID,
                                ProductionOrderID = inputDtos.S_POID.ToInt(),
                                PartID = inputDtos.S_PartID.ToInt()
                            };
                            var insertBoxSnRes = await Public_Repository.Get_CreatePackageSN(BoxSNFormatName, xmlProdOrder, xmlPart,
                                xmlStation, xmlExtraData, mesUnit, 1);

                            if (insertBoxSnRes.Item1 != "1")
                            {
                                resValueTuple.Item1 = insertBoxSnRes.Item1;
                                break;
                            }
                            newBoxSn = insertBoxSnRes.Item2;

                            if (!Regex.IsMatch(newBoxSn,
                                    mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.BoxSN_Pattern))
                            {
                                resValueTuple.Item1 = "20027";
                                break;
                            }
                        }
                        else
                        {
                            resValueTuple.Item1 = P_MSG_Public.MSG_Public_6038;
                            break;
                        }
                    }

                    resValueTuple.Item1 = "1";
                }
                else
                {
                    resValueTuple.Item1 = res.strOutput;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return resValueTuple;
    }
    private async Task<string> ScanSubmit(BoxPackageAutoInput input)
    {
        //获取UPC/FG条码
        var upcDtos = input.DataList.Values.Where(x => x.Name == SnLinkUpcInputNames.IsScanUPCSN.ToString() && !string.IsNullOrEmpty(x.Value)).ToList();
        if (upcDtos is not { Count: 1 })
            return P_MSG_Public.MSG_Public_6040;

        string hadExist = await Public_Repository.CheckExistsLinkedAsync(upcDtos[0].Value, input.S_SN, input.S_POID);
        if (hadExist ==  "1")
        {
            return hadExist;
        }
        else
        {
            var kitBoxPackagingSql = await DataCommit_Repository.uspKitBoxPackagingNew(input.S_PartID, input.S_POID, upcDtos[0].Value, input.S_SN, List_Login,
                FullBoxQty, ActiveBoxQty + 1, IsScanOnlyFGSN ? -1 : 0);
            if (kitBoxPackagingSql.Item1 != "1")
            {
                Log4NetHelper.Error($"error code : {kitBoxPackagingSql.Item1}");
                return kitBoxPackagingSql.Item1;
            }
            return await ExecuteTransactionSqlAsync(kitBoxPackagingSql.Item2);
        }
    }

    public async Task<BoxPackageMesOutputDtos> LastBoxSubmitAsync(MesSnInputDto input)
    {
        BoxPackageMesOutputDtos mMesOutputDto = new BoxPackageMesOutputDtos();
        if (string.IsNullOrEmpty(input?.S_SN))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20007);

        cartonBoxConfirmeds = await Public_Repository.GetBoxPackageDataAsync(input.S_SN);
        mMesOutputDto.CartonBoxConfirmeds = cartonBoxConfirmeds;

        var packageUnit = await Public_Repository.GetMesPackageBySNAsync(input.S_SN);

        if (packageUnit?.StatusID != 0 || cartonBoxConfirmeds.Count == 0 || packageUnit.ParentID.ToInt() != 0 || packageUnit?.Stage != 1)
            return mMesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6043);

        var kitBoxPackagingSql = await DataCommit_Repository.uspKitBoxPackagingNew(input.S_PartID, input.S_POID, null, input.S_SN, List_Login,FullBoxQty, cartonBoxConfirmeds.Count, cartonBoxConfirmeds.Count);
        if (kitBoxPackagingSql.Item1 != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(kitBoxPackagingSql.Item1));
        var res = await ExecuteTransactionSqlAsync(kitBoxPackagingSql.Item2);
        if (res != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(res));
        return mMesOutputDto;
    }
}