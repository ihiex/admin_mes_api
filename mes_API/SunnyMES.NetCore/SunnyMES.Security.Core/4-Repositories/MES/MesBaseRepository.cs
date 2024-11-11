using API_MSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Enums.DynamicItemName;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.IRepositories;

namespace SunnyMES.Security.Repositories;

public class MesBaseRepository : BaseCommonRepository<string>, IMesBaseRepository
{

    protected MSG_Public P_MSG_Public;
    protected  MSG_Sys msgSys;

    protected LoginList List_Login = new LoginList();
    protected PublicMiniRepository Public_Repository;

    protected DataCommitRepository DataCommit_Repository;

    protected InitPageInfo mInitPageInfo = new InitPageInfo();
    protected SetConfirmPoOutput mSetConfirmPoOutput = new SetConfirmPoOutput();
    #region 存储过程参数
    protected string xmlProdOrder = string.Empty;
    protected string xmlPart = string.Empty;
    protected string xmlExtraData = string.Empty;
    protected string xmlStation = string.Empty;
    #endregion

    protected string mainSnDynamicSnKey = string.Empty;

    public MesBaseRepository(IDbContextCore contextCore) : base(contextCore)
    {
        _dbContext = contextCore;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();
    }


    public virtual void GetConfInfo(CommonHeader commonHeader)
    {
        baseCommonHeader = commonHeader;
        List_Login.LanguageID = commonHeader.Language;
        List_Login.LineID = commonHeader.LineId;
        List_Login.StationID = commonHeader.StationId;
        List_Login.EmployeeID = commonHeader.EmployeeId;
        List_Login.CurrentLoginIP = commonHeader.CurrentLoginIp;

        P_MSG_Public ??= new MSG_Public(List_Login.LanguageID);
        msgSys ??= new MSG_Sys(List_Login.LanguageID);
        Public_Repository ??= new PublicMiniRepository(_dbContext, List_Login.LanguageID);
        DataCommit_Repository ??= new DataCommitRepository(_dbContext, List_Login.LanguageID);
    }

    public virtual async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
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

                    var stationTypes = await Public_Repository.MesGetStationTypeAsync(List_Login.StationTypeID.ToString());
                    List_Login.StationType = stationTypes?[0].Description;
                    pageInitializeOutput.CurrentSettingInfo.StationType = List_Login.StationType;
                    pageInitializeOutput.CurrentSettingInfo.StationTypeID = List_Login.StationTypeID.ToInt();
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

    public virtual async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
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

            //确认PO时，前端在UniversalConfirmPoOutput中获取工单数量
            //mConfirmPoOutput.ProductionOrderQTY = await Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            mConfirmPoOutput.ErrorMsg = e.Message;
        }
        return mSetConfirmPoOutput = mConfirmPoOutput;
    }

}