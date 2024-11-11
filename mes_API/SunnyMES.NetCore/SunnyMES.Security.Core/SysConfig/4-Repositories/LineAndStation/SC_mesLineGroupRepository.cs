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


namespace SunnyMES.Security.Repositories
{
    public class SC_mesLineGroupRepository : BaseRepositoryReport<string>, ISC_mesLineGroupRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesLineGroupRepository()
        {
        }
        public SC_mesLineGroupRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesLineGroup v_SC_mesLineGroupDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = @"select * from mesLineGroup where LineGroupName='" + 
                                    v_SC_mesLineGroupDto.LineGroupName.Trim() + @"'
                                    and LineID='"+ v_SC_mesLineGroupDto.LineID + @"'
                                    and LineNumber='" + v_SC_mesLineGroupDto.LineNumber + @"'
                                    and LineType='" + v_SC_mesLineGroupDto.LineType + @"'
                                    and PartFamilyTypeID='" + v_SC_mesLineGroupDto.PartFamilyTypeID + @"'
                                "                                
                               ;
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineGroupDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                S_Sql = "insert into mesLineGroup(LineGroupName,LineID,LineNumber,LineType,PartFamilyTypeID) Values(" +
                     "'" + v_SC_mesLineGroupDto.LineGroupName.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.LineNumber + "'" + "\r\n" +
                      ",'" + v_SC_mesLineGroupDto.LineType + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.PartFamilyTypeID + "'" + "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesLineGroupDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesLine", S_MSG);
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
                SC_mesLineGroupDto v_SC_mesLineGroupDto = new SC_mesLineGroupDto();
                string S_Sql = "select * from mesLineGroup where Id='" + Id + "'";
                v_SC_mesLineGroupDto = await DapperConnRead2.QueryFirstAsync<SC_mesLineGroupDto>(S_Sql);

                S_Sql = "delete mesLineGroup where Id='" + Id + "' ";                        
                       
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesLineGroupDto != null)
                {
                    string S_MSG = v_SC_mesLineGroupDto.ToJson() + "\r\n" +
                           v_SC_mesLineGroupDto.ToJson()
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesLineGroup", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesLineGroupDto v_SC_mesLineGroupDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            try
            {
                string S_Sql = @"select * from mesLineGroup where LineGroupName='" +
                                    v_SC_mesLineGroupDto.LineGroupName.Trim() + @"'
                                    and LineID='" + v_SC_mesLineGroupDto.LineID + @"'
                                    and LineNumber='" + v_SC_mesLineGroupDto.LineNumber + @"'
                                    and LineType='" + v_SC_mesLineGroupDto.LineType + @"'
                                    and PartFamilyTypeID='" + v_SC_mesLineGroupDto.PartFamilyTypeID + @"'
                                    and ID<>'" + v_SC_mesLineGroupDto.ID + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineGroupDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesLineGroupDto v_mesLineDto_before = new SC_mesLineGroupDto();
                S_Sql = "select * from mesLineGroup where Id='" + v_SC_mesLineGroupDto.ID + "'";
                v_mesLineDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLineGroupDto>(S_Sql);

                S_Sql = "Update mesLineGroup set " +
                        " LineGroupName='" + v_SC_mesLineGroupDto.LineGroupName + "'" +
                        " ,LineID='" + v_SC_mesLineGroupDto.LineID + "'" +
                        " ,LineNumber='" + v_SC_mesLineGroupDto.LineNumber + "'" +
                        " ,LineType='" + v_SC_mesLineGroupDto.LineType + "'" +
                        " ,PartFamilyTypeID='" + v_SC_mesLineGroupDto.PartFamilyTypeID + "'" +
                        " where Id='" + v_SC_mesLineGroupDto.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesLineDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesLineDto_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesLineGroupDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesLineGroup", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesLineGroupDto v_SC_mesLineGroupDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = @"select * from mesLineGroup where LineGroupName='" +
                                    v_SC_mesLineGroupDto.LineGroupName.Trim() + @"'
                                    and LineID='" + v_SC_mesLineGroupDto.LineID + @"'
                                    and LineNumber='" + v_SC_mesLineGroupDto.LineNumber + @"'
                                    and LineType='" + v_SC_mesLineGroupDto.LineType + @"'
                                    and PartFamilyTypeID='" + v_SC_mesLineGroupDto.PartFamilyTypeID + @"'
                                "
                               ;
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineGroupDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                SC_mesLineGroupDto v_mesLineDto_before = new SC_mesLineGroupDto();
                S_Sql = "select * from mesLineGroup where Id='" + v_SC_mesLineGroupDto.ID + "'";
                v_mesLineDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLineGroupDto>(S_Sql);

                S_Sql = "insert into mesLineGroup(LineGroupName,LineID,LineNumber,LineType,PartFamilyTypeID) Values(" +
                     "'" + v_SC_mesLineGroupDto.LineGroupName.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.LineNumber + "'" + "\r\n" +
                      ",'" + v_SC_mesLineGroupDto.LineType + "'" + "\r\n" +
                     ",'" + v_SC_mesLineGroupDto.PartFamilyTypeID + "'" + "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesLineDto_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesLineDto_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesLineGroupDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesLineGroup", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesLineGroupSearch>> FindWithPagerMyAsync(SC_mesLineGroupSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.LineGroupName = search.LineGroupName ?? "";
            search.LineID = search.LineID ?? "";
            search.LineNumber = search.LineNumber ?? "";
            search.LineType = search.LineType ?? "";
            search.PartFamilyTypeID = search.PartFamilyTypeID ?? "";

            search.Line = search.Line ?? "";
            search.PartFamilyType = search.PartFamilyType ?? "";
            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesLineGroupSearch> List_Result = new List<SC_mesLineGroupSearch>();
            try
            {
                string S_Page =
                @"SELECT A.ID,A.LineGroupName, A.LineID,B.[Description] Line, A.LineNumber, A.LineType,
                      A.PartFamilyTypeID,C.Name PartFamilyType" + "\r\n";

                string S_Count = "SELECT COUNT(A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesLineGroup A 
                    LEFT JOIN mesLine B ON A.LineID=B.ID 
                    LEFT JOIN luPartFamilyType C ON A.PartFamilyTypeID=C.ID             
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.LineID + @"'='' or B.ID  IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                   and ('" + search.PartFamilyTypeID + @"'='' or A.PartFamilyTypeID  IN (SELECT value FROM dbo.F_Split('" + search.PartFamilyTypeID + @"',',')))
                   and ('" + search.LineGroupName + @"'='' or A.LineGroupName Like '%" + search.LineGroupName + @"%' )
                   and ('" + search.Line + @"'='' or B.Description  Like '%" + search.Line + @"%'  )
                   and ('" + search.PartFamilyType + @"'='' or C.Name Like '%" + search.PartFamilyType + @"%' )

                   and ( '" + search.LikeQuery + @"'='' or A.LineGroupName Like '%" + search.LikeQuery + @"%'
                            or B.Description Like '%" + search.LikeQuery + @"%'
                            or C.Name Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" 
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesLineGroupSearch>(S_SqlPage, null, null, I_DBTimeout, null);
                List_Result = v_query.ToList();


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
