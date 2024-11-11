
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
    public class SC_mesSNFormatMapRepository : BaseRepositoryReport<string>, ISC_mesSNFormatMapRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesSNFormatMapRepository()
        {
        }
        public SC_mesSNFormatMapRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "";
            v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "";
            v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "";
            v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "";
            v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "";

 
            try
            {
                string S_Sql = "select * from mesSNFormatMap where SNFormatID='" + v_SC_mesSNFormatMap.SNFormatID + "'" + "\r\n" +
                    @"and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartFamilyID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.LineID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.ProductionOrderID + @"', ',')))
                      and(StationTypeID is NULL or StationTypeID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.StationTypeID + @"', ',')))
                    ";


                    //"' and  PartID='" + v_SC_mesSNFormatMap.PartID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormatMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "NULL";
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "NULL";
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "NULL";
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "NULL";
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "NULL";

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID == "" ? "NULL" : v_SC_mesSNFormatMap.PartID;
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID == "" ? "NULL" : v_SC_mesSNFormatMap.PartFamilyID;
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID == "" ? "NULL" : v_SC_mesSNFormatMap.LineID;
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID == "" ? "NULL" : v_SC_mesSNFormatMap.ProductionOrderID;
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID == "" ? "NULL" : v_SC_mesSNFormatMap.StationTypeID;


                S_Sql = "INSERT INTO mesSNFormatMap(SNFormatID,PartID,PartFamilyID,LineID,ProductionOrderID,StationTypeID) VALUES(" +
                        v_SC_mesSNFormatMap.SNFormatID  + "\r\n" +
                     "," + v_SC_mesSNFormatMap.PartID  + "\r\n" +
                     "," + v_SC_mesSNFormatMap.PartFamilyID  + "\r\n" +
                     "," + v_SC_mesSNFormatMap.LineID  + "\r\n" +
                     "," + v_SC_mesSNFormatMap.ProductionOrderID  + "\r\n" +
                     "," + v_SC_mesSNFormatMap.StationTypeID  + "\r\n" +
                    ")";


                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesSNFormatMap.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesSNFormatMap", S_MSG);
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
                SC_mesSNFormatMap v_SC_mesSNFormatMap = new SC_mesSNFormatMap();
                string S_Sql = "select * from mesSNFormatMap where Id='" + Id + "'";
                v_SC_mesSNFormatMap = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormatMap>(S_Sql);

                S_Sql = "delete mesSNFormatMap where Id='" + Id + "' " + "\r\n" 
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesSNFormatMap != null)
                {
                    string S_MSG = v_SC_mesSNFormatMap.ToJson() + "\r\n" 
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesSNFormatMap", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "";
            v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "";
            v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "";
            v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "";
            v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "";
            try
            {
                string S_Sql = "select * from mesSNFormatMap where SNFormatID='" + v_SC_mesSNFormatMap.SNFormatID+"'" + "\r\n" +
                    //"' and  PartID='" +v_SC_mesSNFormatMap.PartID +
                    @"and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartFamilyID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.LineID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.ProductionOrderID + @"', ',')))
                      and(StationTypeID is NULL or StationTypeID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.StationTypeID + @"', ',')))

                      and ID<>'" + v_SC_mesSNFormatMap.ID.ToString() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormatMap>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result =  " Data already exists.";
                }

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "NULL";
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "NULL";
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "NULL";
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "NULL";
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "NULL";

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID == "" ? "NULL" : v_SC_mesSNFormatMap.PartID;
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID == "" ? "NULL" : v_SC_mesSNFormatMap.PartFamilyID;
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID == "" ? "NULL" : v_SC_mesSNFormatMap.LineID;
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID == "" ? "NULL" : v_SC_mesSNFormatMap.ProductionOrderID;
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID == "" ? "NULL" : v_SC_mesSNFormatMap.StationTypeID;


                SC_mesSNFormatMap v_mesSNFormatMap_before = new SC_mesSNFormatMap();
                S_Sql = "select * from mesSNFormatMap where Id='" + v_SC_mesSNFormatMap.ID + "'";
                v_mesSNFormatMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormatMap>(S_Sql);

                S_Sql = "UPDATE mesSNFormatMap SET" + "\r\n" +
                            "SNFormatID=" + v_SC_mesSNFormatMap.SNFormatID + "," + "\r\n" +
                            "PartID=" + v_SC_mesSNFormatMap.PartID + "," + "\r\n" +
                            "PartFamilyID=" + v_SC_mesSNFormatMap.PartFamilyID + "," + "\r\n" +
                            "LineID =" + v_SC_mesSNFormatMap.LineID + "," + "\r\n" + 
                            "StationTypeID =" + v_SC_mesSNFormatMap.StationTypeID + "," + "\r\n" + 
                            "ProductionOrderID =" + v_SC_mesSNFormatMap.ProductionOrderID + "\r\n" +
                        "WHERE ID=" + v_SC_mesSNFormatMap.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesSNFormatMap_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesSNFormatMap_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesSNFormatMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesSNFormatMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "";
            v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "";
            v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "";
            v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "";
            v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "";

            try
            {
                string S_Sql = "select * from mesSNFormatMap where SNFormatID='" + v_SC_mesSNFormatMap.SNFormatID +"'"+"\r\n"+
                    @"and(PartID is NULL or PartID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartID + @"', ',')))
                      and(PartFamilyID is NULL or PartFamilyID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.PartFamilyID + @"', ',')))
                      and(LineID is NULL or LineID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.LineID + @"', ',')))
                      and(ProductionOrderID is NULL or ProductionOrderID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.ProductionOrderID + @"', ',')))
                      and(StationTypeID is NULL or StationTypeID  IN(SELECT value FROM dbo.F_Split('" + v_SC_mesSNFormatMap.StationTypeID + @"', ',')))
                    ";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormatMapDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesSNFormatMap v_mesSNFormatMap_before = new SC_mesSNFormatMap();
                S_Sql = "select * from mesSNFormatMap where Id='" + v_SC_mesSNFormatMap.ID + "'";
                v_mesSNFormatMap_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormatMap>(S_Sql);

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID ?? "NULL";
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID ?? "NULL";
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID ?? "NULL";
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID ?? "NULL";
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID ?? "NULL";

                v_SC_mesSNFormatMap.PartID = v_SC_mesSNFormatMap.PartID == "" ? "NULL" : v_SC_mesSNFormatMap.PartID;
                v_SC_mesSNFormatMap.PartFamilyID = v_SC_mesSNFormatMap.PartFamilyID == "" ? "NULL" : v_SC_mesSNFormatMap.PartFamilyID;
                v_SC_mesSNFormatMap.LineID = v_SC_mesSNFormatMap.LineID == "" ? "NULL" : v_SC_mesSNFormatMap.LineID;
                v_SC_mesSNFormatMap.ProductionOrderID = v_SC_mesSNFormatMap.ProductionOrderID == "" ? "NULL" : v_SC_mesSNFormatMap.ProductionOrderID;
                v_SC_mesSNFormatMap.StationTypeID = v_SC_mesSNFormatMap.StationTypeID == "" ? "NULL" : v_SC_mesSNFormatMap.StationTypeID;


                S_Sql = "INSERT INTO mesSNFormatMap(SNFormatID,PartID,PartFamilyID,LineID,ProductionOrderID,StationTypeID) VALUES(" +
                        v_SC_mesSNFormatMap.SNFormatID + "\r\n" +
                     "," + v_SC_mesSNFormatMap.PartID + "\r\n" +
                     "," + v_SC_mesSNFormatMap.PartFamilyID + "\r\n" +
                     "," + v_SC_mesSNFormatMap.LineID + "\r\n" +
                     "," + v_SC_mesSNFormatMap.ProductionOrderID + "\r\n" +
                     "," + v_SC_mesSNFormatMap.StationTypeID + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesSNFormatMap_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesSNFormatMap_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesSNFormatMap.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesSNFormatMap", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesSNFormatMapDto>> FindWithPagerMyAsync(SC_mesSNFormatMapSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";

            search.SNFormatID = search.SNFormatID ?? "";
            search.SNFormat = search.SNFormat ?? "";
            search.PartID = search.PartID ?? "";
            search.Part = search.Part ?? "";
            search.PartFamilyID = search.PartFamilyID ?? "";
            search.PartFamily = search.PartFamily ?? "";
            search.LineID = search.LineID ?? "";
            search.Line = search.Line ?? "";
            search.ProductionOrderID = search.ProductionOrderID ?? "";
            search.ProductionOrder = search.ProductionOrder ?? "";
            search.StationTypeID = search.StationTypeID ?? "";
            search.StationType = search.StationType ?? "";

            search.LikeQuery = search.LikeQuery ?? "";


            List<SC_mesSNFormatMapDto> List_Result = new List<SC_mesSNFormatMapDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesSNFormatMap A 
                    LEFT JOIN mesPart B ON A.PartID=B.ID
                    LEFT JOIN luPartFamily C ON A.PartFamilyID=C.ID
                    LEFT JOIN mesLine D ON A.LineID=D.ID
                    LEFT JOIN mesProductionOrder E ON A.ProductionOrderID=E.ID
                    LEFT JOIN mesStationType F ON A.StationTypeID=F.ID    
                    LEFT JOIN mesSNFormat G ON A.SNFormatID=G.ID
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))

                   and ('" + search.SNFormatID + @"'='' or A.SNFormatID   IN (SELECT value FROM dbo.F_Split('" + search.SNFormatID + @"',',')))
                   and ('" + search.PartID + @"'='' or A.PartID   IN (SELECT value FROM dbo.F_Split('" + search.PartID + @"',',')))
                   and ('" + search.PartFamilyID + @"'='' or A.PartFamilyID   IN (SELECT value FROM dbo.F_Split('" + search.PartFamilyID + @"',',')))
                   and ('" + search.LineID + @"'='' or A.LineID   IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                   and ('" + search.ProductionOrderID + @"'='' or A.ProductionOrderID   IN (SELECT value FROM dbo.F_Split('" + search.ProductionOrderID + @"',',')))
                   and ('" + search.StationTypeID + @"'='' or A.StationTypeID   IN (SELECT value FROM dbo.F_Split('" + search.StationTypeID + @"',',')))
                  
                   and ( '" + search.LikeQuery + @"'='' or G.Name Like '%" + search.LikeQuery + @"%'
                            or B.PartNumber Like '%" + search.LikeQuery + @"%'                            
                            or C.Name Like '%" + search.LikeQuery + @"%'
                            or D.Description  Like '%" + search.LikeQuery + @"%'

                            or E.ProductionOrderNumber Like '%" + search.LikeQuery + @"%'
                            or F.Description Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.ID,A.SNFormatID,G.Name SNFormat,A.PartID, B.PartNumber Part,A.PartFamilyID,C.Name  PartFamily,
                          A.LineID,D.[Description] Line,A.ProductionOrderID ,E.ProductionOrderNumber ProductionOrder,
                          A.StationTypeID ,F.[Description] StationType
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
                    LEFT JOIN mesPart B ON A.PartID=B.ID
                    LEFT JOIN luPartFamily C ON A.PartFamilyID=C.ID
                    LEFT JOIN mesLine D ON A.LineID=D.ID
                    LEFT JOIN mesProductionOrder E ON A.ProductionOrderID=E.ID
                    LEFT JOIN mesStationType F ON A.StationTypeID=F.ID
                    LEFT JOIN mesSNFormat G ON A.SNFormatID=G.ID
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesSNFormatMapDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesSNFormatMapDto> list = v_query.ToList();
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



