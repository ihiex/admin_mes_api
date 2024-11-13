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
    public class IQCRepository : BaseRepositoryReport<string>, IIQCRepository
    {
        MSG_Public P_MSG_Public;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        MSG_Sys P_MSG_Sys;

        string S_Batch_Pattern = "";
        string S_SN_Pattern = "";

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;


        public IQCRepository()
        {
        }

        public IQCRepository(IDbContextCore dbContext) : base(dbContext)
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
                if (item.ValStr2 != null && item.ValStr2.Trim() == "TTBoxSN_Pattern")
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
                if (item.ValStr2 != null && item.ValStr2.Trim() == "IsTTBoxUnpack")
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
                   // S_TTScanType = List_IsCheckPOPN[0].TTScanType;
                   // S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                   // S_TTBoxType = List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO = List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                   // S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                   // S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                   // S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                   // S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;
                    S_OperationType = List_IsCheckPOPN[0].OperationType;

                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" +
                           // S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                           // "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                          //  S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                          //  S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                          //  S_JumpStatusID + "' as JumpStatusID,'" +
                          //  S_JumpUnitStateID + "' as JumpUnitStateID,'" +
                            S_OperationType + "' as OperationType,'" +
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

            int I_PartID = Convert.ToInt32(S_PartID);
            List<mesPartDetail> List_mesPartDetail = await Public_Repository.GetmesPartDetail(I_PartID, "InnerSN_Pattern");

            if (List_mesPartDetail.Count > 0)
            {
                string S_InnerSN_Pattern = "";
                S_InnerSN_Pattern = List_mesPartDetail[0].Content.Trim();
                if (S_InnerSN_Pattern != "0")
                {
                    S_InnerSN_Pattern = PublicF.EncryptPassword(S_InnerSN_Pattern, S_PwdKey);
                }

                List_SetConfirmPO.InnerSN_Pattern = S_InnerSN_Pattern;
            }
            else
            {
                List_SetConfirmPO.InnerSN_Pattern = "0";
            }
            
            return List_SetConfirmPO;
        }

        public async Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
                string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID,string S_InnerSN_Pattern, string S_URL)
        {
            S_SN = S_SN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";


            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            SetScanSNOutputDto F_SetScanSNOutputDto = new SetScanSNOutputDto();
            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;

            Boolean B_ReturnValue = true;
            string S_ERROR = "";

            try 
            {
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

                List<mesUnit> List_mesUnit = new List<mesUnit>();
                List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
                List<mesHistory> List_mesHistory = new List<mesHistory>();
                List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
                List<mesUnitComponent> List_mesUnitComponent = new List<mesUnitComponent>();
                List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

                //正则值
                List<TabVal> List_Pattern = await Public_Repository.GetPattern(S_PartID, List_Login);
                if (List_Pattern[0].ValStr2 == "ERROR")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;

                }
                else
                {
                    S_Batch_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr1, S_PwdKey);
                    S_SN_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr2, S_PwdKey);
                }

                
                if (S_SN == "")
                {
                    //条码不能为空.   
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                if (!Regex.IsMatch(S_SN, S_SN_Pattern))
                {
                    //正则校验未通过，请确认.  
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_SN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;

                }

                List<mesSerialNumber> v_mesSerialNumber = await Public_Repository.GetmesSerialNumber("", S_SN);
                if (v_mesSerialNumber != null)
                {
                    if (v_mesSerialNumber.Count > 0)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20006, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }

                //S_InnerSN_Pattern = PublicF.DecryptPassword(S_InnerSN_Pattern, S_PwdKey);
                if (S_InnerSN_Pattern == "0")
                {
                    List<TabVal> List_ProCheck = await Public_Repository.List_ExecProc("", S_SN, S_POID, S_PartID, null, null, List_Login);
                    string S_ProCheck = List_ProCheck[0].ValStr1;
                    if (S_ProCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ProCheck, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }


                    List<TabVal> List_PONumberCheck = await Public_Repository.GetPONumberCheck(S_POID, "1");
                    string S_PONumberCheck = List_PONumberCheck[0].ValStr1;

                    if (S_PONumberCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_PONumberCheck, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }

                    List<IdDescription> List_mesUnitState = new List<IdDescription>();
                    int I_PartID = Convert.ToInt32(S_PartID);
                    int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    int I_LineID = List_Login.LineID;
                    int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                    int I_ProductionOrderID = Convert.ToInt32(S_POID);
                    int I_StatusID = Convert.ToInt32(S_UnitStatus);

                    List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                        I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);

                    if (List_mesUnitState == null)
                    {
                        List_mesUnitState = new List<IdDescription>();

                        if (List_mesUnitState.Count == 0)
                        {
                            //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }

                    int I_UnitStateID = List_mesUnitState[0].ID;

                    ///////////////////////////////////////////////////////////////////
                    //二次检查是否有指定线路  20231214
                    var specialUnitState = await Public_Repository.GetmesUnitStateSecondAsync(I_PartID.ToString(), I_PartFamilyID.ToString(), "",
                        I_LineID.ToString(), I_StationTypeID, I_ProductionOrderID.ToString(), I_StatusID.ToString(), S_SN);

                    if (!string.IsNullOrEmpty(specialUnitState.errorCode))
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(specialUnitState.errorCode, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                    I_UnitStateID = specialUnitState.unitStateId.ToInt();

                    ////////////////////////////////////////////////////////////////////



                    mesUnit v_mesUnit = new mesUnit();
                    v_mesUnit.UnitStateID = I_UnitStateID;
                    v_mesUnit.StatusID = I_StatusID;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.CreationTime = DateTime.Now;
                    v_mesUnit.LastUpdate = DateTime.Now;
                    v_mesUnit.PanelID = 0;
                    v_mesUnit.LineID = List_Login.LineID;
                    v_mesUnit.ProductionOrderID = I_ProductionOrderID;
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = I_PartID;
                    v_mesUnit.LooperCount = 1;

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

                    List_mesHistory.Add(v_mesHistory);

                    mesSerialNumber v_SN = new mesSerialNumber();
                    v_SN.SerialNumberTypeID = 0;
                    v_SN.Value = S_SN;

                    List_mesSerialNumber.Add(v_SN);

                    mesUnitDetail v_mesDetail = new mesUnitDetail();
                    v_mesDetail.reserved_01 = "";
                    v_mesDetail.reserved_02 = "";
                    v_mesDetail.reserved_03 = "";
                    v_mesDetail.reserved_04 = "";
                    v_mesDetail.reserved_05 = "";

                    List_mesUnitDetail.Add(v_mesDetail);

                    string[] Array_Defect = S_DefectID.Split(';');
                    if (I_StatusID != 1)
                    {
                        foreach (var item in Array_Defect)
                        {
                            try
                            {
                                if (item.Trim() != "")
                                {
                                    int I_DefectID = Convert.ToInt32(item);

                                    mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                                    v_mesUnitDefect.DefectID = I_DefectID;
                                    v_mesUnitDefect.StationID = List_Login.StationID;
                                    v_mesUnitDefect.EmployeeID = List_Login.EmployeeID;

                                    List_mesUnitDefect.Add(v_mesUnitDefect);
                                }
                            }
                            catch (Exception ex)
                            {
                                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                                return F_SetScanSNOutputDto;
                            }
                        }
                    }


                    Tuple<bool, string> Tuple_Commit =
                        await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
                        (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory, List_mesUnitComponent, List_mesUnitDefect);
                    B_ReturnValue = Tuple_Commit.Item1;
                    S_ERROR = Tuple_Commit.Item2;

                    if (B_ReturnValue == false)
                    {
                        for (int i = 0; i < 10 && S_ERROR.Contains("PRIMARY KEY") && S_ERROR.Contains("Unit_PK"); i++) 
                        {
                            Tuple_Commit =
                                await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
                                (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory, List_mesUnitComponent, List_mesUnitDefect);
                            B_ReturnValue = Tuple_Commit.Item1;
                            S_ERROR = Tuple_Commit.Item2;
                        }

                        if (B_ReturnValue == false)
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(S_ERROR, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }

                    List<dynamic> List_ProductionOrderQTY = await
                        Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                    F_SetScanSNOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;
                }

            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSNOutputDto;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);
            return F_SetScanSNOutputDto;
        }

        public async Task<SetScanSNOutputDto> SetScanChildSN(string S_SN,string S_ChildSN, string S_PartFamilyTypeID, string S_PartFamilyID,
                string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_InnerSN_Pattern, string S_URL)
        {
            S_SN = S_SN ?? "";
            S_ChildSN = S_ChildSN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";


            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            SetScanSNOutputDto F_SetScanSNOutputDto = new SetScanSNOutputDto();
            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;

            Boolean B_ReturnValue = true;
            string S_ERROR = "";

            try
            {
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

                List<mesUnit> List_mesUnit = new List<mesUnit>();
                List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
                List<mesHistory> List_mesHistory = new List<mesHistory>();
                List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
                List<mesUnitComponent> List_mesUnitComponent = new List<mesUnitComponent>();
                List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

                //正则值
                //List<TabVal> List_Pattern = await Public_Repository.GetPattern(S_PartID, List_Login);
                //if (List_Pattern[0].ValStr2 == "ERROR")
                //{
                //    TabVal_MSGERROR = Public_Repository.GetERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                //    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                //    return F_SetScanSNOutputDto;

                //}
                //else
                //{
                //    S_Batch_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr1, S_PwdKey);
                //    S_SN_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr2, S_PwdKey);
                //}
                
                if (S_ChildSN == "")
                {
                    //条码不能为空.   
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_ChildSN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                S_SN_Pattern = PublicF.DecryptPassword(S_InnerSN_Pattern, S_PwdKey);
                if (!Regex.IsMatch(S_ChildSN, S_SN_Pattern))
                {
                    //正则校验未通过，请确认.  
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_ChildSN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;

                }

                List<TabVal> List_ProCheck = await Public_Repository.List_ExecProc("", S_ChildSN, S_POID, S_PartID, null, null, List_Login);
                string S_ProCheck = List_ProCheck[0].ValStr1;
                if (S_ProCheck != "1")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_ProCheck, "0", List_Login, S_ChildSN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }


                List<TabVal> List_PONumberCheck = await Public_Repository.GetPONumberCheck(S_POID, "1");
                string S_PONumberCheck = List_PONumberCheck[0].ValStr1;

                if (S_PONumberCheck != "1")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_PONumberCheck, "0", List_Login, S_ChildSN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                List<mesSerialNumber> v_mesSerialNumber = await Public_Repository.GetmesSerialNumber("", S_SN);
                if (v_mesSerialNumber != null)
                {
                    if (v_mesSerialNumber.Count > 0)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20006, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }


                Boolean B_ChildSN =await Public_Repository.MESCheckChildSerialNumberAsync(S_ChildSN);
                if (B_ChildSN == true) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20006, "0", List_Login, S_ChildSN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }


                List<IdDescription> List_mesUnitState = new List<IdDescription>();
                int I_PartID = Convert.ToInt32(S_PartID);
                int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                int I_LineID = List_Login.LineID;
                int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                int I_ProductionOrderID = Convert.ToInt32(S_POID);
                int I_StatusID = Convert.ToInt32(S_UnitStatus);

                List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                    I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);

                if (List_mesUnitState != null)
                {                    
                    if (List_mesUnitState.Count == 0)
                    {
                        //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }
                else 
                {
                    List_mesUnitState = new List<IdDescription>();
                    //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                int I_UnitStateID = List_mesUnitState[0].ID;

                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.UnitStateID = I_UnitStateID;
                v_mesUnit.StatusID = I_StatusID;
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.CreationTime = DateTime.Now;
                v_mesUnit.LastUpdate = DateTime.Now;
                v_mesUnit.PanelID = 0;
                v_mesUnit.LineID = List_Login.LineID;
                v_mesUnit.ProductionOrderID = I_ProductionOrderID;
                v_mesUnit.RMAID = 0;
                v_mesUnit.PartID = I_PartID;
                v_mesUnit.LooperCount = 1;

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

                mesSerialNumber v_SN = new mesSerialNumber();
                v_SN.SerialNumberTypeID = 0;
                v_SN.Value = S_SN;

                List_mesSerialNumber.Add(v_SN);

                mesUnitComponent v_mesUnitComponent = new mesUnitComponent();
                v_mesUnitComponent.UnitComponentTypeID = 1;
                v_mesUnitComponent.ChildUnitID = 0;
                v_mesUnitComponent.ChildSerialNumber = S_ChildSN;
                v_mesUnitComponent.ChildLotNumber = "";
                v_mesUnitComponent.ChildPartID = 0;
                v_mesUnitComponent.ChildPartFamilyID = 0;
                v_mesUnitComponent.Position = "";
                v_mesUnitComponent.InsertedEmployeeID = List_Login.EmployeeID;
                v_mesUnitComponent.InsertedStationID = List_Login.StationID;
                v_mesUnitComponent.StatusID = 1;

                List_mesUnitComponent.Add(v_mesUnitComponent);


                mesUnitDetail v_mesDetail = new mesUnitDetail();
                v_mesDetail.reserved_01 = "";
                v_mesDetail.reserved_02 = "";
                v_mesDetail.reserved_03 = "";
                v_mesDetail.reserved_04 = "";
                v_mesDetail.reserved_05 = "";

                List_mesUnitDetail.Add(v_mesDetail);

                string[] Array_Defect = S_DefectID.Split(',');
                if (I_StatusID != 1)
                {
                    foreach (var item in Array_Defect)
                    {
                        try
                        {
                            if (item.Trim() != "")
                            {
                                int I_DefectID = Convert.ToInt32(item);

                                mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                                v_mesUnitDefect.DefectID = I_DefectID;
                                v_mesUnitDefect.StationID = List_Login.StationID;
                                v_mesUnitDefect.EmployeeID = List_Login.EmployeeID;

                                List_mesUnitDefect.Add(v_mesUnitDefect);
                            }
                        }
                        catch (Exception ex)
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }
                }


                Tuple<bool, string> Tuple_Commit =
                    await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
                    (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory, List_mesUnitComponent, List_mesUnitDefect);
                B_ReturnValue = Tuple_Commit.Item1;
                S_ERROR = Tuple_Commit.Item2;

                if (B_ReturnValue == false)
                {
                    for (int i = 0; i < 10 && S_ERROR.Contains("PRIMARY KEY") && S_ERROR.Contains("Unit_PK"); i++)
                    {
                        Tuple_Commit =
                            await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
                            (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory, List_mesUnitComponent, List_mesUnitDefect);
                        B_ReturnValue = Tuple_Commit.Item1;
                        S_ERROR = Tuple_Commit.Item2;
                    }

                    if (B_ReturnValue == false)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ERROR, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }

                List<dynamic> List_ProductionOrderQTY = await
                    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                F_SetScanSNOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;
                

            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSNOutputDto;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_ChildSN + "  Scan success", "OK", List_Login);
            return F_SetScanSNOutputDto;
        }
    }
}