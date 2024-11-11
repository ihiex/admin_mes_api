#define IsSql
using System.Collections.Generic;
using System.Threading.Tasks;
using API_MSG;
using Microsoft.Extensions.Configuration;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using System;
using System.Data;
using System.Text.RegularExpressions;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Log;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.CarrierLinkMaterialBatch;
using System.DirectoryServices.Protocols;
using SunnyMES.Security.ToolExtensions;
using NPOI.OpenXmlFormats.Shared;
using SunnyMES.Commons.Core.Dtos;

namespace SunnyMES.Security.Repositories;

public class CarrierLinkMaterialBatchRepository : BaseCommonRepository<string>, ICarrierLinkMaterialBatchRepository
{
    MSG_Public P_MSG_Public;
    private MSG_Sys msgSys;

    private LoginList List_Login = new LoginList();
    PublicRepository Public_Repository;

    DataCommitRepository DataCommit_Repository;

    private InitPageInfo mInitPageInfo = new InitPageInfo();

    #region 存储过程参数
    private string xmlProdOrder = string.Empty;
    private string xmlPart = string.Empty;
    private string xmlExtraData = string.Empty;
    private string xmlStation = string.Empty;
    #endregion

    private string carrierSNKey =>
        $"{baseCommonHeader.CurrentLoginIp}_{baseCommonHeader.StationId}_{baseCommonHeader.EmployeeId}_carrierSN";

    public CarrierLinkMaterialBatchRepository(IDbContextCore contextCore):base(contextCore)
    {
        _dbContext = contextCore;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();
    }
    public void GetConfInfo(CommonHeader commonHeader)
    {
        baseCommonHeader = commonHeader;
        List_Login.LanguageID = commonHeader.Language;
        List_Login.LineID = commonHeader.LineId;
        List_Login.StationID = commonHeader.StationId;
        List_Login.EmployeeID = commonHeader.EmployeeId;
        List_Login.CurrentLoginIP = commonHeader.CurrentLoginIp;

        P_MSG_Public ??= new MSG_Public(List_Login.LanguageID);
        msgSys ??= new MSG_Sys(List_Login.LanguageID);
        Public_Repository ??= new PublicRepository(_dbContext, List_Login.LanguageID);
        DataCommit_Repository ??= new DataCommitRepository(_dbContext, List_Login.LanguageID);
    }

