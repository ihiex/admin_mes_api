using API_MSG;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
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
    public class OverStationRepository : BaseRepositoryReport<string>, IOverStationRepository
    {
        MSG_Public P_MSG_Public;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        MSG_Sys P_MSG_Sys;

        string S_Batch_Pattern = "";
        string S_SN_Pattern = "";
       
        // TT 盒子绑定使用
        //bool B_IsTTRegistSN = false;              //是否注册SN        
        //string S_SNFormatName = "";               //条码生成格式
        //string S_LabelPath = "";                  //打印文件路径
        //int I_FullBoxQTY = 0;                    //整箱数量

        //int I_CurrQTY = 0;                       //当前装箱数量        
        //string S_BoxType = "";                   //1:只注册 2:注册且系统生成SN 3:系统Machine表存在
        //string S_UnitStatusID = "";
        //string S_BoxUnitID = "";                 //Box UnitID

        //string S_xmlStation = "";
        //string S_xmlPart = "";
        //string S_xmlProdOrder = "";

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;
        
        public OverStationRepository()
        {
        }

        public OverStationRepository(IDbContextCore dbContext) : base(dbContext)
        {            
            DB_Context = dbContext;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language,int I_LineID,int I_StationID,int I_EmployeeID,string S_CurrentLoginIP) 
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
                if (item.ValStr2 != null)
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
            }
            List_Login.TTBoxSN_Pattern = S_TTBoxSN_Pattern;

            string S_IsTTBoxUnpack = "";
            foreach (var item in List_StationTypeDetail)
            {
                if (item.ValStr2 != null)
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
            IEnumerable<dynamic> List_Result=null;
            List<IsCheckPOPN> List_IsCheckPOPN = await Public_Repository.GetPageInitialize(List_Login, S_URL);
            if (List_IsCheckPOPN.Count > 0) 
            {
                if (List_IsCheckPOPN[0].TabVal == null) 
                {
                    S_IsCheckPO= List_IsCheckPOPN[0].IsCheckPO;
                    S_IsCheckPN = List_IsCheckPOPN[0].IsCheckPN;
                    S_ApplicationType = List_IsCheckPOPN[0].ApplicationType;
                    S_IsLegalPage= List_IsCheckPOPN[0].IsLegalPage;
                    S_TTScanType=List_IsCheckPOPN[0].TTScanType;
                    S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                    S_TTBoxType=List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO=List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                    S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                    S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                    S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                    S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;

                    S_IsCheckCardID= List_IsCheckPOPN[0].IsCheckCardID;
                    S_CardIDPattern= List_IsCheckPOPN[0].CardIDPattern;

                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                            "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '"+
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'"+
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                            S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                            S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                            S_JumpStatusID + "' as JumpStatusID,'" +
                            S_JumpUnitStateID + "' as JumpUnitStateID,'" +

                            S_IsCheckCardID + "' as IsCheckCardID,'" +
                            S_CardIDPattern + "' as CardIDPattern,'" +

                            List_Login.PrintIPPort + "' as PrintIPPort, '"+ S_IsPrint + "' as IsPrint"; 
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
            string S_PartID, string S_POID, string S_UnitStatus,string S_URL)
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

        public async Task<ConfirmPOOutputDto_TTBindBox> SetConfirmPO_TTBindBox(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus,string S_URL)
        {
            //List<dynamic> List_Result = new List<dynamic>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            ConfirmPOOutputDto_TTBindBox F_ConfirmPOOutputDto_TTBindBox = new ConfirmPOOutputDto_TTBindBox();
            F_ConfirmPOOutputDto_TTBindBox.MSG = TabVal_MSGERROR;

            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);
            
            //string  S_IsTTRegistSN = "0";              //是否注册SN        
            string S_SNFormatName = "";               //条码生成格式
            string S_LabelPath = "";                  //打印文件路径
            int I_FullBoxQTY = 0;                     //整箱数量
               
            //string S_BoxType = "";                   //1:只注册 2:注册且系统生成SN 3:系统Machine表存在
            //string S_UnitStatusID = "";
            //string S_BoxUnitID = "";                 //Box UnitID

            //获取料号满箱数量
            int I_PartID = Convert.ToInt32(S_PartID);
            List<mesPartDetail> List_FullNumber = await Public_Repository.GetmesPartDetail(I_PartID, "FullNumber");
            if (List_FullNumber.Count > 0)
            {
                I_FullBoxQTY = Convert.ToInt32(List_FullNumber[0].Content.Trim());
            }
            else 
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_032, "0", List_Login, "");//料号未配置参数
                F_ConfirmPOOutputDto_TTBindBox.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputDto_TTBindBox;
            }
             

            //是否自动生成Panel条码
            //List<mesPartDetail> List_TTBoxType = await Public_Repository.GetmesPartDetail(I_PartID, "TTBoxType");
            //if (List_TTBoxType.Count > 0) 
            {
                //S_BoxType= List_TTBoxType[0].Content.Trim();
                if (S_TTBoxType == "2" || S_TTBoxType == "3") 
                {
                    // 获取条码生成格式
                    S_SNFormatName = await Public_Repository.GetSNFormatName(
                        List_Login.LineID.ToString(), S_PartID, S_PartFamilyID, S_POID, List_Login.StationTypeID.ToString());
                    if (S_SNFormatName != "")
                    {
                        S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                            S_PartFamilyID, S_PartID, S_POID, List_Login.LineID.ToString());
                        if (string.IsNullOrEmpty(S_LabelPath)) 
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_037, "0", List_Login, "");//未配置打印文件路径
                            F_ConfirmPOOutputDto_TTBindBox.MSG = TabVal_MSGERROR;
                            return F_ConfirmPOOutputDto_TTBindBox;
                        }
                    }
                    else 
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_038, "0", List_Login, "");//料号未关联生成SN的格式
                        F_ConfirmPOOutputDto_TTBindBox.MSG = TabVal_MSGERROR;
                        return F_ConfirmPOOutputDto_TTBindBox;
                    }
                }
            }

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);


            if (List_SetConfirmPO.MSG.ValStr1!="1")
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(List_SetConfirmPO.MSG.ValStr2, "0", List_Login, "");
                F_ConfirmPOOutputDto_TTBindBox.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputDto_TTBindBox;
            }
            else 
            {
                F_ConfirmPOOutputDto_TTBindBox.mesProductStructures = List_SetConfirmPO.mesProductStructures;
                F_ConfirmPOOutputDto_TTBindBox.Route = List_SetConfirmPO.Route;

                F_ConfirmPOOutputDto_TTBindBox.Route = List_SetConfirmPO.Route;
                F_ConfirmPOOutputDto_TTBindBox.RouteDataDiagram1 = List_SetConfirmPO.RouteDataDiagram1;
                F_ConfirmPOOutputDto_TTBindBox.RouteDataDiagram2 = List_SetConfirmPO.RouteDataDiagram2;
                F_ConfirmPOOutputDto_TTBindBox.RouteDetail = List_SetConfirmPO.RouteDetail;
                F_ConfirmPOOutputDto_TTBindBox.ProductionOrderQTY = List_SetConfirmPO.ProductionOrderQTY;

                F_ConfirmPOOutputDto_TTBindBox.IsTTRegistSN = S_IsTTRegistSN;
                F_ConfirmPOOutputDto_TTBindBox.SNFormatName= S_SNFormatName;
                F_ConfirmPOOutputDto_TTBindBox.LabelPath= S_LabelPath;
                F_ConfirmPOOutputDto_TTBindBox.FullBoxQTY=I_FullBoxQTY.ToString();
                F_ConfirmPOOutputDto_TTBindBox.BoxType = S_TTBoxType;
            }
            return F_ConfirmPOOutputDto_TTBindBox;
        }


        public async Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID,string S_URL)
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
                    S_SN, S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID,S_UnitStatus,S_DefectID, List_Login);
                string S_ScanSNCheck = List_ScanSNCheck[0].ValStr1;

                S_PartID = List_ScanSNCheck[0].PartID;
                S_PartFamilyID= List_ScanSNCheck[0].PartFamilyID;
                S_POID= List_ScanSNCheck[0].ProductionOrderID;
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

            Public_Repository.SetMesLogVal("SN:  " + S_SN+ "  Scan success", "OK", List_Login);
            return F_SetScanSNOutputDto;
        }

        private async Task<List<TabVal>> CheckMixScan(string inputSN)
        {
            string tmpScanType = "0";
            List<TabVal> List_SN = await Public_Repository.MesGetBatchIDByBarcodeSN(inputSN);
            if (List_SN != null && List_SN.Count > 0)
            {
                //过滤设备机器条码
                List<mesSerialNumber> List_mesSerialNumber =await Public_Repository.GetmesSerialNumber(List_SN[0].ValStr2,"");
                
                tmpScanType = List_mesSerialNumber[0].SerialNumberTypeID.ToString() == "9" ? "3" : "-2";
            }
            else
            {
                //查询条码是否有绑定箱号，理论上不论是箱号或者单个产品条码都应该是可以找到记录的。
                List<mesUnitSerialNumber> List_mesUnitSerialNumber = await Public_Repository.GetmesUnitSerialNumber(inputSN);

                if (List_mesUnitSerialNumber.Count <= 0)
                {
                    Console.WriteLine("类型为0时， 条码不存在");
                    //return tmpScanType;
                }
                else
                {
                    string panelId = List_mesUnitSerialNumber[0].PanelID.ToString();
                    //panelId为空并且条码的类型为8 则扫描类型为2， 条码类型不为8，则是扫描的条码是单品条码，且不存在绑定关系 -1
                    //panelId不为空，则扫描的条码为单品条码且与箱号存在绑定关系。
                    panelId = panelId == "0" ? null : panelId;

                    //tmpScanType = string.IsNullOrEmpty(panelId) ?
                    //    (List_mesUnitSerialNumber[0].SerialNumberTypeID.ToString().Trim() == "8" ? "2" : "-1") : "1";

                    if (string.IsNullOrEmpty(panelId))
                    {
                        if (List_mesUnitSerialNumber[0].SerialNumberTypeID.ToString().Trim() == "8" ||
                            List_mesUnitSerialNumber[0].SerialNumberTypeID.ToString().Trim() == "10")
                        {
                            tmpScanType = "2";
                        }                       
                        else if (new string[4] { "0","1", "3", "5" }.Contains(List_mesUnitSerialNumber[0].SerialNumberTypeID.ToString().Trim())) 
                        {
                            tmpScanType = "1";
                        }
                        else
                        {
                            tmpScanType = "-1";
                        }
                    }
                    else 
                    {
                        tmpScanType = "1";
                    }
                }
            }
            string S_Sql = "select '"+ tmpScanType + "' as ValStr1";
            var v_Query= await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);

            return v_Query.ToList();
        }

        private async Task<List<TabVal>> GetTTSNType(string inputSN)
        {
            string S_Sql = "select '0' as ValStr1";
            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            
            try
            {
                switch (S_TTScanType)
                {
                    case "1":
                    case "2":
                    case "3":
                        S_Sql = "select '" + S_TTScanType + "' as ValStr1";
                        v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                        return v_Query.ToList();
                    case "4":
                        v_Query = await CheckMixScan(inputSN);
                        break;
                    default:
                        return v_Query.ToList();
                }
                
                switch (v_Query.ToList()[0].ValStr1)
                {
                    case "1":
                        Console.WriteLine("有绑定，且扫描的是单品条码");
                        break;
                    case "2":
                        Console.WriteLine("");
                        break;
                    case "3":

                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return v_Query.ToList();
        }

        public async Task<string> GetScanTTCardID(string S_CardID, string S_IsCheckCardID, string S_CardIDPattern) 
        {
            string S_Result = "OK";
            S_CardID = S_CardID ?? "";

            if (S_CardID == "") 
            {
                return S_Result = "Card ID cannot be empty";
            }

            if (S_IsCheckCardID == "1") 
            {
                if (!Regex.IsMatch(S_CardID, S_CardIDPattern)) 
                {
                    return S_Result = "Card ID  regular check failed, please confirm.";
                }
            }

            string S_Sql = "select 0 ID,'' Description";
            var v_query = await DapperConnRead2.QueryAsync<IdDescription>(S_Sql, null, null, I_DBTimeout, null);

            return S_Result;
        }

        public async Task<SetScanSNOutputDto> SetScanSNTT(string S_CardID,string S_IsCheckCardID,
            string S_CardIDPattern, string S_SN,string S_URL)
        {
            S_SN = S_SN ?? "";
            S_URL = S_URL ?? "";

            string S_PartFamilyTypeID =  "";
            string S_PartFamilyID =  "";
            string S_PartID = "";
            string S_POID = "";
            string S_UnitStatus =  "1";
            string S_DefectID =  "";

            string S_UnitID = "";
            string S_MachineSN = S_SN;

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();
            List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            SetScanSNOutputDto F_SetScanSNOutputDto = new SetScanSNOutputDto();
            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;

            //List<mesUnit> Data_mesUnit = new List<mesUnit>();
            string S_ReturnValue = "OK";

            try
            {
                //初始化
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

                List<TabVal> List_TTScanType_SN = await CheckMixScan(S_SN);
                string S_TTScanType_SN= List_TTScanType_SN[0].ValStr1;

                if (S_TTScanType != "4")
                {
                    if (S_TTScanType != S_TTScanType_SN)
                    {
                        //扫描类型不匹配
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_057, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                }

                //这个增加判断 成品SN  有没有已经装箱。需要增加一个方法，需要判断 SerialNumberTypeID 0，1，3，5  PanelId！=null
                //  2022-11-02  扫描类型！=4 的
                if (S_TTScanType != "4") 
                {
                    string S_Sql =
                        @"SELECT COUNT(1) AS Valint1 FROM mesSerialNumber A JOIN mesUnit B ON A.UnitID=B.ID
                            WHERE B.PanelID>0
		                          AND A.SerialNumberTypeID IN(0,1,3,5) AND A.[Value]='"+ S_SN + "'";
                    var List_InBox = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                    if (List_InBox.AsList()[0].Valint1 > 0) 
                    {
                        //产品已经装箱
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_058, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }                   
                }

                //TT 专用验证
                List<TabVal> List_TTSNType = await GetTTSNType(S_SN);
                S_TTScanType = List_TTSNType[0].ValStr1;

                if (!new string[3] { "1", "2", "3" }.Contains(S_TTScanType))
                {
                    //条码类型错误
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_025, "0", List_Login, S_SN);
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                if ( S_TTScanType == "2")
                {
                    List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(S_SN);

                    if (v_mesUnitTTBox.Count == 0)
                    {
                        //这个箱子没有绑定的数据
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                    else
                    {
                        if (v_mesUnitTTBox[0].ChildCount == 0)
                        {
                            //这个箱子没有绑定的数据
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }
                }

                if (S_TTScanType == "3")
                {
                    List<TabVal> List_SN = await Public_Repository.MesGetBatchIDByBarcodeSN(S_SN);
                    List_SN = List_SN ?? new List<TabVal>();

                    if (List_SN.Count() < 1)
                    {
                        Tuple<Boolean, string> Tuple_MachineStatus = await SetNullBoxMachineStatus(S_SN);
                        if (Tuple_MachineStatus.Item1 == false)
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_MachineStatus.Item2, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                        //没有找到夹具绑定的条码
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_026, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                    else 
                    {
                        List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(List_SN[0].ValStr1);

                        if (v_mesUnitTTBox.Count == 0)
                        {
                            Tuple<Boolean, string> Tuple_MachineStatus = await SetNullBoxMachineStatus(S_SN);
                            if (Tuple_MachineStatus.Item1 == false)
                            {
                                TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_MachineStatus.Item2, "0", List_Login, S_SN);
                                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                                return F_SetScanSNOutputDto;
                            }

                            //这个箱子没有绑定的数据
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_SN);
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                        else
                        {
                            if (v_mesUnitTTBox[0].ChildCount == 0)
                            {
                                Tuple<Boolean, string> Tuple_MachineStatus = await SetNullBoxMachineStatus(S_SN);
                                if (Tuple_MachineStatus.Item1 == false)
                                {
                                    TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_MachineStatus.Item2, "0", List_Login, S_SN);
                                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                                    return F_SetScanSNOutputDto;

                                }

                                //这个箱子没有绑定的数据
                                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_SN);
                                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                                return F_SetScanSNOutputDto;

                            }
                        }
                    }

                    //得到实际  SN
                    S_SN = List_SN[0].ValStr1;
                    S_UnitID = List_SN[0].ValStr2;

                    List<SNBaseData> List_SNBaseData = await Public_Repository.GetSNBaseData(S_SN);
                    S_PartID = List_SNBaseData[0].PartID.ToString();
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();
                    S_PartFamilyID = List_SNBaseData[0].PartFamilyID.ToString();
                    S_PartFamilyTypeID = List_SNBaseData[0].PartFamilyTypeID.ToString();

                    string S_ProCheck = await Public_Repository.GetMachineToolingCheck(S_MachineSN, "", S_PartID, List_Login);
                    if (S_ProCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ProCheck, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;

                    }
                }
                else 
                {
                    List<mesUnitSerialNumber> List_UnitSN=await Public_Repository.GetmesUnitSerialNumber(S_SN);
                    if (List_UnitSN.Count() < 1) 
                    {
                        //SN不存在
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_027, "0", List_Login, S_SN);
                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_027, "0", List_Login, S_SN);
                        //return List_Result;
                    }
                    //得到实际  SN
                    S_SN = List_UnitSN[0].Value;
                    S_UnitID = List_UnitSN[0].UnitID.ToString();


                    List<SNBaseData> List_SNBaseData = await Public_Repository.GetSNBaseData(S_SN);
                    S_PartID=List_SNBaseData[0].PartID.ToString();
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();
                    S_PartFamilyID = List_SNBaseData[0].PartFamilyID.ToString();
                    S_PartFamilyTypeID= List_SNBaseData[0].PartFamilyTypeID.ToString();
                }


                //获取通用验证信息
                List<TabVal> List_ScanSNCheck = await Public_Repository.GetScanSNCheckTT(
                    S_SN, S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_DefectID, List_Login);
                string S_ScanSNCheck = List_ScanSNCheck[0].ValStr1;
                //string S_ScanSNCheck_MSG = List_ScanSNCheck[0].ValStr2;

                S_PartID = List_ScanSNCheck[0].PartID;
                S_PartFamilyID = List_ScanSNCheck[0].PartFamilyID;
                S_POID = List_ScanSNCheck[0].ProductionOrderID;
                //string S_StatusID=List_ScanSNCheck[0].StatusID;
                int I_UnitStateID = Convert.ToInt32(List_ScanSNCheck[0].UnitStateID);

                if (S_ScanSNCheck == "1")
                {
                    string xmlExtraData = "<ExtraData UnitStateID =\"" + I_UnitStateID.ToString() + "\" " +
                                         "LineID =\"" + List_Login.LineID.ToString() + "\" " +
                                         "EmployeeID =\"" + List_Login.EmployeeID.ToString() + "\"> </ExtraData>";
                    List<TabVal> List_TTCheck = await Public_Repository.List_ExecProc("uspTTCheck",
                            S_SN, S_POID, S_PartID, xmlExtraData, S_TTScanType, List_Login);
                    string S_TTCheck = List_TTCheck[0].ValStr1;
                    if (S_TTCheck != "1")
                    {
                        if (!string.IsNullOrEmpty(List_TTCheck[0].ValStr2))
                        {
                            F_SetScanSNOutputDto.MSG = List_TTCheck[0];
                            return F_SetScanSNOutputDto;
                        }
                        else
                        {
                            // uspTTCheck 检查失败
                            var v_Query_TTCheck = Public_Repository.List_ERROR_TabVal(P_MSG_Public.MSG_Public_027 +
                                "  " + S_TTCheck, "0", List_Login, S_SN);

                            TabVal_MSGERROR = v_Query_TTCheck[0];
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }

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
                    v_mesUnit.PartID = I_PartID;
                    //修改 Unit
                    List_mesUnit.Add(v_mesUnit);


                    S_ReturnValue = await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_TT
                        (List_mesUnit, List_mesHistory, S_TTScanType, S_MachineSN,S_CardID,S_IsCheckCardID, List_Login);

                    if (S_ReturnValue != "OK")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ReturnValue, "0", List_Login, S_SN);
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

                //List<dynamic> List_ProductionOrderQTY = await
                //    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                //List_Result.Add(List_ProductionOrderQTY);
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

        public async Task<SetScanSN_TTRegisterOutputDto> SetScanSN_TTRegister(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
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
            List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
            List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();


            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            SetScanSN_TTRegisterOutputDto F_SetScanSN_TTRegisterOutputDto = new SetScanSN_TTRegisterOutputDto();
            F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;

            Boolean B_ReturnValue = true;
            string S_ERROR = "";

            try
            {
                //初始化
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);
                //正则值
                List<TabVal> List_Pattern = await Public_Repository.GetPattern(S_PartID, List_Login);
                if (List_Pattern[0].ValStr1 == "ERROR")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                    F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTRegisterOutputDto;

                    //List_Result = Public_Repository.List_ERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                    //return List_Result;
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
                    F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTRegisterOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_016, "0", List_Login,S_SN);//条码不能为空.                
                    //return List_Result;
                }

                if (!Regex.IsMatch(S_SN, S_SN_Pattern)) 
                {
                    //正则校验未通过，请确认.  
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_SN);
                    F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTRegisterOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_SN);//正则校验未通过，请确认.                
                    //return List_Result;
                }
                // uspTTCheck 检查 
                List<TabVal> List_TTCheck = await Public_Repository.List_ExecProc("uspTTCheck", S_SN, S_POID, S_PartID, null, null, List_Login);
                string S_TTCheck = List_TTCheck[0].ValStr1;
                
                if (S_TTCheck != "1")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_TTCheck, "0", List_Login, S_SN);
                    F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTRegisterOutputDto;

                    //var v_Query_ProCheck = Public_Repository.List_ERROR_TabVal(S_TTCheck, "0", List_Login, S_SN);
                    //List_Result = new List<dynamic>();
                    //List_Result.Add(v_Query_ProCheck);
                    //return List_Result;
                }

                List<mesUnitSerialNumber> List_UnitSN = await Public_Repository.GetmesUnitSerialNumber(S_SN);
                if (List_UnitSN.Count() < 1)
                {
                    List<TabVal> List_PONumberCheck = await Public_Repository.GetPONumberCheck(S_POID, "1");
                    string S_PONumberCheck = List_PONumberCheck[0].ValStr1;
                    
                    if (S_PONumberCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_PONumberCheck, "0", List_Login, S_SN);
                        F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTRegisterOutputDto;

                        //List_Result = Public_Repository.List_ERROR(S_PONumberCheck, "0", List_Login, S_SN);
                        //return List_Result;
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
                            F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTRegisterOutputDto;

                            //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                            //return List_Result;
                        }
                    }
                    int I_UnitStateID = List_mesUnitState[0].ID;

                    mesUnit v_mesUnit = new mesUnit();

                    v_mesUnit.UnitStateID = I_UnitStateID;
                    v_mesUnit.StatusID = I_StatusID;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.ProductionOrderID = I_ProductionOrderID;
                    v_mesUnit.PanelID = 0;
                    v_mesUnit.LineID = List_Login.LineID;
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                    v_mesUnit.PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    //增加 Unit
                    List_mesUnit.Add(v_mesUnit);

                    mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                    v_mesSerialNumber.SerialNumberTypeID = 0;
                    v_mesSerialNumber.Value = S_SN;
                    //增加 mesSerialNumber
                    List_mesSerialNumber.Add(v_mesSerialNumber);

                    //写入UnitDetail表
                    mesUnitDetail msDetail = new mesUnitDetail();
                    msDetail.reserved_01 = "";
                    msDetail.reserved_02 = "";
                    msDetail.reserved_03 = "";
                    msDetail.reserved_04 = "";
                    msDetail.reserved_05 = "";
                    // 增加 UnitDetail
                    List_mesUnitDetail.Add(msDetail);

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

                    Tuple<bool, string> Tuple_Commit = await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd
                        (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);
                    B_ReturnValue = Tuple_Commit.Item1;
                    S_ERROR = Tuple_Commit.Item2;

                }
                else 
                {
                    var Query_SetScanSNTT = await SetScanSNTT("0","","",S_SN, S_URL);
                    if (Query_SetScanSNTT.MSG.ValStr1 != "1")
                    {
                        F_SetScanSN_TTRegisterOutputDto.MSG = Query_SetScanSNTT.MSG;
                        return F_SetScanSN_TTRegisterOutputDto;

                        //List_Result = new List<dynamic>();
                        //List_Result.Add(Query_SetScanSNTT);
                        //return List_Result;
                    }
                }

                if (B_ReturnValue ==false )
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_ERROR, "0", List_Login, S_SN);
                    F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTRegisterOutputDto;

                    //List_Result = Public_Repository.List_ERROR(S_ERROR, "0", List_Login, S_SN);
                    //return List_Result;
                }

                List<dynamic> List_ProductionOrderQTY = await
                    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                F_SetScanSN_TTRegisterOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;

                //List_Result.Add(List_ProductionOrderQTY);
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                F_SetScanSN_TTRegisterOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSN_TTRegisterOutputDto;

                //List_Result = Public_Repository.List_ERROR(ex.Message, "0", List_Login, S_SN);
                //return List_Result;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);
            return F_SetScanSN_TTRegisterOutputDto;
        }

        private async Task<Tuple<bool, string,string,string>> LoadMainSNData_TTBindBox(string S_BoxSN,string S_SN, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus,string S_BoxType,string S_MachineSN, string S_LabelPath) 
        {
            string S_Result = "1";
            Tuple<bool, string,string, string> Tuple_Result = new Tuple<bool, string,string, string>(false,"", "","");
            string S_PrintData = "";

            try
            {
                List<mesUnit> List_mesUnit = new List<mesUnit>();
                List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
                List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
                List<mesHistory> List_mesHistory = new List<mesHistory>();

                List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                string S_TTBindBoxUnitID = v_mesUnitTTBox[0].ID.ToString();
                int I_FullNumber = v_mesUnitTTBox[0].FullNumber;
                int I_ChildCount = v_mesUnitTTBox[0].ChildCount;
                int I_BoxSNStatus = Convert.ToInt32(v_mesUnitTTBox[0].BoxSNStatus);

                if (I_BoxSNStatus == 2)
                {
                    S_Result = P_MSG_Public.MSG_Public_039; //主条码已经存在,不能重复扫描
                    Tuple_Result = new Tuple<bool, string, string, string>(false, S_Result, "","");
                    return Tuple_Result;
                }

                //判断是否重复
                List<mesSerialNumber> List_SN = await Public_Repository.GetmesSerialNumber("", S_BoxSN);
                if (List_SN != null)
                {
                    if (List_SN.Count > 0)
                    {
                        if (v_mesUnitTTBox.Count > 0)
                        {
                            if (I_ChildCount >= I_FullNumber)
                            {
                                S_Result = P_MSG_Public.MSG_Public_039; //主条码已经存在,不能重复扫描
                                Tuple_Result = new Tuple<bool, string, string, string>(false, S_Result, "", "");
                                return Tuple_Result;
                            }
                        }
                    }
                }

                S_PartID = S_PartID == "" ? "0" : S_PartID;
                S_PartFamilyID = S_PartFamilyID == "" ? "0" : S_PartFamilyID;
                S_POID = S_POID == "" ? "0" : S_POID;

                int I_PartID = Convert.ToInt32(S_PartID);
                int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                int I_LineID = List_Login.LineID;
                int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                int I_ProductionOrderID = Convert.ToInt32(S_POID);
                int I_StatusID = Convert.ToInt32(S_UnitStatus);

                List<IdDescription> List_mesUnitState = new List<IdDescription>();
                List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                    I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);
                if (List_mesUnitState == null)
                {
                    //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                    S_Result =P_MSG_Public.MSG_Public_021;
                    Tuple_Result = new Tuple<bool, string, string, string>(false, S_Result, "", "");
                    return Tuple_Result;
                }
                string S_UnitStateID = List_mesUnitState[0].ID.ToString();

                //////////////////////////////////////////////

                if (I_ChildCount < I_FullNumber)
                {
                    if (S_BoxType == "1" || S_BoxType == "3")
                    {
                        Tuple_Result = new Tuple<bool, string, string, string>(true, v_mesUnitTTBox[0].ID.ToString(), S_UnitStateID,"");

                        if (v_mesUnitTTBox[0].ID > 0)
                        {
                            return Tuple_Result;
                        }
                    }
                    else if (S_BoxType == "2" && I_ChildCount > 0)
                    {
                        string S_PanelUnitID = S_TTBindBoxUnitID;
                        //UnitStatusID = S_UnitStateID;
                        //  //   //   //   //   //  ///   //
                        //打印条码

                        S_PrintData = "PRINT|"+List_Login.Station+"|"+ S_BoxSN + "|"+ S_LabelPath + "|"+S_PartID;
                    }
                }


                mesUnit v_mesUnit = new mesUnit();
                v_mesUnit.UnitStateID = Convert.ToInt32(S_UnitStateID);
                v_mesUnit.StatusID = 1;
                v_mesUnit.StationID = List_Login.StationID;
                v_mesUnit.EmployeeID = List_Login.EmployeeID;
                v_mesUnit.LineID = List_Login.LineID;
                v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                v_mesUnit.PanelID = 0;
                v_mesUnit.RMAID = 0;
                v_mesUnit.LooperCount = 1;

                List_mesUnit.Add(v_mesUnit);

                //PanelUnitID = UnitID;
                //UnitStatusID = S_UnitStateID;

                mesUnitDetail msDetail = new mesUnitDetail();
                if (S_BoxType == "1")
                {
                    msDetail.reserved_01 = "";
                    msDetail.reserved_02 = S_BoxSN;
                    msDetail.reserved_03 = "1";
                }
                else if (S_BoxType == "2")
                {
                    msDetail.reserved_01 = "";
                    msDetail.reserved_02 = S_BoxSN;
                    msDetail.reserved_03 = "1";
                }
                else if (S_BoxType == "3")
                {
                    msDetail.reserved_01 = S_MachineSN;
                    msDetail.reserved_02 = S_BoxSN;
                    msDetail.reserved_03 = "1";
                }
                msDetail.reserved_04 = "1";
                msDetail.reserved_05 = "";

                List_mesUnitDetail.Add(msDetail);

                mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                if (S_BoxType == "3")
                {
                    v_mesSerialNumber.SerialNumberTypeID = 9;
                }
                else if (S_BoxType == "2")
                {
                    v_mesSerialNumber.SerialNumberTypeID = 8;
                }
                else if (S_BoxType == "1")
                {
                    v_mesSerialNumber.SerialNumberTypeID = 10;
                }

                v_mesSerialNumber.Value = S_BoxSN;

                List_mesSerialNumber.Add(v_mesSerialNumber);

                mesHistory v_mesHistory = new mesHistory();
                v_mesHistory.UnitStateID = v_mesUnit.UnitStateID;
                v_mesHistory.EmployeeID = v_mesUnit.EmployeeID;
                v_mesHistory.StationID = v_mesUnit.StationID;
                v_mesHistory.ProductionOrderID = v_mesUnit.ProductionOrderID;
                v_mesHistory.PartID = Convert.ToInt32(v_mesUnit.PartID);
                v_mesHistory.LooperCount = 1;
                v_mesHistory.StatusID = 1;

                List_mesHistory.Add(v_mesHistory);

                Tuple<bool, string> Tuple_Commit = await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd
                    (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);
                Tuple_Result = new Tuple<bool, string, string, string>(Tuple_Commit.Item1, Tuple_Commit.Item2, S_UnitStateID, S_PrintData);

                //  //  //   //  //
                //if (S_BoxType == "2")
                //{
                //    //打印
                //}
            }
            catch (Exception ex) 
            {
                S_Result =ex.Message;
                Tuple_Result = new Tuple<bool, string, string, string>(false, S_Result, "", "");
                return Tuple_Result;
            }

            return Tuple_Result;
        }

        private async Task<List<TabVal>> GetTTBindBoxStatus(string S_PartID,string S_ProductionOrderID)
        {
            string S_Sql = "";
            string S_SerialNumberTypeID = "0";
            if (S_TTBoxType == "1" ) 
            {
                S_SerialNumberTypeID = "10";
            }
            else if ( S_TTBoxType == "2")
            {
                S_SerialNumberTypeID = "8";
            }
            else if (S_TTBoxType == "3") 
            {
                S_SerialNumberTypeID = "9";
            }

            if (List_Login.IsCheckPO == true)
            {
                S_Sql = @"SELECT A.ID ValStr1,C.Value ValStr2 
                        FROM mesUnit A JOIN mesUnitDetail B ON A.ID=B.UnitID 
                            JOIN mesSerialNumber C ON A.ID=C.UnitID  and C.SerialNumberTypeID='"+ S_SerialNumberTypeID + @"' 
                        WHERE A.ProductionOrderID ='" + S_ProductionOrderID + @"' 
                            AND B.reserved_04 = 1 AND A.ID =
                            (
                                SELECT max(A.ID)
                                FROM mesUnit A JOIN mesUnitDetail B ON A.ID = B.UnitID                                             
                                WHERE A.ProductionOrderID ='" + S_ProductionOrderID + @"' AND B.reserved_04 = 1
                                and A.StationID='" + List_Login.StationID.ToString() + @"'
   	                    )";

            }
            else 
            {
                S_Sql = @"SELECT A.ID ValStr1,C.Value ValStr2 
                        FROM mesUnit A JOIN mesUnitDetail B ON A.ID=B.UnitID 
                            JOIN mesSerialNumber C ON A.ID=C.UnitID and C.SerialNumberTypeID='"+ S_SerialNumberTypeID + @"'  
                        WHERE A.PartID ='" + S_PartID + @"' AND B.reserved_04 = 1 AND A.ID =
                            (
                                SELECT max(A.ID)
                                FROM mesUnit A JOIN mesUnitDetail B ON A.ID = B.UnitID                                             
                                WHERE A.PartID ='" + S_PartID + @"' AND B.reserved_04 = 1
                                and A.StationID='" + List_Login.StationID.ToString() + @"'
   	                    )";
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        private async Task<Tuple<bool, string>> SetmesUnitDetail(string S_UnitID) 
        {
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                string S_Sql = "UPDATE mesUnitDetail SET reserved_04 = '2' where reserved_04 = '1' and  UnitID='" + S_UnitID + "'";

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

            }
            catch (Exception ex) 
            {
                string S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }
            return Tuple_Result;
        }

        private async Task<Tuple<bool, string>> SetNullBoxMachineStatus(string S_mesMachineSN)
        {
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                string S_Sql = "UPDATE mesMachine SET StatusID = 1 WHERE SN='" + S_mesMachineSN + "'";

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

            }
            catch (Exception ex)
            {
                string S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }
            return Tuple_Result;
        }


        public async Task<SetScanSN_TTBoxSNOutputDto> SetScanSN_TTBoxSN(string S_BoxSN,string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL,TTBindBox v_TTBindBox)
        {
            S_BoxSN = S_BoxSN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";

            //List<dynamic> List_Result = new List<dynamic>();
            string S_MachineSN = "";
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            SetScanSN_TTBoxSNOutputDto F_SetScanSN_TTBoxSNOutputDto = new SetScanSN_TTBoxSNOutputDto();
            F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                string S_TT_SN = "";

                if (S_BoxSN == "")
                {
                    //条码不能为空.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_BoxSN);
                    F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTBoxSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_BoxSN);
                    //return List_Result;
                }

                if (v_TTBindBox.BoxType == "1")
                {
                    if (!Regex.IsMatch(S_BoxSN, List_Login.TTBoxSN_Pattern))
                    {
                        //正则校验未通过，请确认. 
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_BoxSN);
                        F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTBoxSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_BoxSN);//正则校验未通过，请确认.                
                        //return List_Result;
                    }
                }

                if (v_TTBindBox.BoxType == "3")
                {
                    //List<TabVal> List_ProCheck = await Public_Repository.List_ExecProc("uspMachineToolingCheck",
                    //    S_BoxSN, S_POID, S_PartID, null, List_Login.StationTypeID.ToString(), List_Login);
                    //string S_ProCheck = List_ProCheck[0].ValStr1;

                    string S_ProCheck =await Public_Repository.GetMachineToolingCheck(S_BoxSN, "", S_PartID, List_Login);
                    if (S_ProCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_ProCheck, "0", List_Login, S_BoxSN);
                        F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTBoxSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(S_ProCheck, "0", List_Login, S_BoxSN);
                        //return List_Result;
                    }
                    S_MachineSN=S_BoxSN;

                    string v_Sql = "select reserved_01 ValStr1,reserved_02 ValStr2,reserved_04 ValStr3 from mesUnitDetail where reserved_01='" + 
                        S_MachineSN + "' and reserved_04='1'";
                    var v_Query = await DapperConnRead2.QueryAsync<TabVal>(v_Sql, null, null, I_DBTimeout, null);

                    if (v_Query.Count() > 0)
                    {
                        S_TT_SN = v_Query.AsList()[0].ValStr2;
                    }
                    else
                    {
                        string S_xmlPart = "'<BoxSN SN=" + "\"" + S_BoxSN + "\"" + "> </BoxSN>'";
                        S_TT_SN = await SNFormat_Repository.GetSNRGetNext(v_TTBindBox.SNFormatName, "0", null, S_xmlPart, null, null);
                    }

                    S_BoxSN = S_TT_SN;
                }
                else
                {
                    S_TT_SN = S_BoxSN;
                }

                string S_xmlExtraData = "<ExtraData LineID =\"" + List_Login.LineID.ToString() + "\" " +
                                         "EmployeeID =\"" + List_Login.EmployeeID.ToString() + "\"> </ExtraData>";
                List<TabVal> List_TTCheck = await Public_Repository.List_ExecProc("uspTTCheck",
                    S_TT_SN, S_POID, S_PartID, S_xmlExtraData, List_Login.StationTypeID.ToString(), List_Login);
                string S_TTCheck = List_TTCheck[0].ValStr1;
                if (S_TTCheck != "1")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_TTCheck, "0", List_Login, S_BoxSN);
                    F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTBoxSNOutputDto;

                    //var v_Query_ProCheck = Public_Repository.List_ERROR(S_TTCheck, "0", List_Login, S_BoxSN);
                    //List_Result = new List<dynamic>();
                    //List_Result.Add(v_Query_ProCheck);
                    //return List_Result;
                }

                Tuple<bool, string, string,string> Tuple_TTBindBox= await LoadMainSNData_TTBindBox(S_BoxSN, S_TT_SN, S_PartFamilyID,
                    S_PartID, S_POID, S_UnitStatus, v_TTBindBox.BoxType, S_MachineSN,List_Login.PrintIPPort);
                F_SetScanSN_TTBoxSNOutputDto.PrintData = Tuple_TTBindBox.Item4;


                Boolean B_LoadSN = Tuple_TTBindBox.Item1;
                if (B_LoadSN == true)
                {
                    if (v_TTBindBox.BoxType == "3")
                    {
                        string S_ModMachine = await DataCommit_Repository.SubmitData_MesModMachineBySNStationTypeID(S_MachineSN, List_Login);
                        if (S_ModMachine != "OK")
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(S_ModMachine, "0", List_Login, S_BoxSN);
                            F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTBoxSNOutputDto;

                            //var v_Query_ProCheck = Public_Repository.List_ERROR(S_ModMachine, "0", List_Login, S_BoxSN);
                            //List_Result = new List<dynamic>();
                            //List_Result.Add(v_Query_ProCheck);
                            //return List_Result;
                        }
                        // //  //   //
                        //Edt_SN.Text = PanelSN;
                    }
                    //TabVal v_TabVal_BoxUnitID = new TabVal();
                    //v_TabVal_BoxUnitID.ValStr1= Tuple_TTBindBox.Item2;
                    //List_Result.Add(v_TabVal_BoxUnitID);
                                        
                    //yuebonCacheHelper.Add("Cache_" +List_Login.CurrentLoginIP+"_"+ List_Login.EmployeeID+"_"+ List_Login.StationID
                    //    + "_BoxUnitID", Tuple_TTBindBox.Item2);
                   
                }
                else                                
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_TTBindBox.Item2, "0", List_Login, S_BoxSN);
                    F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTBoxSNOutputDto;

                    //var v_Query = Public_Repository.List_ERROR(Tuple_TTBindBox.Item2, "0", List_Login, S_BoxSN);
                    //List_Result = new List<dynamic>();
                    //List_Result.Add(v_Query);

                    //if (v_TTBindBox.BoxType == "1") 
                    //{                       
                    //    List_Result = Public_Repository.List_ERROR(Tuple_TTBindBox.Item2, "0", List_Login, S_BoxSN);
                    //    return List_Result;
                    //}
                }
            }
            catch (Exception ex) 
            {
                TabVal_MSGERROR = Public_Repository.GetERROR("ERROR:" + ex.Message, "0", List_Login, S_BoxSN);
                F_SetScanSN_TTBoxSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSN_TTBoxSNOutputDto;

                //var v_Query = Public_Repository.List_ERROR("ERROR:"+ex.Message, "0", List_Login, S_BoxSN);
                //List_Result = new List<dynamic>();
                //List_Result.Add(v_Query);
                //return List_Result;
            }

            F_SetScanSN_TTBoxSNOutputDto.BoxSN = S_BoxSN;

            //string S_Sql = "select '" + S_BoxSN + "' as BoxSN";
            //var v_Query_BoxSN = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);            
            //List_Result.Add(v_Query_BoxSN);

            yuebonCacheHelper.Add("Cache_" + List_Login.CurrentLoginIP + "_" + List_Login.EmployeeID + "_" + List_Login.StationID
                + "_BoxUnitSN", S_BoxSN);


            string S_Sql_ChildSN =
                @"SELECT B.[Value] ChildSN FROM mesUnit A 
	                JOIN mesSerialNumber B ON A.ID=B.UnitID
                WHERE A.PanelID IN 
                (
	                SELECT UnitID FROM mesSerialNumber WHERE [Value]='"+ S_BoxSN + @"'
                )";
            var v_Query_ChildSN = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_ChildSN, null, null, I_DBTimeout, null);
            F_SetScanSN_TTBoxSNOutputDto.ChildList = v_Query_ChildSN.AsList();
            //List_Result.Add(v_Query_ChildSN);


            List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
            F_SetScanSN_TTBoxSNOutputDto.BindQTY = v_mesUnitTTBox[0].ChildCount;

            //S_Sql = "select " + v_mesUnitTTBox[0].ChildCount + " as BindQTY";
            //var v_BindQTY = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            //List_Result.Add(v_BindQTY.AsList());

            Public_Repository.SetMesLogVal("BoxSN:  " + S_BoxSN + "  Scan success", "OK", List_Login);
            return F_SetScanSN_TTBoxSNOutputDto;
        }

        public async Task<SetScanSN_TTChildSNOutputDto> SetScanSN_TTChildSN(string S_BoxSN, string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL, TTBindBox v_TTBindBox,Boolean B_IsEndBox)
        {
            S_BoxSN = S_BoxSN ?? "";
            S_SN = S_SN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
            List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();

            //List<dynamic> List_Result = new List<dynamic>();
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            SetScanSN_TTChildSNOutputDto F_SetScanSN_TTChildSNOutputDto = new SetScanSN_TTChildSNOutputDto();
            F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;


            string S_TT_SN = "";
            Tuple<bool, string, string,string> Tuple_TTBindBox = new Tuple<bool, string, string,string>(false, "", "","");
            string S_BoxUnitID = "";
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();

            try
            {

                if (B_IsEndBox == true)
                {
                    if (S_BoxSN == "")
                    {
                        //条码不能为空.
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);
                        //return List_Result;
                    }

                    List<mesUnitTTBox> v_mesUnitTTBox =await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                    if (v_mesUnitTTBox.Count == 0) 
                    {
                        //这个箱子没有绑定的数据
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_BoxSN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_043, "0", List_Login, S_SN);
                        //return List_Result;
                    }

                    S_BoxUnitID = v_mesUnitTTBox[0].ID.ToString();
                    Tuple<Boolean, string> Tuple_End = await SetmesUnitDetail(S_BoxUnitID);
                    if (Tuple_End.Item1 == false)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_End.Item2, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;

                        //List_Result = Public_Repository.List_ERROR(Tuple_End.Item2, "0", List_Login, S_SN);                        
                    }

                    F_SetScanSN_TTChildSNOutputDto.BoxSN= S_BoxSN;
                    //string S_Sql_Result_End = "select '" + S_BoxSN + "' as BoxSN";
                    //var v_Query_BoxSN_End = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_Result_End, null, null, I_DBTimeout, null);
                    //List_Result.Add(v_Query_BoxSN_End);

                    Public_Repository.SetMesLogVal("BoxSN:  " + S_BoxSN + "  tail box binding success", "OK", List_Login);
                    return F_SetScanSN_TTChildSNOutputDto;

                    //return List_Result;
                }

                if (S_SN == "")
                {
                    //条码不能为空.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);//条码不能为空.
                    //return List_Result;
                }

                List<mesUnitSerialNumber> List_SN = await Public_Repository.GetmesUnitSerialNumber(S_SN);
                if (List_SN.Count == 0) 
                {
                    //条码不存在或者状态不符.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_018, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_018, "0", List_Login, S_SN);//条码不存在或者状态不符.
                    //return List_Result;
                }

                if (S_IsCheckPO == "0" )
                {
                    List<SNBaseData> List_SNBaseData = await Public_Repository.GetSNBaseData(S_SN);
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();
                }

                if (S_IsCheckPN == "0")
                {
                    List<SNBaseData> List_SNBaseData = await Public_Repository.GetSNBaseData(S_SN);
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();
                    S_PartID = List_SNBaseData[0].PartID.ToString();
                }

                int I_PartID = Convert.ToInt32(S_PartID);
                int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                int I_LineID = List_Login.LineID;
                int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                int I_ProductionOrderID = Convert.ToInt32(S_POID);
                int I_StatusID = Convert.ToInt32(S_UnitStatus);

                string S_UnitID = List_SN[0].UnitID.ToString();
                List<mesUnit> List_mesUnit_Get = await Public_Repository.GetCheckUnitInfo(S_SN, S_POID, S_PartID);
                if (List_mesUnit_Get.Count == 0) 
                {
                    //SN 和工单料号不匹配.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_056, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_056, "0", List_Login, S_SN);//SN 和工单料号不匹配.
                    //return List_Result;
                }

                string S_RouteCheck = await Public_Repository.GetRouteCheck(I_StationTypeID,
                    List_Login.StationID, List_Login.LineID.ToString(), List_mesUnit_Get[0], S_SN);

                //初始化
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);
                //正则值
                List<TabVal> List_Pattern = await Public_Repository.GetPattern(S_PartID, List_Login);
                if (List_Pattern[0].ValStr2 == "ERROR")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(List_Pattern[0].ValStr1, "0", List_Login, S_SN);
                    //return List_Result;
                }
                else
                {
                    S_Batch_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr1, S_PwdKey);
                    S_SN_Pattern = PublicF.DecryptPassword(List_Pattern[0].ValStr2, S_PwdKey);
                }

                if (!Regex.IsMatch(S_SN, S_SN_Pattern))
                {
                    //正则校验未通过，请确认. 
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_029, "0", List_Login, S_SN);//正则校验未通过，请确认.                
                    //return List_Result;
                }
                // uspTTCheck 检查 
                List<TabVal> List_TTCheck = await Public_Repository.List_ExecProc("uspTTCheck", S_SN, S_POID, S_PartID, null, null, List_Login);
                string S_TTCheck = List_TTCheck[0].ValStr1;

                if (S_TTCheck != "1")
                {                    
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_TTCheck, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = new List<dynamic>();
                    //List_Result.Add(v_Query_ProCheck);
                    //return List_Result;
                }

                List<mesUnitTTBox> v_mesUnitTTBoxCheck = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                if (S_BoxSN != "")
                {                    
                    if ((v_mesUnitTTBoxCheck[0].ChildCount >= v_mesUnitTTBoxCheck[0].FullNumber)||
                         v_mesUnitTTBoxCheck[0].BoxSNStatus=="2")
                    {
                        //TT箱子已经扫描结束
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_042, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_042, "0", List_Login, S_SN);
                        //return List_Result;
                    }
                }


                if (S_BoxSN=="" && v_TTBindBox.BoxType == "2") 
                {
                    if (S_RouteCheck != "1")
                    {
                        List<TabVal> List_TTBindBoxStatus = await GetTTBindBoxStatus(S_PartID, S_POID);
                        if (List_TTBindBoxStatus.Count > 0)
                        {
                            S_BoxUnitID = List_TTBindBoxStatus[0].ValStr1;
                            S_BoxSN = List_TTBindBoxStatus[0].ValStr2;

                            List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                            F_SetScanSN_TTChildSNOutputDto.BindQTY = v_mesUnitTTBox[0].ChildCount;
                            //string S_Sql = "select " + v_mesUnitTTBox[0].ChildCount + " as BindQTY";
                            //var v_BindQTY = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                            //List_Result.Add(v_BindQTY.AsList());

                            //string S_Sql_Old = "select '" + S_BoxSN + "' as BoxSN";
                            //var v_Query_BoxSN_Old = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_Old, null, null, I_DBTimeout, null);
                            //List_Result.Add(v_Query_BoxSN_Old);

                            F_SetScanSN_TTChildSNOutputDto.BoxSN = S_BoxSN;

                            string S_Sql_ChildSN_T2 =
                                @"SELECT B.[Value] ChildSN FROM mesUnit A 
	                                JOIN mesSerialNumber B ON A.ID=B.UnitID
                                WHERE A.PanelID IN 
                                (
	                                SELECT UnitID FROM mesSerialNumber WHERE [Value]='" + S_BoxSN + @"'
                                )";
                            var v_Query_ChildSN_T2 = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_ChildSN_T2, null, null, I_DBTimeout, null);
                            F_SetScanSN_TTChildSNOutputDto.ChildList = v_Query_ChildSN_T2.ToList();
                            //List_Result.Add(v_Query_ChildSN_T2);

                            //var v_MSG_Route = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                            //List_Result.Add(v_MSG_Route);

                            TabVal TabVal_MSG2 = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                            F_SetScanSN_TTChildSNOutputDto.MSG2 = TabVal_MSG2;
                            

                            //yuebonCacheHelper.Add("Cache_" + List_Login.CurrentLoginIP + "_" + List_Login.EmployeeID + "_" + List_Login.StationID
                            //    + "_BoxUnitID", S_BoxUnitID);

                            return F_SetScanSN_TTChildSNOutputDto;
                        }
                        else
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                            F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTChildSNOutputDto;

                            //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                            //return List_Result;
                        }
                    }
                    else 
                    {
                        List<TabVal> List_TTBindBoxStatus = await GetTTBindBoxStatus(S_PartID, S_POID);
                        if (List_TTBindBoxStatus.Count > 0)
                        {
                            S_BoxUnitID = List_TTBindBoxStatus[0].ValStr1;
                            S_BoxSN = List_TTBindBoxStatus[0].ValStr2;
                        }
                    }                    
                }

                if (S_BoxSN!=""  && v_TTBindBox.BoxType == "2")
                {                    
                    if (S_RouteCheck != "1")
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                        //return List_Result;
                    }
                }

                if (v_TTBindBox.BoxType == "3") 
                {                    
                    S_BoxSN= yuebonCacheHelper.Get("Cache_" + List_Login.CurrentLoginIP + "_" + List_Login.EmployeeID + "_" + List_Login.StationID
                        + "_BoxUnitSN").ToString();

                    if (S_BoxUnitID == "")
                    {
                        List<mesSerialNumber> v_mesSerialNumber = await Public_Repository.GetmesSerialNumber("", S_BoxSN);
                        S_BoxUnitID = v_mesSerialNumber[0].UnitID.ToString();
                    }
                }

                //生成BoxSN
                if (string.IsNullOrEmpty(S_BoxSN) && v_TTBindBox.BoxType == "2") 
                {
                    if (v_TTBindBox.SNFormatName == "")
                    {
                        //条码格式没找到  
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_044, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_044, "0", List_Login, S_SN);//条码格式没找到                
                        //return List_Result;
                    }
                    else 
                    {
                        string S_xmlStation = "<Station StationId=\"" + List_Login.StationID.ToString() + "\"> </Station>";
                        string S_xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_POID + "\"> </ProdOrder>";
                        string S_xmlPart = "<Part PartID=\"" + S_PartID + "\"> </Part>";
                        S_TT_SN = await SNFormat_Repository.GetSNRGetNext(v_TTBindBox.SNFormatName, "0", S_xmlProdOrder, S_xmlPart, S_xmlStation, null);

                        if (S_TT_SN == "") 
                        {
                            //条码生成失败..
                            TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_040, "0", List_Login, S_SN);
                            F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTChildSNOutputDto;

                            //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_040, "0", List_Login, S_SN);//条码生成失败..                
                            //return List_Result;
                        }

                        S_BoxSN = S_TT_SN;
                        Tuple_TTBindBox = await LoadMainSNData_TTBindBox(S_BoxSN, S_SN, S_PartFamilyID,
                            S_PartID, S_POID, S_UnitStatus, v_TTBindBox.BoxType,"",List_Login.PrintIPPort);
                        F_SetScanSN_TTChildSNOutputDto.PrintData = Tuple_TTBindBox.Item4;

                        Boolean B_LoadSN = Tuple_TTBindBox.Item1;
                        if (B_LoadSN == true)
                        {
                            S_BoxUnitID = Tuple_TTBindBox.Item2;
                        }
                        else 
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_TTBindBox.Item2, "0", List_Login, S_SN);
                            F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTChildSNOutputDto;

                            //var v_Query = Public_Repository.List_ERROR_TabVal(Tuple_TTBindBox.Item2, "0", List_Login, S_SN);
                            //List_Result = new List<dynamic>();
                            //List_Result.Add(v_Query);
                            //return List_Result;
                        }                        
                    }
                }

                List<IdDescription> List_mesUnitState = new List<IdDescription>();
                List_mesUnitState = await Public_Repository.GetmesUnitState(I_PartID, I_PartFamilyID,
                    I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);
                if (List_mesUnitState == null)
                {
                    //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                    F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSN_TTChildSNOutputDto;

                    //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                    //return List_Result;
                }
                string S_UnitStateID = List_mesUnitState[0].ID.ToString();                
                int I_UnitStateID = Convert.ToInt32(S_UnitStateID);


                if (S_BoxUnitID == "")
                {
                    List<mesSerialNumber> v_mesSerialNumber = await Public_Repository.GetmesSerialNumber("", S_BoxSN);
                    S_BoxUnitID = v_mesSerialNumber[0].UnitID.ToString();
                }

                if (v_TTBindBox.IsTTRegistSN=="1")
                {

                    if (List_SN.Count >= 1)
                    {
                        //不能重复扫描.
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_041, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_041, "0", List_Login, S_SN);//不能重复扫描.                
                        //return List_Result;
                    }

                    mesUnit v_mesUnit = new mesUnit();

                    v_mesUnit.UnitStateID = I_UnitStateID;
                    v_mesUnit.StatusID = I_StatusID;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.ProductionOrderID = I_ProductionOrderID;
                    v_mesUnit.PanelID = Convert.ToInt32(S_BoxUnitID);
                    v_mesUnit.LineID = List_Login.LineID;
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                    v_mesUnit.PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    v_mesUnit.LooperCount = 1;
                    //增加 Unit
                    List_mesUnit.Add(v_mesUnit);

                    mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                    v_mesSerialNumber.SerialNumberTypeID = 0;
                    v_mesSerialNumber.Value = S_SN;
                    //增加 mesSerialNumber
                    List_mesSerialNumber.Add(v_mesSerialNumber);

                    //写入UnitDetail表
                    mesUnitDetail msDetail = new mesUnitDetail();
                    msDetail.reserved_01 = "";
                    msDetail.reserved_02 = "";
                    msDetail.reserved_03 = "";
                    msDetail.reserved_04 = "";
                    msDetail.reserved_05 = "";
                    // 增加 UnitDetail
                    List_mesUnitDetail.Add(msDetail);

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

                    //string
                    //S_ReturnValue = await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd
                    //    (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);

                    Tuple<bool, string> Tuple_Commit = await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd
                        (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);

                    if (Tuple_Commit.Item1 == false)
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_Commit.Item2, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(Tuple_Commit.Item2, "0", List_Login, S_SN);
                        //return List_Result;
                    }

                    v_mesUnitTTBoxCheck = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                    F_SetScanSN_TTChildSNOutputDto.BindQTY= v_mesUnitTTBoxCheck[0].ChildCount;
                    //TabVal v_UnitTTBox = new TabVal();
                    //v_UnitTTBox.Valint1 = v_mesUnitTTBoxCheck[0].ChildCount;
                    //List_Result.Add(v_UnitTTBox);


                }
                else 
                {
                    if (List_SN.Count < 1)
                    {
                        //SN 不存在.  
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_027, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_027, "0", List_Login, S_SN);//SN 不存在.                
                        //return List_Result;
                    }

                    if (S_RouteCheck == "1")
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit.ID= Convert.ToInt32(S_UnitID);
                        v_mesUnit.UnitStateID = I_UnitStateID;
                        v_mesUnit.StatusID = I_StatusID;
                        v_mesUnit.StationID = List_Login.StationID;
                        v_mesUnit.LineID = List_Login.LineID;
                        v_mesUnit.EmployeeID = List_Login.EmployeeID;
                        v_mesUnit.ProductionOrderID =Convert.ToInt32(S_POID);
                        v_mesUnit.PanelID = Convert.ToInt32(S_BoxUnitID);
                        v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                        v_mesUnit.PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                        //Update Unit
                        List_mesUnit.Add(v_mesUnit);

                        mesHistory v_mesHistory = new mesHistory();
                        v_mesHistory.UnitID = Convert.ToInt32(S_UnitID);
                        v_mesHistory.UnitStateID = v_mesUnit.UnitStateID;
                        v_mesHistory.EmployeeID = v_mesUnit.EmployeeID;
                        v_mesHistory.StationID = v_mesUnit.StationID;
                        v_mesHistory.ProductionOrderID = v_mesUnit.ProductionOrderID;
                        v_mesHistory.PartID = Convert.ToInt32(v_mesUnit.PartID);
                        v_mesHistory.StatusID = I_StatusID;
                        v_mesHistory.LooperCount = 1;
                        // Add History
                        List_mesHistory.Add(v_mesHistory);

                        string  S_ReturnValue = await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_DefectAdd
                            (List_mesUnit, List_mesHistory, null);
                        if (S_ReturnValue != "OK") 
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(S_ReturnValue, "0", List_Login, S_SN);
                            F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSN_TTChildSNOutputDto;

                            //List_Result = Public_Repository.List_ERROR(S_ReturnValue, "0", List_Login, S_SN);
                            //return List_Result;
                        }

                        List<mesUnitTTBox> v_mesUnitTTBox = await Public_Repository.GetmesUnitTTBox(S_BoxSN);
                        F_SetScanSN_TTChildSNOutputDto.BindQTY = v_mesUnitTTBox[0].ChildCount;

                        //string S_Sql = "select " + v_mesUnitTTBox[0].ChildCount + " as BindQTY";
                        //var v_BindQTY= await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);                        
                        //List_Result.Add(v_BindQTY.AsList());

                        if ((v_mesUnitTTBox[0].ChildCount>= v_mesUnitTTBox[0].FullNumber) || B_IsEndBox==true)
                        {
                            Tuple<Boolean,string> Tuple_End=await  SetmesUnitDetail(v_mesUnitTTBox[0].ID.ToString());
                            
                            //yuebonCacheHelper.Remove("Cache_" + List_Login.CurrentLoginIP + "_" + List_Login.EmployeeID +
                            //    "_" + List_Login.StationID + "_BoxUnitID");

                            if (Tuple_End.Item1 == false) 
                            {
                                TabVal_MSGERROR = Public_Repository.GetERROR(Tuple_End.Item2, "0", List_Login, S_SN);
                                F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                                return F_SetScanSN_TTChildSNOutputDto;

                                //List_Result = Public_Repository.List_ERROR(Tuple_End.Item2, "0", List_Login, S_SN);
                                //return List_Result;
                            }

                            Public_Repository.SetMesLog("BoxSN:  " + S_BoxSN + " Binding success", "OK", List_Login);
                        }

                    }
                    else 
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                        F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSN_TTChildSNOutputDto;

                        //List_Result = Public_Repository.List_ERROR(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                        //return List_Result;
                    }
                }

            }
            catch (Exception ex) 
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, S_SN);
                F_SetScanSN_TTChildSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSN_TTChildSNOutputDto;

                //List_Result = Public_Repository.List_ERROR(ex.Message, "0", List_Login, S_SN);
                //return List_Result;
            }

            //string S_Sql_Result = "select '" + S_BoxSN + "' as BoxSN";
            //var v_Query_BoxSN = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_Result, null, null, I_DBTimeout, null);
            //List_Result.Add(v_Query_BoxSN);

            F_SetScanSN_TTChildSNOutputDto.BoxSN= S_BoxSN;

            string S_Sql_ChildSN =
                @"SELECT B.[Value] ChildSN FROM mesUnit A 
	                JOIN mesSerialNumber B ON A.ID=B.UnitID
                WHERE A.PanelID IN 
                (
	                SELECT UnitID FROM mesSerialNumber WHERE [Value]='"+ S_BoxSN + @"'
                )";
            var v_Query_ChildSN = await DapperConnRead2.QueryAsync<dynamic>(S_Sql_ChildSN, null, null, I_DBTimeout, null);
            F_SetScanSN_TTChildSNOutputDto.ChildList = v_Query_ChildSN.AsList();
            //List_Result.Add(v_Query_ChildSN);

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);

            return F_SetScanSN_TTChildSNOutputDto;
        }


    }
}
