using API_MSG;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
using NPOI.POIFS.Properties;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using Quartz.Impl.Triggers;
using SQLitePCL;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using static Dapper.SqlMapper;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;

namespace SunnyMES.Security.Repositories
{
    public class QCRepository : BaseRepositoryReport<string>, IRepositories.IQCRepository
    {
        MSG_Public P_MSG_Public;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        MSG_Sys P_MSG_Sys;

        //string S_Batch_Pattern = "";
        //string S_SN_Pattern = "";

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;


        public QCRepository()
        {
        }

        public QCRepository(IDbContextCore dbContext) : base(dbContext)
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

            //string S_TTBoxSN_Pattern = "[\\s\\S]*";
            //foreach (var item in List_StationTypeDetail)
            //{
            //    if (item.ValStr2 != null)
            //    {
            //        if (item.ValStr2.Trim() == "TTBoxSN_Pattern")
            //        {
            //            S_TTBoxSN_Pattern = item.ValStr1.Trim();

            //            foreach (var item_StationConfig in List_StationConfig)
            //            {
            //                if (item_StationConfig.ValStr1 == "TTBoxSN_Pattern")
            //                {
            //                    S_TTBoxSN_Pattern = item_StationConfig.ValStr2.Trim();
            //                    continue;
            //                }
            //            }
            //        }
            //    }
            //}
            //List_Login.TTBoxSN_Pattern = S_TTBoxSN_Pattern;

            //string S_IsTTBoxUnpack = "";
            //foreach (var item in List_StationTypeDetail)
            //{
            //    if (item.ValStr2 != null)
            //    {
            //        if (item.ValStr2.Trim() == "IsTTBoxUnpack")
            //        {
            //            S_IsTTBoxUnpack = item.ValStr1.Trim();

            //            foreach (var item_StationConfig in List_StationConfig)
            //            {
            //                if (item_StationConfig.ValStr1 == "IsTTBoxUnpack")
            //                {
            //                    S_IsTTBoxUnpack = item_StationConfig.ValStr2.Trim();
            //                    continue;
            //                }
            //            }
            //        }
            //    }
            //}
            //List_Login.IsTTBoxUnpack = S_IsTTBoxUnpack;

            //foreach (var item_StationConfig in List_StationConfig)
            //{
            //    if (item_StationConfig.ValStr1 == "PrintIPPort")
            //    {
            //        List_Login.PrintIPPort = item_StationConfig.ValStr2.Trim();
            //        continue;
            //    }
            //}


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
                Public_Repository = new PublicRepository(DB_Context, I_Language);
            }

            if (DataCommit_Repository == null)
            {
                DataCommit_Repository = new DataCommitRepository(DB_Context, I_Language);
            }

