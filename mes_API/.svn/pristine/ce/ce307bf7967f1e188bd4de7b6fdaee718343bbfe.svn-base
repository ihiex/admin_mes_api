using API_MSG;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
using NPOI.POIFS.Properties;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using Quartz.Impl.Triggers;
using SQLitePCL;
using SqlSugar;
using StackExchange.Profiling.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
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
    public class SC_mesStationTypeAccessRepository : BaseRepositoryReport<string>, ISC_mesStationTypeAccessRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesStationTypeAccessRepository()
        {
        }
        public SC_mesStationTypeAccessRepository(IDbContextCore context) : base(context)
        {
            DB_Context = context;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            List_Login.LanguageID = I_Language;
            List_Login.LineID = I_LineID;
            List_Login.StationID = I_StationID;
            List_Login.EmployeeID = I_EmployeeID;
            List_Login.CurrentLoginIP = S_CurrentLoginIP;

            string S_Sql = "select Lastname+'.'+Firstname ValStr1  from mesEmployee where ID='" + I_EmployeeID + "'";
            var v_Query_Employee = await DapperConnRead2.QueryFirstAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query_Employee != null)
            {
                List_Login.UserName = v_Query_Employee.ValStr1;
            }


            if (P_PublicSCRepository == null)
            {
                P_PublicSCRepository = new PublicSCRepository(DB_Context, I_Language);
            }
            if (P_MSG_Sys == null)
            {
                P_MSG_Sys = new MSG_Sys(I_Language);
            }


            if (Public_Repository == null)
            {
                Public_Repository = new PublicMiniRepository(DB_Context, I_Language);
            }


            S_Sql = "select 'ok' as ValStr1,'' ValStr2,'1' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<string> Insert(SC_mesStationTypeAccess v_SC_mesStationTypeAccess, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeAccess.StationTypeID = v_SC_mesStationTypeAccess.StationTypeID ?? "NULL";
            v_SC_mesStationTypeAccess.EmployeeID = v_SC_mesStationTypeAccess.EmployeeID ?? "NULL";
            v_SC_mesStationTypeAccess.Status = v_SC_mesStationTypeAccess.Status ?? "NULL";

            if (v_SC_mesStationTypeAccess.Status == "true"  || v_SC_mesStationTypeAccess.Status == "1")
            {
                v_SC_mesStationTypeAccess.Status = "1";
            }
            else 
            {
                v_SC_mesStationTypeAccess.Status = "0";
            }

            try
            {
                string S_Sql = "select * from mesStationTypeAccess where StationTypeID='" + v_SC_mesStationTypeAccess.StationTypeID +
                    "' and  EmployeeID='" + v_SC_mesStationTypeAccess.EmployeeID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeAccessDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                S_Sql = "INSERT INTO mesStationTypeAccess(StationTypeID,EmployeeID,Status) VALUES(" +
                     "'" + v_SC_mesStationTypeAccess.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesStationTypeAccess.EmployeeID + "'" + "\r\n" +

                     ",'" + v_SC_mesStationTypeAccess.Status + "'" + "\r\n" +
                    ")";


                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesStationTypeAccess.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStationTypeAccess", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Delete(string Id, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                SC_mesStationTypeAccess v_SC_mesStationTypeAccess = new SC_mesStationTypeAccess();
                string S_Sql = "select * from mesStationTypeAccess where Id='" + Id + "'";
                v_SC_mesStationTypeAccess = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeAccess>(S_Sql);

                S_Sql = "delete mesStationTypeAccess where Id='" + Id + "' " + "\r\n"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesStationTypeAccess != null)
                {
                    string S_MSG = v_SC_mesStationTypeAccess.ToJson() + "\r\n"
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStationTypeAccess", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesStationTypeAccess v_SC_mesStationTypeAccess, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeAccess.StationTypeID = v_SC_mesStationTypeAccess.StationTypeID ?? "NULL";
            v_SC_mesStationTypeAccess.EmployeeID = v_SC_mesStationTypeAccess.EmployeeID ?? "NULL";
            v_SC_mesStationTypeAccess.Status = v_SC_mesStationTypeAccess.Status ?? "NULL";

            if (v_SC_mesStationTypeAccess.Status == "true" || v_SC_mesStationTypeAccess.Status == "1")
            {
                v_SC_mesStationTypeAccess.Status = "1";
            }
            else
            {
                v_SC_mesStationTypeAccess.Status = "0";
            }

            try
            {
                string S_Sql = "select * from mesStationTypeAccess where StationTypeID='" + v_SC_mesStationTypeAccess.StationTypeID + "' and  EmployeeID='" +
                    v_SC_mesStationTypeAccess.EmployeeID + "' and ID<>'" + v_SC_mesStationTypeAccess.ID.ToString() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeAccess>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesStationTypeAccess v_mesStationTypeAccess_before = new SC_mesStationTypeAccess();
                S_Sql = "select * from mesStationTypeAccess where Id='" + v_SC_mesStationTypeAccess.ID + "'";
                v_mesStationTypeAccess_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeAccess>(S_Sql);

                S_Sql = "UPDATE mesStationTypeAccess SET" + "\r\n" +
                            "StationTypeID=" + v_SC_mesStationTypeAccess.StationTypeID + "," + "\r\n" +
                            "EmployeeID=" + v_SC_mesStationTypeAccess.EmployeeID + "," + "\r\n" +

                            "Status =" + v_SC_mesStationTypeAccess.Status + "\r\n" +
                        "WHERE ID=" + v_SC_mesStationTypeAccess.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStationTypeAccess_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesStationTypeAccess_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesStationTypeAccess.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStationTypeAccess", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesStationTypeAccess v_SC_mesStationTypeAccess, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeAccess.StationTypeID = v_SC_mesStationTypeAccess.StationTypeID ?? "NULL";
            v_SC_mesStationTypeAccess.EmployeeID = v_SC_mesStationTypeAccess.EmployeeID ?? "NULL";
            v_SC_mesStationTypeAccess.Status = v_SC_mesStationTypeAccess.Status ?? "NULL";

            if (v_SC_mesStationTypeAccess.Status == "true")
            {
                v_SC_mesStationTypeAccess.Status = "1";
            }
            else
            {
                v_SC_mesStationTypeAccess.Status = "0";
            }

            try
            {
                string S_Sql = "select * from mesStationTypeAccess where StationTypeID='" + v_SC_mesStationTypeAccess.StationTypeID +
                    "' and  EmployeeID='" + v_SC_mesStationTypeAccess.EmployeeID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeAccessDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesStationTypeAccess v_mesStationTypeAccess_before = new SC_mesStationTypeAccess();
                S_Sql = "select * from mesStationTypeAccess where Id='" + v_SC_mesStationTypeAccess.ID + "'";
                v_mesStationTypeAccess_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeAccess>(S_Sql);



                S_Sql = "INSERT INTO mesStationTypeAccess(StationTypeID,EmployeeID,Status) VALUES(" +
                     "'" + v_SC_mesStationTypeAccess.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesStationTypeAccess.EmployeeID + "'" + "\r\n" +

                     ",'" + v_SC_mesStationTypeAccess.Status + "'" + "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStationTypeAccess_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesStationTypeAccess_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesStationTypeAccess.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesStationTypeAccess", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesStationTypeAccessDto>> FindWithPagerMyAsync(SC_mesStationTypeAccessSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";

            search.StationTypeID = search.StationTypeID ?? "";
            search.EmployeeID = search.EmployeeID ?? "";
            search.Status = search.Status ?? "";

            search.LikeQuery = search.LikeQuery ?? "";


            List<SC_mesStationTypeAccessDto> List_Result = new List<SC_mesStationTypeAccessDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesStationTypeAccess A 
					join mesStationType B on A.StationTypeID=B.ID
		    		join mesEmployee C on A.EmployeeID=C.ID	
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))

                   and ('" + search.StationTypeID + @"'='' or A.StationTypeID   IN (SELECT value FROM dbo.F_Split('" + search.StationTypeID + @"',',')))
                   and ('" + search.EmployeeID + @"'='' or A.EmployeeID   IN (SELECT value FROM dbo.F_Split('" + search.EmployeeID + @"',',')))
                    and ('" + search.Status + @"'='' or A.[Status]   IN (SELECT value FROM dbo.F_Split('" + search.Status + @"',',')))
                  
                   and ( '" + search.LikeQuery + @"'='' or B.Description Like '%" + search.LikeQuery + @"%'

                            or C.Lastname Like '%" + search.LikeQuery + @"%'
                            or C.Firstname Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    select A.ID, A.StationTypeID,B.Description StationType,A.EmployeeID, 
				        C.Lastname+C.Firstname Employee,A.Status
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
					join mesStationType B on A.StationTypeID=B.ID
		    		join mesEmployee C on A.EmployeeID=C.ID	
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesStationTypeAccessDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesStationTypeAccessDto> list = v_query.ToList();
                List_Result = list;

                RefAsync<int> totalCount = 0;
                string S_SqlCount = S_Count + S_Sql;

                var v_Count = await DapperConnRead2.QueryAsync<TabVal>(S_SqlCount, null, null, I_DBTimeout, null);
                List<TabVal> List_TabVal = v_Count.ToList();
                totalCount = List_TabVal[0].Valint1;
                info.RecordCount = totalCount;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return List_Result;
        }


    }
}




