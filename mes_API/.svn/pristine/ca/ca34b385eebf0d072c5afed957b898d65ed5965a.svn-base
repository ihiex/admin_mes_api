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

namespace SunnyMES.Security.Repositories
{
    public class RePrintRepository : BaseRepositoryReport<string>, IRePrintRepository
    {

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        MSG_Public P_MSG_Public;
        MSG_Sys P_MSG_Sys;
        PublicMiniRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        private MSG_Sys msgSys;

        public RePrintRepository()
        {
        }

        public RePrintRepository(IDbContextCore dbContext) : base(dbContext)
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

            PublicRepository v_Public_Repository = new PublicRepository();
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

            List<IsCheckPOPN> List_IsCheckPOPN = await Public_Repository.GetPageInitialize(List_Login, S_URL);
            if (List_IsCheckPOPN.Count > 0)
            {
                if (List_IsCheckPOPN[0].TabVal == null)
                {
                    S_IsCheckPO = List_IsCheckPOPN[0].IsCheckPO;
                    S_IsCheckPN = List_IsCheckPOPN[0].IsCheckPN;
                }
            }

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
            string S_PartID, string S_POID, string S_UnitStatus, string Type, string S_URL)
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
            //string S_LineNumber = "";//List_Login.LineID.ToString();
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