    public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
    {
        GetPageInitializeOutput pageInitializeOutput = new GetPageInitializeOutput()
        {
            CurrentSettingInfo =
            {
                LineId = baseCommonHeader.LineId,
                LineName = baseCommonHeader.LineName,
                StationId = baseCommonHeader.StationId,
                StationName = baseCommonHeader.StationName,
                CurrentLoginIp = baseCommonHeader.CurrentLoginIp,
                Url = S_URL,
            }
        };


        try
        {
            //获取站点名称
            if (List_Login.StationID != 0 && List_Login.StationID != -1)
            {
                var mmesStations = await Public_Repository.MesGetStationAsync(List_Login.StationID.ToString());
                if (mmesStations?.Count < 1)
                {
                    pageInitializeOutput.ErrorMsg = P_MSG_Public.MSG_Public_6017;
                }
                else
                {
                    List_Login.Station = mmesStations[0].Description;
                    //如果前端未传递站点名称，则自动赋值
                    if (string.IsNullOrEmpty(pageInitializeOutput.CurrentSettingInfo.StationName))
                        pageInitializeOutput.CurrentSettingInfo.StationName = List_Login.Station;

                    List_Login.StationTypeID = mmesStations[0].StationTypeID;
                }
            }
            else
            {
                pageInitializeOutput.ErrorMsg = P_MSG_Public.MSG_Public_6017;
            }
            //mInitPageInfo = await Public_Repository.GetPageAllStationInfoAsync(List_Login, S_URL);
            var tmpStationInfo = await Public_Repository.GetPageAllStationInfoAsync(List_Login, S_URL);
            pageInitializeOutput.ErrorMsg = tmpStationInfo.Item1;
            pageInitializeOutput.CurrentInitPageInfo = mInitPageInfo = tmpStationInfo.Item2;
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            pageInitializeOutput.ErrorMsg = e.Message;
        }

        return pageInitializeOutput;
    }

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {

        SetConfirmPoOutput mConfirmPoOutput = new SetConfirmPoOutput();
        try
        {
            var pageInit = await GetPageInitializeAsync(S_URL);

            UniversalConfirmPoOutput universalConfirmPoOutput = await Public_Repository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);
            mConfirmPoOutput = new SetConfirmPoOutput()
            {
                CurrentInitPageInfo = pageInit.CurrentInitPageInfo,
                CurrentSettingInfo = pageInit.CurrentSettingInfo,
                UniversalConfirmPoOutput = universalConfirmPoOutput.ErrorMsg == null ? universalConfirmPoOutput : null,
                ErrorMsg = universalConfirmPoOutput.ErrorMsg
            };

            mConfirmPoOutput.CurrentSettingInfo.PartFamilyTypeID = S_PartFamilyTypeID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.PartFamilyID = S_PartFamilyID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.PartID = S_PartID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.POID = S_POID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.UnitStatus = S_UnitStatus.ToInt();

            //存储过程参数
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + S_PartID + "\"> </Part>";
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId + "\"> </ExtraData>";
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";

            ////初始化数据
            mConfirmPoOutput.CurrentInitPageInfo = await Public_Repository.GetAllPagePoInfoAsync(S_POID, mConfirmPoOutput.CurrentInitPageInfo);
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            mConfirmPoOutput.ErrorMsg = e.Message;
        }
        return mConfirmPoOutput;


    }


    public async Task<MesOutputDto> CarrierSnVerifyAsync(CarrierLinkMaterialBatch_CarrierSN_Input carrierSNInput)
    {
        MesOutputDto mesOutputDto = new MesOutputDto();
        string result = string.Empty;

        
        var pageInit = await GetPageInitializeAsync(carrierSNInput.S_URL);
        
#if IsSql
        result = await Public_Repository.GetMachineToolingCheck(carrierSNInput.S_CarrierSN, "", carrierSNInput.S_PartID, List_Login);
#else
        string xmlPartStr = "<Part PartID=\"" + carrierSNInput.S_PartID.ToString() + "\"> </Part>";

        result = await Public_Repository.uspCallProcedureAsync("uspMachineToolingCheck", carrierSNInput.S_CarrierSN, null, xmlPartStr,
            null, null, List_Login.StationTypeID.ToString());
#endif
        mesOutputDto.ErrorMsg = result != "1" ? msgSys.GetLanguage(result) : null;
        cacheHelper.Add(carrierSNKey, carrierSNInput.S_CarrierSN,new TimeSpan(5,0,0));
        return mesOutputDto;
    }

    public async Task<MesOutputDto> BatchNumberVerifyAsync(CarrierLinkMaterialBatch_BN_Input batchNumberInput)
    {
        MesOutputDto mesOutputDto = new MesOutputDto();

        await GetPageInitializeAsync(batchNumberInput.S_URL);
        var confirmPoOutput = await SetConfirmPOAsync(batchNumberInput.S_PartFamilyTypeID, batchNumberInput.S_PartFamilyID,
            batchNumberInput.S_PartID, batchNumberInput.S_POID, batchNumberInput.S_UnitStatus, batchNumberInput.S_URL);

        if (string.IsNullOrEmpty(confirmPoOutput.UniversalConfirmPoOutput.S_Batch_Pattern))
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20025);

        if (!Regex.IsMatch(batchNumberInput.S_BatchNumber, confirmPoOutput.UniversalConfirmPoOutput.S_Batch_Pattern))
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20047);

        if (batchNumberInput.S_UnitStatus is "2" or "3")
        {
            if (string.IsNullOrEmpty(batchNumberInput.S_DefectID))
                return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20049);
        }

        string mUnitStateId = await Public_Repository.GetMesUnitState(batchNumberInput.S_PartID,
            batchNumberInput.S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0, batchNumberInput.S_POID,
            batchNumberInput.S_UnitStatus);
        if (string.IsNullOrEmpty(mUnitStateId))
            return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20203);

        string S_FormatSN = await Public_Repository.mesGetSNFormatIDByListAsync(batchNumberInput.S_PartID, batchNumberInput.S_PartFamilyID, List_Login.LineID.ToString(), batchNumberInput.S_POID, List_Login.StationTypeID.ToString());
        if (string.IsNullOrEmpty(S_FormatSN))
            return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20132);
        //判断输入的托盘条码和批次号是否相同
        string carrierSN = cacheHelper.Get(carrierSNKey)?.ToString() ?? "";
        if (string.IsNullOrEmpty(carrierSN))
            return Public_Repository.SetMsgPublic(mesOutputDto, P_MSG_Public.MSG_Public_6005);

        //输入的托盘条码和批次号相同
        if (carrierSN == batchNumberInput.S_BatchNumber)
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20048);

        //治具状态再次确认 
        List<mesMachine> machines = await Public_Repository.GetmesMachineAsync(carrierSN);
        if (machines?.Count <= 0)
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20242);
        if (machines[0].StatusID != 1)
            return Public_Repository.SetMsgSys(mesOutputDto, P_MSG_Public.MSG_Public_018);

        //料号检查
        var POs = await Public_Repository.GetmesProductionOrderAsync(batchNumberInput.S_POID);
        if (POs?.Count <= 0)
            return Public_Repository.SetMsgPublic(mesOutputDto, P_MSG_Public.MSG_Public_6027);

        if (POs[0].PartID.ToString() != batchNumberInput.S_PartID)
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20050);

        //获取新条码
        string xmlProdOrder = "<ProdOrder ProductionOrder=" + "\"" + batchNumberInput.S_POID + "\"" + "> </ProdOrder>";
        string xmlStation = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
        string xmlPart = "<Part PartID=" + "\"" + batchNumberInput.S_PartID + "\"" + "> </Part>";
        string xmlExtraData = "<ExtraData BoxSN=" + "\"" + carrierSN + "\"" +
                              " LineID = " + "\"" + List_Login.LineID.ToString() + "\"" +
                              " PartFamilyTypeID=" + "\"" + batchNumberInput.S_PartFamilyTypeID + "\"" +
                              " LineType=" + "\"" + "M" + "\"" +
                              "> </ExtraData>";

        string New_SN = await Public_Repository.GetSNRGetNextAsync(S_FormatSN, "0", xmlProdOrder, xmlPart,
            xmlStation, xmlExtraData);
        //批次校验
        string outputResult = string.Empty;