            if (SNFormat_Repository == null)
            {
                SNFormat_Repository = new SNFormatRepository();
            }

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
            List<IsCheckPOPN> List_IsCheckPOPN = await Public_Repository.GetPageInitialize(List_Login, S_URL);
            if (List_IsCheckPOPN.Count > 0)
            {
                if (List_IsCheckPOPN[0].TabVal == null)
                {
                    S_IsCheckPO = List_IsCheckPOPN[0].IsCheckPO;
                    S_IsCheckPN = List_IsCheckPOPN[0].IsCheckPN;
                    S_ApplicationType = List_IsCheckPOPN[0].ApplicationType;
                    S_IsLegalPage = List_IsCheckPOPN[0].IsLegalPage;
                    //S_TTScanType = List_IsCheckPOPN[0].TTScanType;
                    S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                    S_TTBoxType = List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO = List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                    //S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                    //S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                    //S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                    //S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;

                    //S_IsCheckCardID = List_IsCheckPOPN[0].IsCheckCardID;
                    //S_CardIDPattern = List_IsCheckPOPN[0].CardIDPattern;

                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" + 
                            //S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                            // "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                            //S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                            //S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                            //S_JumpStatusID + "' as JumpStatusID,'" +
                            //S_JumpUnitStateID + "' as JumpUnitStateID,'" +

                            S_IsCheckCardID + "' as IsCheckCardID,'" +
                            S_CardIDPattern + "' as CardIDPattern,'" +

                            List_Login.PrintIPPort + "' as PrintIPPort, '" + S_IsPrint + "' as IsPrint";
                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                    List_Result = v_Query;
                }
            }
            return List_Result;
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            List<dynamic> List_Result = new List<dynamic>();
            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);

            List_SetConfirmPO.JumpFromUnitStateID = S_JumpFromUnitStateID;
            List_SetConfirmPO.JumpToUnitStateID = S_JumpToUnitStateID;
            List_SetConfirmPO.JumpStatusID = S_JumpStatusID;
            List_SetConfirmPO.JumpUnitStateID = S_JumpUnitStateID;

            return List_SetConfirmPO;
        }

        public async Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL)
        {
            S_SN = S_SN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();
            List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            SetScanSNOutputDto F_SetScanSNOutputDto = new SetScanSNOutputDto();
            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                //初始化
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);
                //获取验证信息
                List<TabVal> List_ScanSNCheck = await Public_Repository.GetScanSNCheck(
                    S_SN, S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_DefectID, List_Login);
                string S_ScanSNCheck = List_ScanSNCheck[0].ValStr1;

                S_PartID = List_ScanSNCheck[0].PartID;
                S_PartFamilyID = List_ScanSNCheck[0].PartFamilyID;
                S_POID = List_ScanSNCheck[0].ProductionOrderID;
                int I_UnitStateID = Convert.ToInt32(List_ScanSNCheck[0].UnitStateID);

                //验证通过后处理
                if (S_ScanSNCheck == "1")
                {
                    int I_PartID = Convert.ToInt32(S_PartID);
                    int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    int I_LineID = List_Login.LineID;
                    int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                    int I_ProductionOrderID = Convert.ToInt32(S_POID);
                    int I_StatusID = Convert.ToInt32(S_UnitStatus);

                    mesUnit v_mesUnit = new mesUnit();
                    v_mesUnit.ID = Convert.ToInt32(List_ScanSNCheck[0].UnitID);
                    v_mesUnit.UnitStateID = I_UnitStateID;
                    v_mesUnit.StatusID = I_StatusID;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.ProductionOrderID = I_ProductionOrderID;
                    //修改 Unit
                    List_mesUnit.Add(v_mesUnit);

                    mesHistory v_mesHistory = new mesHistory();
                    v_mesHistory.UnitID = Convert.ToInt32(v_mesUnit.ID);
                    v_mesHistory.UnitStateID = I_UnitStateID;
                    v_mesHistory.EmployeeID = List_Login.EmployeeID;
                    v_mesHistory.StationID = List_Login.StationID;
                    v_mesHistory.ProductionOrderID = I_ProductionOrderID;
                    v_mesHistory.PartID = I_PartID;
                    v_mesHistory.LooperCount = 1;
                    v_mesHistory.StatusID = I_StatusID;
                    //插入 mesHistory
                    List_mesHistory.Add(v_mesHistory);

                    string S_ReturnValue = "OK";
                    UnitStatus Enum_StatusID = PublicF.ToEnum<UnitStatus>(S_UnitStatus);
                    if (Enum_StatusID == UnitStatus.PASS)
                    {
                        S_ReturnValue = await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_DefectAdd
                            (List_mesUnit, List_mesHistory, null);
                    }
                    else
                    {
                        string[] Array_Defect = S_DefectID.Split(',');
                        foreach (var item in Array_Defect)
                        {
                            try
                            {
                                if (item.Trim() != "")
                                {
                                    int I_DefectID = Convert.ToInt32(item);
                                    mesUnitDefect v_mesUnitDefect = new mesUnitDefect();

                                    v_mesUnitDefect.UnitID = v_mesUnit.ID;
                                    v_mesUnitDefect.DefectID = I_DefectID;
                                    v_mesUnitDefect.StationID = List_Login.StationID;
                                    v_mesUnitDefect.EmployeeID = List_Login.EmployeeID;

                                    List_mesUnitDefect.Add(v_mesUnitDefect);
                                }
                            }
                            catch (Exception ex)
                            {
                                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                                return F_SetScanSNOutputDto;
                            }
                        }
                        S_ReturnValue = await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_DefectAdd
                            (List_mesUnit, List_mesHistory, List_mesUnitDefect);
                    }

                    if (S_ReturnValue != "OK")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ReturnValue, "0", List_Login, "");
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }
                else
                {
                    TabVal_MSGERROR = List_ScanSNCheck[0];
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                List<dynamic> List_ProductionOrderQTY = await
                    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                F_SetScanSNOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, "");
                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSNOutputDto;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);
            return F_SetScanSNOutputDto;
        }

    }
}