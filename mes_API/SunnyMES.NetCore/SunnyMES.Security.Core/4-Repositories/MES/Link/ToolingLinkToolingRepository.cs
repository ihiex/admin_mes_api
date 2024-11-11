#define IsSql
using API_MSG;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories;

public class ToolingLinkToolingRepository : BaseCommonRepository<string>, IToolingLinkToolingRepository
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

    public ToolingLinkToolingRepository(IDbContextCore contextCore):base(contextCore)
    {
        _dbContext = contextCore;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();
    }
    private string NewToolingSNKey =>
        $"{baseCommonHeader.CurrentLoginIp}_{baseCommonHeader.StationId}_{baseCommonHeader.EmployeeId}_NewToolingSN";
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

    public async Task<MesOutputDto> NewToolingSnVerifyAsync(ToolingLinkTooling_NewTooling_Input newToolingInput)
    {
        MesOutputDto mesOutputDto = new MesOutputDto();
        string result = string.Empty;
        var pageInit = await GetPageInitializeAsync(newToolingInput.S_URL);

#if IsSql
        result = await Public_Repository.GetMachineToolingCheck(newToolingInput.S_NewToolingSN, "", newToolingInput.S_PartID, List_Login);
#else
        string xmlPartStr = "<Part PartID=\"" + newToolingInput.S_PartID.ToString() + "\"> </Part>";

        result = await Public_Repository.uspCallProcedureAsync("uspMachineToolingCheck", newToolingInput.S_NewToolingSN, null, xmlPartStr,
            null, null, List_Login.StationTypeID.ToString());
#endif
        mesOutputDto.ErrorMsg = result != "1" ? msgSys.GetLanguage(result) : null;
        cacheHelper.Add(NewToolingSNKey, newToolingInput.S_NewToolingSN, new TimeSpan(5, 0, 0));
        return mesOutputDto;
    }

    public async Task<MesOutputDto> OldToolingSnVerifyAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput)
    {
        MesOutputDto mesOutputDto = new MesOutputDto();

        if (string.IsNullOrEmpty(oldToolingInput.S_OldToolingSN) ||
            string.IsNullOrEmpty(oldToolingInput.S_NewToolingSN))
            return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20007);

        if (oldToolingInput.S_OldToolingSN == oldToolingInput.S_NewToolingSN)
            return mesOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6028);


        await GetPageInitializeAsync(oldToolingInput.S_URL);
        var confirmPoOutput = await SetConfirmPOAsync(oldToolingInput.S_PartFamilyTypeID, oldToolingInput.S_PartFamilyID,
                    oldToolingInput.S_PartID, oldToolingInput.S_POID, oldToolingInput.S_UnitStatus, oldToolingInput.S_URL);

        string MachineSN = oldToolingInput.S_OldToolingSN.ToString();
        if (string.IsNullOrEmpty(MachineSN))
            return Public_Repository.SetMsgSys(mesOutputDto, msgSys.MSG_Sys_20007);
        
        string result = string.Empty;
#if IsSql
        result = await Public_Repository.GetMachineToolingCheck(oldToolingInput.S_OldToolingSN, "", oldToolingInput.S_PartID, List_Login);
#else
        string xmlPartStr = "<Part PartID=\"" + oldToolingInput.S_PartID.ToString() + "\"> </Part>";

        result = await Public_Repository.uspCallProcedureAsync("uspMachineToolingCheck", oldToolingInput.S_NewToolingSN, null, xmlPartStr,
            null, null, List_Login.StationTypeID.ToString());
