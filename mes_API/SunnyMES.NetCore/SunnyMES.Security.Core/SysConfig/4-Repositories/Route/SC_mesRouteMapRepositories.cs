
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
    public class SC_mesRouteMapRepository : BaseRepositoryReport<string>, ISC_mesRouteMapRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesRouteMapRepository()
        {
        }
        public SC_mesRouteMapRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesRouteMap v_SC_mesRouteMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID ?? "0";
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID ?? "0";            
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID ?? "0";
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID ?? "0";
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID ?? "0";


            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID==""?"0": v_SC_mesRouteMap.PartFamilyID;
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID==""?"0": v_SC_mesRouteMap.PartID;
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID==""?"0": v_SC_mesRouteMap.LineID;
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID == "" ? "0" : v_SC_mesRouteMap.RouteID;
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID == "" ? "0" : v_SC_mesRouteMap.ProductionOrderID;

            try
            {
                string S_Sql = "select * from mesRouteMap where PartFamilyID='" + v_SC_mesRouteMap.PartFamilyID + "'" +
                                                            " and LineID='" + v_SC_mesRouteMap.LineID + "'" +
                                                            " and ProductionOrderID='" + v_SC_mesRouteMap.ProductionOrderID + "'" +
                                                            " and PartID='" + v_SC_mesRouteMap.PartID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                S_Sql = "INSERT INTO mesRouteMap(PartFamilyID,PartID,LineID,RouteID,ProductionOrderID) VALUES(" +
                     "'" + v_SC_mesRouteMap.PartFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.PartID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.RouteID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.ProductionOrderID + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesRouteMap.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesRouteMap", S_MSG);
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
                SC_mesRouteMap v_SC_mesRouteMap = new SC_mesRouteMap();
                string S_Sql = "select * from mesRouteMap where Id='" + Id + "'";
                v_SC_mesRouteMap = await DapperConnRead2.QueryFirstAsync<SC_mesRouteMap>(S_Sql);

                S_Sql = "delete mesRouteMap where Id='" + Id + "' " + "\r\n"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesRouteMap != null)
                {
                    string S_MSG = v_SC_mesRouteMap.ToJson() + "\r\n"
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesRouteMap", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesRouteMap v_SC_mesRouteMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID ?? "0";
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID ?? "0";
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID ?? "0";
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID ?? "0";
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID ?? "0";

            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID == "" ? "0" : v_SC_mesRouteMap.PartFamilyID;
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID == "" ? "0" : v_SC_mesRouteMap.PartID;
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID == "" ? "0" : v_SC_mesRouteMap.LineID;
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID == "" ? "0" : v_SC_mesRouteMap.RouteID;
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID == "" ? "0" : v_SC_mesRouteMap.ProductionOrderID;

            try
            {
                string S_Sql = "select * from mesRouteMap where  PartFamilyID='" + v_SC_mesRouteMap.PartFamilyID + "'" +
                                                                   " and PartID='" + v_SC_mesRouteMap.PartID + "'" +
                                                                   " and LineID='" + v_SC_mesRouteMap.LineID + "'" +
                                                                   " and ProductionOrderID='" + v_SC_mesRouteMap.ProductionOrderID + "'" +
                                                                   " and ID<>'" + v_SC_mesRouteMap.ID.ToString() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteMap>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRouteMap v_mesRouteMap_before = new SC_mesRouteMap();
                S_Sql = "select * from mesRouteMap where Id='" + v_SC_mesRouteMap.ID + "'";
                v_mesRouteMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesRouteMap>(S_Sql);

                S_Sql = "UPDATE mesRouteMap SET" + "\r\n" +
                            "PartFamilyID=" + v_SC_mesRouteMap.PartFamilyID + "," + "\r\n" +
                            "PartID=" + v_SC_mesRouteMap.PartID + "," + "\r\n" +
                            "LineID=" + v_SC_mesRouteMap.LineID + "," + "\r\n" +
                            "ProductionOrderID =" + v_SC_mesRouteMap.ProductionOrderID + "," + "\r\n" +
                            "RouteID =" + v_SC_mesRouteMap.RouteID + "\r\n" +
                        "WHERE ID=" + v_SC_mesRouteMap.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRouteMap_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesRouteMap_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesRouteMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesRouteMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesRouteMap v_SC_mesRouteMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID ?? "NULL";
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID ?? "NULL";
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID ?? "NULL";
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID ?? "NULL";
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID ?? "NULL";

            v_SC_mesRouteMap.PartFamilyID = v_SC_mesRouteMap.PartFamilyID == "" ? "0" : v_SC_mesRouteMap.PartFamilyID;
            v_SC_mesRouteMap.PartID = v_SC_mesRouteMap.PartID == "" ? "0" : v_SC_mesRouteMap.PartID;
            v_SC_mesRouteMap.LineID = v_SC_mesRouteMap.LineID == "" ? "0" : v_SC_mesRouteMap.LineID;
            v_SC_mesRouteMap.RouteID = v_SC_mesRouteMap.RouteID == "" ? "0" : v_SC_mesRouteMap.RouteID;
            v_SC_mesRouteMap.ProductionOrderID = v_SC_mesRouteMap.ProductionOrderID == "" ? "0" : v_SC_mesRouteMap.ProductionOrderID;

            try
            {
                string S_Sql = "select * from mesRouteMap where PartFamilyID='" + v_SC_mesRouteMap.PartFamilyID + "'" +
                                                            " and LineID='" + v_SC_mesRouteMap.LineID + "'" +
                                                            " and ProductionOrderID='" + v_SC_mesRouteMap.ProductionOrderID + "'" +
                                                            " and PartID='" + v_SC_mesRouteMap.PartID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRouteMap v_mesRouteMap_before = new SC_mesRouteMap();
                S_Sql = "select * from mesRouteMap where Id='" + v_SC_mesRouteMap.ID + "'";
                v_mesRouteMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesRouteMap>(S_Sql);



                S_Sql = "INSERT INTO mesRouteMap(PartFamilyID,PartID,LineID,RouteID,ProductionOrderID) VALUES(" +
                     "'" + v_SC_mesRouteMap.PartFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.PartID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.RouteID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteMap.ProductionOrderID + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRouteMap_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesRouteMap_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesRouteMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesRouteMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesRouteMapDto>> FindWithPagerMyAsync(SC_mesRouteMapSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";

            search.PartFamilyID = search.PartFamilyID ?? "";
            search.PartFamily = search.PartFamily ?? "";

            search.PartID = search.PartID ?? "";
            search.Part = search.Part ?? "";
            search.LineID = search.LineID ?? "";
            search.Line = search.Line ?? "";
            search.RouteID = search.RouteID ?? "";
            search.Route = search.Route ?? "";
            search.ProductionOrderID = search.ProductionOrderID ?? "";
            search.ProductionOrder = search.ProductionOrder ?? "";

            search.LikeQuery = search.LikeQuery ?? "";


            List<SC_mesRouteMapDto> List_Result = new List<SC_mesRouteMapDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesRouteMap A 
		            left join luPartFamily B on A.PartFamilyID=B.ID
			        left join mesPart C on A.PartID=C.ID
			        left join mesLine D on A.LineID=D.ID
			        left join mesRoute E on A.RouteID=E.ID
			        left join mesProductionOrder F on A.ProductionOrderID=F.ID	
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))

                   and ('" + search.PartFamilyID + @"'='' or A.PartFamilyID    IN (SELECT value FROM dbo.F_Split('" + search.PartFamilyID + @"',',')))
                   and ('" + search.PartID + @"'='' or A.PartID   IN (SELECT value FROM dbo.F_Split('" + search.PartID + @"',',')))
                   and ('" + search.LineID + @"'='' or A.LineID   IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                   and ('" + search.RouteID + @"'='' or A.RouteID    IN (SELECT value FROM dbo.F_Split('" + search.RouteID + @"',',')))
                   and ('" + search.ProductionOrderID + @"'='' or A.ProductionOrderID   IN (SELECT value FROM dbo.F_Split('" + search.ProductionOrderID + @"',',')))
                  
                   and ( '" + search.LikeQuery + @"'='' or B.Name Like '%" + search.LikeQuery + @"%'
                            or C.PartNumber Like '%" + search.LikeQuery + @"%'                            
                            or D.Description Like '%" + search.LikeQuery + @"%'
                            or E.Name  Like '%" + search.LikeQuery + @"%'

                            or F.ProductionOrderNumber Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
		            select A.*,B.Name PartFamily,C.PartNumber,D.Description Line,
			                E.Name Route,F.ProductionOrderNumber
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
		                left join luPartFamily B on A.PartFamilyID=B.ID
			            left join mesPart C on A.PartID=C.ID
			            left join mesLine D on A.LineID=D.ID
			            left join mesRoute E on A.RouteID=E.ID
			            left join mesProductionOrder F on A.ProductionOrderID=F.ID	
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesRouteMapDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesRouteMapDto> list = v_query.ToList();
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



