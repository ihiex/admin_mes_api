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
    public class PrintRepository : BaseRepositoryReport<string>, IPrintRepository
    {

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        MSG_Public P_MSG_Public;
        MSG_Sys P_MSG_Sys;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        private MSG_Sys msgSys;

        public PrintRepository()
        {
        }

        public PrintRepository(IDbContextCore dbContext) : base(dbContext)
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
                    List_Login.PrintIPPort=item_StationConfig.ValStr2.Trim();
                    continue;
                }
            }

            List_Login.PrintQTY = 90;
            foreach (var item_StationConfig in List_StationConfig)
            {
                if (item_StationConfig.ValStr1 == "PrintQTY")
                {
                    List_Login.PrintQTY=Convert.ToInt32(item_StationConfig.ValStr2.Trim());
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

            //List<TabVal> List_StationTypeDetail = await Public_Repository.GetStationTypeDetail(List_Login.StationTypeID.ToString());
            //S_ApplicationType = List_StationTypeDetail[0].ValStr3;

            string S_Sql = "SELECT B.[Description] ValStr1 FROM mesStationType A JOIN luApplicationType B ON " +
                " A.ApplicationTypeID=B.ID WHERE A.ID='"+ List_Login.StationTypeID.ToString() + "' ";
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

        public async Task<List<mesLineGroup>> mesLineGroup(string LineType, int PartFamilyTypeID)
        {
            string S_Sql = @"select A.*,B.Description LineName  
                                from  mesLineGroup A join mesLine B on A.LineID=B.ID" +
                            " where LineType='" + LineType + "' and PartFamilyTypeID='" + PartFamilyTypeID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<mesLineGroup>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_LineNumber, string S_UnitStatus, string Type, string S_URL)
        {
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";
            ConfirmPOOutputDto F_ConfirmPOOutputDto = new ConfirmPOOutputDto();
            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;

            string S_LabelPath = "";

            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

            if (Type == "FG" || Type == "UPC" )
            {
                if (S_LineNumber == "" || S_LineNumber == null)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20101, "0", List_Login, "");
                    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputDto;
                }
            }

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);

            // 获取条码生成格式
            string S_SNFormatName = await Public_Repository.GetSNFormatName(
                S_LineNumber, S_PartID, S_PartFamilyID, S_POID, List_Login.StationTypeID.ToString());
            if (S_SNFormatName != "")
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
            else
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_038, "0", List_Login, "");//料号未关联生成SN的格式
                F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                return F_ConfirmPOOutputDto;
            }

            List_SetConfirmPO.LabelPath = S_LabelPath;
            List_SetConfirmPO.SNFormatName= S_SNFormatName;

            if (Type == "FG_DOE"|| Type == "FG_DOENew") 
            {
                int I_PartID = Convert.ToInt32(S_PartID);
                List<mesPartDetail> List_DOE_Pattern = await Public_Repository.GetmesPartDetail(I_PartID, "DOE_Parameter1");

                if (List_DOE_Pattern.Count < 1) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Sys.MSG_Sys_20240+ "  DOE_Parameter1", "0", List_Login, "");
                    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputDto;
                }

                string[] ListPara1 = List_DOE_Pattern[0].Content.Split(',');
                List<string> List_ET=new List<string>();
                foreach (string str in ListPara1)
                {
                    List_ET.Add(str);
                }
                List_SetConfirmPO.ET = List_ET;

                if (Type == "FG_DOE")
                {
                    List<PoDetailConfig> List_PoDetail = await Public_Repository.GetmesPoDetailConfigAsync(S_POID);
                    List_PoDetail = List_PoDetail.Where(a => a.Description == "DOE_ConfigNumber").ToList();

                    if (List_PoDetail.Count > 0)
                    {                        
                        string S_SPAC = List_PoDetail[0].Content;
                        List_SetConfirmPO.SPAC = S_SPAC;
                    }
                }
            }

            return List_SetConfirmPO;
        }


        public async Task<CreateSNOutputDto> CreateSN(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, int I_QTY, string S_LineNumber, ConfirmPOOutputDto v_ConfirmPOOutputDto,
            string ET,string SPAC,
            string B,string PP, string PredictQTY,
            string Type)
        {

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            List<string> ListSN = new List<string>();

            CreateSNOutputDto F_CreateSNOutputDto = new CreateSNOutputDto();
            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
            F_CreateSNOutputDto.LabelPath = v_ConfirmPOOutputDto.LabelPath;

            if (Type == "FG")
            {
                if (I_QTY > List_Login.PrintQTY)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20109 + " " + List_Login.PrintQTY.ToString(), "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            }
            else if (Type == "UPC" || Type=="FG_DOE" || Type == "FG_NPI" || Type == "FG_DOENew") 
            {
                if (I_QTY > List_Login.PrintQTY)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20100 + " " + List_Login.PrintQTY.ToString(), "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            }

            if (Type == "FG" || Type == "UPC" )
            {
                if (S_LineNumber == "" || S_LineNumber == null)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20101, "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            }

            if (Type == "FG_DOE")
            {
                if (ET.Trim().Length != 2) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("ET length must be 2 characters", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (SPAC.Trim().Length != 4)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("SPAC length must be 4 characters", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            }

            if (Type== "FG_DOENew")
            {
                if (!Regex.IsMatch(PredictQTY.Trim(), "^[0-9]*$")) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("Predict QTY must be a positive integer", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                int I_PredictQTY = Convert.ToInt32(PredictQTY);
                if (I_QTY > I_PredictQTY) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("Predict QTY is not enough to print the current number of barcodes, please confirm and reset", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (ET.Trim().Length != 2)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("ColorCode length must be 2 characters", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (SPAC.Trim().Length != 4)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("CCCC length must be 4 characters", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (!Regex.IsMatch(B.Trim(), "[0-9]")) 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("Parameter [Build] can contain only 0 to 9 digits", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }

                if (PP.Trim().Length != 2)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR("PP length must be 2 characters", "0", List_Login, "");
                    F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_CreateSNOutputDto;
                }
            }



            List<TabVal> List_PONumberCheck = await Public_Repository.GetPONumberCheck(S_POID, "1");
            string S_PONumberCheck = List_PONumberCheck[0].ValStr1;

            if (S_PONumberCheck != "1")
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(S_PONumberCheck, "0", List_Login, "");
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
                TabVal_MSGERROR = Public_Repository.GetERROR(P_MSG_Public.MSG_Public_021, "0", List_Login, "");
                F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                return F_CreateSNOutputDto;
            }
            int I_UnitStateID = List_mesUnitState[0].ID;

            string S_SN_Pattern = "";
            List<mesPartDetail> List_mesPartDetail = await Public_Repository.GetmesPartDetail(I_PartID, "SN_Pattern");
            if (List_mesPartDetail.Count > 0)
            {
                S_SN_Pattern = List_mesPartDetail[0].Content.Trim();                
            }


            string S_xmlProduction0rder = "<ProdOrder ProductionOrder=" + "\"" + S_POID + "\"" + "> </ProdOrder>";
            string S_xmlStation = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
            string S_xmlPart = "<Part PartID=" + "\"" + I_PartID + "\"" + "> </Part>";
            string S_xmlExtraData = "<ExtraData LineID=" + "\"" + S_LineNumber + "\"" +
                                             " PartFamilyTypeID=" + "\"" + S_PartFamilyTypeID + "\"";
            if (Type == "FG")
            {
                S_xmlExtraData += " LineType=" + "\"" + "M" + "\"" ;
            }
            else if (Type == "UPC")
            {
                S_xmlExtraData += " LineType=" + "\"" + "K" + "\"";                                 
            }
            else if (Type == "FG_DOE" || Type == "FG_NPI")
            {
                S_xmlExtraData += " LineType=" + "\"" + "M" + "\"" +
                                  " ET=" + "\"" + ET + "\"" +
                                  " SPCA=" + "\"" + SPAC.ToUpper() + "\"";
            }
            else if (Type == "FG_DOENew" )
            {
                S_xmlExtraData += " LineType=" + "\"" + "M" + "\"" +
                                  " ET=" + "\"" + ET + "\"" +
                                  " SPCA=" + "\"" + SPAC.ToUpper() + "\"" +
                                  " PP=" + "\"" + PP.ToUpper() + "\"" +
                                  " B=" + "\"" + B.ToUpper() + "\"";
            }

            S_xmlExtraData += " ></ExtraData>";
            try 
            {
                for (; I_QTY > 0; I_QTY--) 
                {
                    string S_SN = await SNFormat_Repository.GetSNRGetNext(v_ConfirmPOOutputDto.SNFormatName, "0",
                        S_xmlProduction0rder, S_xmlPart, S_xmlStation, S_xmlExtraData);

                    if (S_SN == "") 
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20087, "0", List_Login, "");
                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }

                    if (S_SN.IndexOf("ERROR:") > -1) 
                    {
                        TabVal_MSGERROR = Public_Repository.GetERROR(S_SN, "0", List_Login, "");

                        F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_CreateSNOutputDto;
                    }


                    List<dynamic> List_SN =await Public_Repository.GetData("mesSerialNumber", " Value='"+ S_SN + "'");
                    if (List_SN.Count > 0) 
                    {
                        continue;
                    }

                    if (S_SN_Pattern != "") 
                    {
                        if (!Regex.IsMatch(S_SN, S_SN_Pattern)) 
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(msgSys.MSG_Sys_20027, "0", List_Login, "");
                            F_CreateSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_CreateSNOutputDto;
                        }
                    }

                    ListSN.Add(S_SN);
                }

                F_CreateSNOutputDto.ListSN= ListSN;


                List<mesUnit> List_mesUnit = new List<mesUnit>();
                List<mesUnitDetail> List_mesUnitDetail = new List<mesUnitDetail>();
                List<mesSerialNumber> List_mesSerialNumber = new List<mesSerialNumber>();
                List<mesHistory> List_mesHistory = new List<mesHistory>();

                Boolean B_ReturnValue = true;
                string S_ERROR = "";

                for (int i = 0; i < F_CreateSNOutputDto.ListSN.Count; i++) 
                {
                    mesUnit v_mesUnit = new mesUnit();
                    v_mesUnit.UnitStateID = I_UnitStateID;
                    v_mesUnit.StatusID = 1;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.PanelID = 0;
                    v_mesUnit.LineID = Convert.ToInt32(S_LineNumber);                   
                    v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = I_PartID;
                    v_mesUnit.LooperCount = 1;
                    v_mesUnit.PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    v_mesUnit.StatusID = 1;

                    if (Type == "FG" || Type=="FG_DOE" || Type== "FG_DOENew" || Type == "FG_NPI") 
                    {
                        v_mesUnit.SerialNumberType = 5;
                    }
                    else if (Type == "UPC")
                    {
                        //如下做类别区分，不是实际表要保存的字段
                        v_mesUnit.SNFamilyID = 10;        //UPC
                        v_mesUnit.SerialNumberType = 6;   //UPC SerialNumber
                    }

                    List_mesUnit.Add(v_mesUnit);

                    mesSerialNumber v_mesSerialNumber = new mesSerialNumber();                    
                    v_mesSerialNumber.SerialNumberTypeID = v_mesUnit.SerialNumberType;
                    v_mesSerialNumber.Value = F_CreateSNOutputDto.ListSN[i];
                    List_mesSerialNumber.Add(v_mesSerialNumber);

                    mesUnitDetail msDetail = new mesUnitDetail();                    
                    msDetail.reserved_01 = "";
                    msDetail.reserved_02 = "";
                    msDetail.reserved_03 = "";
                    msDetail.reserved_04 = "";
                    msDetail.reserved_05 = "";
                    List_mesUnitDetail.Add(msDetail);

                    mesHistory v_mesHistory = new mesHistory();
                    v_mesHistory.UnitStateID = v_mesUnit.UnitStateID;
                    v_mesHistory.EmployeeID = v_mesUnit.EmployeeID;
                    v_mesHistory.StationID = v_mesUnit.StationID;
                    v_mesHistory.EnterTime = DateTime.Now;
                    v_mesHistory.ExitTime = DateTime.Now;
                    v_mesHistory.ProductionOrderID = v_mesUnit.ProductionOrderID;
                    v_mesHistory.PartID = Convert.ToInt32(v_mesUnit.PartID);
                    v_mesHistory.LooperCount = 1;
                    v_mesHistory.StatusID = 1;
                    List_mesHistory.Add(v_mesHistory);  
                }

                Tuple<bool, string> Tuple_Commit = await DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd
                    (List_mesUnit, List_mesUnitDetail, List_mesSerialNumber, List_mesHistory);
                B_ReturnValue = Tuple_Commit.Item1;
                S_ERROR = Tuple_Commit.Item2;

                if (B_ReturnValue == false)
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(S_ERROR, "0", List_Login, "");
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




    }
}