#endif
        if (result != "1")
            return mesOutputDto.SetErrorCode(msgSys.GetLanguage(result));

        string mUnitStateId = await Public_Repository.GetMesUnitState(oldToolingInput.S_PartID,
            oldToolingInput.S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0, oldToolingInput.S_POID,
            oldToolingInput.S_UnitStatus);
        if (string.IsNullOrEmpty(mUnitStateId))
            return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20203);


        string S_FormatSN = await Public_Repository.mesGetSNFormatIDByListAsync(oldToolingInput.S_PartID, oldToolingInput.S_PartFamilyID, List_Login.LineID.ToString(), oldToolingInput.S_POID, List_Login.StationTypeID.ToString());

        if (string.IsNullOrEmpty(S_FormatSN))
            return mesOutputDto.SetErrorCode((msgSys.MSG_Sys_20132));

        
        //获取新条码
        string xmlProdOrder = "<ProdOrder ProductionOrder=" + "\"" + oldToolingInput.S_POID + "\"" + "> </ProdOrder>";
        string xmlStation = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
        string xmlPart = "<Part PartID=" + "\"" + oldToolingInput.S_PartID + "\"" + "> </Part>";
        string xmlExtraData = "<ExtraData BoxSN=" + "\"" + MachineSN + "\"" +
                              " LineID = " + "\"" + List_Login.LineID.ToString() + "\"" +
                              " PartFamilyTypeID=" + "\"" + oldToolingInput.S_PartFamilyTypeID + "\"" +
                              " LineType=" + "\"" + "M" + "\"" +
                              "> </ExtraData>";
        string New_SN = await Public_Repository.GetSNRGetNextAsync(S_FormatSN, "0", xmlProdOrder, xmlPart,
            xmlStation, xmlExtraData);

        List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

        try
        {
            mesUnit v_mesUnit = new mesUnit();
            v_mesUnit.UnitStateID = Convert.ToInt32(mUnitStateId);
            v_mesUnit.StatusID = Convert.ToInt32(oldToolingInput.S_UnitStatus);
            v_mesUnit.StationID = List_Login.StationID;
            v_mesUnit.EmployeeID = List_Login.EmployeeID;
            v_mesUnit.CreationTime = DateTime.Now;
            v_mesUnit.LastUpdate = DateTime.Now;

            v_mesUnit.PanelID = 0;
            v_mesUnit.LineID = List_Login.LineID;
            v_mesUnit.ProductionOrderID = Convert.ToInt32(oldToolingInput.S_POID);

            v_mesUnit.RMAID = 0;
            v_mesUnit.PartID = Convert.ToInt32(oldToolingInput.S_PartID);
            v_mesUnit.LooperCount = 1;

            mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
            v_mesSerialNumber.SerialNumberTypeID = 8;
            v_mesSerialNumber.Value = New_SN;
            
            string S_TLinkT = oldToolingInput.S_NewToolingSN + ";" + oldToolingInput.S_OldToolingSN + ";";
            string[] L_TLinkT = S_TLinkT.Split(';');


            //////////////////////////  Defect ///////////////////////////////////////////
            string[] Array_Defect = oldToolingInput.S_DefectID?.Split(',');
            if (oldToolingInput.S_UnitStatus == "2")
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
            List<mesMachine> listMesMachines = new List<mesMachine>();

            mesMachine v_mesMachineNew = new mesMachine();
            v_mesMachineNew.SN = oldToolingInput.S_NewToolingSN;
            listMesMachines.Add(v_mesMachineNew);

            mesMachine v_MesMachineOld = new mesMachine();
            v_MesMachineOld.SN = oldToolingInput.S_OldToolingSN;
            listMesMachines.Add(v_MesMachineOld);

            string ReturnValue = string.Empty;
            string sql = DataCommit_Repository.SubmitData_Units_UnitDetails_SNs_Historys_Defects_Machines(new List<mesUnit>(){v_mesUnit }, null,
                new List<mesSerialNumber>(){v_mesSerialNumber}, null,  List_mesUnitDefect, listMesMachines, L_TLinkT,
                List_Login);

            ReturnValue = await ExecuteTransactionSqlAsync(sql);

            if (ReturnValue != "1")
                return mesOutputDto.SetErrorCode(ReturnValue);

            cacheHelper.Remove(NewToolingSNKey);
        }
        catch (Exception ex)
        {
            return mesOutputDto.SetErrorCode(ex.Message);
        }

        return mesOutputDto;
    }

    public async Task<MesOutputDto> OldToolingSnReleaseAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput)
    {
        MesOutputDto mesOutputDto = new MesOutputDto();

        if (string.IsNullOrEmpty(oldToolingInput.S_OldToolingSN))
            return mesOutputDto.SetErrorCode(msgSys.MSG_Sys_20007);

        
        var pageInit = await GetPageInitializeAsync(oldToolingInput.S_URL);

        string result =
            Public_Repository.MesToolingReleaseCheck(oldToolingInput.S_OldToolingSN, List_Login.StationTypeID.ToString());

        if (result != "1")
            return mesOutputDto.SetErrorCode(msgSys.GetLanguage(result));
        Public_Repository.ModMachine(oldToolingInput.S_OldToolingSN,"1",true);

        return mesOutputDto;
    }
}