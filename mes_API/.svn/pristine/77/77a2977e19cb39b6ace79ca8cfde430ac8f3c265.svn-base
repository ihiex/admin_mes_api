using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using static Dapper.SqlMapper;
using SunnyMES.Commons.Core.PublicFun;
using Quartz.Impl.Triggers;
using SunnyMES.Commons.Enums;
using API_MSG;
using SunnyMES.Security.MSGCode;
using NPOI.Util;
using System.Text.RegularExpressions;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.MES.Query;
using NPOI.SS.Formula.Functions;
using SunnyMES.Commons.Extend;

namespace SunnyMES.Security.Repositories
{
    public class ScanToolingPrintRepository : BaseRepositoryReport<string>, IScanToolingPrintRepository
    {

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        MSG_Public P_MSG_Public;
        MSG_Sys P_MSG_Sys;
        PublicMiniRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        private MSG_Sys msgSys;

        public ScanToolingPrintRepository()
        {
        }

        public ScanToolingPrintRepository(IDbContextCore dbContext) : base(dbContext)
        {
            DB_Context = dbContext;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            List_Login.LanguageID = I_Language;
            List_Login.LineID = I_LineID;
            List_Login.StationID = I_StationID;
            List_Login.EmployeeID = I_EmployeeID;
            List_Login.CurrentLoginIP = S_CurrentLoginIP;

            PublicMiniRepository v_Public_Repository = new PublicMiniRepository();
            List<TabVal> List_Station = await v_Public_Repository.MesGetStationNoTask(I_StationID.ToString(), "", "");
            List_Login.StationTypeID = List_Station[0].Valint1;
            List_Login.Station = List_Station[0].ValStr1;

            List<TabVal> List_StationType = await v_Public_Repository.MesGetStationTypeNoTask(List_Login.StationTypeID.ToString());
            List_Login.StationType = List_StationType[0].ValStr1;

            List<TabVal> List_StationConfig = await v_Public_Repository.GetStationConfigSetting("", List_Login.StationID.ToString());
            List<TabVal> List_StationTypeDetail = await v_Public_Repository.GetStationTypeDetail(List_Login.StationTypeID.ToString());

            string S_TTBoxSN_Pattern = "[\\s\\S]*";
            foreach (var item in List_StationTypeDetail)
            {
                if (item.ValStr2.Trim() == "TTBoxSN_Pattern")
                {
                    S_TTBoxSN_Pattern = item.ValStr1.Trim();

                    foreach (var item_StationConfig in List_StationConfig)
                    {
                        if (item_StationConfig.ValStr1 == "TTBoxSN_Pattern")
                        {
                            S_TTBoxSN_Pattern = item_StationConfig.ValStr2.Trim();
                            continue;
                        }
                    }
                }
            }
            List_Login.TTBoxSN_Pattern = S_TTBoxSN_Pattern;

            string S_IsTTBoxUnpack = "";
            foreach (var item in List_StationTypeDetail)
            {
                if (item.ValStr2.Trim() == "IsTTBoxUnpack")
                {
                    S_IsTTBoxUnpack = item.ValStr1.Trim();

                    foreach (var item_StationConfig in List_StationConfig)
                    {
                        if (item_StationConfig.ValStr1 == "IsTTBoxUnpack")
                        {
                            S_IsTTBoxUnpack = item_StationConfig.ValStr2.Trim();
                            continue;
                        }
                    }
                }
            }
            List_Login.IsTTBoxUnpack = S_IsTTBoxUnpack;


            foreach (var item_StationConfig in List_StationConfig)
            {
                if (item_StationConfig.ValStr1 == "PrintIPPort")
                {
                    List_Login.PrintIPPort = item_StationConfig.ValStr2.Trim();
                    continue;
                }
            }

            List_Login.PrintQTY = 90;
            foreach (var item_StationConfig in List_StationConfig)
            {
                if (item_StationConfig.ValStr1 == "PrintQTY")
                {
                    List_Login.PrintQTY = Convert.ToInt32(item_StationConfig.ValStr2.Trim());
                    continue;
                }
            }


            if (P_MSG_Public == null)
            {
                P_MSG_Public = new MSG_Public(I_Language);
            }
            if (P_MSG_Sys == null)
            {
                P_MSG_Sys = new MSG_Sys(I_Language);
            }

            if (Public_Repository == null)
            {
                Public_Repository = new PublicMiniRepository(DB_Context, I_Language);
            }

            if (DataCommit_Repository == null)
            {
                DataCommit_Repository = new DataCommitRepository(DB_Context, I_Language);
            }

            if (SNFormat_Repository == null)
            {
                SNFormat_Repository = new SNFormatRepository();
            }

            msgSys ??= new MSG_Sys(I_Language);

            string S_Sql = "select 'ok' as ValStr1,'' ValStr2,'1' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            IEnumerable<dynamic> List_Result = null;

            string S_Sql = "SELECT B.[Description] ValStr1 FROM mesStationType A JOIN luApplicationType B ON " +
                " A.ApplicationTypeID=B.ID WHERE A.ID='" + List_Login.StationTypeID.ToString() + "' ";
            var v_ApplicationType = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            S_ApplicationType = v_ApplicationType.AsList()[0].ValStr1;

            string S_IsLegalPage = "0";
            S_Sql =
              @"SELECT EnCode AS ValStr1,FullName AS ValStr2 FROM API_Menu  WHERE EnCode='" +
                    S_URL + "' AND FullName='" + S_ApplicationType + "'";
            var v_Query_Menu = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query_Menu != null && v_Query_Menu.Count() > 0)
            {
                S_IsLegalPage = "1";
            }

