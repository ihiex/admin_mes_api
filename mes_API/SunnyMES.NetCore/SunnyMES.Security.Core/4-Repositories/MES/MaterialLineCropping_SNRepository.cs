using API_MSG;
using Google.Protobuf.Collections;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
using NPOI.OpenXmlFormats.Wordprocessing;
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
using System.Security.Cryptography;
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
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using static Dapper.SqlMapper;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;


namespace SunnyMES.Security.Repositories
{
    public class MaterialLineCropping_SNRepository : BaseRepositoryReport<string>, IMaterialLineCropping_SNRepository
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


        public MaterialLineCropping_SNRepository()
        {
        }

        public MaterialLineCropping_SNRepository(IDbContextCore dbContext) : base(dbContext)
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
                    S_TTScanType = List_IsCheckPOPN[0].TTScanType;
                    S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                    S_TTBoxType = List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO = List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                    S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                    S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                    S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                    S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;
                    S_OperationType = List_IsCheckPOPN[0].OperationType;

                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                            "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                            S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                            S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                            S_JumpStatusID + "' as JumpStatusID,'" +
                            S_JumpUnitStateID + "' as JumpUnitStateID,'" +
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

        public async Task<ConfirmPOOutputMateialDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_LineNumber, string Type, string S_URL)
        {            
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            ConfirmPOOutputMateialDto F_ConfirmPOOutputMateialDto = new ConfirmPOOutputMateialDto();
            F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;

            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, null, List_Login);

            if (Type == "BatchToSN" || Type == "BatchToSNDOE")
            {
                if (S_LineNumber == "" || S_LineNumber == null)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20101, "0", List_Login, "");
                    F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputMateialDto;
                }
            }


            F_ConfirmPOOutputMateialDto.mesProductStructures = List_SetConfirmPO.mesProductStructures;
            F_ConfirmPOOutputMateialDto.Route = List_SetConfirmPO.Route;
            F_ConfirmPOOutputMateialDto.RouteDataDiagram1 = List_SetConfirmPO.RouteDataDiagram1;
            F_ConfirmPOOutputMateialDto.RouteDataDiagram2 = List_SetConfirmPO.RouteDataDiagram2;
            F_ConfirmPOOutputMateialDto.RouteDetail = List_SetConfirmPO.RouteDetail;
            F_ConfirmPOOutputMateialDto.ProductionOrderQTY = List_SetConfirmPO.ProductionOrderQTY;

            F_ConfirmPOOutputMateialDto.BatchQTY = "";
            F_ConfirmPOOutputMateialDto.Unit = "";
            F_ConfirmPOOutputMateialDto.LotCode = false;
            F_ConfirmPOOutputMateialDto.TranceCode = false;

            F_ConfirmPOOutputMateialDto.DOE_BuildNameEnabled = false;
            F_ConfirmPOOutputMateialDto.List_DOE_BuildName = null;
            F_ConfirmPOOutputMateialDto.DOE_CCCCEnabled = false;
            F_ConfirmPOOutputMateialDto.DOE_CCCC_Length = 0;

            int I_PartID = Convert.ToInt32(S_PartID);
            //批次正则
            List<mesPartDetail> List_Batch_Pattern = await Public_Repository.GetmesPartDetail(I_PartID, "Batch_Pattern");

            if (List_Batch_Pattern.Count > 0)
            {
                string S_Batch_Pattern = List_Batch_Pattern[0].Content.Trim();
                S_Batch_Pattern = PublicF.EncryptPassword(S_Batch_Pattern, S_PwdKey);
                F_ConfirmPOOutputMateialDto.Batch_Pattern = S_Batch_Pattern;
            }
            else
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20025, "0", List_Login, "");
                F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputMateialDto;
            }

            //SN 正则
            List<mesPartDetail> List_SN_Pattern = await Public_Repository.GetmesPartDetail(I_PartID, "SN_Pattern");
            if (List_SN_Pattern.Count > 0)
            {
                string S_SN_Pattern = List_SN_Pattern[0].Content.Trim();
                S_SN_Pattern = PublicF.EncryptPassword(S_SN_Pattern, S_PwdKey);
                F_ConfirmPOOutputMateialDto.SN_Pattern = S_SN_Pattern;
            }


            List<mesPartDetail> List_SplitBatchQTY = await Public_Repository.GetmesPartDetail(I_PartID, "SplitBatchQTY");
            if (List_SplitBatchQTY.Count > 0)
            {
                string S_SplitBatchQTY = List_SplitBatchQTY[0].Content.Trim();
                F_ConfirmPOOutputMateialDto.SplitBatchQTY = S_SplitBatchQTY;
            }

            List<mesPartDetail> List_IsForceSplit = await Public_Repository.GetmesPartDetail(I_PartID, "IsForceSplit");
            if (List_IsForceSplit.Count > 0)
            {
                string S_IsForceSplit = List_IsForceSplit[0].Content.Trim();
                F_ConfirmPOOutputMateialDto.IsForceSplit = S_IsForceSplit != "1";
            }
            else 
            {
                F_ConfirmPOOutputMateialDto.IsForceSplit = true;
            }

            DataSet DS_MaterailBatchData = GetMaterailBatchData(S_PartID,List_Login.StationID.ToString());
            if (DS_MaterailBatchData.Tables.Count > 1) 
            {
                DataTable DT_MaterailBatchData = DS_MaterailBatchData.Tables[0];
                DataTable DT_Result= DS_MaterailBatchData.Tables[1];

                string S_Result = DT_Result.Rows[0][0].ToString();
                if (S_Result != "1") 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_Result, "0", List_Login, "");
                    F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputMateialDto;
                }

                List<MaterailBatchData> List_MaterailBatchData= DataTableEx.ToList<MaterailBatchData>(DT_MaterailBatchData);
                F_ConfirmPOOutputMateialDto.List_MaterailBatchData= List_MaterailBatchData;
            }

            string S_LabelName = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
            if (S_LabelName == "")
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20104, "0", List_Login, "");
                F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputMateialDto;
            }
            F_ConfirmPOOutputMateialDto.LabelPath = S_LabelName;

            if (Type == "BatchToSNDOE")
            {
                List<PoDetailDefs> List_PoDetail_DOE_BuildName = await Public_Repository.GetluPODetailDefs(Convert.ToInt32(S_POID), "DOE_BuildName");

                string S_BuildName = List_PoDetail_DOE_BuildName[0].Content.Trim();
                if (S_BuildName != "") 
                {
                    F_ConfirmPOOutputMateialDto.DOE_BuildNameEnabled = true;

                    string[] List_Value = S_BuildName.Split(',');
                    List<string> List_Build = new List<string>(); 

                    foreach (string str in List_Value)
                    {
                        List_Build.Add(str);                      
                    }

                    F_ConfirmPOOutputMateialDto.List_DOE_BuildName = List_Build;
                }


                List<PoDetailDefs> List_PoDetail_DOECCCC = await Public_Repository.GetluPODetailDefs(Convert.ToInt32(S_POID), "DOE_CCCCConfig");
                if (List_PoDetail_DOECCCC.Count > 0) 
                {
                    string S_Length= List_PoDetail_DOECCCC[0].Content.Trim();
                    if (S_Length != "") 
                    {
                        int I_Length=Convert.ToInt32(S_Length);
                        F_ConfirmPOOutputMateialDto.DOE_CCCC_Length = I_Length;
                        F_ConfirmPOOutputMateialDto.DOE_CCCCEnabled = true;
                    }
                }
            }
            return F_ConfirmPOOutputMateialDto;
        }

        public async Task<CreateSNOutputDto> SetSplit
            (
                string PartFamilyTypeID, string PartFamilyID,
                string PartID, string POID, string LineNumber, string URL,

                string Type, string SN_Pattern,string Batch_Pattern, string BatchNo, string RollNO, int SpecQTY,

                int ParentID,int BatchQTY, int UsageQTY,
                Boolean DOE_BuildNameEnabled,string DOE_BuildName,Boolean DOE_CCCCEnabled,int DOE_CCCC_Length,string CCCC
            )
        {
            PartFamilyTypeID = PartFamilyTypeID ?? "";
            PartFamilyID = PartFamilyID ?? "";
            PartID = PartID ?? "";
            POID = POID ?? "";
            LineNumber = LineNumber ?? "";

            Type = Type ?? "";
            BatchNo = BatchNo ?? "";
            RollNO = RollNO ?? "";

            List<string> ListSN = new List<string>();
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            CreateSNOutputDto F_CreateSNOutputDto = new CreateSNOutputDto();
            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

            List<mesMaterialUnit> List_mesMaterialUnit = new List<mesMaterialUnit>();
            List<mesMaterialUnitDetail> List_mesMaterialUnitDetail = new List<mesMaterialUnitDetail>();

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
            List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();

            if (SN_Pattern != "")
            {
                SN_Pattern = PublicF.DecryptPassword(SN_Pattern, S_PwdKey);
            }

            if (Batch_Pattern != "")
            {
                Batch_Pattern = PublicF.DecryptPassword(Batch_Pattern, S_PwdKey);
            }

            try 
            {
                if (BatchNo == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20144, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                int SplitQTY = SpecQTY;
                if (SplitQTY < 1)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20146, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }


                if (BatchQTY - UsageQTY < SplitQTY) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20172+"  "+
                        (BatchQTY - UsageQTY).ToString(), "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (Type == "BatchToSNDOE") 
                {
                    if (DOE_CCCC_Length != CCCC.Trim().Length)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(" CCCC Config length should be "+DOE_CCCC_Length.ToString()+" bits", "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                }



                string S_FormatSN = Public_Repository.mesGetSNFormatIDByList(PartID, PartFamilyID,
                    List_Login.LineID.ToString(), POID, List_Login.StationTypeID.ToString());

                List<IdDescription> List_mesUnitState = new List<IdDescription>();
                int I_PartID = Convert.ToInt32(PartID);
                int I_PartFamilyID = Convert.ToInt32(PartFamilyID);
                int I_LineID = List_Login.LineID;
                int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                int I_ProductionOrderID = Convert.ToInt32(POID);
                int I_StatusID = 1;

                List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                    I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);
                if (List_mesUnitState == null)
                {
                    //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20203, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
                int I_UnitStateID = List_mesUnitState[0].ID;

                if (string.IsNullOrEmpty(S_FormatSN)) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20132, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }


                mesMaterialUnit mesMaterial = new mesMaterialUnit();
                mesMaterial.PartID = Convert.ToInt32(PartID);
                mesMaterial.StationID = List_Login.StationID;
                mesMaterial.EmployeeID = List_Login.EmployeeID; ;
                mesMaterial.LineID = List_Login.LineID;
                mesMaterial.ParentID =ParentID;
                mesMaterial.RollCode = RollNO;
                
                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.UnitStateID = I_UnitStateID;
                v_mesUnit.StatusID = 1;
                v_mesUnit.PartFamilyID = Convert.ToInt32(PartFamilyID);
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.CreationTime = DateTime.Now;
                v_mesUnit.LastUpdate = DateTime.Now;
                v_mesUnit.PanelID = 0;
                v_mesUnit.LineID = List_Login.LineID;
                v_mesUnit.ProductionOrderID = Convert.ToInt32(POID);
                v_mesUnit.RMAID = 0;
                v_mesUnit.PartID = Convert.ToInt32(PartID);
                v_mesUnit.LooperCount = 1;
                v_mesUnit.StatusID = 1;

                string S_LineID = LineNumber;
                string S_Production0rder = "<ProdOrder ProductionOrder=" + "\"" + POID + "\"" + "> </ProdOrder>";
                string S_Station = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
                string S_xmlPart = "<Part PartID=" + "\"" + PartID + "\"" + "> </Part>";

                string S_CreateBathSN = "";
                if (Type == "SmallBatch")
                {
                     string xmlExtraData = "'<ExtraData BatchNo=" + "\"" + BatchNo + "\" " +
                     "RollCode =\"" + RollNO + "\" " + "> </ExtraData>'";

                    S_CreateBathSN = await SNFormat_Repository.GetSNRGetNext(S_FormatSN, "0",
                        null, null, null, xmlExtraData);

                    if (S_CreateBathSN == "")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20087, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                    
                    if (!Regex.IsMatch(S_CreateBathSN, Batch_Pattern))
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20027, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    mesMaterial.SerialNumber = S_CreateBathSN;
                    mesMaterial.Quantity = SplitQTY;
                    mesMaterial.RemainQTY = SplitQTY;
                    mesMaterial.MaterialTypeID = 1;

                    mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
                    //v_mesUnitDetail.UnitID = Convert.ToInt32(S_InsertUnit);
                    v_mesUnitDetail.reserved_01 = mesMaterial.SerialNumber;
                    v_mesUnitDetail.reserved_02 = "";
                    v_mesUnitDetail.reserved_03 = "";
                    v_mesUnitDetail.reserved_04 = "";
                    v_mesUnitDetail.reserved_05 = "";

                    mesHistory v_mesHistory = new mesHistory();
                    //v_mesHistory.UnitID = Convert.ToInt32(S_InsertUnit);
                    v_mesHistory.UnitStateID = v_mesUnit.UnitStateID;
                    v_mesHistory.EmployeeID = v_mesUnit.EmployeeID;
                    v_mesHistory.StationID = v_mesUnit.StationID;
                    v_mesHistory.EnterTime = DateTime.Now;
                    v_mesHistory.ExitTime = DateTime.Now;
                    v_mesHistory.ProductionOrderID = v_mesUnit.ProductionOrderID;
                    v_mesHistory.PartID = Convert.ToInt32(v_mesUnit.PartID);
                    v_mesHistory.LooperCount = 1;
                    v_mesHistory.StatusID = 1;

                    mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                    //v_mesSerialNumber.UnitID = Convert.ToInt32(S_InsertUnit);
                    v_mesSerialNumber.SerialNumberTypeID = 2;
                    v_mesSerialNumber.Value = mesMaterial.SerialNumber;


                    List_mesMaterialUnit.Add(mesMaterial);
                    List_mesUnit.Add(v_mesUnit);
                    List_mesUnitDetail.Add(v_mesUnitDetail);
                    List_mesSerialNumber.Add(v_mesSerialNumber);
                    List_mesHistory.Add(v_mesHistory);

                    Tuple<Boolean, string> Tuple_Result = await DataCommit_Repository.SubmitData_MaterialInitial(List_mesMaterialUnit,
                        List_mesMaterialUnitDetail, List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);

                    if (Tuple_Result.Item1 == false)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_Result.Item2, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    SetMaterailBatchData(BatchNo);

                    ListSN.Clear();
                    ListSN.Add(mesMaterial.SerialNumber);
                    F_CreateSNOutputDto.ListSN = ListSN;
                }
                else 
                {
                    string xmlExtraData = @"<ExtraData BatchNo=" + "\"" + BatchNo + "\"" +
                                               " RollCode =\"" + RollNO + "\"" +
                                               " PartFamilyTypeID=" + "\"" + PartFamilyTypeID + "\"" +
                                               " LineType=" + "\"" + "M" + "\"" +
                                               " LineID = " + "\"" + S_LineID + "\"" ;
                    if (Type == "BatchToSN")
                    {
                    }
                    else if (Type == "BatchToSN_NotLineNumber")
                    {
                        xmlExtraData = @"<ExtraData BatchNo=" + "\"" + BatchNo + "\"" +
                                            " RollCode =\"" + RollNO + "\"" +
                                            " PartFamilyTypeID=" + "\"" + PartFamilyTypeID + "\"" +
                                            " LineType=" + "\"" + "M" + "\"";                                            
                    }
                    else if (Type == "BatchToSNDOE")
                    {
                        if (DOE_BuildNameEnabled == true) 
                        {
                            xmlExtraData = xmlExtraData + " ET=" + "\"" + DOE_BuildName + "\"";
                        }
                        if (DOE_CCCCEnabled == true) 
                        {
                            xmlExtraData = xmlExtraData + " SPCA = " + "\"" + CCCC + "\"";
                        }
                    }
                    xmlExtraData += "> </ExtraData>";

                    int number = Convert.ToInt32(SpecQTY);
                    //DataSet dsMaterial = null;
                    mesMaterial.Quantity = 1;
                    mesMaterial.RemainQTY = 0;
                    mesMaterial.MaterialTypeID = 2;

                    ListSN.Clear();
                    for (; number > 0; number--)
                    {
                        List_mesMaterialUnit = new List<mesMaterialUnit>();
                        List_mesMaterialUnitDetail = new List<mesMaterialUnitDetail>();

                        List_mesUnit = new List<mesUnit>();
                        List_mesUnitDetail = new List<mesUnitDetail>();
                        List_mesSerialNumber = new List<mesSerialNumber>();
                        List_mesHistory = new List<mesHistory>();

                        S_CreateBathSN = await SNFormat_Repository.GetSNRGetNext(S_FormatSN, "0",
                            S_Production0rder, S_xmlPart, S_Station, xmlExtraData);

                        if (S_CreateBathSN == "")
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20087, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }
                        
                        if (!Regex.IsMatch(S_CreateBathSN, SN_Pattern))
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20027, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }

                        //写入mesSerialNumber
                        mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                        //v_mesSerialNumber.UnitID = Convert.ToInt32(unitId);
                        v_mesSerialNumber.SerialNumberTypeID = 2;
                        v_mesSerialNumber.Value = S_CreateBathSN;
     
                        //写入UnitDetail表
                        mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
                        //msDetail.UnitID = Convert.ToInt32(unitId);
                        v_mesUnitDetail.reserved_01 = "";
                        v_mesUnitDetail.reserved_02 = "";
                        v_mesUnitDetail.reserved_03 = "";
                        v_mesUnitDetail.reserved_04 = "";
                        v_mesUnitDetail.reserved_05 = "";

                        //写过站履历
                        mesHistory v_mesHistory = new mesHistory();
                        //v_mesHistory.UnitID = Convert.ToInt32(unitId);
                        v_mesHistory.UnitStateID = v_mesUnit.UnitStateID;
                        v_mesHistory.EmployeeID = v_mesUnit.EmployeeID;
                        v_mesHistory.StationID = v_mesUnit.StationID;
                        v_mesHistory.EnterTime = DateTime.Now;
                        v_mesHistory.ExitTime = DateTime.Now;
                        v_mesHistory.ProductionOrderID = v_mesUnit.ProductionOrderID;
                        v_mesHistory.PartID = Convert.ToInt32(v_mesUnit.PartID);
                        v_mesHistory.LooperCount = 1;
                        v_mesHistory.StatusID = 1;


                        //写入MaterialUnit表
                        mesMaterial.SerialNumber = S_CreateBathSN;
                        // ListSN
                        ListSN.Add(S_CreateBathSN);

                        List_mesMaterialUnit.Add(mesMaterial);
                        List_mesUnit.Add(v_mesUnit);
                        List_mesUnitDetail.Add(v_mesUnitDetail);
                        List_mesSerialNumber.Add(v_mesSerialNumber);
                        List_mesHistory.Add(v_mesHistory);

                        Tuple<Boolean, string> Tuple_Result = await DataCommit_Repository.SubmitData_MaterialSplit(List_mesMaterialUnit,
                            List_mesMaterialUnitDetail, List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);

                        if (Tuple_Result.Item1 == false)
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_Result.Item2, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }
                    }
                    F_CreateSNOutputDto.ListSN = ListSN;

                    //
                    SetMaterailBatchData(BatchNo);
                    //
                    UsageQTY = UsageQTY + Convert.ToInt32(SpecQTY);
                    F_CreateSNOutputDto.UsageQTY = UsageQTY;
                    F_CreateSNOutputDto.SplitQTY=SplitQTY;

                    string S_LabelName = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                        PartFamilyID, PartID, POID, List_Login.LineID.ToString());
                    if (S_LabelName == "")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20104, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                    F_CreateSNOutputDto.LabelPath = S_LabelName;
                }
            }
            catch ( Exception ex )
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                return F_CreateSNOutputDto;
            }

            return F_CreateSNOutputDto;
        }

        public  CroppingSNDto ParentBatchNo_ValueChanged(string S_PartID, string ParentID, string S_URL)
        {
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            CroppingSNDto F_CroppingSNDto = new CroppingSNDto();
            F_CroppingSNDto.MSG = TabVal_MSGERROR;

            int BatchQTY = 0;
            int UsageQTY = 0;
            string RoolNo = "";

            try
            {
                DataSet DS_MaterailBatchData = GetMaterailBatchData(S_PartID, List_Login.StationID.ToString());

                if (DS_MaterailBatchData.Tables.Count > 1)
                {
                    DataTable DT_MaterailBatchData = DS_MaterailBatchData.Tables[0];

                    DataRow drBatch = DT_MaterailBatchData.Select("ID=" + ParentID.ToString()).FirstOrDefault();
                    if (drBatch != null && !drBatch.IsNull("ID"))
                    {
                        BatchQTY = Convert.ToInt32(drBatch["Quantity"].ToString()) +
                                Convert.ToInt32(drBatch["BalanceQty"].ToString());

                        UsageQTY = GetMesMaterialUseQTY(ParentID);
                        RoolNo = drBatch["RollCode"].ToString();
                    }
                }
            }
            catch (Exception ex) 
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                F_CroppingSNDto.MSG = TabVal_MSGERROR;

                return F_CroppingSNDto;
            }

            F_CroppingSNDto.ParentID = Convert.ToInt32(ParentID);
            F_CroppingSNDto.BatchQTY = BatchQTY;
            F_CroppingSNDto.UsageQTY = UsageQTY;
            F_CroppingSNDto.RoolNo= RoolNo;
            
            return F_CroppingSNDto;
        }


        private DataSet GetMaterailBatchData(string S_PartID,string S_StationID) 
        {
            DataSet DS=new DataSet ();
            string S_Sql = "";

            try 
            {
                S_Sql =
                @"
	            declare	@strOutput        nvarchar(200),
			            @PartId				int=" + S_PartID + @",
			            @MaterialUnitID		int,
			            @PartValue			varchar(500),
			            @MaterialUnitIDList	varchar(500),
			            @StationID int='"+ S_StationID + @"',
			            @StationTypeID int

	            BEGIN TRY
		

		            select @StationTypeID=isnull(StationTypeID,0 ) from mesStation where id=@StationID

		            --获取所有数据(bom表对应料号数据)--20200907修改 增加联合索引/修改获取数据逻辑
		            SELECT distinct A.ID,A.SerialNumber,A.RollCode,A.Quantity,B.Reserved_01,ISNULL(A.BalanceQty,0) BalanceQty
                            ,A.PartID
                         INTO #Material
		            FROM mesMaterialUnit A 
			             JOIN mesMaterialUnitDetail B ON A.ID=B.MaterialUnitID
			             left JOIN mesProductStructure C ON A.PartID=C.ParentPartID and C.StationTypeID=@StationTypeID
                         left join luVendor D on A.PartID=D.PartID   
			            WHERE (C.PartID=@PartId OR A.PartID=@PartId)  AND A.StatusID=1 and A.ExpiringTime>GETDATE() and MaterialTypeID=1  --and A.PartID=@PartId
		
		            SELECT M.ID,M.SerialNumber,M.RollCode,ISNULL(M.Quantity,'') Quantity,M.Reserved_01,M.BalanceQty,ISNULL(S.UsageQTY,0) UsageQTY   
                         ,M.PartID 
			            INTO #MaterialBomTemp FROM #Material M 
		            LEFT JOIN 
			            (SELECT A.SerialNumber,isnull(SUM(isnull(B.Quantity,0))
			            +SUM(isnull(B.BalanceQty,0)),0) UsageQTY FROM #Material A
			            INNER JOIN mesMaterialUnit B ON B.ParentID=A.ID
			            GROUP BY A.SerialNumber) S ON M.SerialNumber=S.SerialNumber
		            WHERE M.Quantity-isnull(S.UsageQTY,0)+M.BalanceQty>0

		            --SELECT * INTO #MaterialBomTemp FROM (SELECT A.ID,A.SerialNumber,A.RollCode,A.Quantity,B.Reserved_01,
		            --	ISNULL(A.BalanceQty,0) BalanceQty,isnull((SELECT SUM(isnull(B.Quantity,0))
		            --	+SUM(isnull(B.BalanceQty,0))FROM mesMaterialUnit B WHERE B.ParentID=A.ID),0) UsageQTY
		            --	FROM mesMaterialUnit A 
		            --		INNER JOIN mesMaterialUnitDetail B ON A.ID=B.MaterialUnitID
		            --		--INNER JOIN mesProductStructure C ON A.PartID=C.ParentPartID
		            --	--WHERE (C.PartID=@PartId OR A.PartID=@PartId) AND A.StatusID=1 and A.ExpiringTime>GETDATE() and MaterialTypeID=1) WS 
		            --	WHERE  A.PartID=@PartId AND A.StatusID=1 and A.ExpiringTime>GETDATE() and MaterialTypeID=1) WS 
		            --WHERE WS.Quantity-UsageQTY+BalanceQty>0

		
		            DECLARE cursorAc CURSOR FOR
		            SELECT ID,Reserved_01 FROM #MaterialBomTemp WHERE ISNULL(Reserved_01,'')<>''
		            OPEN cursorAc
		            FETCH NEXT FROM cursorAc INTO @MaterialUnitID,@PartValue
			            WHILE @@FETCH_STATUS = 0
			            BEGIN
				            IF EXISTS(SELECT 1 FROM dbo.F_Split(@PartValue,',') WHERE value = @PartId)
				            BEGIN
					            SET @MaterialUnitIDList = isnull(@MaterialUnitIDList,'')+','+CAST(@MaterialUnitID AS VARCHAR);
				            END
		            FETCH NEXT FROM cursorAc INTO @MaterialUnitID,@PartValue
		            END
		            CLOSE cursorAc
		            DEALLOCATE cursorAc

					SELECT * INTO #MaterialBomTemp2 FROM 
					(
						SELECT * FROM (
						SELECT * FROM #MaterialBomTemp WHERE ISNULL(Reserved_01,'')='' 
						UNION ALL
						SELECT * FROM #MaterialBomTemp WHERE ID IN (SELECT value FROM dbo.F_Split(@MaterialUnitIDList,','))) RS
					)A

					SELECT *  FROM #MaterialBomTemp2 


		            DROP TABLE #MaterialBomTemp
                    DROP TABLE #MaterialBomTemp2
		            DROP TABLE #Material
		            SELECT @strOutput='1' 
	            END TRY
	            BEGIN CATCH
		            SELECT @strOutput=ERROR_MESSAGE()
	            END CATCH

                SELECT @strOutput as strOutput
                ";

                DS = Data_Set(S_Sql);
            }
            catch(Exception ex)
            {
                try
                {
                    S_Sql = "select '" + ex.Message + "' as ERROR" + "\r\n" +
                            "select 0  as strOutput";
                    DS = Data_Set(S_Sql);
                }
                catch { }
            }

            return DS;
        }

        private int GetMesMaterialUseQTY(string MaterialUnitID)
        {
            int UseQTY = 0;
            string sql = string.Format(@"SELECT isnull(SUM(isnull(B.Quantity,0))+SUM(isnull(B.BalanceQty,0)),0) UseQTY 
                    FROM mesMaterialUnit B WHERE B.ParentID={0}", MaterialUnitID);

            DataTable dt = Data_Table(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                UseQTY = Convert.ToInt32(dt.Rows[0]["UseQTY"].ToString());
            }
            return UseQTY;
        }

        private int SetMaterailBatchData(string MaterialSN) 
        {
            string S_Sql =
                @"
                    declare @MaterialUnitID int  
                    select   @MaterialUnitID=ID from mesMaterialUnit where SerialNumber='"+ MaterialSN + @"' 

			        UPDATE A SET A.RemainQTY=A.Quantity-isnull((SELECT SUM(isnull(B.Quantity,0))+
				        SUM(isnull(B.BalanceQty,0))FROM mesMaterialUnit B WHERE B.ParentID=A.ID),0)
			        FROM mesMaterialUnit A 
			        WHERE A.ID=@MaterialUnitID
                ";
            var v_Query = DapperConnRead2.Execute(S_Sql);
            return v_Query;
        }

    }
}