#if IsSql
        outputResult = await Public_Repository.uspBatchDataCheckAsync(batchNumberInput.S_BatchNumber, null, xmlPart, null, null, "1");

#else
        outputResult = await Public_Repository.uspCallProcedureAsync("uspBatchDataCheck", batchNumberInput.S_BatchNumber, null, xmlPart,
            null, null, "1");
#endif
        if (outputResult != "1")
            return mesOutputDto.SetErrorCode(msgSys.GetLanguage(outputResult));

        List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

        try
        {
            mesUnit v_mesUnit = new mesUnit();
            v_mesUnit.UnitStateID = Convert.ToInt32(mUnitStateId);
            v_mesUnit.StatusID = Convert.ToInt32(batchNumberInput.S_UnitStatus);
            v_mesUnit.StationID = List_Login.StationID;
            v_mesUnit.EmployeeID = List_Login.EmployeeID;
            v_mesUnit.CreationTime = DateTime.Now;
            v_mesUnit.LastUpdate = DateTime.Now;

            v_mesUnit.PanelID = 0;
            v_mesUnit.LineID = List_Login.LineID;
            v_mesUnit.ProductionOrderID = Convert.ToInt32(batchNumberInput.S_POID);

            v_mesUnit.RMAID = 0;
            v_mesUnit.PartID = Convert.ToInt32(batchNumberInput.S_PartID);
            v_mesUnit.LooperCount = 1;
            


            mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
            v_mesSerialNumber.SerialNumberTypeID = 8;
            v_mesSerialNumber.Value = New_SN;


            //////////////////////////////////////  mesUnitDetail /////////////////////////////////////////////////////                                    
            mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
            //v_mesUnitDetail.UnitID = Convert.ToInt32(v_mesUnit.ID);
            v_mesUnitDetail.reserved_01 = carrierSN;
            v_mesUnitDetail.reserved_02 = batchNumberInput.S_BatchNumber;
            v_mesUnitDetail.reserved_03 = "1";        //开料 1  备胶  2          
            v_mesUnitDetail.reserved_04 = "";
            v_mesUnitDetail.reserved_05 = "";
            

            mesHistory v_mesHistory = new mesHistory();
            v_mesHistory.UnitID = Convert.ToInt32(v_mesUnit.ID);
            v_mesHistory.UnitStateID = Convert.ToInt32(mUnitStateId);
            v_mesHistory.EmployeeID = List_Login.EmployeeID;
            v_mesHistory.StationID = List_Login.StationID;
            v_mesHistory.EnterTime = DateTime.Now;
            v_mesHistory.ExitTime = DateTime.Now;
            v_mesHistory.ProductionOrderID = Convert.ToInt32(batchNumberInput.S_POID);
            v_mesHistory.PartID = Convert.ToInt32(batchNumberInput.S_PartID);
            v_mesHistory.LooperCount = 1;
            v_mesHistory.StatusID = batchNumberInput.S_UnitStatus.ToInt();
            

            //////////////////////////  Defect ///////////////////////////////////////////
            string[] Array_Defect = batchNumberInput.S_DefectID?.Split(',');
            if (batchNumberInput.S_UnitStatus == "2")
            {
                foreach (var item in Array_Defect)
                {
                    if (item.Trim() != "")
                    {
                        int I_DefectID = Convert.ToInt32(item);
                        var v_mesUnitDefect = new mesUnitDefect
                        {
                            //v_mesUnitDefect.UnitID = v_mesUnit.ID;
                            DefectID = I_DefectID,
                            StationID = List_Login.StationID,
                            EmployeeID = List_Login.EmployeeID
                        };

                        List_mesUnitDefect.Add(v_mesUnitDefect);
                    }
                }
            }
            //////////////////////////////////////////////////////////////////////////////
            mesMachine v_mesMachine = new mesMachine();
            v_mesMachine.SN = carrierSN;


            string ReturnValue = string.Empty;
            string sql = DataCommit_Repository.SubmitData_Unit_UnitDetail_SN_History_Defect_Machine(v_mesUnit, v_mesUnitDetail,
                v_mesSerialNumber, null, v_mesHistory, List_mesUnitDefect, v_mesMachine, null,
                List_Login);

            ReturnValue = await ExecuteTransactionSqlAsync(sql);

            if (ReturnValue != "1")
                return mesOutputDto.SetErrorCode(ReturnValue);

            cacheHelper.Remove(carrierSNKey);
        }
        catch (Exception ex)
        {
            return mesOutputDto.SetErrorCode(ex.Message);
        }

        return mesOutputDto;
    }


}