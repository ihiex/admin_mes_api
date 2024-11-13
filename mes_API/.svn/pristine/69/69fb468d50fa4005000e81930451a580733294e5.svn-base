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
    public class SiemensDB: BaseRepositoryReport<string>
    {
        string S_Conn = "";
        string S_StationTypeID = "";
        public string ProjectType = "";

        protected MSG_Public P_MSG_Public;
        protected MSG_Sys msgSys;

        public SiemensDB() { }

        public SiemensDB(IDbContextCore dbContext, int I_Language, string StationTypeID) : base(dbContext)
        {
            S_StationTypeID = StationTypeID;
            P_MSG_Public = new MSG_Public(I_Language);
            msgSys = new MSG_Sys(I_Language);


            S_Conn = DapperConnRead2.ConnectionString;
            string S_Sql =
            @"
                    select *  from mesStationTypeDetail A 
                         join luStationTypeDetailDef B on A.StationTypeDetailDefID=B.ID
                    where A.StationTypeID=" + S_StationTypeID + @" and B.Description='SiemensDBConn'  
                ";
            DataTable DT_SiemensDBConn = Data_Table(S_Sql);
            if (DT_SiemensDBConn.Rows.Count > 0)
            {
                S_Conn = DT_SiemensDBConn.Rows[0]["Content"].ToString();
            }
            ProjectType = GetProjectType();
        }

        private string GetProjectType() 
        {
            string S_Reult = "";

            if (S_StationTypeID == "")
            {
                S_Reult = "";
            }
            else
            {
                string S_Sql =
                @"
                    select *  from mesStationTypeDetail A 
                         join luStationTypeDetailDef B on A.StationTypeDetailDefID=B.ID
                    where A.StationTypeID=" + S_StationTypeID + @" and B.Description='SiemensProjectType'  
                ";
                DataTable DT_SiemensDBName = Data_Table(S_Sql);
                if (DT_SiemensDBName.Rows.Count == 0)
                {
                    S_Reult = "";
                }
                else
                {
                    S_Reult = DT_SiemensDBName.Rows[0]["Content"].ToString();
                }
            }
            return S_Reult;
        }


        public  DataSet DB_Data_Set(string S_Sql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection Conn = new SqlConnection(S_Conn);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                SqlDataAdapter Sql_DA = new SqlDataAdapter(cmd);
                Sql_DA.Fill(ds);
                Conn.Close();
            }
            catch (Exception ex)
            {
                
            }

            return ds;
        }

        public DataTable DB_Data_Table(string S_Sql)
        {
            DataTable DT = new DataTable();
            try
            {
                SqlConnection Conn = new SqlConnection(S_Conn);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                SqlDataAdapter Sql_DA = new SqlDataAdapter(cmd);
                Sql_DA.Fill(DT);
                Sql_DA.Dispose();
                Conn.Close();
            }
            catch (Exception ex)
            {
                
            }
            return DT;
        }

        public string DB_ExecSql(string S_Sql)
        {
            string S_Result = "OK";
            try
            {
                SqlConnection Conn = new SqlConnection(S_Conn);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                cmd.ExecuteNonQuery();
                Conn.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }

    }
}
