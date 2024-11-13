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
using StackExchange.Redis;
using Ubiety.Dns.Core.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Ubiety.Dns.Core.Records.General;
using SunnyMES.Commons.Core.PublicFun.Model;
using System.Drawing;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.Repositories
{
    public class PublicSCRepository : BaseRepositoryReport<string>, IPublicSCRepository
    {
        public PublicSCRepository()
        {
        }

        protected MSG_Public P_MSG_Public;
        protected MSG_Sys msgSys;
        protected static string S_Path = Directory.GetCurrentDirectory();

        public PublicSCRepository(IDbContextCore dbContext, int I_Language) : base(dbContext)
        {
            P_MSG_Public = new MSG_Public(I_Language);
            msgSys = new MSG_Sys(I_Language);
        }

        public async Task<List<SC_IdDesc>> GetIdDescription(string TableName, string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select ID,Description from " + TableName;
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdDescDescEN>> GetIdDescDescEN(string S_Sql) 
        {
            S_Sql = S_Sql ?? "";
            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDescDescEN>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<SC_IdDescDescEN>> GetWinPermission(string UserId)
        {
            string S_Sql = "SELECT * FROM mesEmployee where Id='"+UserId+"'";
            var List_User = await DapperConnRead2.QueryAsync<User>(S_Sql, null, null, I_DBTimeout, null);

            S_Sql = "SELECT A.Code Id,A.CodeName [Description],A.CodeName_EN DescriptionEN " +
                " FROM sysCode A WHERE Catalog='Permission'  ORDER BY A.CodeOrder";
            if (List_User.AsList()[0].UserID.ToLower() != "developer") 
            {
                S_Sql =
                    @"SELECT A.Code Id,A.CodeName [Description],A.CodeName_EN DescriptionEN 
                        FROM sysCode A WHERE Catalog='Permission' 
                    AND A.CodeName_EN NOT IN ('Admin','developer')
                    ORDER BY A.CodeOrder";
            }

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDescDescEN>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdNameDesc>> GetIdNameDescription(string TableName, string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select ID,Name,Description from " + TableName;
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdNameDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<SC_mesPart>> GetmesPart( string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select * from mesPart ";
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConnRead2.QueryAsync<SC_mesPart>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<SC_IdSNDesc>> GetIdSNDescription(string TableName, string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select ID,SN,Description from " + TableName;
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdSNDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_Employee>> GetEmployee()
        {
            string S_Sql = "select ID,UserID,UserID+'_'+Lastname+Firstname Employee from mesEmployee";

            var v_Query = await DapperConnRead2.QueryAsync<SC_Employee>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<SC_mesLine>> GetmesLine(string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select * from mesLine";
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConnRead2.QueryAsync<SC_mesLine>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdDescLineType>> GetmesLinePO(string S_LineTypeID, string S_PartFamilyID) 
        {
            string S_Sql = @"select A.*,C.Description LineType  from mesLine A 
                                        left join mesLineDetail B on A.ID = B.LineID
                                        left join luLineTypeDef C on B.LineTypeDefID = C.ID
                                    where C.ID=(select ID from luLineTypeDef where Description='LineType') 
                                        and  B.Content='" + S_LineTypeID + "'";
            DataTable DT_Result = new DataTable();
            DT_Result.Columns.Add("ID");
            DT_Result.Columns.Add("Description");
            DataRow DR;

            DataTable DT_All = Data_Table(S_Sql);
            for (int i = 0; i < DT_All.Rows.Count; i++)
            {
                string S_LineID = DT_All.Rows[i]["ID"].ToString();

                S_Sql = @" select *  from mesLine A 
                                            left join mesLineDetail B on A.ID = B.LineID
                                            left join luLineTypeDef C on B.LineTypeDefID = C.ID 
                                        where C.ID=(select ID from luLineTypeDef where Description='PartFamilyID') 
                                            and B.LineID=" + S_LineID;
                DataTable DT_CZ = Data_Table(S_Sql);
                if (DT_CZ.Rows.Count == 0)
                {
                    DR = DT_Result.NewRow();
                    DR["ID"] = DT_All.Rows[i]["ID"].ToString();
                    DR["Description"] = DT_All.Rows[i]["Description"].ToString();
                    DT_Result.Rows.Add(DR);
                }
                else
                {
                    S_Sql = @" select A.*,C.Description LineType  from mesLine A 
                                            left join mesLineDetail B on A.ID = B.LineID
                                            left join luLineTypeDef C on B.LineTypeDefID = C.ID 
                                        where C.ID=(select ID from luLineTypeDef where Description='PartFamilyID') 
                                            and B.LineID=" + S_LineID + " and  B.Content='" + S_PartFamilyID + "'";
                    DataTable DT_PartFamilyID = Data_Table(S_Sql);

                    if (DT_PartFamilyID.Rows.Count > 0)
                    {
                        DR = DT_Result.NewRow();
                        DR["ID"] = DT_PartFamilyID.Rows[0]["ID"].ToString();
                        DR["Description"] = DT_PartFamilyID.Rows[0]["Description"].ToString();
                        DT_Result.Rows.Add(DR);
                    }
                }
            }

            DR = DT_Result.NewRow();
            DR["ID"] = -1;
            DR["Description"] = "ALL";
            DT_Result.Rows.Add(DR);

            S_Sql = "select 0 ID,'' Description";
            var v_query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);

            List<SC_IdDescLineType> List_IdDescription = DataTableEx.ToList<SC_IdDescLineType>(DT_Result);

            return List_IdDescription;

        }


        public async Task<List<SC_SIdDesc>> GetLineType() 
        {
            string S_Sql =
                @"select AA ID,AA Description from 
                (
                    select distinct(Content) AA  from mesLineDetail  where  [LineTypeDefID] =
                        (select ID from luLineTypeDef where Description = 'LineType')
                    and Content<>''
                )A ";
            var v_query = await DapperConnRead2.QueryAsync<SC_SIdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_query.ToList();
        }

        public async Task<List<SC_IdDesc>> GetTTWIPReportType()
        {
            string S_Sql =
                @"select ROW_NUMBER() over(order by Description) as ID,A.* from
                (
                    SELECT DISTINCT(ReportType) Description FROM rptTTWIPConfig
                )A";
            var v_query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_query.ToList();
        }


        public async Task<List<SC_IdDesc>> GetTTWIPTableName(string ReportType)
        {
            string S_Sql =
                    @"select  ROW_NUMBER() over(order by Description) as ID,A.* from 
                    (
	                    SELECT DISTINCT(TableName) Description FROM rptTTWIPConfig WHERE ReportType='" + ReportType + @"'
                    )A ";
            var v_query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_query.ToList();
        }

        public async Task<List<SC_SIdName>> GetLineGroupName()
        {
            string S_Sql =
                    @"SELECT 'M' ID,'MainLine' [Name]
                    UNION
                    SELECT 'K' ID,'KittingLine' [Name]";
            var v_query = await DapperConnRead2.QueryAsync<SC_SIdName>(S_Sql, null, null, I_DBTimeout, null);
            return v_query.ToList();
        }

        public async Task<List<SC_IdPOSNDesc>> GetmesProductionOrder() 
        {
            string S_Sql = "select ID,ProductionOrderNumber SN,Description from mesProductionOrder";

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdPOSNDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<V_LabelType>> GetVLabelType() 
        {
            string S_Sql = "select ID,LabelTypeName from V_LabelType";

            var v_Query = await DapperConnRead2.QueryAsync<V_LabelType>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<V_OutputType>> GetVOutputType()
        {
            string S_Sql = "select ID,OutputTypeName from V_OutputType";

            var v_Query = await DapperConnRead2.QueryAsync<V_OutputType>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<V_ModuleName>> GetVModuleName()
        {
            string S_Sql = "select ID,ModuleName from V_ModuleName";

            var v_Query = await DapperConnRead2.QueryAsync<V_ModuleName>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<Q_DBField>> GetDBField(string S_Type)
        {
            string S_Sql = "exec uspGetTableColumn '" + S_Type + "'";

            var v_Query = await DapperConnRead2.QueryAsync<Q_DBField>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<V_FunctionName>> GetVFunctionName()
        {
            string S_Sql = "select ID,FunctionName from V_FunctionName";

            var v_Query = await DapperConnRead2.QueryAsync<V_FunctionName>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdName>> GetLabelFormatPosName()
        {
            string S_Sql = @"SELECT '1' ID,'Header' [Name]
                            UNION
                            SELECT '2' ID,'Line' [Name]
                             ";

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdName>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<SC_IdDesc>> GetSectionType() 
        {
            string S_Sql = @"SELECT '1' ID,'固定值(Fixed)' [Description]
                            UNION
                            SELECT '2' ID,'存储过程(Procedure)' [Description]
                            UNION
                            SELECT '3' ID,'日期时间(DateTime)' [Description]
                            UNION
                            SELECT '4' ID,'计数(Counter)' [Description]
                             ";

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdDesc>> GetSectionStatus()
        {
            string S_Sql = @"SELECT '0' ID,'新增(New)' [Description]
                            UNION
                            SELECT '1' ID,'运行中(Running)' [Description]
                            UNION
                            SELECT '2' ID,'完成(Completed)' [Description]
                            UNION
                            SELECT '3' ID,'F-完成(F-Completed)' [Description]

                             ";

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<SC_IdName>> GetProcesureUDP()
        {
            string S_Sql = @"select ID,Name from sysobjects where xtype='P' and NAME LIKE 'udp%'";

            var v_Query = await DapperConnRead2.QueryAsync<SC_IdName>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_IdDesc>> GetluProductionOrderDetailDef_PMC()
        {
            string S_PMC_Parameter = Configs.GetConfigurationValue("AppSetting", "PMC_Parameter"); 
            if (S_PMC_Parameter == "")
            {
                return null;
            }

            string S_Sql = "SELECT ID,[Description] FROM luProductionOrderDetailDef WHERE [Description] IN('" + S_PMC_Parameter + "')";
            var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }





        public async Task<List<Com_Value>> GetCom_a() 
        {

            string S_Sql =
            @"
            Select  0 'ID' , 'A'  VALUE UNION
            Select  1 'ID' , 'B'  VALUE UNION
            Select  2 'ID' , 'C'  VALUE UNION
            Select  3 'ID' , 'D'  VALUE UNION
            Select  4 'ID' , 'E'  Value UNION
            Select  5 'ID' , 'F'  Value UNION
            Select  6 'ID' , 'G'  Value UNION
            Select  7 'ID' , 'H'  Value UNION
            Select  8 'ID' , 'I'  Value UNION
            Select  9 'ID' , 'J'  Value UNION
            Select  10 'ID' , 'K'  Value UNION
            Select  11 'ID' , 'L'  Value UNION
            Select  12 'ID' , 'M'  Value UNION
            Select  13 'ID' , 'N'  Value UNION
            Select  14 'ID' , 'O'  Value UNION
            Select  15 'ID' , 'P'  Value UNION
            Select  16 'ID' , 'Q'  Value UNION
            Select  17 'ID' , 'R'  Value UNION
            Select  18 'ID' , 'S'  Value UNION
            Select  19 'ID' , 'T'  Value UNION
            Select  20 'ID' , 'U'  Value UNION
            Select  21 'ID' , 'V'  Value UNION
            Select  22 'ID' , 'W'  Value UNION
            Select  23 'ID' , 'X'  Value UNION
            Select  24 'ID' , 'Y'  Value UNION
            Select  25 'ID' , 'Z'  Value 
            ";
            var v_Query = await DapperConnRead2.QueryAsync<Com_Value>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<Com_Value>> GetCom_d() 
        {
            string S_Sql =
            @"
            Select  0 'ID' , '0'   VALUE UNION
            Select  1 'ID' , '1'   Value UNION
            Select  2 'ID' , '2'   Value UNION
            Select  3 'ID' , '3'   Value UNION
            Select  4 'ID' , '4'   Value UNION
            Select  5 'ID' , '5'   Value UNION
            Select  6 'ID' , '6'   Value UNION
            Select  7 'ID' , '7'   Value UNION
            Select  8 'ID' , '8'   Value UNION
            Select  9 'ID' , '9'   Value

            ";
            var v_Query = await DapperConnRead2.QueryAsync<Com_Value>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<Com_Value>> GetCom_h() 
        {
            //20240726 应老板要求所有的数字选项中加个1
            string S_Sql =
            @"
            Select  0 'ID' , '0'   VALUE UNION
            Select  1  'ID' , '1'   VALUE UNION
            Select  2 'ID' , '4'   Value UNION
            Select  3 'ID' , '5'   Value UNION
            Select  4 'ID' , '6'   Value UNION
            Select  5 'ID' , '7'   Value UNION
            Select  6 'ID' , '8'   Value UNION
            Select  7 'ID' , '9'  Value UNION
            Select  8 'ID' , 'A'   Value UNION
            Select  9 'ID' , 'B'   Value UNION
            Select  10'ID' , 'C'   Value UNION
            Select  11 'ID' , 'D'   Value UNION
            Select  12 'ID' , 'E'   Value UNION
            Select  13 'ID' , 'F'   Value 
            ";
            //@"
            //Select  0 'ID' , '0'   VALUE UNION
            //Select  1 'ID' , '4'   Value UNION
            //Select  2 'ID' , '5'   Value UNION
            //Select  3 'ID' , '6'   Value UNION
            //Select  4 'ID' , '7'   Value UNION
            //Select  5 'ID' , '8'   Value UNION
            //Select  6 'ID' , '9'  Value UNION
            //Select  7 'ID' , 'A'   Value UNION
            //Select  8 'ID' , 'B'   Value UNION
            //Select  9 'ID' , 'C'   Value UNION
            //Select  10 'ID' , 'D'   Value UNION
            //Select  11 'ID' , 'E'   Value UNION
            //Select  12 'ID' , 'F'   Value 
            //";
            var v_Query = await DapperConnRead2.QueryAsync<Com_Value>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<Com_Value>> GetCom_n() 
        {
            string S_Sql =
                            @"
            Select  0 'ID' , '0'   VALUE UNION
            Select  1 'ID' , '1'   VALUE UNION
            Select  2  'ID' , '4'   Value UNION
            Select  3 'ID' , '5'   Value UNION
            Select  4 'ID' , '6'   Value UNION
            Select  5 'ID' , '7'   Value UNION
            Select  6 'ID' , '8'   Value UNION
            Select  7 'ID' , '9'  Value UNION
            Select  8 'ID' , 'A'  Value UNION
            Select  9 'ID' , 'B'  Value UNION
            Select  10'ID' , 'C'  Value UNION
            Select  11 'ID' , 'D'  Value UNION
            Select  12 'ID' , 'E'  Value UNION
            Select  13 'ID' , 'F'  Value UNION
            Select  14 'ID' , 'G'  Value UNION
            Select  15 'ID' , 'H'  Value UNION
            Select  16 'ID' , 'I'  Value UNION
            Select  17 'ID' , 'J'  Value UNION
            Select  18 'ID' , 'K'  Value UNION
            Select  19 'ID' , 'L'  Value UNION
            Select  20 'ID' , 'M'  Value UNION
            Select  21 'ID' , 'N'  Value UNION
            Select  22 'ID' , 'O'  Value UNION
            Select  23 'ID' , 'P'  Value UNION
            Select  24 'ID' , 'Q'  Value UNION
            Select  25 'ID' , 'R'  Value UNION
            Select  26 'ID' , 'S'  Value UNION
            Select  27 'ID' , 'T'  Value UNION
            Select  28 'ID' , 'U'  Value UNION
            Select  29 'ID' , 'V'  Value UNION
            Select  30 'ID' , 'W'  Value UNION
            Select  31 'ID' , 'X'  Value UNION
            Select  32 'ID' , 'Y'  Value UNION
            Select  33 'ID' , 'Z'  Value 

            ";
            //@"
            //Select  0 'ID' , '0'   VALUE UNION
            //Select  1 'ID' , '4'   Value UNION
            //Select  2 'ID' , '5'   Value UNION
            //Select  3 'ID' , '6'   Value UNION
            //Select  4 'ID' , '7'   Value UNION
            //Select  5 'ID' , '8'   Value UNION
            //Select  6 'ID' , '9'  Value UNION
            //Select  7 'ID' , 'A'  Value UNION
            //Select  8 'ID' , 'B'  Value UNION
            //Select  9 'ID' , 'C'  Value UNION
            //Select  10 'ID' , 'D'  Value UNION
            //Select  11 'ID' , 'E'  Value UNION
            //Select  12 'ID' , 'F'  Value UNION
            //Select  13 'ID' , 'G'  Value UNION
            //Select  14 'ID' , 'H'  Value UNION
            //Select  15 'ID' , 'I'  Value UNION
            //Select  16 'ID' , 'J'  Value UNION
            //Select  17 'ID' , 'K'  Value UNION
            //Select  18 'ID' , 'L'  Value UNION
            //Select  19 'ID' , 'M'  Value UNION
            //Select  20 'ID' , 'N'  Value UNION
            //Select  21 'ID' , 'O'  Value UNION
            //Select  22 'ID' , 'P'  Value UNION
            //Select  23 'ID' , 'Q'  Value UNION
            //Select  24 'ID' , 'R'  Value UNION
            //Select  25 'ID' , 'S'  Value UNION
            //Select  26 'ID' , 'T'  Value UNION
            //Select  27 'ID' , 'U'  Value UNION
            //Select  28 'ID' , 'V'  Value UNION
            //Select  29 'ID' , 'W'  Value UNION
            //Select  30 'ID' , 'X'  Value UNION
            //Select  31 'ID' , 'Y'  Value UNION
            //Select  32 'ID' , 'Z'  Value 

            //";
            var v_Query = await DapperConnRead2.QueryAsync<Com_Value>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<Com_Value>> GetCom_o() 
        {
            string S_Sql =
                            @"
                Select  0 'ID' , '0'   VALUE UNION
                Select  1 'ID' , '1'   VALUE UNION
                Select  2 'ID' , '4'   Value UNION
                Select  3 'ID' , '5'   Value UNION
                Select  4 'ID' , '6'   Value UNION
                Select  5 'ID' , '7'   Value 

            ";
            //@"
            //    Select  0 'ID' , '0'   VALUE UNION
            //    Select  1 'ID' , '4'   Value UNION
            //    Select  2 'ID' , '5'   Value UNION
            //    Select  3 'ID' , '6'   Value UNION
            //    Select  4 'ID' , '7'   Value 

            //";
            var v_Query = await DapperConnRead2.QueryAsync<Com_Value>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async  Task<string> WriteSysConfigLog(LoginList List_Login,string DataType,string TableName,string MSG) 
        {
            string S_Result = "OK";
            try
            {
                SysConfigLogDIO v_SysConfigLogDIO = new SysConfigLogDIO();
                v_SysConfigLogDIO.CurrentIP = List_Login.CurrentLoginIP;
                v_SysConfigLogDIO.UserName = List_Login.UserName;
                v_SysConfigLogDIO.DataType = DataType;
                v_SysConfigLogDIO.TableName = TableName;
                v_SysConfigLogDIO.MSG = MSG;

                await SysConfigLogHelper.WriteLog(v_SysConfigLogDIO);
            }
            catch (Exception ex) 
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }



        private List<SC_IdDesc> List_luPartFamilyTypeDetailDef;
        private List<SC_IdDesc> List_luPartFamilyDetailDef;
        private List<SC_IdDesc> List_luPartDetailDef;
        private List<SC_IdDesc> List_luProductionOrderDetailDef;
        private List<SC_IdDesc> List_luLineTypeDef;
        private List<SC_IdDesc> List_luApplicationType;

        private List<SC_IdDesc> List_luUnitStatus;
        private List<SC_IdDesc> List_sysStatus;
        private List<SC_IdDesc> List_luMaterialType;
        private List<SC_IdDesc> List_luPackageDetailDef;
        private List<SC_IdDesc> List_luPackageStatus;
        private List<SC_IdDesc> List_luProductionOrderStatus;
        private List<SC_IdDesc> List_luSerialNumberType;
        private List<SC_IdDesc> List_luMachineStatus;
        private List<SC_IdDesc> List_luEmployeeStatus;

        private  async Task<string> GetData() 
        {
            string S_Result = "OK";
            try
            {
                string S_Sql =
                "SELECT 1 ID,'ORTTestResult' [Description]  \n"
                + "UNION SELECT 2 ID,'ORTDayNumber' [Description]  \n"
                + "UNION SELECT 3 ID,'ORTWeekNumber' [Description]  \n"
                + "UNION SELECT 4 ID,'ProductName' [Description]  \n"
                + "UNION SELECT 5 ID,'ProductIcon' [Description] ";

                var Query_luPartFamilyTypeDetailDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luPartFamilyTypeDetailDef = Query_luPartFamilyTypeDetailDef.ToList();

                S_Sql = "SELECT 1 ID,'ISAPN' [Description]  \n"
                + "UNION SELECT 2 ID,'IsInsightModule' [Description]  \n"
                + "UNION SELECT 3 ID,'IsInsightModule_InnerShell' [Description]  \n"
                + "UNION SELECT 4 ID,'IsInsightModuleSN' [Description]  \n"
                + "UNION SELECT 5 ID,'ISModuleDescription' [Description]  \n"
                + "UNION SELECT 6 ID,'IsTimeCheck' [Description]  \n"
                + "UNION SELECT 7 ID,'IsTTWIPModule' [Description]  \n"
                + "UNION SELECT 8 ID,'ISVendor' [Description]  \n"
                + "UNION SELECT 9 ID,'PackBoxWeightLimit' [Description]  \n"
                + "UNION SELECT 10 ID,'PartFamilyName' [Description]  \n"
                + "UNION SELECT 11 ID,'ShippingPalletWeightLimit' [Description]  \n"
                + "UNION SELECT 12 ID,'TimeCheckEndStationType' [Description]  \n"
                + "UNION SELECT 13 ID,'TimeCheckMax' [Description]  \n"
                + "UNION SELECT 14 ID,'TimeCheckMin' [Description]  \n"
                + "UNION SELECT 15 ID,'TimeCheckStartStationType' [Description]  \n"
                + "UNION SELECT 16 ID,'TTWIPModuleDescription' [Description] ";

                var Query_luPartFamilyDetailDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luPartFamilyDetailDef = Query_luPartFamilyDetailDef.ToList();

                S_Sql = "SELECT  1 ID, '7ECode' [Description]  \n"
                    + "UNION SELECT  2 ID, 'APN' [Description]  \n"
                    + "UNION SELECT  3 ID, 'Batch_Pattern' [Description]  \n"
                    + "UNION SELECT  4 ID, 'BoxWeightBase' [Description]  \n"
                    + "UNION SELECT  5 ID, 'BoxWeightBaseOffset' [Description]  \n"
                    + "UNION SELECT  6 ID, 'BoxWeightLowerLimit' [Description]  \n"
                    + "UNION SELECT  7 ID, 'BoxWeightUnit' [Description]  \n"
                    + "UNION SELECT  8 ID, 'BoxWeightUpperLimit' [Description]  \n"
                    + "UNION SELECT  9 ID, 'CCCC Code' [Description]  \n"
                    + "UNION SELECT  10 ID, 'Color' [Description]  \n"
                    + "UNION SELECT  11 ID, 'ColorValue' [Description]  \n"
                    + "UNION SELECT  12 ID, 'DOE_Parameter1' [Description]  \n"
                    + "UNION SELECT  13 ID, 'Expires_Time' [Description]  \n"
                    + "UNION SELECT  14 ID, 'FullNumber' [Description]  \n"
                    + "UNION SELECT  15 ID, 'GS1_PalletLabelName' [Description]  \n"
                    + "UNION SELECT  16 ID, 'GS2_PalletLabelName' [Description]  \n"
                    + "UNION SELECT  17 ID, 'InnerSN_Pattern' [Description]  \n"
                    + "UNION SELECT  18 ID, 'ISAPN' [Description]  \n"
                    + "UNION SELECT  19 ID, 'IsForceSplit' [Description]  \n"
                    + "UNION SELECT  20 ID, 'IsInsightModule' [Description]  \n"
                    + "UNION SELECT  21 ID, 'IsInsightModule_InnerShell' [Description]  \n"
                    + "UNION SELECT  22 ID, 'IsInsightModuleSN' [Description]  \n"
                    + "UNION SELECT  23 ID, 'ISModuleDescription' [Description]  \n"
                    + "UNION SELECT  24 ID, 'IsTimeCheck' [Description]  \n"
                    + "UNION SELECT  25 ID, 'IsTTWIPModule' [Description]  \n"
                    + "UNION SELECT  26 ID, 'ISVendor' [Description]  \n"
                    + "UNION SELECT  27 ID, 'Location' [Description]  \n"
                    + "UNION SELECT  28 ID, 'M_UnitConversion_PCS' [Description]  \n"
                    + "UNION SELECT  29 ID, 'MaterialAuto' [Description]  \n"
                    + "UNION SELECT  30 ID, 'MaterialBatchQTY' [Description]  \n"
                    + "UNION SELECT  31 ID, 'MaterialCodeRules' [Description]  \n"
                    + "UNION SELECT  32 ID, 'MaterialLable' [Description]  \n"
                    + "UNION SELECT  33 ID, 'MM Code' [Description]  \n"
                    + "UNION SELECT  34 ID, 'PackBoxWeightLimit' [Description]  \n"
                    + "UNION SELECT  35 ID, 'PP Code' [Description]  \n"
                    + "UNION SELECT  36 ID, 'Receive_Time' [Description]  \n"
                    + "UNION SELECT  37 ID, 'ScanType' [Description]  \n"
                    + "UNION SELECT  38 ID, 'ShippingPalletWeightLimit' [Description]  \n"
                    + "UNION SELECT  39 ID, 'Size' [Description]  \n"
                    + "UNION SELECT  40 ID, 'SN_Pattern' [Description]  \n"
                    + "UNION SELECT  41 ID, 'SNFormat' [Description]  \n"
                    + "UNION SELECT  42 ID, 'SplitBatchQTY' [Description]  \n"
                    + "UNION SELECT  43 ID, 'TimeCheckEndStationType' [Description]  \n"
                    + "UNION SELECT  44 ID, 'TimeCheckMax' [Description]  \n"
                    + "UNION SELECT  45 ID, 'TimeCheckMin' [Description]  \n"
                    + "UNION SELECT  46 ID, 'TimeCheckStartStationType' [Description]  \n"
                    + "UNION SELECT  47 ID, 'TTAuto' [Description]  \n"
                    + "UNION SELECT  48 ID, 'TTWIPModuleDescription' [Description]  \n"
                    + "UNION SELECT  49 ID, 'Type' [Description]  \n"
                    + "UNION SELECT  50 ID, 'UPCCOOConfig' [Description]"
                    ;
                var Query_luPartDetailDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luPartDetailDef = Query_luPartDetailDef.ToList();


                S_Sql = "SELECT  1 ID, '7ECode' [Description]  \n"
                    + "UNION SELECT  2 ID, '7Q' [Description]  \n"
                    + "UNION SELECT  3 ID, 'BoxBartenderPath' [Description]  \n"
                    + "UNION SELECT  4 ID, 'BoxLabelTemplatePath' [Description]  \n"
                    + "UNION SELECT  5 ID, 'BoxQty' [Description]  \n"
                    + "UNION SELECT  6 ID, 'BoxSN_Pattern' [Description]  \n"
                    + "UNION SELECT  7 ID, 'BoxSNFormatName' [Description]  \n"
                    + "UNION SELECT  8 ID, 'COO' [Description]  \n"
                    + "UNION SELECT  9 ID, 'DOE_BuildName' [Description]  \n"
                    + "UNION SELECT  10 ID, 'DOE_CCCCConfig' [Description]  \n"
                    + "UNION SELECT  11 ID, 'DOE_ConfigNumber' [Description]  \n"
                    + "UNION SELECT  12 ID, 'DOE_Parameter1' [Description]  \n"
                    + "UNION SELECT  13 ID, 'DOE_ProjectPhase' [Description]  \n"
                    + "UNION SELECT  14 ID, 'GSI' [Description]  \n"
                    + "UNION SELECT  15 ID, 'GTIN' [Description]  \n"
                    + "UNION SELECT  16 ID, 'GTINCOUNT' [Description]  \n"
                    + "UNION SELECT  17 ID, 'IsCreateUPCSN' [Description]  \n"
                    + "UNION SELECT  18 ID, 'IsGenerateBoxSN' [Description]  \n"
                    + "UNION SELECT  19 ID, 'IsGeneratePalletSN' [Description]  \n"
                    + "UNION SELECT  20 ID, 'IsMixedPN' [Description]  \n"
                    + "UNION SELECT  21 ID, 'IsMixedPO' [Description]  \n"
                    + "UNION SELECT  22 ID, 'IsScanFGSN' [Description]  \n"
                    + "UNION SELECT  23 ID, 'IsScanJAN' [Description]  \n"
                    + "UNION SELECT  24 ID, 'IsScanUPC' [Description]  \n"
                    + "UNION SELECT  25 ID, 'IsScanUPCSN' [Description]  \n"
                    + "UNION SELECT  26 ID, 'IsTimeCheck' [Description]  \n"
                    + "UNION SELECT  27 ID, 'Jan' [Description]  \n"
                    + "UNION SELECT  28 ID, 'JanTite' [Description]  \n"
                    + "UNION SELECT  29 ID, 'MPN' [Description]  \n"
                    + "UNION SELECT  30 ID, 'MultipackScanOnlyFGSN' [Description]  \n"
                    + "UNION SELECT  31 ID, 'PalletBartenderPath' [Description]  \n"
                    + "UNION SELECT  32 ID, 'PalletLabelTemplatePath' [Description]  \n"
                    + "UNION SELECT  33 ID, 'PalletQty' [Description]  \n"
                    + "UNION SELECT  34 ID, 'PalletSN_Pattern' [Description]  \n"
                    + "UNION SELECT  35 ID, 'PalletSNFormatName' [Description]  \n"
                    + "UNION SELECT  36 ID, 'PRODUCT' [Description]  \n"
                    + "UNION SELECT  37 ID, 'Region' [Description]  \n"
                    + "UNION SELECT  38 ID, 'SCC' [Description]  \n"
                    + "UNION SELECT  39 ID, 'ShipmentRegion' [Description]  \n"
                    + "UNION SELECT  40 ID, 'SN_Pattern' [Description]  \n"
                    + "UNION SELECT  41 ID, 'UPC' [Description]  \n"
                    + "UNION SELECT  42 ID, 'UPC_BartenderPath' [Description]  \n"
                    + "UNION SELECT  43 ID, 'UPC_LabelTemplatePath' [Description]  \n"
                    + "UNION SELECT  44 ID, 'WOStage' [Description] ";

                var Query_luProductionOrderDetailDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luProductionOrderDetailDef = Query_luProductionOrderDetailDef.ToList();

                S_Sql = "SELECT  1 ID, 'LineType' [Description]  \n"
                    + "UNION SELECT  2 ID, 'InSightLineName' [Description]  \n"
                    + "UNION SELECT  3 ID, 'PartFamilyID' [Description]  \n"
                    + "UNION SELECT  4 ID, 'UPH' [Description]  \n"
                    + "UNION SELECT  5 ID, 'IsCheckBuckFailCount' [Description]";

                var Query_luLineTypeDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luLineTypeDef = Query_luLineTypeDef.ToList();


                S_Sql = "SELECT  1 ID, 'Assembly' [Description]  \n"
                    + "UNION SELECT  2 ID, 'AssemblyAuto' [Description]  \n"
                    + "UNION SELECT  3 ID, 'AssemblyAutoNew' [Description]  \n"
                    + "UNION SELECT  4 ID, 'AssemblyAutoNoPo' [Description]  \n"
                    + "UNION SELECT  5 ID, 'AssemblyAutoPart' [Description]  \n"
                    + "UNION SELECT  6 ID, 'AssemblyNew' [Description]  \n"
                    + "UNION SELECT  7 ID, 'AssemblyNoPo' [Description]  \n"
                    + "UNION SELECT  8 ID, 'AssemblyNoPoNew' [Description]  \n"
                    + "UNION SELECT  9 ID, 'AssemblyPanel' [Description]  \n"
                    + "UNION SELECT  10 ID, 'AssemblyPanelNew' [Description]  \n"
                    + "UNION SELECT  11 ID, 'AssemblyPanelNoPo' [Description]  \n"
                    + "UNION SELECT  12 ID, 'AssemblyPanelNoPoNew' [Description]  \n"
                    + "UNION SELECT  13 ID, 'AssemblyPanelPart' [Description]  \n"
                    + "UNION SELECT  14 ID, 'AssemblyPanelPartNew' [Description]  \n"
                    + "UNION SELECT  15 ID, 'AssemblyPart' [Description]  \n"
                    + "UNION SELECT  16 ID, 'AssemblyPartNew' [Description]  \n"
                    + "UNION SELECT  17 ID, 'AssemblyTwoInputV2' [Description]  \n"
                    + "UNION SELECT  18 ID, 'AssemblyV2' [Description]  \n"
                    + "UNION SELECT  19 ID, 'BoxLinkBatch' [Description]  \n"
                    + "UNION SELECT  20 ID, 'BoxLinkBatchV2' [Description]  \n"
                    + "UNION SELECT  21 ID, 'BoxPackage_Weight' [Description]  \n"
                    + "UNION SELECT  22 ID, 'BuckLinkMachine' [Description]  \n"
                    + "UNION SELECT  23 ID, 'CartonBox' [Description]  \n"
                    + "UNION SELECT  24 ID, 'CartonBox_Auto' [Description]  \n"
                    + "UNION SELECT  25 ID, 'CartonBox-Verify' [Description]  \n"
                    + "UNION SELECT  26 ID, 'CartonBox-Verify_Auto' [Description]  \n"
                    + "UNION SELECT  27 ID, 'CartonBox-VerifyNew' [Description]  \n"
                    + "UNION SELECT  28 ID, 'ChangePart' [Description]  \n"
                    + "UNION SELECT  29 ID, 'Disassembly' [Description]  \n"
                    + "UNION SELECT  30 ID, 'FG_OfflineLabelPrint_DOE' [Description]  \n"
                    + "UNION SELECT  31 ID, 'FG_OfflineLabelPrint_NPI' [Description]  \n"
                    + "UNION SELECT  32 ID, 'FGPrintUPC' [Description]  \n"
                    + "UNION SELECT  33 ID, 'FGPrintUPCAuto' [Description]  \n"
                    + "UNION SELECT  34 ID, 'FGSNQueryShell' [Description]  \n"
                    + "UNION SELECT  35 ID, 'Interlock' [Description]  \n"
                    + "UNION SELECT  36 ID, 'IQC' [Description]  \n"
                    + "UNION SELECT  37 ID, 'IQCNew' [Description]  \n"
                    + "UNION SELECT  38 ID, 'IQCV2' [Description]  \n"
                    + "UNION SELECT  39 ID, 'JumpStation' [Description]  \n"
                    + "UNION SELECT  40 ID, 'JumpStationQC' [Description]  \n"
                    + "UNION SELECT  41 ID, 'JumpStationQCV2' [Description]  \n"
                    + "UNION SELECT  42 ID, 'KitQC' [Description]  \n"
                    + "UNION SELECT  43 ID, 'KitQCV2' [Description]  \n"
                    + "UNION SELECT  44 ID, 'L2RollerAssemblyV2' [Description]  \n"
                    + "UNION SELECT  45 ID, 'Laser' [Description]  \n"
                    + "UNION SELECT  46 ID, 'LinkUPC' [Description]  \n"
                    + "UNION SELECT  47 ID, 'LinkUPC_Auto' [Description]  \n"
                    + "UNION SELECT  48 ID, 'LinkUPCV2' [Description]  \n"
                    + "UNION SELECT  49 ID, 'MaterialCropping-Batch' [Description]  \n"
                    + "UNION SELECT  50 ID, 'MaterialCropping-SN' [Description]  \n"
                    + "UNION SELECT  51 ID, 'MaterialCropping-SN_DOE' [Description]  \n"
                    + "UNION SELECT  52 ID, 'MaterialInitialize' [Description]  \n"
                    + "UNION SELECT  53 ID, 'MaterialLineCropping-SN' [Description]  \n"
                    + "UNION SELECT  54 ID, 'ModifyUnitState' [Description]  \n"
                    + "UNION SELECT  55 ID, 'ModifyUnitState_Stop' [Description]  \n"
                    + "UNION SELECT  56 ID, 'ModifyUnitState-Stop' [Description]  \n"
                    + "UNION SELECT  57 ID, 'OOBA' [Description]  \n"
                    + "UNION SELECT  58 ID, 'ORT' [Description]  \n"
                    + "UNION SELECT  59 ID, 'OverStation' [Description]  \n"
                    + "UNION SELECT  60 ID, 'OverStationNew' [Description]  \n"
                    + "UNION SELECT  61 ID, 'OverStationNoPo' [Description]  \n"
                    + "UNION SELECT  62 ID, 'OverStationNoPoNew' [Description]  \n"
                    + "UNION SELECT  63 ID, 'OverStationPart' [Description]  \n"
                    + "UNION SELECT  64 ID, 'OverStationPartNew' [Description]  \n"
                    + "UNION SELECT  65 ID, 'OverStationV2' [Description]  \n"
                    + "UNION SELECT  66 ID, 'PackageOverStation' [Description]  \n"
                    + "UNION SELECT  67 ID, 'Pallet' [Description]  \n"
                    + "UNION SELECT  68 ID, 'Plasma' [Description]  \n"
                    + "UNION SELECT  69 ID, 'PrintFG' [Description]  \n"
                    + "UNION SELECT  70 ID, 'QC' [Description]  \n"
                    + "UNION SELECT  71 ID, 'QC_NoPo' [Description]  \n"
                    + "UNION SELECT  72 ID, 'QC_NoPoNew' [Description]  \n"
                    + "UNION SELECT  73 ID, 'QCNew' [Description]  \n"
                    + "UNION SELECT  74 ID, 'QCPart' [Description]  \n"
                    + "UNION SELECT  75 ID, 'QCPartNew' [Description]  \n"
                    + "UNION SELECT  76 ID, 'QCPrint' [Description]  \n"
                    + "UNION SELECT  77 ID, 'QCV2' [Description]  \n"
                    + "UNION SELECT  78 ID, 'QueryInactive' [Description]  \n"
                    + "UNION SELECT  79 ID, 'QueryInactive-Disable' [Description]  \n"
                    + "UNION SELECT  80 ID, 'ReleaseMachineSN' [Description]  \n"
                    + "UNION SELECT  81 ID, 'RepaiQC' [Description]  \n"
                    + "UNION SELECT  82 ID, 'Repair_QCV2' [Description]  \n"
                    + "UNION SELECT  83 ID, 'RepairKitQCV2' [Description]  \n"
                    + "UNION SELECT  84 ID, 'RePrint' [Description]  \n"
                    + "UNION SELECT  85 ID, 'RePrintFG' [Description]  \n"
                    + "UNION SELECT  86 ID, 'RePrintFGSN' [Description]  \n"
                    + "UNION SELECT  87 ID, 'Rework' [Description]  \n"
                    + "UNION SELECT  88 ID, 'RMAChange' [Description]  \n"
                    + "UNION SELECT  89 ID, 'RollerAssembly' [Description]  \n"
                    + "UNION SELECT  90 ID, 'RollerAssemblyNew' [Description]  \n"
                    + "UNION SELECT  91 ID, 'ScanCheck' [Description]  \n"
                    + "UNION SELECT  92 ID, 'SearchData' [Description]  \n"
                    + "UNION SELECT  93 ID, 'ShipMentRemove' [Description]  \n"
                    + "UNION SELECT  94 ID, 'ShipMentScales' [Description]  \n"
                    + "UNION SELECT  95 ID, 'Shipping' [Description]  \n"
                    + "UNION SELECT  96 ID, 'ShippingV2' [Description]  \n"
                    + "UNION SELECT  97 ID, 'SNLinkBatch' [Description]  \n"
                    + "UNION SELECT  98 ID, 'SNLinkBatchV2' [Description]  \n"
                    + "UNION SELECT  99 ID, 'Stop' [Description]  \n"
                    + "UNION SELECT  100 ID, 'ToolingAssembly' [Description]  \n"
                    + "UNION SELECT  101 ID, 'ToolingAssemblyNew' [Description]  \n"
                    + "UNION SELECT  102 ID, 'ToolingAssemblyV2' [Description]  \n"
                    + "UNION SELECT  103 ID, 'ToolingLinkTooling' [Description]  \n"
                    + "UNION SELECT  104 ID, 'ToolingLinkToolingV2' [Description]  \n"
                    + "UNION SELECT  105 ID, 'ToolingOverStationV2' [Description]  \n"
                    + "UNION SELECT  106 ID, 'ToolingPrint' [Description]  \n"
                    + "UNION SELECT  107 ID, 'ToolingQC' [Description]  \n"
                    + "UNION SELECT  108 ID, 'ToolingQCNGPrint' [Description]  \n"
                    + "UNION SELECT  109 ID, 'ToolingQCPrint' [Description]  \n"
                    + "UNION SELECT  110 ID, 'ToolReplaceUnit' [Description]  \n"
                    + "UNION SELECT  111 ID, 'TT_BindBox' [Description]  \n"
                    + "UNION SELECT  112 ID, 'TT_Registers' [Description]  \n"
                    + "UNION SELECT  113 ID, 'TT-OverStation' [Description]  \n"
                    + "UNION SELECT  114 ID, 'TT-OverStationV2' [Description]  \n"
                    + "UNION SELECT  115 ID, 'UnitReplaceTool' [Description]  \n"
                    + "UNION SELECT  116 ID, 'UPCPrint' [Description]  \n"
                    + "UNION SELECT  117 ID, 'WH' [Description]  \n"
                    + "UNION SELECT  118 ID, 'WH_Old' [Description]  \n"
                    + "UNION SELECT  119 ID, 'WH_Old-In' [Description]  \n"
                    + "UNION SELECT  120 ID, 'WH_Old-Out' [Description]  \n"
                    + "UNION SELECT  121 ID, 'WH_Siemens-In' [Description]  \n"
                    + "UNION SELECT  122 ID, 'WH_Siemens-Out' [Description] ";

                var Query_luApplicationType = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luApplicationType = Query_luApplicationType.ToList();

                //////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////
                S_Sql = "SELECT  1 ID, 'PASS' [Description]  \n"
                    + "UNION SELECT  2 ID, 'FAIL' [Description]  \n"
                    + "UNION SELECT  3 ID, 'SCRAP' [Description] ";
                var Query_luUnitStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luUnitStatus = Query_luUnitStatus.ToList();

                S_Sql = "SELECT  1 ID, 'Active' [Description]  \n"
                        + "UNION SELECT  2 ID, 'InActive' [Description] ";
                var Query_sysStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_sysStatus = Query_sysStatus.ToList();

                S_Sql = "SELECT  1 ID, 'RegisterBatchNo' [Description]  \n"
                    + "UNION SELECT  2 ID, 'RegisterMaterialSN' [Description]  \n"
                    + "UNION SELECT  3 ID, 'NonRegisterBatchNo' [Description] ";
                var Query_luMaterialType = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luMaterialType = Query_luMaterialType.ToList();


                S_Sql = "SELECT  1 ID, 'MovedShipmentDetailID' [Description]  \n"
                        + "UNION SELECT  2 ID, 'MovedShipmentInterID' [Description]  \n"
                        + "UNION SELECT  3 ID, 'MovedShipmentParentID' [Description]  \n"
                        + "UNION SELECT  4 ID, 'PackBoxWeight' [Description]  \n"
                        + "UNION SELECT  5 ID, 'LastBoxCount' [Description]  \n"
                        + "UNION SELECT  6 ID, 'PackPalletWeight' [Description]  ";

                var Query_luPackageDetailDef = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luPackageDetailDef = Query_luPackageDetailDef.ToList();


                S_Sql = "SELECT  0 ID, 'Generate Box' [Description]  \n"
                        + "UNION SELECT  1 ID, 'Complete Box' [Description]  \n"
                        + "UNION SELECT  2 ID, 'Complete Pallet' [Description]  \n"
                        + "UNION SELECT  3 ID, 'Unpack Box' [Description]  \n"
                        + "UNION SELECT  4 ID, 'Unpack Pallet' [Description]  \n"
                        + "UNION SELECT  5 ID, 'Generate Pallet' [Description]  \n"
                        + "UNION SELECT  6 ID, 'RePrint Multipack' [Description]  \n"
                        + "UNION SELECT  7 ID, 'RePrint ShipPallet' [Description]  \n"
                        + "UNION SELECT  8 ID, 'Complete ShipMent' [Description]  \n"
                        + "UNION SELECT  9 ID, 'MoveBox' [Description]  \n"
                        + "UNION SELECT  10 ID, 'MovePallet' [Description]  \n"
                        + "UNION SELECT  11 ID, 'WeighingCompleted' [Description] ";
                var Query_luPackageStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luPackageStatus = Query_luPackageStatus.ToList();


                S_Sql = "SELECT  1 ID, 'Active' [Description]  \n"
                        + "UNION SELECT  2 ID, 'InActive' [Description]  \n"
                        + "UNION SELECT  3 ID, 'Complete' [Description] ";
                var Query_luProductionOrderStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luProductionOrderStatus = Query_luProductionOrderStatus.ToList();


                S_Sql = "SELECT  0 ID, 'SerialNumber' [Description]  \n"
                        + "UNION SELECT  1 ID, 'ComponentSerialNumber' [Description]  \n"
                        + "UNION SELECT  2 ID, 'Material BatchNumber' [Description]  \n"
                        + "UNION SELECT  3 ID, 'S-FG SerialNumber' [Description]  \n"
                        + "UNION SELECT  4 ID, 'CartonBoxSN' [Description]  \n"
                        + "UNION SELECT  5 ID, 'FG SerialNumber' [Description]  \n"
                        + "UNION SELECT  6 ID, 'UPC SerialNumber' [Description]  \n"
                        + "UNION SELECT  7 ID, 'PalletSerialNumber' [Description]  \n"
                        + "UNION SELECT  8 ID, 'BoxBatchNumber' [Description]  \n"
                        + "UNION SELECT  9 ID, 'ToolingNumber' [Description] ";
                var Query_luSerialNumberType = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luSerialNumberType = Query_luSerialNumberType.ToList();

                S_Sql = "SELECT  0 ID, '停用' [Description]  \n"
                        + "UNION SELECT  1 ID, '正常' [Description]  \n"
                        + "UNION SELECT  2 ID, '锁定' [Description] ";
                var Query_luMachineStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luMachineStatus = Query_luMachineStatus.ToList();


                S_Sql = "SELECT  1 ID, 'Active' [Description]  \n"
                        + "UNION SELECT  0 ID, 'InActive' [Description] ";
                var Query_luEmployeeStatus = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                List_luEmployeeStatus = Query_luEmployeeStatus.ToList();

            }
            catch (Exception ex)
            {
                S_Result=ex.Message;
            }

            return S_Result;
        }

        public async Task<string> InitializeBaseData()
        {
            string S_Result = "OK";
            await GetData();
            try
            {
                string S_Sql = "";
                foreach (var item in List_luPartFamilyTypeDetailDef)
                {
                    S_Sql = "select * from luPartFamilyTypeDetailDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luPartFamilyTypeDetailDef \n"
                               + "INSERT INTO luPartFamilyTypeDetailDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result=ExecSql(S_Sql);

                        if (S_Result != "OK") 
                        {                           
                            return S_Result;                            
                        }    
                    }
                }

                foreach (var item in List_luPartFamilyDetailDef)
                {
                    S_Sql = "select * from luPartFamilyDetailDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luPartFamilyDetailDef \n"
                               + "INSERT INTO luPartFamilyDetailDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luPartDetailDef)
                {
                    S_Sql = "select * from luPartDetailDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luPartDetailDef \n"
                               + "INSERT INTO luPartDetailDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luProductionOrderDetailDef)
                {
                    S_Sql = "select * from luProductionOrderDetailDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luProductionOrderDetailDef \n"
                               + "INSERT INTO luProductionOrderDetailDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luLineTypeDef)
                {
                    S_Sql = "select * from luLineTypeDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luLineTypeDef \n"
                               + "INSERT INTO luLineTypeDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luApplicationType)
                {
                    S_Sql = "select * from luApplicationType where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luApplicationType \n"
                               + "INSERT INTO luApplicationType(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luPackageDetailDef)
                {
                    S_Sql = "select * from luPackageDetailDef where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luPackageDetailDef \n"
                               + "INSERT INTO luPackageDetailDef(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luPackageStatus)
                {
                    S_Sql = "select * from luPackageStatus where [Description]='" + item.Description + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "DECLARE @MaxID int \n"
                               + "SELECT  @MaxID=isnull(MAX(ID),0)+1 FROM luPackageStatus \n"
                               + "INSERT INTO luPackageStatus(ID,[Description]) VALUES(@MaxID,'" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }


                //////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////
                foreach (var item in List_luUnitStatus)
                {
                    S_Sql = "select * from luUnitStatus where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luUnitStatus(ID,[Description]) VALUES('" + item.ID + "','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else 
                    {
                        S_Sql = "Update luUnitStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_sysStatus)
                {
                    S_Sql = "select * from sysStatus where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO sysStatus(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update sysStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luMaterialType)
                {
                    S_Sql = "select * from luMaterialType where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luMaterialType(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update luMaterialType set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                //foreach (var item in List_luPackageDetailDef)
                //{
                //    S_Sql = "select * from luPackageDetailDef where ID='" + item.ID + "'";
                //    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                //    if (v_Query.Count() == 0)
                //    {
                //        S_Sql = "INSERT INTO luPackageDetailDef(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                //        S_Result = ExecSql(S_Sql);

                //        if (S_Result != "OK")
                //        {
                //            return S_Result;
                //        }
                //    }
                //}
                 
                //foreach (var item in List_luPackageStatus)
                //{
                //    S_Sql = "select * from luPackageStatus where ID='" + item.ID + "'";
                //    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                //    if (v_Query.Count() == 0)
                //    {
                //        S_Sql = "INSERT INTO luPackageStatus(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                //        S_Result = ExecSql(S_Sql);

                //        if (S_Result != "OK")
                //        {
                //            return S_Result;
                //        }
                //    }
                //    else
                //    {
                //        S_Sql = "Update luPackageStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                //        S_Result = ExecSql(S_Sql);

                //        if (S_Result != "OK")
                //        {
                //            return S_Result;
                //        }
                //    }
                //}

                foreach (var item in List_luProductionOrderStatus)
                {
                    S_Sql = "select * from luProductionOrderStatus where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luProductionOrderStatus(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update luProductionOrderStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

                foreach (var item in List_luSerialNumberType)
                {
                    S_Sql = "select * from luSerialNumberType where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luSerialNumberType(ID,[Description]) VALUES('"+item.ID+"','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update luSerialNumberType set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }


                foreach (var item in List_luMachineStatus)
                {
                    S_Sql = "select * from luMachineStatus where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luMachineStatus(ID,[Description]) VALUES('" + item.ID + "','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update luMachineStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }


                foreach (var item in List_luEmployeeStatus)
                {
                    S_Sql = "select * from luEmployeeStatus where ID='" + item.ID + "'";
                    var v_Query = await DapperConnRead2.QueryAsync<SC_IdDesc>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query.Count() == 0)
                    {
                        S_Sql = "INSERT INTO luEmployeeStatus(ID,[Description]) VALUES('" + item.ID + "','" + item.Description + "')";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                    else
                    {
                        S_Sql = "Update luEmployeeStatus set [Description]='" + item.Description + "' where ID='" + item.ID + "'";
                        S_Result = ExecSql(S_Sql);

                        if (S_Result != "OK")
                        {
                            return S_Result;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                S_Result = "ERROR:"+ex.ToString();
            }
            return S_Result;
        }

    }
}
