﻿using API_MSG;
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
    public class SC_IdNameDescRepository : BaseRepositoryReport<string>, ISC_IdNameDescRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_IdNameDescRepository()
        {
        }
        public SC_IdNameDescRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_IdNameDesc v_SC_IdNameDesc, string S_TabName, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_IdNameDesc.Description = v_SC_IdNameDesc.Description ?? "";

            try
            {
                string S_Sql = "select * from "+ S_TabName + " where Name='" + v_SC_IdNameDesc.Name.Trim() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_IdNameDesc>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_IdNameDesc.Description + " Data already exists.";
                }

                S_Sql = "insert into "+ S_TabName + "([Description], Name) Values(" +
                     "'" + v_SC_IdNameDesc.Description + "'" + "\r\n" +
                     ",'" + v_SC_IdNameDesc.Name + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_IdNameDesc.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", S_TabName, S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Delete(string Id, string S_TabName, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                SC_IdNameDesc v_SC_IdNameDesc = new SC_IdNameDesc();
                string S_Sql = "select * from "+ S_TabName + " where Id='" + Id + "'";
                v_SC_IdNameDesc = await DapperConnRead2.QueryFirstAsync<SC_IdNameDesc>(S_Sql);


                S_Sql = "delete "+ S_TabName + " where Id='" + Id + "' "                       
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_IdNameDesc != null)
                {
                    string S_MSG = v_SC_IdNameDesc.ToJson();
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", S_TabName, S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_IdNameDesc v_SC_IdNameDesc, string S_TabName, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_IdNameDesc.Description = v_SC_IdNameDesc.Description ?? "";

            try
            {
                string S_Sql = "select * from "+ S_TabName + " where Name='" + v_SC_IdNameDesc.Name.Trim() + "'" +
                                " and ID<>'" + v_SC_IdNameDesc.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_IdNameDesc>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_IdNameDesc.Name + " Data already exists.";
                }


                SC_IdNameDesc v_IdNameDesc_before = new SC_IdNameDesc();
                S_Sql = "select * from "+ S_TabName + " where Id='" + v_SC_IdNameDesc.ID + "'";
                v_IdNameDesc_before = await DapperConnRead2.QueryFirstAsync<SC_IdNameDesc>(S_Sql);

                S_Sql = "Update "+ S_TabName + " set " +
                        " Description='" + v_SC_IdNameDesc.Description + "'" +
                        " ,Name='" + v_SC_IdNameDesc.Name + "'" +

                        " where Id='" + v_SC_IdNameDesc.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_IdNameDesc_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_IdNameDesc_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_IdNameDesc.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", S_TabName, S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_IdNameDesc v_SC_IdNameDesc, string S_TabName, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_IdNameDesc.Description = v_SC_IdNameDesc.Description ?? "";

            try
            {
                string S_Sql = "select * from "+ S_TabName + " where Name='" + v_SC_IdNameDesc.Name.Trim() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_IdNameDesc>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result =  v_SC_IdNameDesc.Name + " Data already exists.";
                }


                SC_IdNameDesc v_IdNameDesc_before = new SC_IdNameDesc();
                S_Sql = "select * from "+ S_TabName + " where Id='" + v_SC_IdNameDesc.ID + "'";
                v_IdNameDesc_before = await DapperConnRead2.QueryFirstAsync<SC_IdNameDesc>(S_Sql);



                S_Sql = "insert into "+ S_TabName + "([Description], Name) Values(" +
                     "'" + v_SC_IdNameDesc.Description + "'" + "\r\n" +
                     ",'" + v_SC_IdNameDesc.Name + "'" + "\r\n" +

                    ")";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_IdNameDesc_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_IdNameDesc_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_IdNameDesc.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", S_TabName, S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_IdNameDesc>> FindWithPagerMyAsync(SC_IdNameDescSearch search, string S_TabName, PagerInfo info)
        {
            //search.ID = search.ID ?? "";
            search.Description = search.Description ?? "";
            search.Name = search.Name ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_IdNameDesc> List_Result = new List<SC_IdNameDesc>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM "+ S_TabName + @" A        
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )
                   and ('" + search.Name + @"'='' or A.Name Like '%" + search.Name + @"%' )
                  
                   and ( '" + search.LikeQuery + @"'='' or A.Description Like '%" + search.LikeQuery + @"%'
                            or A.Name Like '%" + search.Name + @"%'                            

                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.ID, A.Name ,A.[Description]            
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
                    "
                    ;

                RefAsync<int> totalCount = 0;
                string S_SqlCount = S_Count + S_Sql;

                var v_query = await DapperConnRead2.QueryAsync<SC_IdNameDesc>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_IdNameDesc> list = v_query.ToList();
                List_Result = list;

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