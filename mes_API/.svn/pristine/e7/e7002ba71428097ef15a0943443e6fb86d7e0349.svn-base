using API_MSG;
using NPOI.Util;
using System;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkBatch;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories;

public class SNLinkBatchRepository : BaseCommonRepository<string>,ISNLinkBatchRepository
{
    MSG_Public P_MSG_Public;
    private MSG_Sys msgSys;

    private LoginList List_Login = new LoginList();
    PublicRepository Public_Repository;

    DataCommitRepository DataCommit_Repository;

    private InitPageInfo mInitPageInfo = new InitPageInfo();
    private SetConfirmPoOutput mSetConfirmPoOutput = new SetConfirmPoOutput();
    #region 存储过程参数
    private string xmlProdOrder = string.Empty;
    private string xmlPart = string.Empty;
    private string xmlExtraData = string.Empty;
    private string xmlStation = string.Empty;
    #endregion

    public int I_RouteSequence = -9;  //当前工序顺序 

    public SNLinkBatchRepository(IDbContextCore contextCore) : base(contextCore)
    {
        _dbContext = contextCore;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();
    }
    //private string NewToolingSNKey =>
    //    $"{baseCommonHeader.CurrentLoginIp}_{baseCommonHeader.StationId}_{baseCommonHeader.EmployeeId}_NewToolingSN";

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
        return mSetConfirmPoOutput = mConfirmPoOutput;
    }

    public async Task<MesOutputDto> BatchSnVerifyAsync(SNLinkBatch_BSN_Input input)
    {
        MesOutputDto mMesOutputDto = new MesOutputDto();
        string S_Batch = input.S_BatchNumber;

        var mConfirmPo = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID,
            input.S_UnitStatus, input.S_URL);
        if (string.IsNullOrEmpty(mConfirmPo.UniversalConfirmPoOutput.S_Batch_Pattern))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20025);


        if (!Regex.IsMatch(S_Batch, mConfirmPo.UniversalConfirmPoOutput.S_Batch_Pattern.Replace("\\\\", "\\")))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20047);

        return mMesOutputDto;
    }

    public async Task<MesOutputDto> LinkSnVerifyAsync(SNLinkBatch_SN_Input input)
    {
        MesOutputDto mMesOutputDto = new MesOutputDto();

        if (string.IsNullOrEmpty(input.S_SN) || string.IsNullOrEmpty(input.S_BatchNumber))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20007);

        if (input.S_BatchNumber == input.S_SN)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20048);

        var mConfirmPo = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID,
            input.S_UnitStatus, input.S_URL);

        if (string.IsNullOrEmpty(mConfirmPo.UniversalConfirmPoOutput.S_Batch_Pattern))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20025);


        if (!Regex.IsMatch(input.S_BatchNumber, mConfirmPo.UniversalConfirmPoOutput.S_Batch_Pattern.Replace("\\\\", "\\")))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20047);


        var mMesUnitState = await Public_Repository.GetMesUnitState(input.S_PartID, input.S_PartFamilyID, List_Login.LineID.ToString(),
            List_Login.StationTypeID.ToInt(), input.S_POID, input.S_UnitStatus);

        if (string.IsNullOrEmpty(mMesUnitState))
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20203);

        int I_FirstScanSequence = FirstScanSequence(input.S_PartID, List_Login.StationTypeID ?? 0);

        #region 代码永远不会执行到，尊重原著
        if (I_RouteSequence > I_FirstScanSequence)
        {
            var mUnit = await Public_Repository.GetmesUnitSNAsync(input.S_SN);
            if (mUnit is null ||  mUnit.ProductionOrderID.ToString()  == "" || mUnit.ProductionOrderID.ToString() == "0")
            {
                return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20014);
            }

            ////////////
            /// PO返回前台
        }
        #endregion

        var mUnits = await Public_Repository.GetmesUnitSNAsync(input.S_SN);
        if (mUnits is null || mUnits.ProductionOrderID.ToString() == "" || mUnits.ProductionOrderID.ToString() == "0")
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20242);

        if (mUnits.StatusID == 2)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20036);

        int mPartId = mUnits.PartID ?? 0;
        if (mPartId.ToString() != input.S_PartID)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20050);

        var S_RouteCheck =await Public_Repository.GetRouteCheckAsync(List_Login.StationTypeID ?? 0, List_Login.StationID, List_Login.LineID.ToString(),
            mUnits, input.S_SN);

        if (S_RouteCheck != "1")
            return mMesOutputDto.SetErrorCode(msgSys.GetLanguage(S_RouteCheck));


        mesUnit v_mesUnit = new mesUnit();
        v_mesUnit.ID = Convert.ToInt32(mUnits.ID);
        v_mesUnit.UnitStateID = Convert.ToInt32(mMesUnitState);
        v_mesUnit.StatusID = Convert.ToInt32(input.S_UnitStatus);
        v_mesUnit.StationID = List_Login.StationID;
        v_mesUnit.EmployeeID = List_Login.EmployeeID;
        v_mesUnit.ProductionOrderID = Convert.ToInt32(mUnits.ProductionOrderID);

        mesUnitDetail v_mesUnitDetail = await Public_Repository.GetmesUnitDetailAsync(mUnits.ID.ToString());
        v_mesUnitDetail.reserved_01 = input.S_SN;
        v_mesUnitDetail.reserved_02 = input.S_BatchNumber;


        var tmpSql = DataCommit_Repository.SubmitData_Unit_Detail_History_Mod(v_mesUnit,v_mesUnitDetail);
        if (string.IsNullOrEmpty(tmpSql) || tmpSql.StartsWith("ERROR"))
            return mMesOutputDto.SetErrorCode(tmpSql);

        var affactRow = await DapperConn.ExecuteAsync(tmpSql);
        if (affactRow <= 0)
            return mMesOutputDto.SetErrorCode(msgSys.MSG_Sys_20120);
        
        return mMesOutputDto;
    }

    /// <summary>
    /// 第一站 扫描顺序号
    /// </summary>
    /// <param name="PartID"></param>
    /// <param name="StationTypeID"></param>
    /// <returns></returns>
    private int FirstScanSequence(string PartID, int StationTypeID)
    {
        int I_Result = 1;
        string PartFamilyID = mSetConfirmPoOutput?.CurrentSettingInfo?.PartFamilyID.ToString();
        if (IsOneStationPrint(PartID, PartFamilyID, StationTypeID, List_Login.LineID.ToString(), mSetConfirmPoOutput?.CurrentSettingInfo?.POID.ToString()))
        {
            I_Result = 2;
        }
        return I_Result;
    }
    /// <summary>
    /// 第一站是否  打印
    /// </summary>
    /// <param name="PartID"></param>
    /// <param name="StationTypeID"></param>
    /// <returns></returns>
    public Boolean IsOneStationPrint(string PartID, string PartFamilyID, int StationTypeID, string LineID, string ProductionOrderID)
    {
        Boolean B_Result = false;
        
        //int RouteID =  Public_Repository.GetRouteID(PartID, PartFamilyID, LineID, ProductionOrderID).Result;
        int RouteID = Public_Repository.ufnRTEGetRouteIDAsync(LineID.ToInt(), PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt()).Result.ToInt();

        var DT_Route = Public_Repository.GetRoute("", RouteID).Result;

        if (DT_Route is not  null && DT_Route.Rows.Count > 0)
        {
            //第一站是打印的情况 确定以后
            var v_RouteSequence = from c in DT_Route.AsEnumerable()
                where c.Field<int>("Sequence") == 1 && c.Field<string>("ApplicationType") == "Print"
                select c;

            if (v_RouteSequence.Any())
            {
                B_Result = true;
            }
        }
        return B_Result;
    }
}