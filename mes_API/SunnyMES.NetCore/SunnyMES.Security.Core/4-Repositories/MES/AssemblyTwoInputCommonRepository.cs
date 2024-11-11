#undef IsSql
#define IsSql
using API_MSG;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyTwoInput;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;


namespace SunnyMES.Security.Repositories;

public class AssemblyTwoInputRepository : BaseCommonRepository<string>, IAssemblyTwoInputRepository
{

    MSG_Public P_MSG_Public;
    private MSG_Sys msgSys;

    private LoginList List_Login = new LoginList();
    PublicMiniRepository Public_Repository;

    DataCommitRepository DataCommit_Repository;

    private InitPageInfo mInitPageInfo = new InitPageInfo();
    
    private string SN_Pattern = string.Empty;
    string ColorCode = string.Empty;
    string SpcaCode = string.Empty;
    string PpCode = string.Empty;
    string BuildCode = string.Empty;
    
    protected string bomPartInfoKey = string.Empty;
    protected string pageInfoKey = string.Empty;
    public AssemblyTwoInputRepository(IDbContextCore dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();
    }
    public async Task<List<TabVal>> GetConfInfoAsync(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
    {
        List_Login.LanguageID = I_Language;
        List_Login.LineID = I_LineID;
        List_Login.StationID = I_StationID;
        List_Login.EmployeeID = I_EmployeeID;
        List_Login.CurrentLoginIP = S_CurrentLoginIP;

        PublicRepository v_Public_Repository = new PublicRepository();

        List<TabVal> List_Station = await v_Public_Repository.MesGetStationNoTask(I_StationID.ToString(), "", "");
        List<TabVal> List_Tab = null;
        if (List_Station?.Count <= 0)
        {

        }
        else
        {
            List_Login.StationTypeID = List_Station?[0].Valint1 ?? 0;


            P_MSG_Public ??= new MSG_Public(I_Language);
            msgSys ??= new MSG_Sys(I_Language);
            Public_Repository ??= new PublicMiniRepository(_dbContext, I_Language);
            DataCommit_Repository ??= new DataCommitRepository(_dbContext, I_Language);

            string S_Sql = "select 'ok' as ValStr1,'' ValStr2,'1' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            List_Tab = new List<TabVal>();
            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            List_Tab = v_Query.AsList();
        }



        return List_Tab;
    }

    public async Task<IEnumerable<dynamic>> GetPageInitializeAsync(string S_URL)
    {
        //获取站点名称
        if (List_Login.StationID != 0 && List_Login.StationID != -1)
        {
            var mmesStations = await Public_Repository.MesGetStationAsync(List_Login.StationID.ToString());
            if (mmesStations?.Count < 1)
            {
                mInitPageInfo.DataList = new { error = "station info no match." };
            }
            else
            {
                List_Login.Station = mmesStations[0].Description;
                List_Login.StationTypeID = mmesStations[0].StationTypeID;
            }
        }
        else
        {
            mInitPageInfo.DataList = new { error = $"can't get station info by station id, station id is {List_Login.StationID}." };
        }
        mInitPageInfo = await Public_Repository.GetPageStationInfoAsync(List_Login, S_URL);

        if (mInitPageInfo.stationAttribute != null)
        {
            string S_Sql = string.Empty;
            S_Sql = $"select '{mInitPageInfo.stationAttribute.IsCheckPN}' IsCheckPN,'{mInitPageInfo.stationAttribute.IsCheckPO}' IsCheckPO,'{mInitPageInfo.IsLegalPage}' IsLegalPage, '{mInitPageInfo.stationAttribute.IsCheckVendor}' IsCheckVendor,'{mInitPageInfo.stationAttribute.COF}' COF,'{mInitPageInfo.stationAttribute.ApplicationType}' ApplicationType,'{oPoAttributes.DOE_Parameter1}' DOE_Parameter1,'{mInitPageInfo.stationAttribute.SNScanType}' SNScanType, '{mInitPageInfo.stationAttribute.IsDOEPrint}' IsDOEPrint";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query ?? null;
        }
        return null;
    }


    public async Task<List<dynamic>> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus,
        string S_URL)
    {
        List<dynamic> List_Result = new List<dynamic>();
        AssemblyTwoInput_SetConfirmPO_Output_Dto mSetConfirmPO_Dto = new AssemblyTwoInput_SetConfirmPO_Output_Dto();

        mSetConfirmPO_Dto.CurrentSettingInfo.StationId = List_Login.StationID;
        mSetConfirmPO_Dto.CurrentSettingInfo.StationName = List_Login.Station;

        #region test

        #endregion
        ////初始化数据
        await GetPageInitializeAsync(S_URL);
        mInitPageInfo = await Public_Repository.GetPagePoInfoAsync(S_POID,mInitPageInfo, List_Login);
        pageInfoKey = GetPageInfoKey();
        cacheHelper.Add(pageInfoKey, mInitPageInfo);
        try
        {
            //List<dynamic> List_SetConfirmPO = await SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID,
            //    S_PartID, S_POID, S_UnitStatus, List_Login);

            //AssemblyTwoInput_SetConfirmPO_Output_Dto mSetConfirmPO_DtoT = List_SetConfirmPO[0] as AssemblyTwoInput_SetConfirmPO_Output_Dto;

            UniversalConfirmPoOutput List_SetConfirmPO = await Public_Repository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);

            mSetConfirmPO_Dto.MesProductStructures = List_SetConfirmPO.MesProductStructures;
            mSetConfirmPO_Dto.ProductionOrderQTY = List_SetConfirmPO.ProductionOrderQTY;
            mSetConfirmPO_Dto.RouteDataDiagram1 = List_SetConfirmPO.RouteDataDiagram1;
            mSetConfirmPO_Dto.RouteDataDiagram2 = List_SetConfirmPO.RouteDataDiagram2;
            mSetConfirmPO_Dto.RouteDetail = List_SetConfirmPO.RouteDetail;
            mSetConfirmPO_Dto.ErrorMsg = List_SetConfirmPO.ErrorMsg;

            mSetConfirmPO_Dto.initPageInfos = mInitPageInfo;
            
            #region 缓存
            var ldBomPartInfoList = await Public_Repository.MESGetBomPartInfoCAsync(int.Parse(S_PartID), List_Login.StationTypeID ?? 0);

            if (ldBomPartInfoList?.Count < 2)
            {
                mSetConfirmPO_Dto.ErrorMsg = P_MSG_Public.MSG_Public_6003;
                List_Result.Add(mSetConfirmPO_Dto);
                return List_Result;
            }

            if (string.IsNullOrEmpty(mInitPageInfo.stationAttribute?.SNScanType))
            {
               mInitPageInfo.stationAttribute.SNScanType = ldBomPartInfoList[0].ScanType.ToString();
            }
            bomPartInfoKey = GetBomPartInfoKey();
            cacheHelper.Add(bomPartInfoKey, ldBomPartInfoList, new TimeSpan(7, 0, 0, 0), true);
            mSetConfirmPO_Dto.AssemblyQty = ldBomPartInfoList.Count - 1;
            mSetConfirmPO_Dto.BomPartInfo = ldBomPartInfoList;
            List_Result.Add(mSetConfirmPO_Dto);
            #endregion
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
        }
        return List_Result;
    }
    private string GetBomPartInfoKey() =>  $"BomPartInfo_" + List_Login.CurrentLoginIP + "_userID_" + List_Login.EmployeeID + "_stationID_" + List_Login.StationID;

    private string GetPageInfoKey() => $"PageInfo_" + List_Login.CurrentLoginIP + "_userID_" + List_Login.EmployeeID + "_stationID_" + List_Login.StationID;

    public async Task<List<dynamic>> GetColorCodeAsync()
    {
        List<dynamic> ldyList = new List<dynamic>();
        ldyList.AddRange(oPoAttributes?.DOE_Parameter1?.Split(',') ?? new string[]{});
        return ldyList;
    }

    public async Task<List<dynamic>> MainSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus,
        string S_URL, string MainSN)
    {

        List<dynamic> List_Result = new List<dynamic>();
        AssemblyTwoInput_MainSnVerify_Output_Dto assemblyTwoInputMainSnVerifyDto = new AssemblyTwoInput_MainSnVerify_Output_Dto
            {
                CurrentSettingInfo =
                {
                    StationId = List_Login.StationID,
                    StationName = List_Login.Station
                }
            };

        List_Result.Add(assemblyTwoInputMainSnVerifyDto);
        if (string.IsNullOrEmpty(MainSN))
        {
            assemblyTwoInputMainSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_016;
            return List_Result;
        }

        //await GetPageInitializeAsync(S_URL);

        string tmpKey = GetBomPartInfoKey();
        var topBomPartInfo = cacheHelper.Get<List<BomPartInfo>>(tmpKey);
        if (topBomPartInfo?.Count < 2)
        {
            assemblyTwoInputMainSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6005;
            return List_Result;
        }

        var verRes = await VerifyMianCodeAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL, MainSN,
            topBomPartInfo);
        assemblyTwoInputMainSnVerifyDto.ErrorMsg = verRes.Item1 ? "" : verRes.Item2;
        return List_Result;
    }

    public async Task<List<dynamic>> ChildSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL, string MainSN, string ChildSN, string sColorCode, string sBuildCode, string sSpcaCode, string sPpCode)
    {
        string tmpAllBarcode = MainSN + "-" + ChildSN;
        AssemblyTwoInput_ChildSnVerify_Output_Dto assemblyTwoInputChildSnVerifyDto = new AssemblyTwoInput_ChildSnVerify_Output_Dto
            {
                CurrentSettingInfo =
                {
                    StationId = List_Login.StationID,
                    StationName = List_Login.Station
                }
            };
        var List_Result = new List<dynamic>();

        ColorCode = sColorCode;
        BuildCode = sBuildCode;
        SpcaCode = sSpcaCode;
        PpCode = sPpCode;


        List_Result.Add(assemblyTwoInputChildSnVerifyDto);
        if (string.IsNullOrEmpty(MainSN) || string.IsNullOrEmpty(ChildSN))
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_016;
            return List_Result;
        }
        if (ChildSN.Contains(MainSN))
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6006;
            return List_Result;
        }
        string[] childSns = ChildSN.Split(',');
        List<BomPartInfo> bomPartInfoList = new List<BomPartInfo>();

        #region 获取缓存
        bomPartInfoKey = GetBomPartInfoKey() + "_" + MainSN.Trim();
        if (string.IsNullOrEmpty(bomPartInfoKey) || !cacheHelper.Exists(bomPartInfoKey))
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6005;
            return List_Result;
        }
        bomPartInfoList = cacheHelper.Get<List<BomPartInfo>>(bomPartInfoKey);
         
        if (bomPartInfoList?.Count <= 0)
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6008;
            return List_Result;
        }
        #endregion

        assemblyTwoInputChildSnVerifyDto.CurrentIndex = bomPartInfoList.Where(x => !string.IsNullOrEmpty(x.Barcode)).Count();
        string LastChildSn = childSns[childSns.Length - 1];

        //在缓存中查找子条码是否已经扫描过
        for (int i = 0; i < childSns.Length - 1; i++)
        {
            if ((!bomPartInfoList?.Exists(x => x.Barcode == childSns[i] && !x.IsMainSn)) ?? true)
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6006;
                return List_Result;
            }

            if (bomPartInfoList.Exists(x => x.IsMainSn && childSns[i] == x.Barcode))
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6010;
                return List_Result;
            }
        }

        //查询最后一个子条码是否未扫过
        if (bomPartInfoList.Exists(x => x.Barcode == LastChildSn))
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6004;
            return List_Result;
        }
        //查找主条码是否已经扫描过
        if (!bomPartInfoList.Exists(x => x.IsMainSn && x.Barcode == MainSN))
        {
            assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6007;
            return List_Result;
        }

        //检查供应商代码
        if (mInitPageInfo.stationAttribute.IsCheckVendor == "1")
        {
            if (await Public_Repository.uspCheckVendorAsync(MainSN, ChildSN, S_POID, S_PartID, List_Login))
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6009;
                return List_Result;
            }
        }
        bool regexRes = false;
        //默认第0个是主条码
        for (int i = 1; i < bomPartInfoList.Count; i++)
        {
            BomPartInfo tmpBomPartInfo = bomPartInfoList[i];
            if (tmpBomPartInfo.IsMainSn)
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_6014;
                return List_Result;
            }

            if (!string.IsNullOrEmpty(tmpBomPartInfo.Barcode))
            {
                continue;
            }

            if (tmpBomPartInfo.ScanType == 1)
            {
                var tmpCI = await Public_Repository.GetmesUnitSerialNumber(LastChildSn);
                if (tmpCI.Count <= 0)
                {
                    continue;
                }
            }

            ScanType scan = (ScanType)tmpBomPartInfo.ScanType;
            switch (scan)
            {
                case ScanType.设备数据_绑定过批次_类似托盘3:
                case ScanType.设备数据_无批次_类似夹具4:
                case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                    int tmpPartId7 = await Public_Repository.GetPartIdByMachineSNAsync(tmpBomPartInfo.Barcode);
                    if (tmpPartId7 != tmpBomPartInfo.PartID)
                        continue;
                    break;
                case ScanType.系统注册批次_Batch校验数量7:
                    mesMaterialUnit tmpMesMaterialUnit = await Public_Repository.GetMaterialUnitBySnAsync(tmpBomPartInfo.Barcode);
                    if (tmpMesMaterialUnit?.PartID != tmpBomPartInfo.PartID)
                        continue;
                    break;
                default:
                    regexRes = Regex.IsMatch(LastChildSn, tmpBomPartInfo.Pattern.Replace("\\\\", "\\"));
                    break;
            }

            bool isEndChildSN = bomPartInfoList.Count - 1 == i;

            if (!regexRes && !isEndChildSN)
            {
                continue;
            }

            if (!regexRes && isEndChildSN)
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_029;
                return List_Result;
            }

            var checkRes = await CheckInputBarcodeAsync(0, MainSN, LastChildSn, tmpBomPartInfo.Pattern, tmpBomPartInfo.ScanType, tmpBomPartInfo.PartID.ToString(), S_POID, S_PartID);
            if (checkRes!= "1")
            {
                assemblyTwoInputChildSnVerifyDto.ErrorMsg = msgSys.GetLanguage( checkRes);
                return List_Result;
            }
            bool isFinalChildSN = bomPartInfoList.Where(x => string.IsNullOrEmpty(x.Barcode)).Count() == 1;
            bomPartInfoList[i].Barcode = LastChildSn;
            if (isFinalChildSN)
            {
                //提交数据
                var subRes = await SubmitDataAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL, MainSN, ChildSN, bomPartInfoList);
                if (subRes.Item1 == "OK")
                {
                    cacheHelper.Remove(bomPartInfoKey);
                }
                else
                {
                    assemblyTwoInputChildSnVerifyDto.ErrorMsg = subRes.Item2;
                }                
                return List_Result;
            }
            else
            {
                cacheHelper.Replace(bomPartInfoKey, bomPartInfoList, new TimeSpan(12, 0, 0), true);
                return List_Result;
            }
        }
        assemblyTwoInputChildSnVerifyDto.ErrorMsg = P_MSG_Public.MSG_Public_029;
        return List_Result;
    }

    
    /// <summary>
    /// 主条码和子条码公共检查部分
    /// </summary>
    /// <param name="MainSN"></param>
    /// <param name="S_PartID"></param>
    /// <param name="S_POID"></param>
    /// <param name="S_PartFamilyID"></param>
    /// <param name="S_PartFamilyTypeID"></param>
    /// <returns></returns>
    private async Task<(bool, string)> VerifyMianCodeAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus,
        string S_URL, string MainSN, List<BomPartInfo> bomPartInfoList, string sChildSns = "")
    {
        List<dynamic> List_Result = new List<dynamic>();
        if (string.IsNullOrEmpty(MainSN))
        {
            return (false, P_MSG_Public.MSG_Public_016);
        }
        List<BomPartInfo> tmpBomPartInfo = bomPartInfoList ?? new List<BomPartInfo>();

        try
        {
            tmpBomPartInfo = await Public_Repository.MESGetBomPartInfoCAsync(int.Parse(S_PartID), List_Login.StationTypeID ?? 0);

            string tmpPartId = string.Empty;
            if (mInitPageInfo.stationAttribute.IsCheckPN == "0" || mInitPageInfo.stationAttribute.IsCheckPO == "0")
            {
                var BaseDataList = await Public_Repository.uspGetBaseDataAsync(MainSN);

                if (BaseDataList?.Item1 != "1" || BaseDataList?.Item2?.Count <= 0)
                {
                    return (false, P_MSG_Public.MSG_Public_018);
                }

                S_POID = BaseDataList.Item2.ConvertDynamic("ProductionOrderID");
                tmpPartId = BaseDataList.Item2.ConvertDynamic("PartID");
                S_PartFamilyID = BaseDataList.Item2.ConvertDynamic("PartFamilyID");
                S_PartFamilyTypeID = BaseDataList.Item2.ConvertDynamic("PartFamilyTypeID");
            }

            if (mInitPageInfo.stationAttribute.IsCheckPO == "0" && mInitPageInfo.stationAttribute.IsCheckPN == "1")
            {
                if (tmpPartId != S_PartID)
                {
                    return (false, P_MSG_Public.MSG_Public_6001);
                }
            }

            var ldPoNumberCheck = await Public_Repository.uspPONumberCheckAsync(S_POID);

            if (ldPoNumberCheck == null || ldPoNumberCheck.Item1 != "1")
            {
                return (false, P_MSG_Public.MSG_Public_6002);
            }

            ScanType scan = (ScanType)tmpBomPartInfo[0].ScanType;
            switch (scan)
            {
                case ScanType.设备数据_绑定过批次_类似托盘3:
                case ScanType.设备数据_无批次_类似夹具4:
                case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                    break;
                default:
                    Console.WriteLine(tmpBomPartInfo[0].Pattern);
                    if (!Regex.IsMatch(MainSN, tmpBomPartInfo[0].Pattern.Replace("\\\\", "\\")))
                    {
                        return (false, P_MSG_Public.MSG_Public_029);
                    }
                    break;
            }

            var checkRes = await CheckInputBarcodeAsync(1, MainSN, "", tmpBomPartInfo[0].Pattern, tmpBomPartInfo[0].ScanType,
                tmpBomPartInfo[0].PartID.ToString(), S_POID, S_PartID);

            if (checkRes?.ToString() != "1")
            {
                //获取异常代码，返回错误，并置空状态
                return (false, msgSys.GetLanguage(checkRes.ToString()));
            }

            if (string.IsNullOrEmpty(sChildSns))
            {
                if (string.IsNullOrEmpty(mInitPageInfo.stationAttribute.SNScanType))
                {
                    mInitPageInfo.stationAttribute.SNScanType = tmpBomPartInfo[0].ScanType.ToString();
                }

                bomPartInfoKey = GetBomPartInfoKey() + "_" + MainSN.Trim();
                tmpBomPartInfo[0].Barcode = MainSN;
                tmpBomPartInfo[0].IsMainSn = true;
                cacheHelper.Add(bomPartInfoKey, tmpBomPartInfo, new TimeSpan(12, 0, 0), true);
            }
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
            return (false, "exception");
        }
        return (true, null);
    }
    /// <summary>
    /// 最终提交所有数据
    /// </summary>
    /// <param name="S_PartFamilyTypeID"></param>
    /// <param name="S_PartFamilyID"></param>
    /// <param name="S_PartID"></param>
    /// <param name="S_POID"></param>
    /// <param name="S_UnitStatus"></param>
    /// <param name="S_URL"></param>
    /// <param name="MainSN"></param>
    /// <param name="ChildSN"></param>
    /// <param name="bomPartInfoList"></param>
    /// <returns></returns>
    private async Task<(string, string)> SubmitDataAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, string ChildSN, List<BomPartInfo> bomPartInfoList)
    {

        List<mesUnit> List_mesUnit = new List<mesUnit>();
        List<mesHistory> List_mesHistory = new List<mesHistory>();
        List<mesUnitComponent> List_mesUnitComponent = new List<mesUnitComponent>();

        List<mesMaterialConsumeInfo> List_mesMaterialConsumeInfo = new List<mesMaterialConsumeInfo>();
        List<mesMachine> List_mesMachine = new List<mesMachine>();
        string res = "0";
        //再次验证主条码
        var vmcRes = await VerifyMianCodeAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL, MainSN, bomPartInfoList, ChildSN);
        if (!vmcRes.Item1)
            return (res,vmcRes.Item2);

        string nextUnitState = await Public_Repository.GetMesUnitState(S_PartID, S_PartFamilyID,  List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0,
            S_POID, S_UnitStatus);

        if (string.IsNullOrEmpty(nextUnitState))
            return (res, msgSys.GetLanguage("20203"));

        List<mesPartDetail> listPartDetail = await Public_Repository.GetmesPartDetail(Convert.ToInt32(S_PartID), "SN_Pattern");
        if (listPartDetail != null && listPartDetail.Count > 0)
        {
            SN_Pattern = listPartDetail[0].Content.ToString().Trim();
        }

        string msg = string.Empty;
        int MainUnitID = 0;

        try
        {
            for (int i = 0; i < bomPartInfoList.Count; i++)
            {
                var tmpBomPartEntity = bomPartInfoList[i];
                int tmpScanType = tmpBomPartEntity.ScanType;

                string Batch = "";
                if (tmpBomPartEntity.ScanType == 6)
                {
                    var tmpMac = await Public_Repository.MesGetBatchIDByBarcodeSNAsync(tmpBomPartEntity.Barcode);
                    tmpScanType = tmpMac == null || tmpMac.Count <= 0 ? 4 : 3;
                }
                ScanType st = (ScanType)tmpScanType;
                if (i == 0)
                {
                    //获取main sn unit id
                    if (st == ScanType.设备数据_无批次_类似夹具4)
                    {
                        var mVirtualBarCode4 = await VirtualBarCodeAsync(tmpBomPartEntity.Barcode, S_PartID, S_PartFamilyID,
                            S_PartFamilyTypeID, S_UnitStatus, S_POID);

                        if (string.IsNullOrEmpty(mVirtualBarCode4.Item1))
                        {
                            return (res, mVirtualBarCode4.Item2);
                        }

                        MainUnitID = int.Parse(mVirtualBarCode4.Item1);

                    }
                    else
                    {
                        if (tmpScanType == 3)
                        {
                            var tmpMachine3 = await Public_Repository.BoxSnToBatch(tmpBomPartEntity.Barcode);
                            if (tmpMachine3.Item1 == "")
                            {
                                return (res, msgSys.GetLanguage("20030"));
                            }

                            Batch = tmpMachine3.Item1;
                            MainUnitID = tmpMachine3.Item2.UnitID;
                        }
                        else
                        {
                            var tUnitId = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                            if (tUnitId?.Count <= 0)
                            {
                                return (res, P_MSG_Public.MSG_Public_6012);
                            }
                            MainUnitID = tUnitId[0].UnitID;
                        }
                        mesUnit F_mesUnit = new mesUnit();
                        F_mesUnit.ID = MainUnitID;
                        F_mesUnit.UnitStateID = Convert.ToInt32(nextUnitState);
                        F_mesUnit.StatusID = 1;
                        F_mesUnit.StationID = List_Login.StationID;
                        F_mesUnit.EmployeeID = List_Login.EmployeeID;
                        F_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                        List_mesUnit.Add(F_mesUnit);
                    }

                    var mesUnit_Part = await Public_Repository.GetmesUnit(MainUnitID.ToString());
                    string S_POPartID = mesUnit_Part[0].PartID.ToString();
                    //string S_POPartID = DT_ProductionOrder.Rows[0]["PartID"].ToString();

                    mesHistory F_mesHistory = new mesHistory();
                    F_mesHistory.UnitID = MainUnitID;
                    F_mesHistory.UnitStateID = Convert.ToInt32(nextUnitState);
                    F_mesHistory.EmployeeID = List_Login.EmployeeID;
                    F_mesHistory.StationID = List_Login.StationID;
                    F_mesHistory.ProductionOrderID = Convert.ToInt32(S_POID);
                    F_mesHistory.PartID = Convert.ToInt32(S_POPartID);
                    F_mesHistory.LooperCount = 1;
                    F_mesHistory.StatusID = 1;
                    F_mesHistory.EnterTime = DateTime.Now;

                    List_mesHistory.Add(F_mesHistory);
                    msg = msg = "Mian Code:" + tmpBomPartEntity.Barcode;
                }
                else
                {
                    mesUnitComponent v_mesUnitComponent = new mesUnitComponent();
                    switch (st)
                    {
                        case ScanType.非系统注册批次_Batch校验数量_不校验唯一性2:
                        case ScanType.非系统注册SN_校验唯一性5:
                        case ScanType.系统注册批次_Batch校验数量7:
                            v_mesUnitComponent.UnitID = MainUnitID;
                            v_mesUnitComponent.UnitComponentTypeID = 1;
                            v_mesUnitComponent.ChildUnitID = 0;
                            if (st == ScanType.系统注册批次_Batch校验数量7)
                            {
                                var tChildUnitId = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                                if (tChildUnitId?.Count <= 0)
                                {
                                    return (res, P_MSG_Public.MSG_Public_6011);
                                }

                                v_mesUnitComponent.ChildUnitID = tChildUnitId[0].UnitID;
                            }

                            if (st == ScanType.非系统注册批次_Batch校验数量_不校验唯一性2)
                            {
                                v_mesUnitComponent.ChildSerialNumber = "";
                                v_mesUnitComponent.ChildLotNumber = tmpBomPartEntity.Barcode;
                            }
                            else
                            {
                                v_mesUnitComponent.ChildSerialNumber = tmpBomPartEntity.Barcode;
                                v_mesUnitComponent.ChildLotNumber = "";
                            }

                            var tPartFamilys = await Public_Repository.MesGetPart(tmpBomPartEntity.PartID.ToString(), "");
                            if (tPartFamilys?.Count() <= 0)
                            {
                                return (res, P_MSG_Public.MSG_Public_6013);
                            }
                            v_mesUnitComponent.ChildPartID = tmpBomPartEntity.PartID;
                            v_mesUnitComponent.ChildPartFamilyID = tPartFamilys.ToList()[0].PartFamilyID;
                            v_mesUnitComponent.Position = "";
                            v_mesUnitComponent.InsertedEmployeeID = List_Login.EmployeeID;
                            v_mesUnitComponent.InsertedStationID = List_Login.StationID;
                            v_mesUnitComponent.StatusID = 1;
                            break;
                        default:
                            int ChildUnitID;
                            if (st == ScanType.设备数据_绑定过批次_类似托盘3)
                            {
                                var tmpcMachine3 = await Public_Repository.BoxSnToBatch(tmpBomPartEntity.Barcode);
                                if (tmpcMachine3.Item1 == "")
                                {
                                    return (res, msgSys.GetLanguage("20030"));
                                }

                                Batch = tmpcMachine3.Item1;
                                ChildUnitID = tmpcMachine3.Item2.UnitID;
                            }
                            else if (st == ScanType.设备数据_无批次_类似夹具4)
                            {
                                var tChildPos = await Public_Repository.GetPO(tmpBomPartEntity.PartID.ToString(), "");
                                string S_TollPOID = tChildPos?.Count <= 0 ? "0" : tChildPos.ConvertDynamic("ID");
                                var cVirtualBarCode4 = await VirtualBarCodeAsync(tmpBomPartEntity.Barcode, S_PartID, S_PartFamilyID,
                                    S_PartFamilyTypeID, S_UnitStatus, S_POID);

                                if (string.IsNullOrEmpty(cVirtualBarCode4.Item1))
                                {
                                    return (res, cVirtualBarCode4.Item2);
                                }

                                ChildUnitID = int.Parse(cVirtualBarCode4.Item1);
                            }
                            else
                            {
                                var ctUnitid = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                                if (ctUnitid?.Count <= 0)
                                {
                                    return (res, P_MSG_Public.MSG_Public_6012);
                                }

                                ChildUnitID = ctUnitid[0].UnitID;
                            }

                            var tComponents = await Public_Repository.GetComponent(ChildUnitID);
                            if (tComponents == null || tComponents.Count <= 0)
                            {
                                return (res, msgSys.GetLanguage("20032"));
                            }
                            string S_ChildSerialNumber = tComponents.ConvertDynamic("Value").ToString();
                            int ChildPartFamilyID = Convert.ToInt32(tComponents.ConvertDynamic("PartFamilyID").ToString());

                            v_mesUnitComponent.UnitID = MainUnitID;
                            v_mesUnitComponent.UnitComponentTypeID = 1;
                            v_mesUnitComponent.ChildUnitID = Convert.ToInt32(ChildUnitID);
                            v_mesUnitComponent.ChildSerialNumber = S_ChildSerialNumber;
                            v_mesUnitComponent.ChildLotNumber = Batch;
                            v_mesUnitComponent.ChildPartID = Convert.ToInt32(tmpBomPartEntity.PartID);
                            v_mesUnitComponent.ChildPartFamilyID = ChildPartFamilyID;
                            v_mesUnitComponent.Position = "";
                            v_mesUnitComponent.InsertedEmployeeID = List_Login.EmployeeID;
                            v_mesUnitComponent.InsertedStationID = List_Login.StationID;
                            v_mesUnitComponent.StatusID = 1;
                            break;
                    }

                    List_mesUnitComponent.Add(v_mesUnitComponent);

                    msg = msg + " Child barcode:" + tmpBomPartEntity.Barcode;
                }

                if (st == ScanType.设备数据_绑定过批次_类似托盘3 ||
                    st == ScanType.设备数据_无批次_类似夹具4 ||
                    st == ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6)
                {
                    mesMachine v_mesMachine = new mesMachine();
                    v_mesMachine.SN = tmpBomPartEntity.Barcode;
                    List_mesMachine.Add(v_mesMachine);
                }

                if (st == ScanType.非系统注册批次_Batch校验数量_不校验唯一性2 ||
                    st == ScanType.设备数据_绑定过批次_类似托盘3 ||
                    st == ScanType.系统注册批次_Batch校验数量7)
                {
                    string SN = string.Empty;
                    string MachineSN = string.Empty;
                    int BatchType = 1;
                    if (st == ScanType.设备数据_绑定过批次_类似托盘3)
                    {
                        MachineSN = tmpBomPartEntity.Barcode;
                        BatchType = 2;
                    }
                    else
                    {
                        SN = tmpBomPartEntity.Barcode;

                    }

                    mesMaterialConsumeInfo v_mesMaterialConsumeInfo = new mesMaterialConsumeInfo();
                    v_mesMaterialConsumeInfo.ScanType = BatchType;
                    v_mesMaterialConsumeInfo.SN = SN;
                    v_mesMaterialConsumeInfo.MachineSN = MachineSN;
                    v_mesMaterialConsumeInfo.PartID = Convert.ToInt32(S_PartID);
                    v_mesMaterialConsumeInfo.ProductionOrderID = Convert.ToInt32(S_POID);

                    List_mesMaterialConsumeInfo.Add(v_mesMaterialConsumeInfo);

                }
            }
            res = await DataCommit_Repository.SubmitDataUHCAsync(List_mesUnit, List_mesHistory, List_mesUnitComponent, List_mesMaterialConsumeInfo,
               List_mesMachine, List_Login);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
        }
        if (res != "OK")
        {
            return (res, "update database failed.");
        }

        return (res, "update database success.");
    }
    

    /// <summary>
    ///虚拟条码检查，没有则生成
    /// </summary>
    /// <param name="S_SN"></param>
    /// <param name="S_PartID"></param>
    /// <param name="S_PartFamilyID"></param>
    /// <param name="S_PartFamilyTypeID"></param>
    /// <param name="S_UnitStateID"></param>
    /// <param name="S_POID"></param>
    /// <returns></returns>
    private async Task<(string, string)> VirtualBarCodeAsync(string S_SN, string S_PartID, string S_PartFamilyID, string S_PartFamilyTypeID, string S_UnitStateID, string S_POID)
    {
        try
        {
            string S_InsertUnit = string.Empty;
            //判断是否已经生成了虚拟条码
            string S_FG_SN = Public_Repository.BuckToFGSN(S_SN);
            if (string.IsNullOrEmpty(S_FG_SN))
            {
                string S_FormatSN = await Public_Repository.mesGetSNFormatIDByListAsync(S_PartID, S_PartFamilyID, List_Login.LineID.ToString(), S_POID, List_Login.StationTypeID.ToString());
                if (string.IsNullOrEmpty(S_FormatSN))
                {
                    return ("", msgSys.GetLanguage("20034"));
                }

                string xmlProdOrder = "<ProdOrder ProductionOrder=" + "\"" + S_POID + "\"" + "> </ProdOrder>";
                string xmlStation = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
                string xmlPart = "<Part PartID=" + "\"" + S_PartID + "\"" + "> </Part>";
                string xmlExtraData = "<ExtraData BoxSN=" + "\"" + S_SN + "\"" +
                                               " LineID = " + "\"" + List_Login.LineID.ToString() + "\"" +
                                               " PartFamilyTypeID=" + "\"" + S_PartFamilyTypeID + "\"" +
                                               " LineType=" + "\"" + "M" + "\"" +
                                               " BB=" + "\"" + ColorCode + "\"" +
                                               " CCCC=" + "\"" + SpcaCode + "\"" +
                                               " PP=" + "\"" + PpCode.ToUpper() + "\"" +
                                               " B=" + "\"" + BuildCode + "\"" +
                                               " ET=" + "\"" + ColorCode + "\"" +
                                               " SPCA = " + "\"" + SpcaCode.ToUpper() + "\"" +
                                               "> </ExtraData>";

                string New_SN = await Public_Repository.GetSNRGetNextAsync(S_FormatSN, "0", xmlProdOrder, xmlPart,
                    xmlStation, xmlExtraData);

                if (!string.IsNullOrEmpty(SN_Pattern))
                {
                    if (!Regex.IsMatch(New_SN, SN_Pattern))
                    {
                        return ("", msgSys.GetLanguage("20034"));
                    }
                }

                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.UnitStateID = Convert.ToInt32(S_UnitStateID);
                v_mesUnit.StatusID = 1;
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.CreationTime = DateTime.Now;
                v_mesUnit.LastUpdate = DateTime.Now;
                v_mesUnit.PanelID = 0;
                v_mesUnit.LineID = List_Login.LineID;
                v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                v_mesUnit.RMAID = 0;
                v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                v_mesUnit.LooperCount = 1;

                mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
                v_mesUnitDetail.reserved_01 = S_SN;
                v_mesUnitDetail.reserved_02 = "";
                v_mesUnitDetail.reserved_03 = "1";
                v_mesUnitDetail.reserved_04 = "";
                v_mesUnitDetail.reserved_05 = "";
                //mesUnitDetailSVC.Insert(v_mesUnitDetail);

                mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                v_mesSerialNumber.SerialNumberTypeID = 5;
                v_mesSerialNumber.Value = New_SN;


                string ReturnValue = DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAddStr(
                    new List<mesUnit>() { v_mesUnit }, new List<mesUnitDetail>() { v_mesUnitDetail },
                    new List<mesSerialNumber>() { v_mesSerialNumber }, new List<mesHistory>() { null });

                if (ReturnValue.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    S_InsertUnit = "NG";
                    return ("", msgSys.MSG_Sys_20130);
                }
                else
                {
                    //ReturnValue = "SN:" + New_SN;
                }
                S_InsertUnit = ReturnValue;
            }
            else
            {
                string S_FirstStationType = await Public_Repository.GetFirstStationTypeAsync(S_SN);
                S_InsertUnit = "";
                if (S_FirstStationType == List_Login.StationTypeID.ToString())
                {
                    var ds = await Public_Repository.GetmesSerialNumber("", S_FG_SN);
                    S_InsertUnit = ds[0].UnitID.ToString();
                    mesUnit v_mesUnit = new mesUnit();
                    v_mesUnit.UnitStateID = Convert.ToInt32(S_UnitStateID);
                    v_mesUnit.StatusID = 1;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.CreationTime = DateTime.Now;
                    v_mesUnit.LastUpdate = DateTime.Now;
                    v_mesUnit.PanelID = 0;
                    v_mesUnit.LineID = List_Login.LineID;
                    v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                    v_mesUnit.LooperCount = 1;
                    v_mesUnit.ID = Convert.ToInt32(S_InsertUnit);
                    _dbContext.Update(v_mesUnit);
                }
                else
                {
                    return ("", msgSys.GetLanguage("20154"));
                }
            }
            return (S_InsertUnit, null);
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", ex);
            return ("", ex.Message);
        }
    }
    private List<dynamic> List_ERROR(string S_Error, LoginList List_Login, string S_SN, string S_DataStatus = "0")
        => Public_Repository?.List_ERROR(S_Error, S_DataStatus, List_Login, S_SN);
    private List<dynamic> List_ERROR(Exception ex, LoginList List_Login, string S_SN, string S_DataStatus = "0")
        => Public_Repository?.List_ERROR(ex, S_DataStatus, List_Login, S_SN);

    private List<dynamic> ListError(string code, string S_SN, string S_DataStatus = "0") =>
        Public_Repository?.List_ERROR(msgSys.GetLanguage(code), S_DataStatus, List_Login, S_SN);

    private async Task<string> CheckInputBarcodeAsync(int type, string MainBarcode, string ChildBarcode, string Pattern, int scanType, string currentPartID, string productOrderID, string mainPartID)
    {
        //var resulTuple = (Result: "1", Data: new List<dynamic>());
        string resultCode = "0";
        string xmlPartStr = "<Part PartID=\"" + currentPartID + "\"> </Part>";
        string allBarcode = MainBarcode + "_" + ChildBarcode;
        string newBarcode = string.Empty;
        string msg = string.Empty;
        ScanType st;

        try
        {
            st = (ScanType)scanType;
            switch (st)
            {
                case ScanType.系统存在数据_SN1:
                    resultCode = type == 1
                        ? await Public_Repository.MESAssembleCheckMianSNAsync(productOrderID, List_Login.LineID, List_Login.StationID, List_Login.StationTypeID ?? 0, MainBarcode, mInitPageInfo.stationAttribute.COF == "1") : await Public_Repository.MESAssembleCheckOtherSNAsync(ChildBarcode, currentPartID, mInitPageInfo.stationAttribute.COF == "1", List_Login);
                    break;
                case ScanType.非系统注册批次_Batch校验数量_不校验唯一性2:
                case ScanType.系统注册批次_Batch校验数量7:
#if IsSql
                    resultCode =
                        await Public_Repository.uspBatchDataCheckAsync(ChildBarcode, null, xmlPartStr, null, null, "1");
#else
                    List<TabVal> tab27 = await Public_Repository.uspCallProcedureAsync("uspBatchDataCheck", ChildBarcode, null, xmlPartStr, null, null, "1");
                    resultCode = tab27?[0]?.ValStr1 ?? "0";
#endif
                    break;
                case ScanType.设备数据_绑定过批次_类似托盘3:
                case ScanType.设备数据_无批次_类似夹具4:
                case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                    string res346 = string.Empty;
#if IsSql
                    res346 = await Public_Repository.GetMachineToolingCheck(ChildBarcode, MainBarcode, currentPartID, List_Login);
#else
                    string xmlExtraData = "<ExtraData MainCode=\"" + MainBarcode + "\"> </ExtraData>";
                    List<TabVal> tab346 = await Public_Repository.uspCallProcedureAsync("uspMachineToolingCheck", ChildBarcode, null, xmlPartStr, null, xmlExtraData, List_Login.StationTypeID?.ToString() ?? "0");

                    if (tab346?.Count <= 0)
                    {
                        resultCode = "0";
                        break;
                    }

                    res346 = tab346[0]?.ValStr1 ?? "0";
#endif
                    if (type == 1 && res346 == "1")
                    {
                        List<mesMachine> machines = await Public_Repository.GetmesMachineAsync(MainBarcode);
                        if (machines?.Count <= 0)
                        {
                            resultCode = "0";
                            break;
                        }
                        //是否夹具初始化工位
                        if (machines[0].ValidFrom != List_Login.StationTypeID.ToString())
                        {
                            //Tooling 转换BarCode
                            newBarcode = Public_Repository.BuckToFGSN(MainBarcode);
                            resultCode = await Public_Repository.MESAssembleCheckMianSNAsync(productOrderID, List_Login.LineID,
                                List_Login.StationID, List_Login.StationTypeID ?? 1, newBarcode, mInitPageInfo.stationAttribute.COF == "1");
                        }

                    }

                    if (scanType == 3 && resultCode == "1")
                    {
#if IsSql
                        resultCode =
                            await Public_Repository.uspBatchDataCheckAsync(ChildBarcode, null, xmlPartStr, null, null, "1");
#else
                        List<TabVal> tab3463 = await Public_Repository.uspCallProcedureAsync("uspBatchDataCheck", ChildBarcode, null, xmlPartStr, null, null, "1");
                        resultCode = tab3463?[0]?.ValStr1 ?? "0";
#endif
                    }
                    break;
                case ScanType.非系统注册SN_校验唯一性5:
                    bool ischeck = await Public_Repository.MESCheckChildSerialNumberAsync(ChildBarcode);
                    resultCode = ischeck ? "20029" : "1";
                    break;
                default:
                    break;
            }

            if (resultCode != "1")
            {
                if (resultCode == "20243")
                {
                    var tmpUnit = await Public_Repository.GetSerialNumber2Async(newBarcode);

                    string nowUnitStateID = tmpUnit?[0].UnitStateID.ToString();

                }

                return resultCode;
            }

            //var checkList = await Public_Repository.uspAssembleCheckAsync(String.IsNullOrEmpty(ChildBarcode) ? MainBarcode : ChildBarcode, productOrderID,mainPartID , List_Login, currentPartID);
            var checkList = await Public_Repository.uspAssembleCheck2Async(String.IsNullOrEmpty(ChildBarcode) ? MainBarcode : ChildBarcode, MainBarcode, productOrderID, mainPartID, List_Login, currentPartID);

            resultCode = checkList?.ToString();
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
        }
        return resultCode;
    }
    /// <summary>
    /// SetConfirmPOAsync 确认工单 
    /// </summary>
    /// <param name="S_PartFamilyTypeID"></param>
    /// <param name="S_PartFamilyID"></param>
    /// <param name="S_PartID"></param>
    /// <param name="S_POID"></param>
    /// <param name="S_UnitStatus"></param>
    /// <param name="List_Login"></param>
    /// <returns></returns>
    private async Task<List<dynamic>> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, LoginList List_Login)
    {
        string S_LineID = List_Login.LineID.ToString();

        S_PartFamilyTypeID = S_PartFamilyTypeID ?? "0";
        S_PartFamilyID = S_PartFamilyID ?? "0";
        S_PartID = S_PartID ?? "0";
        S_POID = S_POID ?? "0";
        S_UnitStatus = S_UnitStatus ?? "0";

        List<dynamic> List_Result = new List<dynamic>();
        //string S_Batch_Pattern = "";
        //string S_SN_Pattern = "";

        AssemblyTwoInput_SetConfirmPO_Output_Dto assemblyTwoInput_SetConfirmPO_Dto = new AssemblyTwoInput_SetConfirmPO_Output_Dto();

        try
        {
            if (S_POID == "0" && S_IsCheckPO == "1")
            {
                List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_009, "0", List_Login, "");//工单不能为空,请确认
                return List_Result;
            }
            else
            {
                if (S_IsCheckPN == "1")
                {
                    if (S_PartFamilyTypeID == "0")
                    {
                        List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_010, "0", List_Login, "");//未选择料号类别,请确认
                        return List_Result;
                    }
                    if (S_PartFamilyID == "0")
                    {
                        List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_011, "0", List_Login, "");//未选择料号群,请确认.
                        return List_Result;
                    }
                    if (S_PartID == "0")
                    {
                        List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_012, "0", List_Login, "");//未选择料号,请确认.
                        return List_Result;
                    }
                }

                if (S_IsCheckPO == "0")
                {
                    List<mesLineOrder> list_mesLineOrder = await Public_Repository.GetmesLineOrderAsync(List_Login.LineID.ToString(), S_PartID, "");
                    if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                    {
                        List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_024, "0", List_Login, "");//料号和线别不匹配.
                        return List_Result;
                    }
                }

                List<TabVal> List_Pattern = await Public_Repository.GetPatternAsync(S_PartID, List_Login);
                if (List_Pattern[0].ValStr2 == "0")
                {
                    List_Result = Public_Repository.List_ERROR(List_Pattern[0].ValStr1, "0", List_Login, "");
                    return List_Result;
                }


                // BOM
                List<mesProductStructure> List_mesProductStructure = await Public_Repository.GetBOMStructureAsync(S_PartID, "", "");
                assemblyTwoInput_SetConfirmPO_Dto.MesProductStructures = List_mesProductStructure;

                //Route
                List<mesRoute> List_Route = await Public_Repository.GetmesRouteAsync(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                                Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_POID));
                List_Route = List_Route ?? new List<mesRoute>();

                if (List_Route.Count == 0)
                {
                    List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_001, "0", List_Login, "");//料号未配置工艺流程路线.
                    return List_Result;
                }

                int I_RouteID = List_Route[0].ID;
                int I_RouteType = List_Route[0].RouteType;
                if (I_RouteType == 1)
                {
                    List<dynamic> List_RouteDataDiagram1 = await
                        Public_Repository.GetRouteDataDiagramAsync(List_Login.StationTypeID.ToString(), I_RouteID, "1");

                    assemblyTwoInput_SetConfirmPO_Dto.RouteDataDiagram1 = List_RouteDataDiagram1;
                    List<dynamic> List_RouteDataDiagram2 = await
                        Public_Repository.GetRouteDataDiagramAsync(List_Login.StationTypeID.ToString(), I_RouteID, "2");
                    assemblyTwoInput_SetConfirmPO_Dto.RouteDataDiagram2 = List_RouteDataDiagram2;
                    
                }
                else
                {
                    List<dynamic> List_RouteDetail = await
                        Public_Repository.GetRouteDetailAsync(S_LineID, S_PartID, S_PartFamilyID, S_POID);
                    assemblyTwoInput_SetConfirmPO_Dto.RouteDetail = List_RouteDetail;
                }
                //工单数量
                List<dynamic> List_ProductionOrderQTY = await
                    Public_Repository.GetProductionOrderQTYAsync(List_Login.StationID, Convert.ToInt32(S_POID));
                assemblyTwoInput_SetConfirmPO_Dto.ProductionOrderQTY = List_ProductionOrderQTY;

                List_Result.Add(assemblyTwoInput_SetConfirmPO_Dto);
            }
        }
        catch (Exception ex)
        {
            List_Result = Public_Repository.List_ERROR(ex, "0", List_Login, "");
        }

        return List_Result;
    }


}