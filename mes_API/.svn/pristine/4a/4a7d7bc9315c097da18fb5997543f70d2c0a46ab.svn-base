using API_MSG;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using static Dapper.SqlMapper;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.MSGCode;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Quartz.Impl.Triggers;
using System.Data.SqlClient;

namespace SunnyMES.Security.Repositories
{
    public class SiemensRepository : BaseRepositoryReport<string>, ISiemensRepository
    {
        SiemensDB v_SiemensDB;
        string S_ProjectType = "";

        MSG_Public P_MSG_Public;
        MSG_Sys P_MSG_Sys;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;


        IDbContextCore DB_Context;
        LoginList List_Login = new LoginList();

        public SiemensRepository()
        {
        }
        public SiemensRepository(IDbContextCore dbContext) : base(dbContext)
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

                    S_IsCheckCardID = List_IsCheckPOPN[0].IsCheckCardID;
                    S_CardIDPattern = List_IsCheckPOPN[0].CardIDPattern;

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                            "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_IsCheckCardID + "' as IsCheckCardID,'" +
                            S_CardIDPattern + "' as CardIDPattern,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                            S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                            S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                            S_JumpStatusID + "' as JumpStatusID,'" +
                            S_JumpUnitStateID + "' as JumpUnitStateID" 
                           ;
                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                    List_Result = v_Query;
                }
            }
            return List_Result;
        }

        public async Task<List<IdDescription>> GetTemp() 
        {
            string S_Sql = "select 0 ID,'' Description";
            var v_Query = await DapperConnRead2.QueryAsync<IdDescription>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public WH_InDTDto WHIn(string S_MPN, string S_BoxSN, string S_Type)
        {
            WH_InDTDto v_WHIn = new WH_InDTDto(); 

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {                
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }
            S_ProjectType = v_SiemensDB.ProjectType;

            string S_Result = "";
            DataSet DS = new DataSet();

            try
            {
                if (S_ProjectType == "IpadCover")
                {
                    S_Result = WHIn_IpadCover(S_MPN, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "PhoneCase")
                {
                    S_Result = WHIn_PhoneCase(S_MPN, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "PhoneCase2020")
                {
                    S_Result = WHIn_PhoneCase2020(S_MPN, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "KB")
                {
                    S_Result = WHIn_KB(S_MPN, S_BoxSN, S_Type, S_StationTypeID);
                }

                v_WHIn.ScanResult = S_Result;
                v_WHIn.MSG = "";


                try
                {
                    if (S_ProjectType == "IpadCover")
                    {
                        DS = WHIn_IpadCover_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "PhoneCase")
                    {
                        DS = WHIn_PhoneCase_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "PhoneCase2020")
                    {
                        DS = WHIn_PhoneCase2020_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "KB")
                    {
                        DS = WHIn_KB_DT(S_StationTypeID, S_BoxSN);
                    }
                }
                catch 
                { }

                if (S_Result != "OK")
                {
                    switch (S_Result)
                    {
                        case "60000":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60000;
                            break;
                        case "60001":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60001;
                            break;
                        case "60002":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60002;
                            break;
                        case "60003":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60003;
                            break;
                        case "60004":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60004;
                            break;
                        case "60005":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60005;
                            break;
                        case "60006":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60006;
                            break;
                        case "60007":
                            v_WHIn.MSG = P_MSG_Public.MSG_Public_60007;
                            break;
                    }
                }


                if (DS.Tables.Count > 2)
                {
                    List<WH_InDT> List_WH_InDT = DataTableEx.ToList<WH_InDT>(DS.Tables[0]);
                    v_WHIn.WH_InGrid = List_WH_InDT;

                    List<WH_LCount> List_WH_LCount = DataTableEx.ToList<WH_LCount>(DS.Tables[1]);
                    v_WHIn.WH_InCount = List_WH_LCount;

                    int I_LCountS = Convert.ToInt32(DS.Tables[2].Rows[0][0].ToString());
                    v_WHIn.LCountS = I_LCountS;
                }                
            }
            catch { }

            return v_WHIn;
        }

        public WH_InDTDto WHIn_DT( string S_BoxSN)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }
            S_ProjectType = v_SiemensDB.ProjectType;

            DataSet DS = new DataSet();
            try
            {
                if (S_ProjectType == "IpadCover")
                {
                    DS = WHIn_IpadCover_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "PhoneCase")
                {
                    DS = WHIn_PhoneCase_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "PhoneCase2020")
                {
                    DS = WHIn_PhoneCase2020_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "KB")
                {
                    DS = WHIn_KB_DT(S_StationTypeID, S_BoxSN);
                }
            }
            catch
            { }

            WH_InDTDto v_WH_InDTDto = new WH_InDTDto();
            if (DS.Tables.Count > 2) 
            {
                List<WH_InDT> List_WH_InDT = DataTableEx.ToList<WH_InDT>(DS.Tables[0]);
                v_WH_InDTDto.WH_InGrid = List_WH_InDT;

                List<WH_LCount> List_WH_LCount = DataTableEx.ToList<WH_LCount>(DS.Tables[1]);
                v_WH_InDTDto.WH_InCount = List_WH_LCount;

                int I_LCountS=Convert.ToInt32(DS.Tables[2].Rows[0][0].ToString());
                v_WH_InDTDto.LCountS = I_LCountS;
            }  


            return v_WH_InDTDto;
        }

        public WH_OutDTDto WHOut(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type)
        {
            WH_OutDTDto v_WHOut = new WH_OutDTDto(); 

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }
            S_ProjectType = v_SiemensDB.ProjectType;

            string S_Result = "";
            DataSet DS = new DataSet();

            try
            {
                if (S_ProjectType == "IpadCover")
                {
                    S_Result = WHOut_IpadCover(S_MPN, S_BillNo, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "PhoneCase")
                {
                    S_Result = WHOut_PhoneCase(S_MPN, S_BillNo, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "PhoneCase2020")
                {
                    S_Result = WHOut_PhoneCase2020(S_MPN, S_BillNo, S_BoxSN, S_Type, S_StationTypeID);
                }
                else if (S_ProjectType == "KB")
                {
                    S_Result = WHOut_KB(S_MPN, S_BillNo, S_BoxSN, S_Type, S_StationTypeID);
                }

                v_WHOut.ScanResult = S_Result;
                v_WHOut.MSG = "";

                try
                {
                    if (S_ProjectType == "IpadCover")
                    {
                        DS = WHOut_IpadCover_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "PhoneCase")
                    {
                        DS = WHOut_PhoneCase_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "PhoneCase2020")
                    {
                        DS = WHOut_PhoneCase2020_DT(S_StationTypeID, S_BoxSN);
                    }
                    else if (S_ProjectType == "KB")
                    {
                        DS = WHOut_KB_DT(S_StationTypeID, S_BoxSN);
                    }
                }
                catch 
                {

                }

                if (S_Result != "OK")
                {
                    switch (S_Result)
                    {
                        case "60000":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60000;
                            break;
                        case "60001":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60001;
                            break;
                        case "60002":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60002;
                            break;
                        case "60003":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60003;
                            break;
                        case "60004":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60004;
                            break;
                        case "60005":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60005;
                            break;
                        case "60006":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60006;
                            break;
                        case "60007":
                            v_WHOut.MSG = P_MSG_Public.MSG_Public_60007;
                            break;
                    }

                    if (S_Result.Substring(0, 3) != "600")
                    {
                        v_WHOut.MSG = S_Result;
                    }
                }


                if (DS.Tables.Count > 2)
                {
                    List<WH_OutDT> List_WH_OutDT = DataTableEx.ToList<WH_OutDT>(DS.Tables[0]);
                    v_WHOut.WH_OutGrid = List_WH_OutDT;

                    List<WH_LCount> List_WH_LCount = DataTableEx.ToList<WH_LCount>(DS.Tables[1]);
                    v_WHOut.WH_OutCount = List_WH_LCount;

                    int I_LCountS = Convert.ToInt32(DS.Tables[2].Rows[0][0].ToString());
                    v_WHOut.LCountS = I_LCountS;
                }

                string S_Sql = @"select B.* from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID 
                                where A.FStatus=1 and A.FBillNo='" + S_BillNo + "'";
                DataTable DT_BillNo = v_SiemensDB.DB_Data_Table(S_Sql);
                List<WH_BillNo> List_BillNo = DataTableEx.ToList<WH_BillNo>(DT_BillNo);

                v_WHOut.POMPN = List_BillNo;

            }
            catch { }

            return v_WHOut;
        }
        public WH_OutDTDto WHOut_DT( string S_BoxSN)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }
            S_ProjectType = v_SiemensDB.ProjectType;

            DataSet DS = new DataSet();
            try
            {
                if (S_ProjectType == "IpadCover")
                {
                    DS = WHOut_IpadCover_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "PhoneCase")
                {
                    DS = WHOut_PhoneCase_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "PhoneCase2020")
                {
                    DS = WHOut_PhoneCase2020_DT(S_StationTypeID, S_BoxSN);
                }
                else if (S_ProjectType == "KB")
                {
                    DS = WHOut_KB_DT(S_StationTypeID, S_BoxSN);
                }
            }
            catch (Exception ex)
            {
                string ss = ex.ToString();
            }

            WH_OutDTDto v_WH_OutDTDto = new WH_OutDTDto();
            if (DS.Tables.Count > 2)
            {
                List<WH_OutDT> List_WH_OutDT = DataTableEx.ToList<WH_OutDT>(DS.Tables[0]);
                v_WH_OutDTDto.WH_OutGrid = List_WH_OutDT;

                List<WH_LCount> List_WH_LCount = DataTableEx.ToList<WH_LCount>(DS.Tables[1]);
                v_WH_OutDTDto.WH_OutCount = List_WH_LCount;

                int I_LCountS = Convert.ToInt32(DS.Tables[2].Rows[0][0].ToString());
                v_WH_OutDTDto.LCountS = I_LCountS;
            }

            return v_WH_OutDTDto;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////

        public List<WH_BillNo> CheckBillNo(string S_BillNo,  out string S_Result)
        {
            S_Result = "1";

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = @"select B.* from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID 
                                where A.FStatus=1 and A.FBillNo='" + S_BillNo + "'";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            if (DT.Rows.Count == 0)
            {
                S_Result = "60007";   //无效卡板或位审核
            }
            DataSet DS = new DataSet();
            DT.TableName = "DT";
            DS.Tables.Add(DT);

            List<WH_BillNo> List_BillNo = DataTableEx.ToList<WH_BillNo>(DT);

            return List_BillNo;
        }

        public List<WH_IPBB> GetIpad_BB()
        {
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context,List_Login.LanguageID,"Ipad");
            }
            DataSet DS = new DataSet();
            DataTable DT = v_SiemensDB.DB_Data_Table("exec usp_SelectionList 'QueryBBCode',''");
            DT.TableName = "DT_BB";
            DS.Tables.Add(DT);

            List<WH_IPBB> List_WH_IPBB = DataTableEx.ToList<WH_IPBB>(DT);

            return List_WH_IPBB;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private string WHIn_IpadCover(string S_MPN, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";

            try
            {
                if (S_Type == "0") //入库操作
                {
                    string S_Sql = "select * from COSMO_GBF2_PACKING_QTY where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["PACKING_QTY_ID"].ToString();

                        S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

                        S_Sql = "select * from COSMO_RA_Receipt_PC where ReceiptDate is not null and BoxPacking_ID='" + LBoxID + "'";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            S_Result = "60000";  //已入库
                        }
                        else
                        {
                            S_Sql =
                            @"
                                insert into COSMO_RA_Receipt_PC(BoxPacking_ID,PalletID)
                                select  distinct B.PACKING_QTY_ID,C.TOP_Label  from COSMO_GBF2_PACKING_QTY  B  
                                    left join COSMO_GBF2_PACKING C on C.PARENT_LABEL=B.BOX_SN 
                                where not exists(select * from COSMO_RA_Receipt_PC where B.PACKING_QTY_ID=BoxPacking_ID) 
                                    and C.TOP_Label='" + LTop_Label + @"';

                                update A set ReceiptDate=getdate(),FStatus=1,PalletID='" + LTop_Label + "' from COSMO_RA_Receipt_PC A where A.BoxPacking_ID='" + LBoxID + "'";
                            S_Result = v_SiemensDB.ExecSql(S_Sql);
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                else if (S_Type == "1") //反入库操作
                {
                    string S_Sql = "select * from COSMO_GBF2_PACKING_QTY where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["PACKING_QTY_ID"].ToString();

                        S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

                        S_Sql =
                        @"select * from COSMO_GBF2_PACKING_QTY A
                    inner join COSMO_RA_Receipt_PC B on A.PACKING_QTY_ID=B.BoxPacking_ID
                    where  B.ReceiptDate is not null and A.BOX_SN='" + S_BoxSN + @"'
                ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["shippingdate"].ToString().Trim() != "")
                            {
                                S_Result = "60002";  // "已出库";
                            }
                            else
                            {
                                S_Sql =
                                @"update A set ReceiptDate=null,FStatus=0,PalletID=null 
                                from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"'
                        ";
                                S_Result = v_SiemensDB.ExecSql(S_Sql);
                            }
                        }
                        else
                        {
                            S_Result = "60003";  // "未入库";
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        private DataSet WHIn_IpadCover_DT(string S_StationTypeID, string S_BoxSN)
        {
            string S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            //卡板号
            string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

            DataSet DS = new DataSet();
            S_Sql =
            @"SELECT	C.PalletID,A.BOX_SN SerialNumber,
                  case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate
              FROM    COSMO_GBF2_PACKING_QTY A
                  inner join COSMO_RA_Receipt_PC C on C.BoxPacking_ID=A.PACKING_QTY_ID  
              WHERE  C.PalletID='" + LTop_Label + @"'
            ";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "DT_Grid";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select  count(*) LCount,max(C.ReceiptDate) LDate
                    FROM    COSMO_RA_Receipt_PC C
                WHERE C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCount";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select count(*) LCountS
                     FROM    COSMO_RA_Receipt_PC C
                WHERE   C.ReceiptDate is not null and
                C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCountS";
            DS.Tables.Add(DT);

            return DS;
        }
        private string WHIn_PhoneCase(string S_MPN, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";
            if (S_Type == "0") //入库操作
            {
                string S_Sql = "select * from CO_Box_SN where BOX_SN='" + S_BoxSN + "'";
                DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                    S_Sql =
                    @"select a.Box_SN, a.Box_Qty, a.MPN, c. Box_SN as PalletSN from CO_Box_SN a
                          join CO_Packing_SN b on a.BoxID=b.SNID
                          join CO_Box_SN c on c.BoxID=b.ParentSNID
                      where a.Box_SN='" + S_BoxSN + @"' 
                    ";
                    DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                    //卡板号
                    string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                    //MPN
                    //string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                    //if (S_MPN == S_Box_MPN)
                    {
                        S_Sql = "select * from COSMO_RA_Receipt_PC where ReceiptDate is not null and BoxPacking_ID='" + LBoxID + "'";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            S_Result = "60000";  //已入库
                        }
                        else
                        {
                            S_Sql =
                            @"
                        insert into COSMO_RA_Receipt_PC(BoxPacking_ID,PalletID)
                        select distinct a.BoxID, c. Box_SN from CO_Box_SN a
                            join CO_Packing_SN b on a.BoxID=b.SNID
                            join CO_Box_SN c on c.BoxID=b.ParentSNID
                         where not exists(select * from COSMO_RA_Receipt_PC where a.BoxID=BoxPacking_ID) 
                            and C.Box_SN='" + LTop_Label + @"';

                        update A set ReceiptDate=getdate(),FStatus=1,PalletID='" + LTop_Label + "' from COSMO_RA_Receipt_PC A where A.BoxPacking_ID='" + LBoxID + "'";
                            S_Result = v_SiemensDB.ExecSql(S_Sql);
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            else if (S_Type == "1") //反入库操作
            {
                string S_Sql = "select * from CO_Box_SN where BOX_SN='" + S_BoxSN + "'";
                DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                    S_Sql = @"select a.Box_SN, a.Box_Qty, a.MPN, c. Box_SN as PalletSN from CO_Box_SN a
                                join CO_Packing_SN b on a.BoxID=b.SNID
                                join CO_Box_SN c on c.BoxID=b.ParentSNID
                              where a.Box_SN='" + S_BoxSN + @"'
                            ";
                    DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                    //卡板号
                    string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                    //MPN
                    // string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                    //  if (S_MPN == S_Box_MPN)
                    {
                        S_Sql =
                        @"select * from CO_Box_SN A
                        inner join COSMO_RA_Receipt_PC B on A.BoxID=B.BoxPacking_ID 
                      where  B.ReceiptDate is not null and A.BOX_SN='" + S_BoxSN + @"'
                    ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["shippingdate"].ToString().Trim() != "")
                            {
                                S_Result = "60002";  // "已出库";
                            }
                            else
                            {
                                S_Sql =
                                @"update A set ReceiptDate=null,FStatus=0,PalletID=null 
                                  from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"'
                            ";
                                S_Result = v_SiemensDB.ExecSql(S_Sql);
                            }
                        }
                        else
                        {
                            S_Result = "60003";  // "未入库";
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            return S_Result;
        }
        private DataSet WHIn_PhoneCase_DT(string S_StationTypeID, string S_BoxSN)
        {
            string S_Sql = @"	select c.Box_SN TOP_Label from
	                                 CO_Box_SN a join CO_Packing_SN b 
	                                 on a.BoxID = b.SNID join CO_Box_SN c on c.BoxID = b.ParentSNID where 
                            a.Box_SN = '" + S_BoxSN + "'   ";

            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            //卡板号
            string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

            DataSet DS = new DataSet();
            S_Sql =
            @"SELECT	D.PalletID,A.BOX_SN SerialNumber,
                  case when D.ReceiptDate is null then 0 else 1 end isReceipt, D.ReceiptDate
              FROM    CO_Box_SN A
                  join CO_Packing_SN B on A.BoxID=B.SNID
                  join CO_Box_SN C on C.BoxID=B.ParentSNID
                  join COSMO_RA_Receipt_PC D on D.BoxPacking_ID=A.BoxID
              WHERE  D.PalletID='" + LTop_Label + @"'
            ";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "DT_Grid";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select  count(*) LCount,max(C.ReceiptDate) LDate
                    FROM    COSMO_RA_Receipt_PC C
                WHERE C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCount";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select count(*) LCountS
                     FROM    COSMO_RA_Receipt_PC C
                WHERE   C.ReceiptDate is not null and
                C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCountS";
            DS.Tables.Add(DT);

            return DS;
        }


        private string WHIn_KB(string S_MPN, string S_BoxSN, string S_Type, string S_StationTypeID) 
        {
            string S_Result = "";
            if (S_Type == "0") //入库操作
            {
                string S_Sql = @" select * from CO_PKG_Box A 
                                    inner join CO_PKG_BoxPacking  B on  A.BoxID = B.BoxID and B.UnpackDateTime IS NULL
                                  where B.SerialNumber ='" + S_BoxSN + "' and B.BoxType='Pallet'";

                DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                    //卡板号
                    string LBoxPacking_ID = DT_Box.Rows[0]["BoxPacking_ID"].ToString();
                    //MPN

                    //if (S_MPN == S_Box_MPN)
                    {
                        S_Sql = @"select * from CO_PKG_BoxPacking A 
                                  inner join COSMO_RA_Receipt B on A.BoxPacking_ID=B.BoxPacking_ID 
                                 where B.ReceiptDate is not null and A.BoxPacking_ID='" + LBoxPacking_ID + "'";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            S_Result = "60000";  //已入库
                        }
                        else
                        {
                            S_Sql =
                            @"
                         insert into COSMO_RA_Receipt(BoxPacking_ID)
                          select  B.BoxPacking_ID  from CO_PKG_BoxPacking  B 
                            where not exists(select * from COSMO_RA_Receipt where B.BoxPacking_ID=BoxPacking_ID)
                               and B.BoxID='" + LBoxID + @"'
                          update A set ReceiptDate=getdate(),FStatus=1,PalletID='" + LBoxPacking_ID + "' from COSMO_RA_Receipt A where A.BoxPacking_ID='" + LBoxPacking_ID + "'";

                            S_Result = v_SiemensDB.ExecSql(S_Sql);
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            else if (S_Type == "1") //反入库操作
            {
                string S_Sql = @"select * from CO_PKG_Box A 
                                    inner join CO_PKG_BoxPacking  B on  A.BoxID = B.BoxID and B.UnpackDateTime IS NULL 
                                  where B.SerialNumber='" + S_BoxSN + "'";
                DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                    //卡板号
                    string LBoxPacking_ID = DT_Box.Rows[0]["BoxPacking_ID"].ToString();
                    //MPN
                    // string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                    //  if (S_MPN == S_Box_MPN)
                    {
                        S_Sql =
                        @"select * from CO_PKG_BoxPacking A
                        inner join COSMO_RA_Receipt B on A.BoxPacking_ID=B.BoxPacking_ID 
                      where A.UnpackDateTime IS NULL and B.ReceiptDate is not null and A.SerialNumber='" + S_BoxSN + "'";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["shippingdate"].ToString().Trim() != "")
                            {
                                S_Result = "60002";  // "已出库";
                            }
                            else
                            {
                                S_Sql =
                                @"update A set ReceiptDate=null,FStatus=0,PalletID=null 
                                  from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxPacking_ID + @"'
                            ";
                                S_Result = v_SiemensDB.ExecSql(S_Sql);
                            }
                        }
                        else
                        {
                            S_Result = "60003";  // "未入库";
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            return S_Result;
        }
        private DataSet WHIn_KB_DT(string S_StationTypeID, string S_BoxSN)
        {
            string S_Sql = @" select * from CO_PKG_Box A 
                                    inner join CO_PKG_BoxPacking  B on  A.BoxID = B.BoxID and B.UnpackDateTime IS NULL
                                  where B.SerialNumber ='" + S_BoxSN + "' and B.BoxType='Pallet'";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            //箱号内码
            string LBoxID = DT_Top.Rows[0]["BoxID"].ToString();

            //卡板号
            string LBoxPacking_ID = DT_Top.Rows[0]["BoxPacking_ID"].ToString();

            DataSet DS = new DataSet();
            S_Sql =
            @"SELECT	Pallet.BoxSN AS PalletID,PalletBox.SerialNumber, 
                     case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate 
              FROM    dbo.CO_PKG_Box AS Pallet INNER JOIN
                  dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                  inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID 
              WHERE  Pallet.BoxID='" + LBoxID + @"'
            ";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "DT_Grid";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select  count(*) LCount,max(C.ReceiptDate) LDate,max(Pallet.BoxSN) BoxSN
                    FROM    dbo.CO_PKG_Box AS Pallet INNER JOIN 
                        dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                        inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID
                WHERE  (PalletBox.UnpackDateTime IS NULL)  and Pallet.BoxID='" + LBoxID + "'";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCount";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select  count(*) LCountS,max(C.ReceiptDate) LDate,max(Pallet.BoxSN) BoxSN
                    FROM    dbo.CO_PKG_Box AS Pallet INNER JOIN 
                        dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                        inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID
                WHERE  (PalletBox.UnpackDateTime IS NULL)  and Pallet.BoxID='" + LBoxID + "'  and C.ReceiptDate is not null";

            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCountS";
            DS.Tables.Add(DT);

            return DS;
        }

        private string WHIn_PhoneCase2020(string S_MPN, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";
            if (S_Type == "0") //入库操作
            {
                string S_Sql = "select * from mesPackage where SerialNumber='" + S_BoxSN + "'";
                DataTable DT_Box = Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["ID"].ToString();

                    S_Sql =
                    @"select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN
                          from mespackage a
                          join mesPackage b on a.ParentID=b.ID
                          join mesProductionOrder po on a.CurrProductionOrderID=po.ID
                          join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID 
                          join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN'
                      where a.SerialNumber='" + S_BoxSN + @"' 
                    ";
                    DataTable DT_Top = Data_Table(S_Sql);
                    //卡板号
                    string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();

                    //if (S_MPN == S_Box_MPN)
                    {
                        S_Sql = "select * from COSMO_RA_Receipt where ReceiptDate is not null and BoxPacking_ID='" + LBoxID + "'";
                        DataTable DT = Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            S_Result = "60000";  //已入库
                        }
                        else
                        {
                            S_Sql =
                            @"
                        insert into COSMO_RA_Receipt(BoxPacking_ID,PalletID)
                        select distinct a.ID as BoxID, c.SerialNumber as Box_SN from mesPackage a
                            join mesPackage c on c.ID=a.ParentID
                        where not exists(select * from COSMO_RA_Receipt where a.ID=BoxPacking_ID)                         
                            and C.SerialNumber='" + LTop_Label + @"';

                        update A set ReceiptDate=getdate(),FStatus=1,PalletID='" + LTop_Label + "' from COSMO_RA_Receipt A where A.BoxPacking_ID='" + LBoxID + "'";
                            S_Result = ExecSql(S_Sql);
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            else if (S_Type == "1") //反入库操作
            {
                string S_Sql = "select * from mesPackage  where SerialNumber='" + S_BoxSN + "'";
                DataTable DT_Box = Data_Table(S_Sql);

                if (DT_Box.Rows.Count > 0)
                {
                    //箱号内码
                    string LBoxID = DT_Box.Rows[0]["ID"].ToString();

                    S_Sql = @"select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN 
                                 from mespackage a
                                 join mesPackage b on a.ParentID=b.ID 
                                 join mesProductionOrder po on a.CurrProductionOrderID=po.ID
                                 join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID
                                 join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN'
                              where a.SerialNumber='" + S_BoxSN + @"'
                            ";
                    DataTable DT_Top = Data_Table(S_Sql);
                    //卡板号
                    string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                    //MPN
                    // string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                    //  if (S_MPN == S_Box_MPN)
                    {
                        S_Sql =
                        @"select * from mesPackage  A
                        inner join COSMO_RA_Receipt B on A.ID=B.BoxPacking_ID 
                      where  B.ReceiptDate is not null and A.SerialNumber='" + S_BoxSN + @"'
                    ";
                        DataTable DT = Data_Table(S_Sql);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows[0]["shippingdate"].ToString().Trim() != "")
                            {
                                S_Result = "60002";  // "已出库";
                            }
                            else
                            {
                                S_Sql =
                                @"update A set ReceiptDate=null,FStatus=0,PalletID=null 
                                  from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxID + @"'
                                IF EXISTS(SELECT 1 FROM dbo.COSMO_RA_Receipt WHERE BoxPacking_ID = '" + LBoxID + @"' AND TypeID = 1)
                                BEGIN
                                    UPDATE dbo.COSMO_RA_Receipt SET TypeID = null WHERE BoxPacking_ID = '" + LBoxID + @"'
                                END
                            ";
                                S_Result = ExecSql(S_Sql);
                            }
                        }
                        else
                        {
                            S_Result = "60003";  // "未入库";
                        }
                    }
                    //else
                    //{
                    //    S_Result = "60004"; // "MPN不符";
                    //}
                }
                else
                {
                    S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                }
            }
            return S_Result;
        }
        private DataSet WHIn_PhoneCase2020_DT(string S_StationTypeID, string S_BoxSN)
        {
            string S_Sql = "	select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN" +
                            " from mespackage a " +
                            " join mesPackage b on a.ParentID=b.ID" +
                            " join mesProductionOrder po on a.CurrProductionOrderID=po.ID " +
                            " join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID  " +
                            " join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN' " +
                            " where a.SerialNumber = '" + S_BoxSN + "'";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            //卡板号
            string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();

            DataSet DS = new DataSet();
            S_Sql =
            @"SELECT	D.PalletID,A.SerialNumber,
                     case when D.ReceiptDate is null then 0 else 1 end isReceipt, D.ReceiptDate
              FROM    mesPackage A
                  join mesPackage C on A.ParentID=C.ID
                  join COSMO_RA_Receipt D on D.BoxPacking_ID=A.ID                  
              WHERE  D.PalletID='" + LTop_Label + @"'
            ";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "DT_Grid";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select  count(*) LCount,max(C.ReceiptDate) LDate
                    FROM    COSMO_RA_Receipt C
                WHERE C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCount";
            DS.Tables.Add(DT);

            S_Sql =
            @"
                select count(*) LCountS
                     FROM    COSMO_RA_Receipt C
                WHERE   C.ReceiptDate is not null and
                C.PalletID='" + LTop_Label + @"'
            ";
            DT = v_SiemensDB.DB_Data_Table(S_Sql);
            DT.TableName = "LCountS";
            DS.Tables.Add(DT);

            return DS;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        private string WHOut_IpadCover(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";

            try
            {
                string S_Sql_MPN =
                @"select top 1 la.value MPN from  COSMO_KIT_WO_LINKS kit inner join
                QS_LOT_ATTRIBUTES la on kit.KIT_LOT_ID=la.LOT_ID and kit.DELETED=0
                inner join QS_ATTRIBUTES att on att.ATTRIBUTE_ID=la.ATTRIBUTE_ID and att.NAME='Kit_MPN'
                inner join [COSMO_GBF2_PACKING] packing on kit.sn=packing.FG_LABEL and packing.deleted=0
             where packing.PARENT_LABEL='" + S_BoxSN + @"'
            ";
                DataTable DT_MPN = v_SiemensDB.DB_Data_Table(S_Sql_MPN);
                string S_Box_MPN = "";
                if (DT_MPN.Rows.Count > 0)
                {
                    S_Box_MPN = DT_MPN.Rows[0]["MPN"].ToString();
                }

                //if (S_MPN == S_Box_MPN)
                //{
                if (S_Type == "0") //出库操作
                {
                    string S_Sql = "select * from COSMO_GBF2_PACKING_QTY where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["PACKING_QTY_ID"].ToString();

                        S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

                        S_Sql = @"select * from COSMO_GBF2_PACKING_QTY A
                                        inner join COSMO_RA_Receipt_PC B on A.PACKING_QTY_ID=B.BoxPacking_ID
                                      where A.BOX_SN='" + S_BoxSN + @"' 
                                    ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Shippingdate = DT.Rows[0]["shippingdate"].ToString().Trim();

                        if (S_Shippingdate != "")
                        {
                            S_Result = "60002";  //已出库
                        }
                        else
                        {
                            S_Sql = "select * from COSMO_RA_Receipt_PC where BoxPacking_ID='" + LBoxID + "'";
                            DataTable DT_Re = v_SiemensDB.DB_Data_Table(S_Sql);
                            string S_ReceiptDate = DT_Re.Rows[0]["ReceiptDate"].ToString().Trim();

                            if (S_ReceiptDate == "")
                            {
                                S_Result = "60003";  //未入库
                            }
                            else
                            {
                                S_Sql =
                                @"select *  from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                                         where A.FStatus=1  and isnull(B.FStatus,0)<2 and  A.FBillNo='" + S_BillNo + @"'
                                      and B.FMPNNO='" + S_Box_MPN + @"'      
                                    ";
                                DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                                if (DT_FMPNNO.Rows.Count == 0)
                                {
                                    S_Result = "60005";  //MPN已关闭或不正确
                                }
                                else
                                {
                                    string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                                    string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                                    S_Sql =
                                    @"update A set shippingdate=getdate(),FStatus=2,ShipmentSN='" + S_BillNo + @"',FShipmentID=
                                              '" + S_FShipmentDetailID + @"' from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"';

                                          declare   @FCount int
                                          select @FCount=count(*)  from COSMO_RA_Receipt_PC where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                                          update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                                          update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                          S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                                          update A set FStatus=2 from CO_WH_Shipment A where A.FStatus=1
                                                and not exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                                        ";

                                    S_Result = v_SiemensDB.ExecSql(S_Sql);
                                }
                            }
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                else if (S_Type == "1") //反出库操作
                {
                    string S_Sql = "select * from COSMO_GBF2_PACKING_QTY where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["PACKING_QTY_ID"].ToString();

                        S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["TOP_Label"].ToString();

                        S_Sql = @"select * from COSMO_GBF2_PACKING_QTY A
                                        inner join COSMO_RA_Receipt_PC B on A.PACKING_QTY_ID=B.BoxPacking_ID
                                      where A.BOX_SN='" + S_BoxSN + @"' 
                                    ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Shippingdate = DT.Rows[0]["shippingdate"].ToString().Trim();

                        if (S_Shippingdate == "")
                        {
                            S_Result = "60006";  //未出库
                        }
                        else
                        {
                            S_Sql = "select * from COSMO_RA_Receipt_PC where BoxPacking_ID='" + LBoxID + "'";
                            DataTable DT_Re = v_SiemensDB.DB_Data_Table(S_Sql);
                            string S_ReceiptDate = DT_Re.Rows[0]["ReceiptDate"].ToString().Trim();

                            if (S_ReceiptDate == "")
                            {
                                S_Result = "60003";  //未入库
                            }
                            else
                            {
                                S_Sql =
                                @"select *  from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                                         where A.FStatus=1  and  A.FBillNo='" + S_BillNo + @"'
                                      and B.FMPNNO='" + S_Box_MPN + @"'      
                                    ";
                                DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                                if (DT_FMPNNO.Rows.Count == 0)
                                {
                                    S_Result = "60005";  //MPN已关闭或不正确
                                }
                                else
                                {
                                    string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                                    string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                                    S_Sql =
                                    @"
                                            select * from COSMO_GBF2_PACKING_QTY A
                                                inner join COSMO_RA_Receipt_PC B on A.PACKING_QTY_ID=B.BoxPacking_ID
                                            where shippingdate is not null and A.BOX_SN='" + S_BoxSN + @"'
                                        ";
                                    DataTable DT_LBoxID = v_SiemensDB.DB_Data_Table(S_Sql);

                                    if (DT_LBoxID.Rows.Count > 0)
                                    {
                                        LBoxID = DT_LBoxID.Rows[0]["PACKING_QTY_ID"].ToString();

                                        S_Sql =
                        @"update A set shippingdate=null,FStatus=1,ShipmentSN=null,FShipmentID=null
                               from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt_PC where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=1 from CO_WH_Shipment A where A.FStatus=1
                                and  exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                                        ";

                                        S_Result = v_SiemensDB.ExecSql(S_Sql);
                                    }
                                    else
                                    {
                                        S_Result = "60006";  //未出库
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                //}
                //else
                //{
                //    S_Result = "60004"; // "MPN不符";
                //}
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        private DataSet WHOut_IpadCover_DT(string S_StationTypeID, string S_BoxSN)
        {
            DataSet DS = new DataSet();

            string S_Sql = "select * from COSMO_GBF2_PACKING where PARENT_LABEL='" + S_BoxSN + "'";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            //卡板号
            string S_TopSN = DT_Top.Rows[0]["TOP_Label"].ToString();

            S_Sql = @"SELECT row_number() over(PARTITION BY C.PalletID order by C.ReceiptDate Desc) NO,
                                C.BoxPacking_ID BoxID,C.PalletID AS PalletID,PalletBox.BOX_SN SerialNumber,  
                             case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate, 
                             case when C.ShippingDate is null then 0 else 1 end isShipping, 
ISNULL( FORMAT(C.ShippingDate,'yyyy-MM-dd HH:mm:ss'),'') ShippingDate 
                            FROM    dbo.COSMO_GBF2_PACKING_QTY PalletBox
                                 inner join COSMO_RA_Receipt_PC C on C.BoxPacking_ID=PalletBox.PACKING_QTY_ID
                            WHERE  C.PalletID='" + S_TopSN + @"'
                           ";
            DataTable DT_Main = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_Main.TableName = "DT_Grid";
            DS.Tables.Add(DT_Main);

            S_Sql = @"select  count(*) LCount,max(C.ReceiptDate) LDate 
                       FROM  COSMO_RA_Receipt_PC C  WHERE  C.PalletID='" + S_TopSN + @"'
                           ";
            DataTable DT_LCount = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCount.TableName = "DT_LCount";
            DS.Tables.Add(DT_LCount);

            S_Sql = @"select  count(*) LCountS FROM  COSMO_RA_Receipt_PC C
                       WHERE  C.Shippingdate is not null and  C.PalletID='" + S_TopSN + @"'
                           ";
            DataTable DT_LCountS = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCountS.TableName = "DT_LCountS";
            DS.Tables.Add(DT_LCountS);

            return DS;
        }
        private string WHOut_PhoneCase(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";
            try
            {
                if (S_Type == "0") //出库操作
                {
                    string S_Sql = "select * from CO_Box_SN where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                        S_Sql =
                        @"select a.Box_SN, a.Box_Qty, a.MPN, c. Box_SN as PalletSN from CO_Box_SN a
                               join CO_Packing_SN b on a.BoxID=b.SNID join CO_Box_SN c on c.BoxID=b.ParentSNID
                          where a.Box_SN ='" + S_BoxSN + @"'  
                        ";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                        //MPN
                        string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                        //if (S_MPN != S_Box_MPN)
                        //{
                        //    return S_Result = "60004"; // "MPN不符";
                        //}


                        S_Sql = @"select * from CO_Box_SN A
                                inner join COSMO_RA_Receipt_PC B on A.BoxID=B.BoxPacking_ID
                                where A.BOX_SN='" + S_BoxSN + @"' 
                            ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Shippingdate = DT.Rows[0]["shippingdate"].ToString().Trim();

                        if (S_Shippingdate != "")
                        {
                            return S_Result = "60002";  //已出库
                        }

                        S_Sql = "select * from COSMO_RA_Receipt_PC where BoxPacking_ID='" + LBoxID + "'";
                        DataTable DT_Re = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_ReceiptDate = DT_Re.Rows[0]["ReceiptDate"].ToString().Trim();

                        if (S_ReceiptDate == "")
                        {
                            return S_Result = "60003";  //未入库
                        }

                        S_Sql =
                        @"select *  from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                                where A.FStatus=1  and isnull(B.FStatus,0)<2 and  A.FBillNo='" + S_BillNo + @"'
                            and B.FMPNNO='" + S_Box_MPN + @"'      
                        ";
                        DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005";  //MPN已关闭或不正确
                        }

                        string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                        string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                        S_Sql =
                        @"update A set shippingdate=getdate(),FStatus=2,ShipmentSN='" + S_BillNo + @"',FShipmentID=
                                '" + S_FShipmentDetailID + @"' from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt_PC where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=2 from CO_WH_Shipment A where A.FStatus=1
                                and not exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                        return S_Result = v_SiemensDB.ExecSql(S_Sql);
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                else if (S_Type == "1") //反出库操作
                {
                    string S_Sql = "select * from CO_Box_SN where BOX_SN='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();

                        S_Sql =
                        @"select a.Box_SN, a.Box_Qty, a.MPN, c. Box_SN as PalletSN from CO_Box_SN a
                               join CO_Packing_SN b on a.BoxID=b.SNID join CO_Box_SN c on c.BoxID=b.ParentSNID
                          where a.Box_SN ='" + S_BoxSN + @"'  
                        ";
                        DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                        //MPN
                        string S_Box_MPN = "M" + LTop_Label.Substring(1, 3) + "2" + LTop_Label.Substring(4, 2) + "/A";

                        //if (S_MPN != S_Box_MPN)
                        //{
                        //    return S_Result = "60004"; // "MPN不符";
                        //}

                        S_Sql =
                        @"select * from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                            where A.FStatus =1 and  A.FBillNo='" + S_BillNo + @"' and   B.FMPNNO='" + S_Box_MPN + @"'
                        ";
                        DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005"; // "MPN已关闭或不正确";
                        }

                        S_Sql =
                        @"select * from CO_Box_SN A
                                inner join COSMO_RA_Receipt_PC B on A.BoxID=B.BoxPacking_ID
                            where shippingdate is not null and A.BOX_SN='" + S_BoxSN + @"'
                        ";
                        DataTable DT_LBoxID = v_SiemensDB.DB_Data_Table(S_Sql);

                        if (DT_LBoxID.Rows.Count > 0)
                        {
                            string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                            string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                            S_Sql =
                            @"update A set shippingdate=null,FStatus=1,ShipmentSN=null,FShipmentID=null
                               from COSMO_RA_Receipt_PC A where  A.BoxPacking_ID='" + LBoxID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt_PC where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                    S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=1 from CO_WH_Shipment A where A.FStatus=1
                                and  exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                            return S_Result = v_SiemensDB.ExecSql(S_Sql);
                        }
                        else
                        {
                            return S_Result = "60006"; // "未出库";
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        private DataSet WHOut_PhoneCase_DT(string S_StationTypeID, string S_TopSN)
        {
            DataSet DS = new DataSet();

            string S_Sql = @"	select c.Box_SN TOP_Label from
	                                 CO_Box_SN a join CO_Packing_SN b 
	                                 on a.BoxID = b.SNID join CO_Box_SN c on c.BoxID = b.ParentSNID where 
                            a.Box_SN = '" + S_TopSN + "'   ";
            DataTable DT_PalletID = v_SiemensDB.DB_Data_Table(S_Sql);
            string LTop_Label = DT_PalletID.Rows[0]["TOP_Label"].ToString();


            S_Sql = @"SELECT row_number() over(PARTITION BY C.PalletID order by C.ReceiptDate Desc) NO,
                                C.BoxPacking_ID BoxID,C.PalletID AS PalletID,PalletBox.BOX_SN SerialNumber,  
                                case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate, 
                                case when C.ShippingDate is null then 0 else 1 end isShipping, 
ISNULL( FORMAT(C.ShippingDate,'yyyy-MM-dd HH:mm:ss'),'') ShippingDate 
                            FROM dbo.CO_Box_SN PalletBox join dbo.CO_Packing_SN b on PalletBox.BoxID=b.SNID
                                 join dbo.CO_Box_SN cc on cc.BoxID=b.ParentSNID
                                 inner join COSMO_RA_Receipt_PC C on C.BoxPacking_ID=PalletBox.BoxID
                            WHERE  C.PalletID='" + LTop_Label + @"'
                           ";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_Top.TableName = "DT_Grid";
            DS.Tables.Add(DT_Top);

            S_Sql = @"select  count(*) LCount,max(C.ReceiptDate) LDate 
                       FROM  COSMO_RA_Receipt_PC C  WHERE  C.PalletID='" + LTop_Label + @"'
                           ";
            DataTable DT_LCount = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCount.TableName = "DT_LCount";
            DS.Tables.Add(DT_LCount);

            S_Sql = @"select  count(*) LCountS FROM  COSMO_RA_Receipt_PC C
                       WHERE  C.Shippingdate is not null and  C.PalletID='" + LTop_Label + @"'
                           ";
            DataTable DT_LCountS = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCountS.TableName = "DT_LCountS";
            DS.Tables.Add(DT_LCountS);

            return DS;
        }


        private string WHOut_KB(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";
            try
            {
                if (S_Type == "0") //出库操作
                {
                    string S_Sql = @"select * from CO_PKG_Box A
                                        inner join CO_PKG_BoxPacking  B on  A.BoxID = B.BoxID and B.UnpackDateTime IS NULL
                                     where B.SerialNumber='" + S_BoxSN + "' and B.BoxType='Pallet'";

                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //
                        string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();
                        string LBoxPacking_ID = DT_Box.Rows[0]["BoxPacking_ID"].ToString();
                        //if (S_MPN != S_Box_MPN)
                        //{
                        //    return S_Result = "60004"; // "MPN不符";
                        //}


                        S_Sql = @"select * from CO_PKG_BoxPacking A
                                 inner join COSMO_RA_Receipt B on A.BoxPacking_ID=B.BoxPacking_ID
                                where A.SerialNumber='" + S_BoxSN + @"' 
                            ";
                        DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Shippingdate = DT.Rows[0]["shippingdate"].ToString().Trim();

                        if (S_Shippingdate != "")
                        {
                            S_Result = "60002";  //已出库
                        }

                        S_Sql = "select * from COSMO_RA_Receipt where BoxPacking_ID='" + LBoxPacking_ID + "'";
                        DataTable DT_Re = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_ReceiptDate = DT_Re.Rows[0]["ReceiptDate"].ToString().Trim();

                        if (S_ReceiptDate == "")
                        {
                            return S_Result = "60003";  //未入库
                        }


                        S_Sql = @"SELECT  top 1 CO_BOX_KitFG.MPN
                                    FROM    dbo.CO_PKG_BoxPacking AS PalletBox inner join 
                                    dbo.CO_PKG_Box AS Multi ON PalletBox.SerialNumber = Multi.BoxSN INNER JOIN
                                    dbo.CO_PKG_BoxPacking AS MultiBox ON Multi.BoxID = MultiBox.BoxID
                                    inner join CO_BOX_KitFG on MultiBox.SerialNumber=CO_BOX_KitFG.ProductSN  
                                 WHERE   (PalletBox.UnpackDateTime IS NULL) AND (MultiBox.UnpackDateTime IS NULL)
                            and PalletBox.SerialNumber='" + S_BoxSN + "'";
                        DataTable DT_MPN = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Box_MPN = "";
                        if (DT_MPN.Rows.Count > 0)
                        {
                            S_Box_MPN = DT_MPN.Rows[0]["MPN"].ToString();
                        }

                        S_Sql =
                        @"select *  from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                                where A.FStatus=1  and isnull(B.FStatus,0)<2 and  A.FBillNo='" + S_BillNo + @"'
                            and B.FMPNNO='" + S_Box_MPN + @"'      
                        ";
                        DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005";  //MPN已关闭或不正确
                        }

                        string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                        string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                        S_Sql =
                        @"update A set shippingdate=getdate(),FStatus=2,ShipmentSN='" + S_BillNo + @"',FShipmentID=
                                '" + S_FShipmentDetailID + @"' from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=2 from CO_WH_Shipment A where A.FStatus=1
                                and not exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                        return S_Result = v_SiemensDB.ExecSql(S_Sql);
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                else if (S_Type == "1") //反出库操作
                {
                    string S_Sql = @"select * from CO_PKG_Box A
                                        inner join CO_PKG_BoxPacking  B on  A.BoxID = B.BoxID and B.UnpackDateTime IS NULL
                                    where B.SerialNumber='" + S_BoxSN + "'";
                    DataTable DT_Box = v_SiemensDB.DB_Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        string LBoxID = DT_Box.Rows[0]["BoxID"].ToString();
                        string LBoxPacking_ID = DT_Box.Rows[0]["BoxPacking_ID"].ToString();

                        S_Sql = @"SELECT  top 1 CO_BOX_KitFG.MPN
                                    FROM    dbo.CO_PKG_BoxPacking AS PalletBox inner join 
                                    dbo.CO_PKG_Box AS Multi ON PalletBox.SerialNumber = Multi.BoxSN INNER JOIN
                                    dbo.CO_PKG_BoxPacking AS MultiBox ON Multi.BoxID = MultiBox.BoxID
                                    inner join CO_BOX_KitFG on MultiBox.SerialNumber=CO_BOX_KitFG.ProductSN  
                                 WHERE   (PalletBox.UnpackDateTime IS NULL) AND (MultiBox.UnpackDateTime IS NULL)
                            and PalletBox.SerialNumber='" + S_BoxSN + "'";
                        DataTable DT_MPN = v_SiemensDB.DB_Data_Table(S_Sql);
                        string S_Box_MPN = "";
                        if (DT_MPN.Rows.Count > 0)
                        {
                            S_Box_MPN = DT_MPN.Rows[0]["MPN"].ToString();
                        }

                        S_Sql =
                        @"select * from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                            where A.FStatus =1 and  A.FBillNo='" + S_BillNo + @"'   and B.FMPNNO='" + S_Box_MPN + @"'
                        ";
                        DataTable DT_FMPNNO = v_SiemensDB.DB_Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005"; // "MPN已关闭或不正确";
                        }

                        S_Sql =
                        @"select * from CO_PKG_BoxPacking A   
                                inner join COSMO_RA_Receipt B on A.BoxPacking_ID=B.BoxPacking_ID
                            where shippingdate is not null and A.SerialNumber='" + S_BoxSN + @"'
                        ";
                        DataTable DT_LBoxID = v_SiemensDB.DB_Data_Table(S_Sql);

                        if (DT_LBoxID.Rows.Count > 0)
                        {
                            string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                            string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();
                            LBoxID = DT_LBoxID.Rows[0]["BoxID"].ToString();

                            S_Sql =
                            @"update A set shippingdate=null,FStatus=1,ShipmentSN=null,FShipmentID=null
                               from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxPacking_ID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                    S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=1 from CO_WH_Shipment A where A.FStatus=1
                                and  exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                            return S_Result = v_SiemensDB.ExecSql(S_Sql);
                        }
                        else
                        {
                            return S_Result = "60006"; // "未出库";
                        }
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        private DataSet WHOut_KB_DT(string S_StationTypeID, string S_TopSN)
        {
            DataSet DS = new DataSet();

            string S_Sql = @"SELECT	row_number() over(PARTITION BY Pallet.BoxSN order by C.ReceiptDate Desc) NO,
                                   CO_PKG_Box.BoxID,Pallet.BoxSN AS PalletID,PalletBox.SerialNumber,
                                case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate, 
                                case when C.ShippingDate is null then 0 else 1 end isShipping, 
ISNULL( FORMAT(C.ShippingDate,'yyyy-MM-dd HH:mm:ss'),'') ShippingDate 
                            FROM    dbo.CO_PKG_Box INNER JOIN
                                dbo.CO_PKG_BoxPacking ON dbo.CO_PKG_Box.BoxID = dbo.CO_PKG_BoxPacking.BoxID INNER JOIN
                                dbo.CO_PKG_Box AS Pallet ON dbo.CO_PKG_BoxPacking.SerialNumber = Pallet.BoxSN INNER JOIN
                                dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                                inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID
                            WHERE   (PalletBox.UnpackDateTime IS NULL) and Pallet.BoxID='" + S_TopSN + "'";

            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_Top.TableName = "DT_Grid";
            DS.Tables.Add(DT_Top);

            S_Sql = @"select  count(*) LCount,max(C.ReceiptDate) LDate,max(Pallet.BoxSN) BoxSN
                            FROM    dbo.CO_PKG_Box AS Pallet INNER JOIN 
                      dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                      inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID
                    WHERE   (PalletBox.UnpackDateTime IS NULL) and Pallet.BoxID='" + S_TopSN + "'";
            DataTable DT_LCount = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCount.TableName = "DT_LCount";
            DS.Tables.Add(DT_LCount);

            S_Sql = @"select  count(*) LCountS FROM   dbo.CO_PKG_Box AS Pallet INNER JOIN
                       dbo.CO_PKG_BoxPacking AS PalletBox ON Pallet.BoxID = PalletBox.BoxID
                        inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.BoxPacking_ID
                    WHERE   (PalletBox.UnpackDateTime IS NULL) and C.Shippingdate is not null
                        and Pallet.BoxID='" + S_TopSN + "'";
            DataTable DT_LCountS = v_SiemensDB.DB_Data_Table(S_Sql);
            DT_LCountS.TableName = "DT_LCountS";
            DS.Tables.Add(DT_LCountS);

            return DS;
        }

        private string WHOut_PhoneCase2020(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type, string S_StationTypeID)
        {
            string S_Result = "";
            try
            {
                if (S_Type == "0") //出库操作
                {
                    string S_Sql = "select * from mesPackage  where SerialNumber='" + S_BoxSN + "'";
                    DataTable DT_Box = Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["ID"].ToString();

                        S_Sql =
                        @"select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN 
                               from mespackage a join mesPackage b on a.ParentID=b.ID
                               join mesProductionOrder po on a.CurrProductionOrderID=po.ID
                               join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID
                              join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN' 
                          where a.SerialNumber ='" + S_BoxSN + @"'  
                        ";
                        DataTable DT_Top = Data_Table(S_Sql);
                        //卡板号
                        string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                        //MPN
                        string S_Box_MPN = DT_Top.Rows[0]["MPN"].ToString();

                        //if (S_MPN != S_Box_MPN)
                        //{
                        //    return S_Result = "60004"; // "MPN不符";
                        //}


                        S_Sql = @"select * from mesPackage  A
                                inner join COSMO_RA_Receipt B on A.ID=B.BoxPacking_ID
                                where A.SerialNumber='" + S_BoxSN + @"' 
                            ";
                        DataTable DT = Data_Table(S_Sql);
                        string S_Shippingdate = DT.Rows[0]["shippingdate"].ToString().Trim();

                        if (S_Shippingdate != "")
                        {
                            return S_Result = "60002";  //已出库
                        }

                        S_Sql = "select * from COSMO_RA_Receipt where BoxPacking_ID='" + LBoxID + "'";
                        DataTable DT_Re = Data_Table(S_Sql);
                        string S_ReceiptDate = DT_Re.Rows[0]["ReceiptDate"].ToString().Trim();

                        if (S_ReceiptDate == "")
                        {
                            return S_Result = "60003";  //未入库
                        }

                        S_Sql =
                        @"select *  from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                                where A.FStatus=1  and isnull(B.FStatus,0)<2 and  A.FBillNo='" + S_BillNo + @"'
                            and B.FMPNNO='" + S_Box_MPN + @"'      
                        ";
                        DataTable DT_FMPNNO = Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005";  //MPN已关闭或不正确
                        }

                        string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                        string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                        S_Sql =
                        @"update A set shippingdate=getdate(),FStatus=2,ShipmentSN='" + S_BillNo + @"',FShipmentID=
                                '" + S_FShipmentDetailID + @"' from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxID + @"';

                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=2 from CO_WH_Shipment A where A.FStatus=1
                                and not exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                        return S_Result = ExecSql(S_Sql);
                    }
                    else
                    {
                        S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
                else if (S_Type == "1") //反出库操作
                {
                    string S_Sql = "select * from mesPackage  where SerialNumber='" + S_BoxSN + "'";
                    DataTable DT_Box = Data_Table(S_Sql);

                    if (DT_Box.Rows.Count > 0)
                    {
                        //箱号内码
                        string LBoxID = DT_Box.Rows[0]["ID"].ToString();

                        S_Sql = "	select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN" +
                                       " from mespackage a " +
                                       " join mesPackage b on a.ParentID=b.ID" +
                                       " join mesProductionOrder po on a.CurrProductionOrderID=po.ID " +
                                       " join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID  " +
                                       " join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN' " +
                                       " where a.SerialNumber = '" + S_BoxSN + "'";
                        DataTable DT_PalletSN = v_SiemensDB.DB_Data_Table(S_Sql);
                        //string S_PalletSN = DT_PalletSN.Rows[0]["PalletSN"].ToString();

                        //卡板号
                        string LTop_Label = DT_PalletSN.Rows[0]["PalletSN"].ToString();
                        //MPN
                        string S_Box_MPN = DT_PalletSN.Rows[0]["MPN"].ToString();

                        //S_Sql =
                        //@"select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN 
                        //       from mespackage a  join mesPackage b on a.ParentID=b.ID
                        //       join mesProductionOrder po on a.CurrProductionOrderID=po.ID
                        //       join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID
                        //       join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN' 
                        //  where a.SerialNumber ='" + S_PalletSN + @"'  
                        //";
                        //DataTable DT_Top = Data_Table(S_Sql);
                        ////卡板号
                        //string LTop_Label = DT_Top.Rows[0]["PalletSN"].ToString();
                        ////MPN
                        //string S_Box_MPN = DT_Top.Rows[0]["MPN"].ToString();

                        //if (S_MPN != S_Box_MPN)
                        //{
                        //    return S_Result = "60004"; // "MPN不符";
                        //}

                        S_Sql =
                        @"select * from CO_WH_Shipment A inner join CO_WH_ShipmentEntry B on A.FInterID=B.FInterID
                            where A.FStatus =1 and  A.FBillNo='" + S_BillNo + @"' and   B.FMPNNO='" + S_Box_MPN + @"'
                        ";
                        DataTable DT_FMPNNO = Data_Table(S_Sql);
                        if (DT_FMPNNO.Rows.Count == 0)
                        {
                            return S_Result = "60005"; // "MPN已关闭或不正确";
                        }

                        S_Sql =
                        @"select A.* from mesPackage A
                                inner join COSMO_RA_Receipt B on A.Id=B.BoxPacking_ID
                            where shippingdate is not null and A.SerialNumber='" + S_BoxSN + @"'
                        ";
                        DataTable DT_LBoxID = Data_Table(S_Sql);

                        if (DT_LBoxID.Rows.Count > 0)
                        {
                            string S_FShipmentDetailID = DT_FMPNNO.Rows[0]["FDetailID"].ToString();
                            string S_FShipmentInterID = DT_FMPNNO.Rows[0]["FInterID"].ToString();

                            S_Sql =
                            @"update A set shippingdate=null,FStatus=1,ShipmentSN=null,FShipmentID=null
                               from COSMO_RA_Receipt A where  A.BoxPacking_ID='" + LBoxID + @"';
                            IF EXISTS(SELECT 1 FROM dbo.COSMO_RA_Receipt WHERE BoxPacking_ID = '" + LBoxID + @"' AND TypeID = 2)
                            BEGIN
                                UPDATE dbo.COSMO_RA_Receipt SET TypeID = 1 WHERE BoxPacking_ID = '" + LBoxID + @"'
                            END
                            declare   @FCount int
                            select @FCount=count(*)  from COSMO_RA_Receipt where FStatus=2 and FShipmentID='" + S_FShipmentDetailID + @"'
                            update A set FStatus=1,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID='" + S_FShipmentDetailID + @"'

                            update A set FStatus=2,FOutSN=@FCount from CO_WH_ShipmentEntry A where FDetailID= '" +
                                    S_FShipmentDetailID + @"'and FCTN=isnull(@FCount,0)

                            update A set FStatus=1 from CO_WH_Shipment A where A.FStatus=1
                                and  exists(select * from CO_WH_ShipmentEntry where FInterID=A.FInterID 
                                and isnull(FStatus,0)<>2) and FInterID='" + S_FShipmentInterID + @"'
                        ";

                            return S_Result = ExecSql(S_Sql);
                        }
                        else
                        {
                            return S_Result = "60006"; // "未出库";
                        }
                    }
                    else
                    {
                        return S_Result = "60001";  // "找不到对应的产品信息，请重新扫描!";
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        private DataSet WHOut_PhoneCase2020_DT(string S_StationTypeID, string S_TopSN)
        {
            string S_Sql = "	select a.SerialNumber as Box_SN, a.CurrentCount as Box_QTY, pod.Content as MPN, b.SerialNumber as PalletSN" +
                            " from mespackage a " +
                            " join mesPackage b on a.ParentID=b.ID" +
                            " join mesProductionOrder po on a.CurrProductionOrderID=po.ID " +
                            " join mesProductionOrderDetail pod on pod.ProductionOrderID=po.ID  " +
                            " join luProductionOrderDetailDef podd on podd.ID=pod.ProductionOrderDetailDefID and podd.Description='MPN' " +
                            " where a.SerialNumber = '" + S_TopSN + "'";
            DataTable DT_Top = v_SiemensDB.DB_Data_Table(S_Sql);
            string S_PalletSN = DT_Top.Rows[0]["PalletSN"].ToString();

            DataSet DS = new DataSet();

            S_Sql =
                       @"SELECT row_number() over(PARTITION BY C.PalletID order by C.ReceiptDate Desc) NO,
                            C.BoxPacking_ID BoxId,C.PalletID AS PalletID,PalletBox.SerialNumber,
                            case when C.ReceiptDate is null then 0 else 1 end isReceipt, C.ReceiptDate, 
                            case when C.ShippingDate is null then 0 else 1 end isShipping,
ISNULL( FORMAT(C.ShippingDate,'yyyy-MM-dd HH:mm:ss'),'') ShippingDate 
                          FROM    dbo.mesPackage PalletBox

                            join dbo.mesPackage cc on cc.ID=PalletBox.ParentID
                            inner join COSMO_RA_Receipt C on C.BoxPacking_ID=PalletBox.ID
                          where C.PalletID ='" + S_PalletSN + @"'  
                        ";
            DataTable DT_Grid = Data_Table(S_Sql);
            DT_Top.TableName = "DT_Grid";
            DS.Tables.Add(DT_Grid);

            S_Sql = @"select  count(*) LCount,max(C.ReceiptDate) LDate 
                       FROM  COSMO_RA_Receipt C  WHERE  C.PalletID='" + S_PalletSN + @"'
                           ";
            DataTable DT_LCount = Data_Table(S_Sql);
            DT_LCount.TableName = "DT_LCount";
            DS.Tables.Add(DT_LCount);

            S_Sql = @"select  count(*) LCountS FROM  COSMO_RA_Receipt C
                       WHERE  C.Shippingdate is not null and  C.PalletID='" + S_PalletSN + @"'
                           ";
            DataTable DT_LCountS = Data_Table(S_Sql);
            DT_LCountS.TableName = "DT_LCountS";
            DS.Tables.Add(DT_LCountS);

            return DS;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////EDIT  EDIT  EDIT  EDIT/////////////////////////////////////////////////////
        public List<CO_WH_Shipment> GetShipment(string S_Start, string S_End, string FStatus)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = "select *," +
                   "(case " +
                   " when FStatus=0     then    '暂未确认NoConfirm'" +
                   " when FStatus=1     then    '已确认栈板纸ConfirmedOK'" +
                   " when FStatus=2     then    '已扫描出库ShipScanOK'" +
                       " when FStatus=3     then    '已确认出货Shipped'" +
                           " when FStatus=4     then    '已生成EDI'" +
                           " when FStatus=5     then    '已发送SAP_Recieved'" +
                           " when FStatus=6     then    '已关闭Closed'" +
                                            " else '未定义' end" +

                            ") MyStatus" +
                    " FROM CO_WH_Shipment" +
                  " where FDate>='" + S_Start + "' and FDate<='" + S_End + "'";
            if (FStatus != "999")
            {
                S_Sql += " and FStatus=" + FStatus;
            }

            DataSet ds = v_SiemensDB.DB_Data_Set(S_Sql);
            List<CO_WH_Shipment> List_CO_WH_Shipment = DataTableEx.ToList<CO_WH_Shipment>(ds.Tables[0]);

            return List_CO_WH_Shipment;
        }

        public List<CO_WH_ShipmentEntry> GetShipmentEntry(string S_FInterID)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = "select FInterID, FDetailID,FMPNNO,FKPONO,FCTN from CO_WH_ShipmentEntry where FInterID='" + S_FInterID + "'";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            int I_Count = 12 - DT.Rows.Count;

            // S_Sql = "select FInterID, FDetailID,FMPNNO,FKPONO,FCTN,'' FProjectNO,FEntryID,'' FLineItem from CO_WH_ShipmentEntry where FInterID='" + S_FInterID + "'" + "\r\n" +
            //          "union select top " + I_Count.ToString() +
            //          "'"+ S_FInterID + "'FInterID,100000000 + column_id FDetailID,'' FMPNNO,'' FKPONO,0 FCTN,'' FProjectNO,'' FEntryID,'' FLineItem from sys.columns";

            S_Sql = "select FInterID, FDetailID,FMPNNO,FKPONO,FCTN,'' FProjectNO,FEntryID,'' FLineItem from CO_WH_ShipmentEntry where FInterID='" + S_FInterID + "'";
            DataSet ds = v_SiemensDB.DB_Data_Set(S_Sql);

            List<CO_WH_ShipmentEntry> List_CO_WH_ShipmentEntry = DataTableEx.ToList<CO_WH_ShipmentEntry>(ds.Tables[0]);

            return List_CO_WH_ShipmentEntry;
        }

        public List<ShipmentReport> GetShipmentReport(string S_Start, string S_End, string FStatus)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = @"select A.*, B.FEntryID, B.FDetailID, B.FKPONO, B.FMPNNO,
                                B.FCTN FCTN2,A.FProjectNO FProjectNO2,
                                B.FStatus, B.FOutSN, 
                                (SELECT value FROM  [dbo].[F_Split](A.FShipNO,'#') WHERE id=1 )  FShipNO_L1  
                            from CO_WH_Shipment A join CO_WH_ShipmentEntry B
                            on A.FInterID = B.FInterID 
                where A.FDate>='" + S_Start + "' and A.FDate<='" + S_End + "'";

            if (FStatus != "999")
            {
                S_Sql += " and A.FStatus=" + FStatus;
            }

            DataSet ds = v_SiemensDB.DB_Data_Set(S_Sql);
            List<ShipmentReport> List_ShipmentReport = DataTableEx.ToList<ShipmentReport>(ds.Tables[0]);

            return List_ShipmentReport;
        }

        public string Edit_CO_WH_Shipment
            (
                string S_FInterID,
                string S_ShipDate,
                string S_HAWB,
                string S_PalletSeq,
                string S_PalletCount,
                string S_GrossWeight,
                string S_EmptyCarton,
                string S_ShipNO,
                string S_Project,

                string S_Type
            )
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_strOutput = "1";
            string S_Sql = "exec Edit_CO_WH_Shipment  " +
                        "'" + S_FInterID + "'," +
                        "'" + S_ShipDate + "'," +
                        "'" + S_HAWB + "'," +
                        "'" + S_PalletSeq + "'," +
                        "'" + S_PalletCount + "'," +
                        "'" + S_GrossWeight + "'," +

                        "'" + S_EmptyCarton + "'," +
                        "'" + S_ShipNO + "'," +
                        "'" + S_Project + "'," +

                        "'" + S_Type + "'";
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            S_strOutput = DT.Rows[0][0].ToString();
            return S_strOutput;
        }

        public string DeleteShipment(string FInterID)
        {
            string S_Result = "1";

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            try
            {
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql = "select FStatus from CO_WH_Shipment where FInterID='" + FInterID + "'";
                DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
                if (DT.Rows[0][0].ToString() != "0")
                {
                    S_Result = "It has been confirmed that deletion is not allowed";
                }
                else
                {
                    S_Sql = "delete CO_WH_Shipment  where FInterID='" + FInterID + "'";
                    v_SiemensDB.ExecSql(S_Sql);

                    S_Sql = "delete CO_WH_ShipmentEntry  where FInterID='" + FInterID + "'";
                    v_SiemensDB.ExecSql(S_Sql);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public string DeleteShipmentEntry(string FDetailID)
        {
            string S_Result = "1";
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            try
            {
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql = "delete CO_WH_ShipmentEntry where FDetailID='" + FDetailID + "'";
                v_SiemensDB.ExecSql(S_Sql);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public string DeleteMultiSelectShipment(string FInterID_List)
        {
            string S_Result = "1";
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            try
            {
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql = "Delete  CO_WH_Shipment where FStatus='0' and FInterID in(" + FInterID_List + ")";
                v_SiemensDB.ExecSql(S_Sql);

                S_Sql = "Delete  CO_WH_ShipmentEntry where FStatus='0' and FInterID in(" + FInterID_List + ")";
                v_SiemensDB.ExecSql(S_Sql);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public string UpdateShipment_FStatus(string FInterID_List, string Status)
        {
            string S_Result = "1";

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            try
            {
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql = "Update  CO_WH_Shipment set FStatus='" + Status + "' where FInterID in(" + FInterID_List + ")";
                v_SiemensDB.ExecSql(S_Sql);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public List<CO_WH_Shipment> GetShipment_One(string FInterID)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = "select * from CO_WH_Shipment where FInterID='" + FInterID + "'";
            DataSet DS = v_SiemensDB.DB_Data_Set(S_Sql);
            List<CO_WH_Shipment> List_CO_WH_Shipment = DataTableEx.ToList<CO_WH_Shipment>(DS.Tables[0]);

            return List_CO_WH_Shipment;
        }

        public List<CO_WH_ShipmentEntry> GetShipmentEntry_One(string FDetailID)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            string S_Sql = "select * from CO_WH_ShipmentEntry where FDetailID='" + FDetailID + "'";
            DataSet DS = v_SiemensDB.DB_Data_Set(S_Sql);
            List<CO_WH_ShipmentEntry> List_CO_WH_ShipmentEntry = DataTableEx.ToList<CO_WH_ShipmentEntry>(DS.Tables[0]);

            return List_CO_WH_ShipmentEntry;
        }

        public string Edit_CO_WH_ShipmentEntry
            (
                string S_FInterID,
                string S_FEntryID,
                string S_FDetailID,
                string S_FKPONO,
                string S_FMPNNO,
                string S_FCTN,
                string S_FStatus,

                string S_Type
            )
        {
            //前段参数传错，不想改前段，这里换一下
            //string S1 = List_Login.StationTypeID.ToString();
            //string S2 = S_Type;

            //string S_StationTypeID = S2;
            //S_Type = S1;

            string S_StationTypeID = List_Login.StationTypeID.ToString();
            string S_strOutput = "1";
            string S_Sql = "exec Edit_CO_WH_ShipmentEntry " +
                            "'" + S_FInterID + "'," +
                             "'" + S_FEntryID + "'," +
                            "'" + S_FDetailID + "'," +
                           
                            
                            "'" + S_FKPONO + "'," +
                            "'" + S_FMPNNO + "'," +
                            "'" + S_FCTN + "'," +


                            "'" + S_Type + "'";

            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }
            DataTable DT = v_SiemensDB.DB_Data_Table(S_Sql);
            S_strOutput = DT.Rows[0][0].ToString();
            return S_strOutput;
        }

        public string DB_ExecSql(string S_Sql)
        {
            string S_StationTypeID = List_Login.StationTypeID.ToString();
            if (v_SiemensDB == null)
            {
                v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
            }

            return v_SiemensDB.DB_ExecSql(S_Sql);
        }

        public WH_ImportDto ImportCheck(List<ExcelDT> v_ExcelDT) 
        {
            WH_ImportDto v_WH_ImportDto = new WH_ImportDto();
            v_WH_ImportDto.WH_ExcelDT = null;
            v_WH_ImportDto.MSG = "OK";

            try
            {
                string S_StationTypeID = List_Login.StationTypeID.ToString();
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql =
                @"
                Drop Table tmpExcelShipment

                CREATE TABLE [dbo].[tmpExcelShipment](
	                [Ship date] [datetime] NULL,
	                [project] [nvarchar](255) NULL,
	                [HAWB#] [nvarchar](255) NULL,
	                [HUB CODE] [nvarchar](255) NULL,
	                [COUNTRY] [nvarchar](255) NULL,
	                [REGION] [nvarchar](255) NULL,
	                [KPO#] [nvarchar](255) NULL,
	                [MPN] [nvarchar](255) NULL,
	                [Q'ty] [nvarchar](255) NULL,
	                [Carton] [nvarchar](255) NULL,
	                [Pallet] [nvarchar](255) NULL,
	                [Line] [nvarchar](255) NULL,
	                [CarNO] [nvarchar](255) NULL,
	                [备注] [nvarchar](255) NULL,
	                [memo] [varchar](1000) NULL,
	                [import] [bit] NULL,
	                [NO.] [int] IDENTITY(1,1) NOT NULL
                ) ON [PRIMARY]
                ";
                string S_Result = v_SiemensDB.ExecSql(S_Sql);
                v_WH_ImportDto.MSG = S_Result;
                if (S_Result != "OK")
                {
                    return v_WH_ImportDto;
                }

                for (int i = 0; i < v_ExcelDT.Count(); i++)
                {
                    try
                    {
                        S_Sql = "insert into tmpExcelShipment([Ship date],[project],[HAWB#],[HUB CODE] ," +
                            "[COUNTRY],[REGION],[KPO#] ,[MPN],[Q'ty],[Carton],[Pallet],[Line],[CarNO],[备注]) " +
                            " Values(" +
                             "'" + v_ExcelDT[i].Ship_date.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                             "'" + v_ExcelDT[i].project + "'," +
                             "'" + v_ExcelDT[i].HAWB_ + "'," +
                             "'" + v_ExcelDT[i].HUB_CODE + "'," +
                             "'" + v_ExcelDT[i].COUNTRY + "'," +
                             "'" + v_ExcelDT[i].REGION + "'," +
                             "'" + v_ExcelDT[i].KPO_ + "'," +
                             "'" + v_ExcelDT[i].MPN + "'," +
                             "'" + v_ExcelDT[i].Q_ty + "'," +
                             "'" + v_ExcelDT[i].Carton + "'," +
                             "'" + v_ExcelDT[i].Pallet + "'," +
                             "'" + v_ExcelDT[i].Line + "'," +
                             "'" + v_ExcelDT[i].CarNO + "'," +
                             "'" + v_ExcelDT[i].备注 +
                             "')";

                        S_Result = v_SiemensDB.ExecSql(S_Sql);
                        v_WH_ImportDto.MSG = S_Result;
                        if (S_Result != "OK")
                        {
                            return v_WH_ImportDto;
                        }
                    }
                    catch (Exception ex)
                    {
                        v_WH_ImportDto.MSG = ex.ToString();
                        return v_WH_ImportDto;
                    }
                }


                S_Sql = "update tmpExcelShipment set import=1,memo=''";
                S_Result = v_SiemensDB.ExecSql(S_Sql);
                v_WH_ImportDto.MSG = S_Result;
                if (S_Result != "OK")
                {
                    return v_WH_ImportDto;
                }

                S_Sql = "update A set memo=isnull(memo,'')+'1.该HAWB已存在',import=0 from tmpExcelShipment A " +
                             " where exists(select FInterID from CO_WH_Shipment where A.[HAWB#]=HAWB)";
                S_Result = v_SiemensDB.ExecSql(S_Sql);
                v_WH_ImportDto.MSG = S_Result;
                if (S_Result != "OK")
                {
                    return v_WH_ImportDto;
                }

                S_Sql = "update A set memo=isnull(memo,'')+'1.非同一个项目',import=0 from tmpExcelShipment A " +
                       " where exists(select count(distinct Project) from tmpExcelShipment group by Project having count(distinct Project)>1)";
                S_Result = v_SiemensDB.ExecSql(S_Sql);
                v_WH_ImportDto.MSG = S_Result;
                if (S_Result != "OK")
                {
                    return v_WH_ImportDto;
                }

                S_Sql = "select memo,import, [Ship date] Ship_date ,[project],[HAWB#] HAWB_,[HUB CODE] HUB_CODE, " +
                    " [COUNTRY],[REGION],[KPO#] KPO_ ,[MPN],[Q'ty] Q_ty,[Carton],[Pallet],[Line],[CarNO],[备注] " +
                    " from tmpExcelShipment";
                DataTable DT = v_SiemensDB.Data_Table(S_Sql);
                List<ExcelDT> List_CO_WH_Shipment = DataTableEx.ToList<ExcelDT>(DT);
                v_WH_ImportDto.WH_ExcelDT = List_CO_WH_Shipment;

            }
            catch (Exception ex) 
            {
                v_WH_ImportDto.MSG = ex.ToString();
                return v_WH_ImportDto;
            }

            return v_WH_ImportDto;
        }


        public string ImportEnter(List<ExcelDT> v_ExcelDT)
        {
            string S_Result = "OK";
            try
            {
                if (v_ExcelDT.Count<1) 
                {
                    return S_Result = "No Data";
                }

                string S_StationTypeID = List_Login.StationTypeID.ToString();
                if (v_SiemensDB == null)
                {
                    v_SiemensDB = new SiemensDB(DB_Context, List_Login.LanguageID, S_StationTypeID);
                }

                string S_Sql = "exec PRO_WH_ImportShipment '0','0','0','0'";
                S_Result = v_SiemensDB.ExecSql(S_Sql);

            }
            catch (Exception ex)
            {
                S_Result = ex.Message;
            }

            return S_Result;
        }


    }
}
