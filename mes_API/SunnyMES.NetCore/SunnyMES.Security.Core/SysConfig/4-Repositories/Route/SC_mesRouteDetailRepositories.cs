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
    public class SC_mesRouteDetailRepository : BaseRepositoryReport<string>, ISC_mesRouteDetailRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesRouteDetailRepository()
        {
        }
        public SC_mesRouteDetailRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteDetail.RouteID = v_SC_mesRouteDetail.RouteID ?? "NULL";
            v_SC_mesRouteDetail.StationTypeID = v_SC_mesRouteDetail.StationTypeID ?? "NULL";
            v_SC_mesRouteDetail.Sequence = v_SC_mesRouteDetail.Sequence ?? "NULL";
            v_SC_mesRouteDetail.Description = v_SC_mesRouteDetail.Description ?? "";
            v_SC_mesRouteDetail.UnitStateID = v_SC_mesRouteDetail.UnitStateID ?? "NULL";

            try
            {
                string S_Sql = "select * from mesRouteDetail where RouteID='" + v_SC_mesRouteDetail.RouteID + "'" +
                                                            " and StationTypeID='" + v_SC_mesRouteDetail.StationTypeID + "'" +
                                                            " and UnitStateID='" + v_SC_mesRouteDetail.UnitStateID + "'" +
                                                            " and Sequence='" + v_SC_mesRouteDetail.SequenceMod + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteDetailDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesRouteDetail";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                
                S_Sql = "INSERT INTO mesRouteDetail(ID,RouteID,StationTypeID,UnitStateID,Sequence,Description) VALUES(" +
                     "'" + v_MaxID.LastID.ToString()+ "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.RouteID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.UnitStateID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.SequenceMod + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.Description + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesRouteDetail.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesRouteDetail", S_MSG);
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
                SC_mesRouteDetail v_SC_mesRouteDetail = new SC_mesRouteDetail();
                string S_Sql = "select * from mesRouteDetail where Id='" + Id + "'";
                v_SC_mesRouteDetail = await DapperConnRead2.QueryFirstAsync<SC_mesRouteDetail>(S_Sql);

                S_Sql = "delete mesRouteDetail where Id='" + Id + "' " + "\r\n"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesRouteDetail != null)
                {
                    string S_MSG = v_SC_mesRouteDetail.ToJson() + "\r\n"
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesRouteDetail", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteDetail.RouteID = v_SC_mesRouteDetail.RouteID ?? "NULL";
            v_SC_mesRouteDetail.StationTypeID = v_SC_mesRouteDetail.StationTypeID ?? "NULL";
            v_SC_mesRouteDetail.Sequence = v_SC_mesRouteDetail.Sequence ?? "NULL";
            v_SC_mesRouteDetail.Description = v_SC_mesRouteDetail.Description ?? "";
            v_SC_mesRouteDetail.UnitStateID = v_SC_mesRouteDetail.UnitStateID ?? "NULL";

            try
            {
                string S_Sql = "select * from mesRouteDetail where  RouteID='" + v_SC_mesRouteDetail.RouteID + "'" +
                                                                   " and StationTypeID='" + v_SC_mesRouteDetail.StationTypeID + "'" +
                                                                   " and Sequence='" + v_SC_mesRouteDetail.SequenceMod + "'" +
                                                                   " and ID<>'" + v_SC_mesRouteDetail.ID.ToString() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteDetail>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRouteDetail v_mesRouteDetail_before = new SC_mesRouteDetail();
                S_Sql = "select * from mesRouteDetail where Id='" + v_SC_mesRouteDetail.ID + "'";
                v_mesRouteDetail_before = await DapperConnRead2.QueryFirstAsync<SC_mesRouteDetail>(S_Sql);

                S_Sql = "UPDATE mesRouteDetail SET" + "\r\n" +
                            "RouteID='" + v_SC_mesRouteDetail.RouteID + "'," + "\r\n" +
                            "StationTypeID='" + v_SC_mesRouteDetail.StationTypeID + "'," + "\r\n" +
                            "UnitStateID='" + v_SC_mesRouteDetail.UnitStateID + "'," + "\r\n" +
                            "Sequence ='" + v_SC_mesRouteDetail.SequenceMod + "'," + "\r\n" +
                            "Description ='" + v_SC_mesRouteDetail.Description +"'"+ "\r\n" +
                        "WHERE ID=" + v_SC_mesRouteDetail.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRouteDetail_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesRouteDetail_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesRouteDetail.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesRouteDetail", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRouteDetail.RouteID = v_SC_mesRouteDetail.RouteID ?? "NULL";
            v_SC_mesRouteDetail.StationTypeID = v_SC_mesRouteDetail.StationTypeID ?? "NULL";
            v_SC_mesRouteDetail.Sequence = v_SC_mesRouteDetail.Sequence ?? "NULL";
            v_SC_mesRouteDetail.Description = v_SC_mesRouteDetail.Description ?? "";
            v_SC_mesRouteDetail.UnitStateID = v_SC_mesRouteDetail.UnitStateID ?? "NULL";

            try
            {
                string S_Sql = "select * from mesRouteDetail where RouteID='" + v_SC_mesRouteDetail.RouteID + "'" +
                                                            " and StationTypeID='" + v_SC_mesRouteDetail.StationTypeID + "'" +
                                                            " and UnitStateID='" + v_SC_mesRouteDetail.UnitStateID + "'" +
                                                            " and Sequence='" + v_SC_mesRouteDetail.SequenceMod + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteDetailDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRouteDetail v_mesRouteDetail_before = new SC_mesRouteDetail();
                S_Sql = "select * from mesRouteDetail where Id='" + v_SC_mesRouteDetail.ID + "'";
                v_mesRouteDetail_before = await DapperConnRead2.QueryFirstAsync<SC_mesRouteDetail>(S_Sql);

                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesRouteDetail";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);

                S_Sql = "INSERT INTO mesRouteDetail(ID,RouteID,StationTypeID,UnitStateID,Sequence,Description) VALUES(" +
                     "'" + v_MaxID.LastID.ToString() + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.RouteID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.UnitStateID + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.SequenceMod + "'" + "\r\n" +
                     ",'" + v_SC_mesRouteDetail.Description + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRouteDetail_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesRouteDetail_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesRouteDetail.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesRouteDetail", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }


        public async Task<SC_mesRouteDetailList> List_mesRouteDetail(string RouteID) 
        {
            SC_mesRouteDetailList v_SC_mesRouteDetailList = new SC_mesRouteDetailList();
            v_SC_mesRouteDetailList.MSG = "OK";
            try
            {
                string S_Sql =
                    @"
                    select * from 
                    (
	                    select A.*,ROW_NUMBER() over(order by A.SequenceMod) as Sequence from 
	                    ( 
		                    select A.ID, A.RouteID, A.StationTypeID, A.Sequence SequenceMod , A.Description,
				                    A.UnitStateID,B.Name RouteName,C.Description StationType, 
				                    D.Description UnitState
			                    from mesRouteDetail A 
				                    join mesRoute B on A.RouteID =B.ID
				                    join mesStationType C on A.StationTypeID=C.ID
				                    join mesUnitState  D on A.UnitStateID=D.ID 
			                    where A.RouteID="+ RouteID + @"
	                    ) A 
                    )B                    
                    Order By B.RouteName,B.Sequence
                    ";
                var v_Detail = await DapperConnRead2.QueryAsync<SC_mesRouteDetailDto>(S_Sql);
                v_SC_mesRouteDetailList.List_mesRouteDetail = v_Detail.ToList();
            }
            catch (Exception ex)
            {
                v_SC_mesRouteDetailList.MSG = ex.ToString();
            }

            return v_SC_mesRouteDetailList;
        }
    }
}



