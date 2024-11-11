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
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using static Dapper.SqlMapper;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;
using NPOI.HSSF.UserModel;


namespace SunnyMES.Security.Repositories
{
    public class SC_ScreenshotRepositories : BaseRepositoryReport<string>, ISC_ScreenshotRepositories
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_ScreenshotRepositories()
        {
        }
        public SC_ScreenshotRepositories(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_ScreenshotDto v_SC_ScreenshotDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_ScreenshotDto.IP = v_SC_ScreenshotDto.IP ?? "";
            v_SC_ScreenshotDto.PCName = v_SC_ScreenshotDto.PCName ?? "";
            v_SC_ScreenshotDto.Feedback = v_SC_ScreenshotDto.Feedback ?? "";
            v_SC_ScreenshotDto.IsFeedback = v_SC_ScreenshotDto.IsFeedback ?? "0";

            try
            {
                string S_Sql=
                     "INSERT INTO mesScreenshot \n"
                       + "( \n"
                               + "	LineID, \n"
                               + "	StationID, \n"
                               + "	PartID, \n"
                               + "	ProductionOrderID, \n"
                               + "	IP, \n"
                               + "	PCName, \n"
                               + "	IMGURL, \n"
                               + "	MSG, \n"
                               + "	Feedback, \n"
                               + "	IsFeedback \n"
                       + ") \n"
                       + "VALUES \n"
                       + "( \n"
                               + "'"+ v_SC_ScreenshotDto.LineID + "' \n"
                               + ",'" + v_SC_ScreenshotDto.StationID + "' \n"
                               + ",'" + v_SC_ScreenshotDto.PartID + "' \n"
                               + ",'" + v_SC_ScreenshotDto.ProductionOrderID + "' \n"
                               + ",'" + v_SC_ScreenshotDto.IP + "' \n"
                               + ",'" + v_SC_ScreenshotDto.PCName + "' \n"
                               + ",'" + v_SC_ScreenshotDto.IMGURL + "' \n"
                               + ",'" + v_SC_ScreenshotDto.MSG + "' \n"
                               + ",'" + v_SC_ScreenshotDto.Feedback + "' \n"
                               + ",'" + v_SC_ScreenshotDto.IsFeedback + "' \n"
                       + ")";


                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                //string S_MSG = v_SC_ScreenshotDto.ToJson();
                //await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesScreenshot", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Update(SC_ScreenshotFeedDto v_SC_ScreenshotDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_ScreenshotDto.IsFeedback = v_SC_ScreenshotDto.IsFeedback ?? "1";

            try
            {
                string S_Sql =
                     "UPDATE mesScreenshot  set \n"+
                     " Feedback='"+ v_SC_ScreenshotDto.Feedback + "' \n"+
                     ",IsFeedback='"+v_SC_ScreenshotDto.IsFeedback+"' \n"+
                     " where Id='"+ v_SC_ScreenshotDto.ID + "'"
                     ;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

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
                SC_ScreenshotDto v_SC_ScreenshotDto = new SC_ScreenshotDto();
                string S_Sql = "select * from mesScreenshot where Id='" + Id + "'";
                v_SC_ScreenshotDto = await DapperConnRead2.QueryFirstAsync<SC_ScreenshotDto>(S_Sql);


                S_Sql = "delete mesScreenshot where Id='" + Id + "'"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_ScreenshotDto != null)
                {
                    string S_MSG = v_SC_ScreenshotDto.ToJson();
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesScreenshot", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }



        public async Task<List<SC_ScreenshotDto>> FindWithPagerMyAsync(SC_mesScreenshotSearch search, PagerInfo info)
        {
            search.IP = search.IP ?? "";
            search.PCName = search.PCName ?? "";
            search.Feedback = search.Feedback ?? "";
           


            search.LikeQuery = search.LikeQuery ?? "";


            List<SC_ScreenshotDto> List_Result = new List<SC_ScreenshotDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                if (search.CreateTimeStart.ToString("yyyy-MM-dd") == "0001-01-01") 
                {
                    //search.CreateTimeStart = DateTime.Now.AddDays(-1);
                    search.CreateTimeStart = Convert.ToDateTime ("2000-1-1");
                }
                
                if (search.CreateTimeEnd.ToString("yyyy-MM-dd") == "0001-01-01")
                {
                    //search.CreateTimeEnd = DateTime.Now;
                    search.CreateTimeEnd = Convert.ToDateTime("2000-1-1");
                }

                string S_Sql =
                @"
	            FROM mesScreenshot A
                    LEFT JOIN mesLine B ON A.LineID=B.ID
                    LEFT JOIN mesStation C ON A.StationID=C.ID
                    LEFT JOIN mesPart D ON A.PartID=D.ID
                    LEFT JOIN mesProductionOrder E ON A.ProductionOrderID=E.ID            
	            where 1=1                   
                   and ('" + search.LineID + @"'='' or A.LineID  IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                   and ('" + search.StationID + @"'='' or A.StationID  IN (SELECT value FROM dbo.F_Split('" + search.StationID + @"',',')))
                   and ('" + search.PartID + @"'='' or A.PartID  IN (SELECT value FROM dbo.F_Split('" + search.PartID + @"',',')))
                   and ('" + search.ProductionOrderID + @"'='' or A.ProductionOrderID  IN (SELECT value FROM dbo.F_Split('" + search.ProductionOrderID + @"',',')))

                   and ('" + search.IP + @"'='' or A.IP='"+ search.IP + @"') 
                   and ('" + search.PCName + @"'='' or A.PCName='" + search.PCName + @"') 
                   and ('" + search.IsFeedback + @"'='' or A.IsFeedback='" + search.IsFeedback + @"') 

                   AND ('"+ search.CreateTimeStart +
                        @"'=CAST('2000-1-1' AS DATETIME) OR CONVERT(DATETIME,CONVERT(VARCHAR, A.CreateTime) ) >= '"+
                        search.CreateTimeStart + @"')

                   AND ('"+ search.CreateTimeEnd +
                        @"'=CAST('2000-1-1' AS DATETIME) OR CONVERT(DATETIME,CONVERT(VARCHAR, A.CreateTime) ) <= '"+
                        search.CreateTimeEnd + @"')

                   and ( '" + search.LikeQuery + @"'='' or A.IP Like '%" + search.LikeQuery + @"%'
                            or A.PCName Like '%" + search.LikeQuery + @"%'
                            or A.IsFeedback Like '%" + search.IsFeedback + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.*,B.[Description] Line,C.[Description] Station,D.PartNumber Part,E.ProductionOrderNumber  ProductionOrder FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID desc offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
                        LEFT JOIN mesLine B ON A.LineID=B.ID
                        LEFT JOIN mesStation C ON A.StationID=C.ID
                        LEFT JOIN mesPart D ON A.PartID=D.ID
                        LEFT JOIN mesProductionOrder E ON A.ProductionOrderID=E.ID
                        ORDER BY A.ID DESC
                        "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_ScreenshotDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_ScreenshotDto> list = v_query.ToList();

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