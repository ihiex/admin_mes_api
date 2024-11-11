
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
    public class SC_mesStationTypeLabelMapRepository : BaseRepositoryReport<string>, ISC_mesStationTypeLabelMapRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesStationTypeLabelMapRepository()
        {
        }
        public SC_mesStationTypeLabelMapRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesStationTypeLabelMap v_SC_mesStationTypeLabelMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "";
            v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "";
            v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "";
            v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "";
            v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "";
            v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "";

            try
            {
                string S_Sql = "select * from mesStationTypeLabelMap where StationTypeID='" + v_SC_mesStationTypeLabelMap.StationTypeID + "'" + "\r\n" +
                    @"and(LabelID is NULL or LabelID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LabelID + @"', ',')))
                      and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartFamilyID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.ProductionOrderID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LineID + @"', ',')))
                    ";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeLabelMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "NULL";
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "NULL";

                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.StationTypeID;
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LabelID;
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartID;
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartFamilyID;
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.ProductionOrderID;
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LineID;


                S_Sql = "INSERT INTO mesStationTypeLabelMap(StationTypeID, LabelID, PartID, PartFamilyID, ProductionOrderID, LineID) VALUES(" +
                        v_SC_mesStationTypeLabelMap.StationTypeID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.LabelID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.PartID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.PartFamilyID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.ProductionOrderID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.LineID + "\r\n" +                                    
                    ")";


                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesStationTypeLabelMap.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStationTypeLabelMap", S_MSG);
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
                SC_mesStationTypeLabelMap v_SC_mesStationTypeLabelMap = new SC_mesStationTypeLabelMap();
                string S_Sql = "select * from mesStationTypeLabelMap where Id='" + Id + "'";
                v_SC_mesStationTypeLabelMap = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeLabelMap>(S_Sql);

                S_Sql = "delete mesStationTypeLabelMap where Id='" + Id + "' " + "\r\n"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesStationTypeLabelMap != null)
                {
                    string S_MSG = v_SC_mesStationTypeLabelMap.ToJson() + "\r\n"
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStationTypeLabelMap", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesStationTypeLabelMap v_SC_mesStationTypeLabelMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "";
            v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "";
            v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "";
            v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "";
            v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "";
            v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "";
            try
            {
                string S_Sql = "select * from mesStationTypeLabelMap where StationTypeID='" + v_SC_mesStationTypeLabelMap.StationTypeID + "'" + "\r\n" +
                    @"and(LabelID is NULL or LabelID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LabelID + @"', ',')))
                      and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartFamilyID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.ProductionOrderID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LineID + @"', ',')))

                      and ID<>'" + v_SC_mesStationTypeLabelMap.ID.ToString() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeLabelMap>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "NULL";
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "NULL";

                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.StationTypeID;
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LabelID;
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartID;
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartFamilyID;
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.ProductionOrderID;
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LineID;

                SC_mesStationTypeLabelMap v_mesStationTypeLabelMap_before = new SC_mesStationTypeLabelMap();
                S_Sql = "select * from mesStationTypeLabelMap where Id='" + v_SC_mesStationTypeLabelMap.ID + "'";
                v_mesStationTypeLabelMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeLabelMap>(S_Sql);

                S_Sql = "UPDATE mesStationTypeLabelMap SET" + "\r\n" +
                            "StationTypeID=" + v_SC_mesStationTypeLabelMap.StationTypeID + "," + "\r\n" +
                            "LabelID=" + v_SC_mesStationTypeLabelMap.LabelID + "," + "\r\n" +
                            "PartID=" + v_SC_mesStationTypeLabelMap.PartID + "," + "\r\n" +
                            "PartFamilyID=" + v_SC_mesStationTypeLabelMap.PartFamilyID + "," + "\r\n" +
                            "LineID =" + v_SC_mesStationTypeLabelMap.LineID + "," + "\r\n" +                            
                            "ProductionOrderID =" + v_SC_mesStationTypeLabelMap.ProductionOrderID + "\r\n" +
                        "WHERE ID=" + v_SC_mesStationTypeLabelMap.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStationTypeLabelMap_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesStationTypeLabelMap_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesStationTypeLabelMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStationTypeLabelMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesStationTypeLabelMap v_SC_mesStationTypeLabelMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "";
            v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "";
            v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "";
            v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "";
            v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "";
            v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "";

            try
            {
                string S_Sql = "select * from mesStationTypeLabelMap where StationTypeID='" + v_SC_mesStationTypeLabelMap.StationTypeID + "'" + "\r\n" +
                    @"and(LabelID is NULL or LabelID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LabelID + @"', ',')))
                      and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.PartFamilyID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.ProductionOrderID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesStationTypeLabelMap.LineID + @"', ',')))
                    ";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeLabelMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesStationTypeLabelMap v_mesStationTypeLabelMap_before = new SC_mesStationTypeLabelMap();
                S_Sql = "select * from mesStationTypeLabelMap where Id='" + v_SC_mesStationTypeLabelMap.ID + "'";
                v_mesStationTypeLabelMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeLabelMap>(S_Sql);


                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID ?? "NULL";
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID ?? "NULL";
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID ?? "NULL";
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID ?? "NULL";

                v_SC_mesStationTypeLabelMap.StationTypeID = v_SC_mesStationTypeLabelMap.StationTypeID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.StationTypeID;
                v_SC_mesStationTypeLabelMap.LabelID = v_SC_mesStationTypeLabelMap.LabelID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LabelID;
                v_SC_mesStationTypeLabelMap.PartID = v_SC_mesStationTypeLabelMap.PartID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartID;
                v_SC_mesStationTypeLabelMap.PartFamilyID = v_SC_mesStationTypeLabelMap.PartFamilyID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.PartFamilyID;
                v_SC_mesStationTypeLabelMap.ProductionOrderID = v_SC_mesStationTypeLabelMap.ProductionOrderID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.ProductionOrderID;
                v_SC_mesStationTypeLabelMap.LineID = v_SC_mesStationTypeLabelMap.LineID == "" ? "NULL" : v_SC_mesStationTypeLabelMap.LineID;


                S_Sql = "INSERT INTO mesStationTypeLabelMap(StationTypeID, LabelID, PartID, PartFamilyID, ProductionOrderID, LineID) VALUES(" +
                        v_SC_mesStationTypeLabelMap.StationTypeID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.LabelID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.PartID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.PartFamilyID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.ProductionOrderID + "\r\n" +
                     "," + v_SC_mesStationTypeLabelMap.LineID + "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStationTypeLabelMap_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesStationTypeLabelMap_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesStationTypeLabelMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesStationTypeLabelMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesStationTypeLabelMapDto>> FindWithPagerMyAsync(SC_mesStationTypeLabelMapSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";

            search.StationTypeID = search.StationTypeID ?? "";
            search.StationType = search.StationType ?? "";
            search.LabelID = search.LabelID ?? "";
            search.Label = search.Label ?? "";

            search.PartID = search.PartID ?? "";
            search.Part = search.Part ?? "";
            search.PartFamilyID = search.PartFamilyID ?? "";
            search.PartFamily = search.PartFamily ?? "";
            search.ProductionOrderID = search.ProductionOrderID ?? "";
            search.ProductionOrder = search.ProductionOrder ?? "";
            search.LineID = search.LineID ?? "";
            search.Line = search.Line ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesStationTypeLabelMapDto> List_Result = new List<SC_mesStationTypeLabelMapDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesStationTypeLabelMap A 
		            left join mesStationType B on A.StationTypeID=B.ID
		            left join mesLabel C on A.LabelID=C.ID
		            left join mesPart D on A.PartID=D.ID
                    left join luPartFamily E on A.PartFamilyID=E.ID
                    left join mesProductionOrder F on A.ProductionOrderID=F.ID
                    left join mesLine G on A.LineID=G.ID 
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))

                   and ('" + search.StationTypeID + @"'='' or A.StationTypeID   IN (SELECT value FROM dbo.F_Split('" + search.StationTypeID + @"',',')))
                   and ('" + search.LabelID + @"'='' or A.LabelID    IN (SELECT value FROM dbo.F_Split('" + search.LabelID + @"',',')))
                   and ('" + search.PartID + @"'='' or A.PartID   IN (SELECT value FROM dbo.F_Split('" + search.PartID + @"',',')))
                   and ('" + search.PartFamilyID + @"'='' or A.PartFamilyID   IN (SELECT value FROM dbo.F_Split('" + search.PartFamilyID + @"',',')))
                   and ('" + search.ProductionOrderID + @"'='' or A.ProductionOrderID   IN (SELECT value FROM dbo.F_Split('" + search.ProductionOrderID + @"',',')))
                   and ('" + search.LineID + @"'='' or A.LineID   IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                  
                   and ( '" + search.LikeQuery + @"'='' or B.Description Like '%" + search.LikeQuery + @"%'                                                   
                            or C.Name Like '%" + search.LikeQuery + @"%'
                            or D.PartNumber Like '%" + search.LikeQuery + @"%' 
                            or E.Name Like '%" + search.LikeQuery + @"%'
                            or F.ProductionOrderNumber  Like '%" + search.LikeQuery + @"%'                           
                            or G.Description Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
		            select A.ID, A.StationTypeID,B.Description StationType,
		                    A.LabelID,C.Name  Label,
		                    A.PartID,D.PartNumber Part,
		                    A.PartFamilyID,E.Name PartFamily,
		                    A.ProductionOrderID,F.ProductionOrderNumber ProductionOrder,
		                    A.LineID,G.Description Line 
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
		            left join mesStationType B on A.StationTypeID=B.ID
		            left join mesLabel C on A.LabelID=C.ID
		            left join mesPart D on A.PartID=D.ID
                    left join luPartFamily E on A.PartFamilyID=E.ID
                    left join mesProductionOrder F on A.ProductionOrderID=F.ID
                    left join mesLine G on A.LineID=G.ID 
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesStationTypeLabelMapDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesStationTypeLabelMapDto> list = v_query.ToList();
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