            S_IsCheckPO = "1";
            S_IsCheckPN = "1";
            S_TTScanType = "";
            S_IsTTRegistSN = "";
            S_TTBoxType = "";

            S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                    "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                    "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                    "' IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                    List_Login.PrintIPPort + "' as PrintIPPort, '1' as IsPrint";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            List_Result = v_Query;

            return List_Result;
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            ConfirmPOOutputDto F_ConfirmPOOutputDto = new ConfirmPOOutputDto();
            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;

            string S_LabelPath = "";

            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);


            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);

            // 获取条码生成格式
            string S_LineNumber = List_Login.LineID.ToString();
            //string S_SNFormatName = await Public_Repository.GetSNFormatName(
            //    S_LineNumber, S_PartID, S_PartFamilyID, S_POID, List_Login.StationTypeID.ToString());
            //if (S_SNFormatName != "")
            //{
            //    S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
            //        S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
            //    if (string.IsNullOrEmpty(S_LabelPath))
            //    {
            //        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_037, "0", List_Login, "");//未配置打印文件路径
            //        F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
            //        return F_ConfirmPOOutputDto;
            //    }
            //}
            //else
            //{
            //    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_038, "0", List_Login, "");//料号未关联生成SN的格式
            //    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
            //    return F_ConfirmPOOutputDto;
            //}


            S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
            if (string.IsNullOrEmpty(S_LabelPath))
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_037, "0", List_Login, "");//未配置打印文件路径
                F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputDto;
            }

            List_SetConfirmPO.LabelPath = S_LabelPath;
            //List_SetConfirmPO.SNFormatName = S_SNFormatName;

            return List_SetConfirmPO;
        }


        public async Task<CreateSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, ConfirmPOOutputDto v_ConfirmPOOutputDto)
        {
            S_SN = S_SN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";


            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            List<string> ListSN = new List<string>();

            CreateSNOutputDto F_CreateSNOutputDto = new CreateSNOutputDto();
            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
            F_CreateSNOutputDto.LabelPath = v_ConfirmPOOutputDto.LabelPath;

            List<mesUnitSerialNumber> List_mesUnitSerialNumber = null;
            string S_ToolingSN = S_SN.Trim();
            string S_ReturnValue = "OK";
            try
            {
                if (string.IsNullOrEmpty(S_SN))
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20007, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                List<TabVal> List_SN = await Public_Repository.MesGetBatchIDByBarcodeSN(S_ToolingSN);
                if (List_SN != null && List_SN.Count > 0)
                {
                    //过滤设备机器条码
                    List_mesUnitSerialNumber = await Public_Repository.GetmesUnitSerialNumber(List_SN[0].ValStr2,"");
                    
                    if (List_mesUnitSerialNumber == null || List_mesUnitSerialNumber.Count < 1) 
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20242, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    //tmpScanType = List_mesSerialNumber[0].SerialNumberTypeID.ToString() == "9" ? "3" : "-2";
                }
                else
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20054, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                string S_FG_SN = List_mesUnitSerialNumber[0].Value;
                string S_FGPOID = List_mesUnitSerialNumber[0].ProductionOrderID.ToString();
                string S_UnitID = List_mesUnitSerialNumber[0].UnitID.ToString();
              
                if (S_FGPOID == "" || S_FGPOID == "0") 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20014, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (S_FGPOID !=S_POID)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20015, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                //获取此料工序路径
                List<mesRoute> List_Route = await Public_Repository.GetmesRoute(Convert.ToInt32(List_Login.LineID), Convert.ToInt32(S_PartID),
                                Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_POID));
                List_Route = List_Route ?? new List<mesRoute>();

                if (List_Route.Count == 0)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_001, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                List<mesUnit> List_mesUnit_Get = await Public_Repository.GetCheckUnitInfo(S_FG_SN, S_POID, S_PartID);
                string S_RouteCheck = await Public_Repository.GetRouteCheck(Convert.ToInt32(List_Login.StationTypeID),
                    List_Login.StationID, List_Login.LineID.ToString(), List_mesUnit_Get[0], S_FG_SN);
                if (S_RouteCheck != "1") 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                List<IdDescription> List_mesUnitState = new List<IdDescription>();
                int I_PartID = Convert.ToInt32(S_PartID);
                int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                int I_LineID = List_Login.LineID;
                int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                int I_ProductionOrderID = Convert.ToInt32(S_POID);
                int I_StatusID = 1;

                List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                    I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);
                if (List_mesUnitState == null)
                {
                    //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_FG_SN);
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                int I_UnitStateID = List_mesUnitState[0].ID;


                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.ID = Convert.ToInt32(S_UnitID);
                v_mesUnit.UnitStateID = I_UnitStateID;
                v_mesUnit.StatusID = 1;
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.LastUpdate = DateTime.Now;
                v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);



                mesHistory v_mesHistory = new mesHistory();

                v_mesHistory.UnitID = v_mesUnit.ID;
                v_mesHistory.UnitStateID = I_UnitStateID;//Convert.ToInt32(v_ConfirmPOOutputDto.PrintUnitStateID);
                v_mesHistory.EmployeeID = List_Login.EmployeeID;
                v_mesHistory.StationID = List_Login.StationID;

                v_mesHistory.ProductionOrderID = Convert.ToInt32(S_POID);
                v_mesHistory.PartID = Convert.ToInt32(S_PartID);
                v_mesHistory.LooperCount = 1;

                S_ReturnValue = await DataCommit_Repository.SubmitData_ToolingPrint(v_mesUnit,v_mesHistory);

                ListSN.Add(S_FG_SN);
                F_CreateSNOutputDto.ListSN = ListSN;
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                return F_CreateSNOutputDto;
            }
            return F_CreateSNOutputDto;
        }




    }
}