            if (Type == "FG")
            {
                S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                    S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
                if (string.IsNullOrEmpty(S_LabelPath))
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_037, "0", List_Login, "");//未配置打印文件路径
                    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputDto;
                }
            }


            if (Type == "UPC") 
            {
                List<TabVal> List_StationConfig = await Public_Repository.GetStationConfigSetting("", List_Login.StationID.ToString());

                List_SetConfirmPO.List_PrintUPCUnitStateID = null;
                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "PrintUPCUnitStateID")
                    {
                        string S_SatationValue = item_StationConfig.ValStr2.Trim();
                        List_SetConfirmPO.List_PrintUPCUnitStateID = S_SatationValue.Split(',');
                        continue;
                    }
                }

                List_SetConfirmPO.PrintUnitStateID = "";
                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "PrintUnitStateID")
                    {
                        string S_SatationValue = item_StationConfig.ValStr2.Trim();
                        List_SetConfirmPO.PrintUnitStateID = S_SatationValue;
                        continue;
                    }
                }

                List_SetConfirmPO.PrintStationTypeID = "";
                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "PrintStationTypeID")
                    {
                        string S_SatationValue = item_StationConfig.ValStr2.Trim();
                        List_SetConfirmPO.PrintStationTypeID = S_SatationValue;
                        continue;
                    }
                }

                S_LabelPath = await Public_Repository.GetLabelName(List_SetConfirmPO.PrintStationTypeID,
                    S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
                if (string.IsNullOrEmpty(S_LabelPath))
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_037, "0", List_Login, "");//未配置打印文件路径
                    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputDto;
                }
            }

            List_SetConfirmPO.LabelPath = S_LabelPath;
            //List_SetConfirmPO.SNFormatName = S_SNFormatName;
            return List_SetConfirmPO;
        }


        public async Task<CreateSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string Type, ConfirmPOOutputDto v_ConfirmPOOutputDto)
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

            string S_ReturnValue = "OK";

            try
            {
                if (string.IsNullOrEmpty(S_SN))
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20007, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

                    return F_CreateSNOutputDto;
                }

                List<SNBaseData> List_SNBaseData = await Public_Repository.GetSNBaseData(S_SN);
                if (List_SNBaseData == null || List_SNBaseData.Count < 1)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_018, "0", List_Login, "");//条码不存在或者状态不符.
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (S_IsCheckPN == "0")
                {
                    S_PartID = List_SNBaseData[0].PartID.ToString();
                    S_PartFamilyID = List_SNBaseData[0].PartFamilyID.ToString();
                }
                if (S_IsCheckPO == "0")
                {
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();

                    List<mesLineOrder> list_mesLineOrder = await Public_Repository.GetmesLineOrder(List_Login.LineID.ToString(), S_POID);
                    if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_024, "0", List_Login, S_SN);//料号和线别不匹配.
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                }

                List<mesUnit> List_mesUnit_Get = await Public_Repository.GetCheckUnitInfo(S_SN, S_POID, S_PartID);
                if (List_mesUnit_Get.Count < 1)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_019, "0", List_Login, S_SN);//此条码和选择的工单不一致.
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }


                List<mesUnitSerialNumber> List_Unit = await Public_Repository.GetmesUnitSerialNumber(S_SN);
                if (Type == "FG")
                {
                    ListSN.Add(S_SN);

                    mesHistory v_mesHistory = new mesHistory();
                    v_mesHistory.UnitID = List_Unit[0].UnitID;
                    v_mesHistory.UnitStateID = -200;
                    v_mesHistory.EmployeeID = List_Login.EmployeeID;
                    v_mesHistory.StationID = List_Login.StationID;
                    v_mesHistory.EnterTime = DateTime.Now;
                    v_mesHistory.ExitTime = DateTime.Now;
                    v_mesHistory.ProductionOrderID = Convert.ToInt32(S_POID);
                    v_mesHistory.PartID = Convert.ToInt32(S_PartID);
                    v_mesHistory.StatusID = 1;
                    S_ReturnValue = await DataCommit_Repository.SubmitData_DataOne(v_mesHistory);
                }
                else if (Type == "UPC")
                {
                    string UPCSN = string.Empty;
                    string UPCID = string.Empty;
                    string UPCProductionOrderID = string.Empty;
                    string UPCPartID = string.Empty;

                    if (List_SNBaseData[0].SerialNumberTypeID == "6")
                    {
                        //转换FG条码检测
                        string FGSN = await Public_Repository.MesGetFGSNByUPCSNAsync(S_SN);
                        if (string.IsNullOrEmpty(FGSN))
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20189, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }

                        UPCSN = S_SN;
                        UPCID = List_Unit[0].UnitID.ToString();
                        UPCProductionOrderID = List_Unit[0].ProductionOrderID.ToString();
                        UPCPartID = List_Unit[0].PartID.ToString();
                    }
                    else
                    {
                        string FGUnitID = List_Unit[0].UnitID.ToString();
                        List<mesUnitDetail> List_mesUnitDetail = await Public_Repository.GetmesUnitDetail(FGUnitID);

                        UPCSN = List_mesUnitDetail[0].KitSerialNumber.ToString();
                        if (string.IsNullOrEmpty(UPCSN))
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20189, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }

                        List<mesUnitSerialNumber> List_UPC = await Public_Repository.GetmesUnitSerialNumber(UPCSN);
                        UPCID = List_UPC[0].UnitID.ToString();
                        UPCProductionOrderID = List_UPC[0].ProductionOrderID.ToString();
                        UPCPartID = List_UPC[0].PartID.ToString();
                    }

                    if (UPCPartID != S_PartID)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20083, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    if (UPCProductionOrderID != S_POID)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20084, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    if (v_ConfirmPOOutputDto.List_PrintUPCUnitStateID.Contains(List_Unit[0].UnitStateID.ToString()) == false)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20045, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    ListSN.Add(UPCSN);

                    mesHistory v_mesHistory = new mesHistory();
                    v_mesHistory.UnitID = Convert.ToInt32(UPCID);
                    v_mesHistory.UnitStateID = Convert.ToInt32(v_ConfirmPOOutputDto.PrintUnitStateID);
                    v_mesHistory.EmployeeID = List_Login.EmployeeID;
                    v_mesHistory.StationID = List_Login.StationID;
                    v_mesHistory.EnterTime = DateTime.Now;
                    v_mesHistory.ExitTime = DateTime.Now;
                    v_mesHistory.ProductionOrderID = Convert.ToInt32(UPCProductionOrderID);
                    v_mesHistory.PartID = Convert.ToInt32(UPCPartID);
                    v_mesHistory.LooperCount = 1;
                    v_mesHistory.StatusID = 1;

                    S_ReturnValue = await DataCommit_Repository.SubmitData_DataOne(v_mesHistory);
                }

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
