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
    public class MaterialInitialRepository : BaseRepositoryReport<string>, IMaterialInitialRepository
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


        public MaterialInitialRepository()
        {
        }

        public MaterialInitialRepository(IDbContextCore dbContext) : base(dbContext)
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
            string S_PartID, string S_POID, string S_URL)
        {
            //List<dynamic> List_Result = new List<dynamic>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            ConfirmPOOutputMateialDto F_ConfirmPOOutputMateialDto = new ConfirmPOOutputMateialDto();
            F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;

            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, null, List_Login);
            F_ConfirmPOOutputMateialDto.mesProductStructures = List_SetConfirmPO.mesProductStructures;
            F_ConfirmPOOutputMateialDto.Route = List_SetConfirmPO.Route;
            F_ConfirmPOOutputMateialDto.RouteDataDiagram1= List_SetConfirmPO.RouteDataDiagram1;
            F_ConfirmPOOutputMateialDto.RouteDataDiagram2= List_SetConfirmPO.RouteDataDiagram2;
            F_ConfirmPOOutputMateialDto.RouteDetail = List_SetConfirmPO.RouteDetail;
            F_ConfirmPOOutputMateialDto.ProductionOrderQTY = List_SetConfirmPO.ProductionOrderQTY;

            F_ConfirmPOOutputMateialDto.M_UnitConversion_PCS = "";
            F_ConfirmPOOutputMateialDto.MaterialCodeRules = "";
            F_ConfirmPOOutputMateialDto.BatchQTY = "";
            F_ConfirmPOOutputMateialDto.Unit = "pcs";
            F_ConfirmPOOutputMateialDto.LotCode = false;
            F_ConfirmPOOutputMateialDto.TranceCode = false;

            List<luVendor> List_luVerdor =await Public_Repository.GetVendor(S_PartID);
            F_ConfirmPOOutputMateialDto.Vendor = List_luVerdor;

            List<MaterailBomData> List_MaterailBomData=await 
                Public_Repository.GetMaterailBomData(S_PartID,List_Login.StationTypeID.ToString());
            F_ConfirmPOOutputMateialDto.GetMaterailBomData = List_MaterailBomData;

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
                TabVal_MSGERROR =Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20025, "0", List_Login, "");
                F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputMateialDto;
            }
            //批次数量
            List<mesPartDetail> List_MaterialBatchQTY = await Public_Repository.GetmesPartDetail(I_PartID, "MaterialBatchQTY");
            if (List_MaterialBatchQTY.Count > 0)
            {
                string S_MaterialBatchQTY = List_MaterialBatchQTY[0].Content.Trim();
                F_ConfirmPOOutputMateialDto.MaterialBatchQTY = S_MaterialBatchQTY;

                //F_ConfirmPOOutputMateialDto.BatchQTY = S_MaterialBatchQTY;
            }
            else
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20148, "0", List_Login, "");
                F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputMateialDto;
            }
            //是否打印条码
            List<mesPartDetail> List_MaterialLable = await Public_Repository.GetmesPartDetail(I_PartID, "MaterialLable");
            if (List_MaterialLable.Count > 0) 
            {
                string S_MaterialLable = List_MaterialLable[0].Content.Trim();
                F_ConfirmPOOutputMateialDto.MaterialLable = S_MaterialLable;

                string S_LabelName = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                    S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
                F_ConfirmPOOutputMateialDto.LabelPath = S_LabelName;

                if (S_LabelName == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20104, "0", List_Login, "");
                    F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputMateialDto;
                }

            }
            //收料单位转换
            List<mesPartDetail> List_M_UnitConversion_PCS = await Public_Repository.GetmesPartDetail(I_PartID, "M_UnitConversion_PCS");
            if (List_M_UnitConversion_PCS.Count > 0)
            {
                string S_M_UnitConversion_PCS = List_M_UnitConversion_PCS[0].Content.Trim();
                F_ConfirmPOOutputMateialDto.M_UnitConversion_PCS = S_M_UnitConversion_PCS;

                F_ConfirmPOOutputMateialDto.Unit = "m";
            }
            else 
            {
                F_ConfirmPOOutputMateialDto.Unit = "pcs";
            }
            //是否自动识别客户条码
            List<mesPartDetail> List_MaterialAuto = await Public_Repository.GetmesPartDetail(I_PartID, "MaterialAuto");
            if (List_MaterialAuto.Count > 0) 
            {
                string S_MaterialAuto = List_MaterialAuto[0].Content.Trim();
                S_MaterialAuto = S_MaterialAuto ?? "";

                F_ConfirmPOOutputMateialDto.MaterialAuto = S_MaterialAuto;
                if (S_MaterialAuto == "1")
                {                    
                    //条码规则
                    List<mesPartDetail> List_MaterialCodeRules = await Public_Repository.GetmesPartDetail(I_PartID, "MaterialCodeRules");
                    if (List_MaterialCodeRules.Count > 0)
                    {
                        string S_MaterialCodeRules = List_MaterialCodeRules[0].Content.Trim();
                        F_ConfirmPOOutputMateialDto.MaterialCodeRules = S_MaterialCodeRules;

                        F_ConfirmPOOutputMateialDto.TranceCode = true;
                    }
                    else
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20149, "0", List_Login, "");
                        F_ConfirmPOOutputMateialDto.MSG = TabVal_MSGERROR;
                        return F_ConfirmPOOutputMateialDto;
                    }


                    List<mesPartDetail> List_Expires_Time = await Public_Repository.GetmesPartDetail(I_PartID, "Expires_Time");
                    if (List_Expires_Time.Count > 0)
                    {
                        string S_Expires_Time = List_Expires_Time[0].Content.Trim();
                        F_ConfirmPOOutputMateialDto.Expires_Time = S_Expires_Time;
                    }
                    F_ConfirmPOOutputMateialDto.LotCode = false;
                }
                else 
                {
                    F_ConfirmPOOutputMateialDto.TranceCode=false;
                    F_ConfirmPOOutputMateialDto.LotCode = true;
                }
            }

            return F_ConfirmPOOutputMateialDto;
        }

        public async Task<CreateSNOutputDto> SetRegister
            (
            string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string URL,

            string Batch_Pattern,string MaterialBatchQTY,string MaterialLable,string M_UnitConversion_PCS,
            string MaterialAuto,string MaterialCodeRules,Boolean B_LotCode,Boolean B_TranceCode,

            string VendorID,string VendorCode,
            string LotCode, string RollCode, string Unit, string Quantity, string MaterialDate,
            string MPN, string DateCode, string ExpiringTime, string TranceCode, string Type, string Assigned
            )
        {
            PartFamilyTypeID = PartFamilyTypeID ?? "";
            PartFamilyID = PartFamilyID ?? "";
            PartID = PartID ?? "";
            POID = POID ?? "";
            Batch_Pattern = Batch_Pattern ?? "";
            MaterialBatchQTY = MaterialBatchQTY ?? "";
            MaterialLable = MaterialLable ?? "";
            M_UnitConversion_PCS = M_UnitConversion_PCS ?? "";
            MaterialAuto = MaterialAuto ?? "";
            MaterialCodeRules = MaterialCodeRules ?? "";
            //B_LotCode = B_LotCode ?? false;
            //B_TranceCode = B_TranceCode ?? false;
            VendorID = VendorID ?? "";
            VendorCode = VendorCode ?? "";
            LotCode = LotCode ?? "";
            RollCode = RollCode ?? "";
            Unit = Unit ?? "";
            Quantity = Quantity ?? "";
            MaterialDate = MaterialDate ?? "";
            MPN = MPN ?? "";
            DateCode = DateCode ?? "";
            ExpiringTime = ExpiringTime ?? "";
            TranceCode = TranceCode ?? "";
            Type = Type ?? "0";
            Assigned = Assigned ?? "";



            List<string> ListSN = new List<string>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            CreateSNOutputDto F_CreateSNOutputDto = new CreateSNOutputDto();
            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                List<mesMaterialUnit> List_mesMaterialUnit = new List<mesMaterialUnit>();     
                List< mesMaterialUnitDetail > List_mesMaterialUnitDetail=new List<mesMaterialUnitDetail> ();

                List<mesUnit> List_mesUnit = new List<mesUnit>();
                List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
                List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
                List<mesHistory> List_mesHistory = new List<mesHistory>();

                if (VendorID == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20143 , "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;                    
                    return F_CreateSNOutputDto;
                }

                if (LotCode == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20144, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (Unit == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20193, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
                else
                {
                    if (Unit == "m" && M_UnitConversion_PCS == "0")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20194, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                }

                if (Quantity == "")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20145, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (Convert.ToInt32(Quantity) <= 0)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20146, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (ExpiringTime == "" || Convert.ToDateTime(ExpiringTime) < DateTime.Now)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20147, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                string S_BatchSN = LotCode.Trim().ToUpper();
                Batch_Pattern = PublicF.DecryptPassword(Batch_Pattern, S_PwdKey);
                if (!Regex.IsMatch(S_BatchSN, Batch_Pattern))
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20047, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

                    return F_CreateSNOutputDto;
                }

                //MaterialBatchQTY = MaterialBatchQTY == "" ? "0" : MaterialBatchQTY;
                int I_BatchQTY = Convert.ToInt32(Quantity);
                int I_MaterialBatchQTY = Convert.ToInt32(MaterialBatchQTY);
                if (I_BatchQTY > I_MaterialBatchQTY)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20150, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
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

                string S_CreateBathSN = "";
                if (!string.IsNullOrEmpty(S_FormatSN))
                {
                    string S_xmlProduction0rder = null;
                    string S_xmlStation = null;
                    string S_xmlPart = null;

                    string
                    S_xmlExtraData = "<ExtraData BatchNo=" + "\"" + LotCode.ToUpper() + "\" " +
                         "VendorCode =\"" + VendorCode + "\" " +
                        "RollCode =\"" + RollCode.Trim().ToUpper() + "\" " + "> </ExtraData>";

                    S_CreateBathSN = await SNFormat_Repository.GetSNRGetNext(S_FormatSN, "0",
                        S_xmlProduction0rder, S_xmlPart, S_xmlStation, S_xmlExtraData);

                    if (S_CreateBathSN == "")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20087, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                }
                else
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20132, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                //校验批次是否重复
                DataSet dsBatch = Public_Repository.GetMesMaterialUnitByLotCode(PartID, S_CreateBathSN);
                if (dsBatch != null && dsBatch.Tables.Count > 0 && dsBatch.Tables[0].Rows.Count > 0)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20151, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                mesMaterialUnit mesMaterial = new mesMaterialUnit();
                if (B_TranceCode && !string.IsNullOrEmpty(TranceCode))
                {
                    mesMaterial.SerialNumber = TranceCode;
                }
                else
                {
                    mesMaterial.SerialNumber = S_CreateBathSN;
                }

                ListSN.Add(mesMaterial.SerialNumber);
                F_CreateSNOutputDto.ListSN = ListSN;

                //判断条码是否重复
                DataSet DsMaterial = Public_Repository.GetMesMaterialUnitData(mesMaterial.SerialNumber);
                if (DsMaterial != null && DsMaterial.Tables.Count > 0 && DsMaterial.Tables[0].Rows.Count > 0)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20029, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                mesMaterial.VendorID = Convert.ToInt32(VendorID);
                mesMaterial.PartID = Convert.ToInt32(PartID);
                mesMaterial.StatusID = 1;
                mesMaterial.EmployeeID = List_Login.EmployeeID;
                mesMaterial.LotCode = LotCode.ToUpper();
                mesMaterial.DateCode = DateCode;
                mesMaterial.TraceCode = TranceCode;
                mesMaterial.MPN = MPN;

                if (Unit.Trim() == "pcs")
                {
                    mesMaterial.Quantity = Convert.ToInt32(Quantity);
                }
                else
                {
                    mesMaterial.Quantity = Convert.ToInt32(Quantity) * Convert.ToInt32(M_UnitConversion_PCS);
                }

                mesMaterial.RemainQTY = Convert.ToInt32(Quantity);
                mesMaterial.LineID = List_Login.LineID;
                mesMaterial.RollCode = RollCode.Trim().ToUpper();
                if (string.IsNullOrEmpty(ExpiringTime))
                {
                    mesMaterial.ExpiringTime = Convert.ToDateTime("2099-12-31");
                }
                else
                {
                    mesMaterial.ExpiringTime = Convert.ToDateTime(ExpiringTime);
                }
                mesMaterial.MaterialTypeID = 1;
                mesMaterial.StationID = List_Login.StationID;
                mesMaterial.MaterialDateCode = MaterialDate;
                //List_mesMaterialUnit
                List_mesMaterialUnit.Add(mesMaterial);


                mesMaterialUnitDetail mesMaterialDetail = new mesMaterialUnitDetail();
                //mesMaterialDetail.MaterialUnitID = MaterialUnitID;
                mesMaterialDetail.LooperCount = 1;
                mesMaterialDetail.Reserved_02 = "";
                mesMaterialDetail.Reserved_03 = "";
                mesMaterialDetail.Reserved_04 = "";
                mesMaterialDetail.Reserved_05 = "";

                //0: Public 1:指定项目 2:排除项目
                if (Type != "" && Type != "0")
                {
                    if (Type == "1" || Type == "2")
                    {
                        mesMaterialDetail.Reserved_02 = Type;
                    }
                    mesMaterialDetail.Reserved_01 = Assigned;  //PartID
                }
                else 
                {
                    mesMaterialDetail.Reserved_01 = "";
                }
                //List_mesMaterialUnitDetail
                List_mesMaterialUnitDetail.Add(mesMaterialDetail);

                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.UnitStateID = I_UnitStateID;
                v_mesUnit.StatusID = 1;
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.CreationTime = DateTime.Now;
                v_mesUnit.LastUpdate = DateTime.Now;
                v_mesUnit.PanelID = 0;
                v_mesUnit.LineID = List_Login.LineID;
                v_mesUnit.ProductionOrderID =Convert.ToInt32(POID);
                v_mesUnit.RMAID = 0;
                v_mesUnit.PartID = Convert.ToInt32(PartID);
                v_mesUnit.LooperCount = 1;
                //List_mesUnit
                List_mesUnit.Add(v_mesUnit);

                mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
                //v_mesUnitDetail.UnitID = Convert.ToInt32(S_InsertUnit);
                v_mesUnitDetail.reserved_01 = mesMaterial.SerialNumber;
                v_mesUnitDetail.reserved_02 = "";
                v_mesUnitDetail.reserved_03 = "";
                v_mesUnitDetail.reserved_04 = "";
                v_mesUnitDetail.reserved_05 = "";
                //List_mesUnitDetail
                List_mesUnitDetail.Add(v_mesUnitDetail);


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
                //List_mesHistory
                List_mesHistory.Add(v_mesHistory);

                mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                //v_mesSerialNumber.UnitID = Convert.ToInt32(S_InsertUnit);
                v_mesSerialNumber.SerialNumberTypeID = 2;
                v_mesSerialNumber.Value = mesMaterial.SerialNumber;
                //List_mesSerialNumber
                List_mesSerialNumber.Add(v_mesSerialNumber);

                Tuple<Boolean,string> Tuple_Result=await  DataCommit_Repository.SubmitData_MaterialInitial(List_mesMaterialUnit,
                    List_mesMaterialUnitDetail, List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);

                if (Tuple_Result.Item1 == false) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_Result.Item2, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                //条码打印
                if (MaterialLable == "1")
                {
                    try 
                    {
                        string S_LabelName =await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                            PartFamilyID, PartID, POID, List_Login.LineID.ToString());
                        if (S_LabelName == "") 
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20104, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }
                        F_CreateSNOutputDto.LabelPath = S_LabelName;
                    }
                    catch (Exception ex)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }                    
                }
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                return F_CreateSNOutputDto;
            }
            return F_CreateSNOutputDto;
        }

        public async Task<CreateSNOutputDto> RePrint(string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string LotCode, string URL) 
        {
            PartFamilyTypeID = PartFamilyTypeID ?? "";
            PartFamilyID = PartFamilyID ?? "";
            PartID = PartID ?? "";
            POID = POID ?? "";
            LotCode = LotCode ?? "";

            List<string> ListSN = new List<string>();
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            CreateSNOutputDto F_CreateSNOutputDto = new CreateSNOutputDto();
            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                if (string.IsNullOrEmpty(LotCode)) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20144, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;

                    return F_CreateSNOutputDto;
                }
                string S_BatchSN = LotCode;
                string S_PartID = PartID;

                DataSet dsBatch = Public_Repository.GetMesMaterialUnitByLotCode(S_PartID, S_BatchSN);
                if (dsBatch == null || dsBatch.Tables.Count == 0 || dsBatch.Tables[0].Rows.Count == 0)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20028, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                string SN = dsBatch.Tables[0].Rows[0]["SerialNumber"].ToString();
                ListSN.Add(SN);
                F_CreateSNOutputDto.ListSN = ListSN;

                try
                {
                    string S_LabelName = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                        PartFamilyID, PartID, POID, List_Login.LineID.ToString());
                    if (S_LabelName == "")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20104, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }
                    F_CreateSNOutputDto.LabelPath = S_LabelName;

                    List<mesUnitSerialNumber> List_mesUnitSerialNumber= await Public_Repository.GetmesUnitSerialNumber(SN);

                    List<mesHistory> List_mesHistory = new List<mesHistory>(); 
                    mesHistory v_mesHistory = new mesHistory();
                    v_mesHistory.UnitID = Convert.ToInt32(List_mesUnitSerialNumber[0].UnitID);
                    v_mesHistory.UnitStateID = -300;
                    v_mesHistory.EmployeeID = List_Login.EmployeeID;
                    v_mesHistory.StationID = List_mesUnitSerialNumber[0].StationID;

                    v_mesHistory.ProductionOrderID = List_mesUnitSerialNumber[0].ProductionOrderID;
                    v_mesHistory.PartID = Convert.ToInt32(List_mesUnitSerialNumber[0].PartID);
                    v_mesHistory.LooperCount = 1;
                    v_mesHistory.StatusID = 1;

                    List_mesHistory.Add(v_mesHistory);
                    await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_DefectAdd(null, List_mesHistory, null);
                }
                catch (Exception ex)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            } 
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                return F_CreateSNOutputDto;
            }

            return F_CreateSNOutputDto;
        }


        public TranceCodeDto TranceCode_KeyDown(string TranceCode, string MaterialCodeRules, int Expires_Time) 
        {
            TranceCode = TranceCode ?? "";
            MaterialCodeRules = MaterialCodeRules ?? "";
            //MaterialDate = MaterialDate ?? "";

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            TranceCodeDto F_TranceCodeDto = new TranceCodeDto();
            F_TranceCodeDto.MSG = TabVal_MSGERROR;

            try
            {
                if (string.IsNullOrEmpty(MaterialCodeRules)) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20149, "0", List_Login, "");
                    F_TranceCodeDto.MSG = TabVal_MSGERROR;
                    return F_TranceCodeDto;
                }

                string StrTrance = TranceCode;
                string[] strList = MaterialCodeRules.Split(';');

                foreach (string str in strList) 
                {
                    string ControlName = str.Split(':')[0].ToString();
                    string[] strListIndex = str.Split(':')[1].Split(',');
                    int StartIndex = Convert.ToInt32(strListIndex[0].ToString());
                    int Strlength = Convert.ToInt32(strListIndex[1].ToString());
                    string Controlvalue = StrTrance.Substring(StartIndex, Strlength);

                    switch (ControlName) 
                    {
                        case "VendorID":
                            F_TranceCodeDto.VendorCode = Controlvalue;
                            break;
                        case "LotCode":
                            F_TranceCodeDto.LotCode = Controlvalue;
                            break;
                        case "MaterialDate":
                            F_TranceCodeDto.MaterialDate = Controlvalue;                            
                            break;
                        case "DateCode":
                            F_TranceCodeDto.DateCode = Controlvalue;
                            break;
                        case "Quantity":
                            F_TranceCodeDto.Quantity = Controlvalue;
                            break;
                    }
                }

                //计算过期日期
                if (!string.IsNullOrEmpty(F_TranceCodeDto.MaterialDate) && Expires_Time > 0) 
                {
                    DateTime dateTime = DateTime.ParseExact(F_TranceCodeDto.MaterialDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    string Expiring = dateTime.AddMonths(Expires_Time).ToString("yyyyMMdd");
                    F_TranceCodeDto.ExpiringTime = DateTime.ParseExact(Expiring, "yyyyMMdd",
                        System.Globalization.CultureInfo.CurrentCulture).ToString("yyyyMMdd");

                }

            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20078+"  "+ex.ToString(), "0", List_Login, "");
                F_TranceCodeDto.MSG = TabVal_MSGERROR;
                return F_TranceCodeDto;
            }
            return F_TranceCodeDto;
        }
    }
}