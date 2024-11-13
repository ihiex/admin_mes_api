using API_MSG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
using NPOI.POIFS.Properties;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using Org.BouncyCastle.Asn1.X509;
using Quartz.Impl.Triggers;
using SQLitePCL;
using SqlSugar;
using StackExchange.Profiling.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
using SunnyMES.Commons;

namespace SunnyMES.Security.Repositories
{
    public class SC_mesRouteRepository : BaseRepositoryReport<string>, ISC_mesRouteRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesRouteRepository()
        {
        }
        public SC_mesRouteRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRoute.Name = v_SC_mesRoute.Name ?? "";
            v_SC_mesRoute.Description = v_SC_mesRoute.Description ?? "";
            v_SC_mesRoute.RouteType = v_SC_mesRoute.RouteType ?? "";
            v_SC_mesRoute.XMLRouteV2 = v_SC_mesRoute.XMLRouteV2 ?? "";


            try
            {
                string S_Sql = "select * from mesRoute where Name='" + v_SC_mesRoute.Name +"'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                S_Sql = "INSERT INTO mesRoute(Name,Description,RouteType) VALUES(" +
                     "'" + v_SC_mesRoute.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesRoute.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesRoute.RouteType + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesRoute.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesRoute", S_MSG);
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
                SC_mesRoute v_SC_mesRoute = new SC_mesRoute();
                string S_Sql = "select * from mesRoute where Id='" + Id + "'";
                v_SC_mesRoute = await DapperConnRead2.QueryFirstAsync<SC_mesRoute>(S_Sql);

                S_Sql = @"INSERT INTO mesRouteHistory (RouteID,XMLRoute,EmployeeID,XMLRouteV2) VALUES
                        (
                            '" + Id + @"'
                            ,'" + v_SC_mesRoute.XMLRoute + @"'
                            ,'" + List_Login.EmployeeID + @"'
                            ,'" + v_SC_mesRoute.XMLRouteV2 + @"'
                       )";

                S_Sql += "delete mesRoute where Id='" + Id + "' " + "\r\n"+
                        "delete mesRouteDetail where RouteID='" + Id + "' " + "\r\n"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesRoute != null)
                {
                    string S_MSG = v_SC_mesRoute.ToJson() + "\r\n"
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesRoute", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRoute.Name = v_SC_mesRoute.Name ?? "";
            v_SC_mesRoute.Description = v_SC_mesRoute.Description ?? "";
            v_SC_mesRoute.RouteType = v_SC_mesRoute.RouteType ?? "";
            v_SC_mesRoute.XMLRouteV2 = v_SC_mesRoute.XMLRouteV2 ?? "";

            try
            {
                string S_Sql = "select * from mesRoute where Name='" + v_SC_mesRoute.Name + "'" +
                    " and ID<>'" + v_SC_mesRoute.ID.ToString() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRoute>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRoute v_mesRoute_before = new SC_mesRoute();
                S_Sql = "select * from mesRoute where Id='" + v_SC_mesRoute.ID + "'";
                v_mesRoute_before = await DapperConnRead2.QueryFirstAsync<SC_mesRoute>(S_Sql);

                S_Sql = "UPDATE mesRoute SET" + "\r\n" +
                            "Name='" + v_SC_mesRoute.Name + "'," + "\r\n" +
                            "Description='" + v_SC_mesRoute.Description + "'," + "\r\n" +                            
                            "RouteType ='" + v_SC_mesRoute.RouteType + "'" + "\r\n" +
                        "WHERE ID=" + v_SC_mesRoute.ID;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRoute_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesRoute_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesRoute.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesRoute", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesRoute.Name = v_SC_mesRoute.Name ?? "";
            v_SC_mesRoute.Description = v_SC_mesRoute.Description ?? "";
            v_SC_mesRoute.RouteType = v_SC_mesRoute.RouteType ?? "";
            v_SC_mesRoute.XMLRouteV2 = v_SC_mesRoute.XMLRouteV2 ?? "";

            try
            {
                string S_Sql = "select * from mesRoute where Name='" + v_SC_mesRoute.Name + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesRouteDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesRoute v_mesRoute_before = new SC_mesRoute();
                S_Sql = "select * from mesRoute where Id='" + v_SC_mesRoute.ID + "'";
                v_mesRoute_before = await DapperConnRead2.QueryFirstAsync<SC_mesRoute>(S_Sql);



                S_Sql = "INSERT INTO mesRoute(Name,Description,RouteType,XMLRoute,XMLRouteV2) VALUES(" +
                     "'" + v_SC_mesRoute.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesRoute.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesRoute.RouteType + "'" + "\r\n" +
                     ",'" + v_mesRoute_before.XMLRoute + "'" + "\r\n" +
                     ",'" + v_mesRoute_before.XMLRouteV2 + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesRoute_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesRoute_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesRoute.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesRoute", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Undo(string RouteId, string HistoryId, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            try
            {
                string S_Sql = "update mesRoute set XMLRouteV2=(SELECT XMLRouteV2 FROM mesRouteHistory WHERE ID='"+ HistoryId 
                    + "') WHERE ID='" + RouteId+"'";
                await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }

        public async Task<List<SC_mesRouteHistory>> GetmesRouteHistory(string RouteId) 
        {
            string S_Sql = 
                @"select A.ID, A.RouteID,B.Lastname+'.'+B.Firstname Employee,A.CreateDate 
                    from mesRouteHistory A
                    left JOIN mesEmployee B ON A.EmployeeID=B.ID
                 where A.RouteId='" + RouteId + "'  ORDER BY A.CreateDate DESC";
            var v_Query = await DapperConnRead2.QueryAsync<SC_mesRouteHistory>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<SC_mesRouteDto>> FindWithPagerMyAsync(SC_mesRouteSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";

            search.Name = search.Name ?? "";
            search.Description = search.Description ?? "";
            search.RouteType = search.RouteType ?? "";
            search.XMLRouteV2 = search.XMLRouteV2 ?? "";
            search.RouteTypeValue = search.RouteTypeValue ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesRouteDto> List_Result = new List<SC_mesRouteDto>();
            try
            {
                string S_Page = "SELECT  A.Id,A.NAME,A.[Description],A.RouteType " + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesRoute A 
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))

                   and ('" + search.Name + @"'='' or A.Name Like '%" + search.Name + @"%' )
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )
                   and ('" + search.RouteType + @"'='' or A.RouteType   IN (SELECT value FROM dbo.F_Split('" + search.RouteType + @"',',')))
                  
                   and ( '" + search.LikeQuery + @"'='' or A.Name Like '%" + search.LikeQuery + @"%'                                          
                            or A.Description Like '%" + search.LikeQuery + @"%'                            

                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.Id,A.NAME,A.[Description],A.RouteType ,
                        (CASE WHEN RouteType=0 THEN 'TableRoute'
                            WHEN RouteType=1 THEN 'DiagramRoute'
                        END) RouteTypeValue 
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

                var v_query = await DapperConnRead2.QueryAsync<SC_mesRouteDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesRouteDto> list = v_query.ToList();
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

        public async Task<string> ShowRouteHistory(string Id, IDbTransaction trans = null)
        {
            string S_Result = "";
            try
            {
                string S_Sql = "";
                S_Sql = "select * from mesRouteHistory where Id='" + Id + "'";
                var v_query = await DapperConnRead2.QueryAsync<SC_mesRouteDto>(S_Sql, null, null, I_DBTimeout, null);
                List<SC_mesRouteDto> list = v_query.ToList();

                string S_XMLRouteV2 = list[0].XMLRouteV2;
                S_Result = S_XMLRouteV2;
                //S_Result = PublicF.DecryptPassword(S_XMLRouteV2, "");

            }
            catch (Exception ex)
            {
                S_Result = "";
            }
            return S_Result;
        }

        private class Diagram_States
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string StationType { get; set; }
            public string StationTypeID { get; set; }
            public string PropsTxt { get; set; }
            public string PropsForm { get; set; }
            public string Attr { get; set; }
            public string PropsDesc { get; set; }

            public string OutputStateID { get; set; }
        }
        private class Diagram_Path
        {
            public string Name { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string DspTxt { get; set; }

            public string PropsTxt { get; set; }
            public string UnitStateID { get; set; }

        }

        private string GetLinkText(string S_OutputState) 
        {
            string S_LinkText = S_OutputState;
            if (S_OutputState == "1")
            {
                S_LinkText = "PASS";
            }
            else if (S_OutputState == "2")
            {
                S_LinkText = "Fail";
            }
            else if (S_OutputState == "3")
            {
                S_LinkText = "SCRAP";
            }
            else if (S_OutputState == "4")
            {
                S_LinkText = "ONHOLD";
            }
            return S_LinkText;
        }


        private List<Diagram_States> List_Diagram_States(string S_Data)
        {
            List<Diagram_States> vList_List_Diagram_States = new List<Diagram_States>();

            JObject jsonObj1 = JObject.Parse(S_Data);
            foreach (var jarry1 in jsonObj1)
            {
                string S_Key1 = jarry1.Key;
                string S_Type1 = S_Key1.Substring(0, 4);
                string S_TaskState = "";

                if (S_Type1 == "rect")
                {
                    Diagram_States v_Diagram_States = new Diagram_States();
                    v_Diagram_States.Name = S_Key1;

                    JObject jsonObj2 = JObject.Parse(jarry1.Value.ToString());
                    foreach (var jarry2 in jsonObj2)
                    {
                        string S_Key2 = jarry2.Key;
                        string S_Value2 = jarry2.Value.ToString();

                        if (S_Key2 == "type")
                        {
                            S_TaskState = S_Value2;
                            v_Diagram_States.Type = S_Value2;
                        }
                        else if (S_Key2 == "text")
                        {
                            JObject jsonObj_Dsp = JObject.Parse(S_Value2);
                            foreach (var jarry_dsp in jsonObj_Dsp)
                            {
                                string S_Dsp_Key = jarry_dsp.Key;
                                string S_Dsp_Value = jarry_dsp.Value.ToString();

                                v_Diagram_States.StationType = S_Dsp_Value;
                            }
                        }
                        else if (S_Key2 == "props")
                        {
                            JObject jsonObj_Props = JObject.Parse(S_Value2);

                            foreach (var jarry_Props in jsonObj_Props)
                            {
                                string S_Props_Key = jarry_Props.Key;
                                string S_Props_Value = jarry_Props.Value.ToString();

                                JObject jsonObj_PropsValue = JObject.Parse(S_Props_Value);
                                string S_PropsValue_Key = "";
                                string S_PropsValue_Value = "";
                                foreach (var jarry_PropsValue in jsonObj_PropsValue)
                                {
                                    S_PropsValue_Key = jarry_PropsValue.Key;
                                    S_PropsValue_Value = jarry_PropsValue.Value.ToString();
                                }

                                if (S_TaskState == "task")
                                {
                                    if (S_Props_Key == "text")
                                    {
                                        v_Diagram_States.PropsTxt = S_PropsValue_Value;
                                    }
                                    else if (S_Props_Key == "assignee")
                                    {
                                        v_Diagram_States.StationTypeID = S_PropsValue_Value;
                                    }
                                    else if (S_Props_Key == "form")
                                    {
                                        v_Diagram_States.PropsForm = S_PropsValue_Value;
                                    }
                                    else if (S_Props_Key == "desc")
                                    {
                                        v_Diagram_States.PropsDesc = S_PropsValue_Value;
                                    }
                                }
                            }
                        }
                        else if (S_Key2 == "attr")
                        {
                            JObject jsonObj_Props = JObject.Parse(S_Value2);

                            string S_xy = "";
                            foreach (var jarry_Props in jsonObj_Props)
                            {
                                string S_Props_Key = jarry_Props.Key;
                                string S_Props_Value = jarry_Props.Value.ToString();

                                if (S_Props_Key == "x" || S_Props_Key == "y") 
                                {
                                    if (S_xy == "") { S_xy = S_Props_Value; }
                                    else { S_xy +=" "+ S_Props_Value; }
                                }
                            }
                            v_Diagram_States.Attr = S_xy;
                        }
                    }
                    vList_List_Diagram_States.Add(v_Diagram_States);
                }
            }
            return vList_List_Diagram_States;
        }
        private List<Diagram_Path> List_Diagram_Path(string S_Data)
        {
            List<Diagram_Path> vList_List_Diagram_Path = new List<Diagram_Path>();

            JObject jsonObj1 = JObject.Parse(S_Data);
            foreach (var jarry1 in jsonObj1)
            {
                string S_Key1 = jarry1.Key;
                string S_Type1 = S_Key1.Substring(0, 4);

                if (S_Type1 == "path")
                {
                    Diagram_Path v_Diagram_Path = new Diagram_Path();
                    v_Diagram_Path.Name = S_Key1;

                    JObject jsonObj2 = JObject.Parse(jarry1.Value.ToString());
                    foreach (var jarry2 in jsonObj2)
                    {
                        string S_Key2 = jarry2.Key;
                        string S_Value2 = jarry2.Value.ToString();

                        if (S_Key2 == "from")
                        {
                            v_Diagram_Path.From = S_Value2;
                        }
                        else if (S_Key2 == "to")
                        {
                            v_Diagram_Path.To = S_Value2;
                        }
                        else if (S_Key2 == "text")
                        {
                            JObject jsonObj_Dsp = JObject.Parse(S_Value2);
                            foreach (var jarry_dsp in jsonObj_Dsp)
                            {
                                string S_Dsp_Key = jarry_dsp.Key;
                                string S_Dsp_Value = jarry_dsp.Value.ToString();

                                if (S_Dsp_Key == "text")
                                {
                                    v_Diagram_Path.DspTxt = S_Dsp_Value;
                                }
                            }
                        }
                        else if (S_Key2 == "props")
                        {
                            JObject jsonObj_Props = JObject.Parse(S_Value2);

                            foreach (var jarry_Props in jsonObj_Props)
                            {
                                string S_Props_Key = jarry_Props.Key;
                                string S_Props_Value = jarry_Props.Value.ToString();

                                JObject jsonObj_ProsTxt = JObject.Parse(S_Props_Value);
                                if (S_Props_Key == "text")
                                {
                                    foreach (var jarry_ProsTxt in jsonObj_ProsTxt)
                                    {
                                        string S_ProdTxt_Key = jarry_ProsTxt.Key;
                                        string S_ProsTxt_Value = jarry_ProsTxt.Value.ToString();

                                        v_Diagram_Path.PropsTxt = S_ProsTxt_Value;
                                    }
                                }

                                if (S_Props_Key == "UnitStateID")
                                {
                                    foreach (var jarry_ProsTxt in jsonObj_ProsTxt)
                                    {
                                        string S_ProdTxt_Key = jarry_ProsTxt.Key;
                                        string S_ProsTxt_Value = jarry_ProsTxt.Value.ToString();

                                        v_Diagram_Path.UnitStateID = S_ProsTxt_Value;
                                    }
                                }
                            }
                        }
                    }
                    vList_List_Diagram_Path.Add(v_Diagram_Path);
                }
            }
            return vList_List_Diagram_Path;
        }

        public async Task<string> SetXMLToXMLV2(string Id, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            string S_Node = "";
            string S_Link = "";
            string S_LinkText = "";

            string S_Sql = "";
            try
            {
                S_Sql = "SELECT * FROM mesRoute WHERE ID='"+Id+"'";
                var query_Route = await DapperConnRead2.QueryAsync<SC_mesRoute>(S_Sql);
                List<SC_mesRoute> List_mesRoute= query_Route.ToList();

                string S_PWD_XML = List_mesRoute[0].XMLRoute.Trim();
                string S_Data = Base64.DecodeBase64(S_PWD_XML);


                JObject MyJO = (JObject)JsonConvert.DeserializeObject(S_Data.ToString());
                string S_states = MyJO["states"].ToString();
                string S_Paths = MyJO["paths"].ToString();

                List<Diagram_States> List_States = List_Diagram_States(S_states);
                List<Diagram_Path> List_Path = List_Diagram_Path(S_Paths);

                foreach (var item in List_States) 
                {
                    if (S_Node == "")
                    {
                        S_Node =
                        "{" + "\r\n" +
                        "\"" + "text" + "\"" + ":" + "\"" + item.PropsTxt + "\"" + "," + "\r\n" +
                        "\"" + "key" + "\"" + ":" + "\"" + item.Name + "\"" + "," + "\r\n" +
                        "\"" + "loc" + "\"" + ":" + "\"" + item.Attr + "\"" + "," + "\r\n" +
                        "\"" + "stationTypeID" + "\"" + ":" + "\"" + item.StationTypeID + "\"" + "\r\n" +
                        "}";
                    }
                    else
                    {
                        string S_NodePwd =
                        "{" + "\r\n" +
                        "\"" + "text" + "\"" + ":" + "\"" + item.PropsTxt + "\"" + "," + "\r\n" +
                        "\"" + "key" + "\"" + ":" + "\"" + item.Name + "\"" + "," + "\r\n" +
                        "\"" + "loc" + "\"" + ":" + "\"" + item.Attr + "\"" + "," + "\r\n" +
                        "\"" + "stationTypeID" + "\"" + ":" + "\"" + item.StationTypeID + "\"" + "\r\n" +
                        "}";
                        S_NodePwd = "," + "\r\n" + S_NodePwd;
                        S_Node += S_NodePwd;
                    }
                }


                foreach (var item in List_Path) 
                {
                    string S_OutputState = "1";
                    if (item.PropsTxt.Trim() != "")
                    {
                        S_OutputState = item.PropsTxt.Trim();
                    }

                    S_LinkText = GetLinkText(S_OutputState);

                    var query_From = (from c in List_States where c.Name == item.From select c).FirstOrDefault();
                    var query_To = (from c in List_States where c.Name == item.To select c).FirstOrDefault();

                    string S_StartP = query_From.Attr.Replace(" ", ",");
                    string S_EndP = query_To.Attr.Replace(" ", ",");

                    if (S_Link == "")
                    {
                        S_Link =
                        "{" + "\r\n" +
                        "\"" + "from" + "\"" + ":" + "\"" + item.From + "\"" + "," + "\r\n" +
                        "\"" + "to" + "\"" + ":" + "\"" + item.To + "\"" + "," + "\r\n" +
                        "\"" + "fromPort" + "\"" + ":" + "\"" + "B" + "\"" + "," + "\r\n" +
                        "\"" + "toPort" + "\"" + ":" + "\"" + "T" + "\"" + "," + "\r\n" +
                        "\"" + "points" + "\"" + ":" + "[" + S_StartP + "," + S_EndP + "]" + "," + "\r\n" +
                        "\"" + "unitStatusID" + "\"" + ":" + "\"" + S_OutputState + "\"" + "," + "\r\n" +
                        "\"" + "text" + "\"" + ":" + "\"" + S_LinkText + "\"" + "," + "\r\n" +
                        "\"" + "unitStateID" + "\"" + ":" + "\"" + item.UnitStateID + "\"" + "\r\n" +

                        "}";
                    }
                    else
                    {

                        string S_LinkPwd =
                        "{" + "\r\n" +
                        "\"" + "from" + "\"" + ":" + "\"" + item.From + "\"" + "," + "\r\n" +
                        "\"" + "to" + "\"" + ":" + "\"" + item.To + "\"" + "," + "\r\n" +
                        "\"" + "fromPort" + "\"" + ":" + "\"" + "B" + "\"" + "," + "\r\n" +
                        "\"" + "toPort" + "\"" + ":" + "\"" + "T" + "\"" + "," + "\r\n" +
                        "\"" + "points" + "\"" + ":" + "[" + S_StartP + "," + S_EndP + "]" + "," + "\r\n" +
                        "\"" + "unitStatusID" + "\"" + ":" + "\"" + S_OutputState + "\"" + "," + "\r\n" +
                        "\"" + "text" + "\"" + ":" + "\"" + S_LinkText + "\"" + "," + "\r\n" +
                        "\"" + "unitStateID" + "\"" + ":" + "\"" + item.UnitStateID + "\"" + "\r\n" +

                        "}";
                        S_LinkPwd = "," + "\r\n" + S_LinkPwd;
                        S_Link += S_LinkPwd;
                    }
                }

                string S_RouteXml2 =
                    "{" + "\r\n" +
                    "\"" + "class" + "\"" + ":" + "\"" + "GraphLinksModel" + "\"" + "," + "\r\n" +
                    "\"" + "copiesArrays" + "\"" + ":" + "\"" + "true" + "\"" + "," + "\r\n" +
                    "\"" + "copiesArrayObjects" + "\"" + ":" + "\"" + "true" + "\"" + "," + "\r\n" +
                    "\"" + "linkFromPortIdProperty" + "\"" + ":" + "\"" + "fromPort" + "\"" + "," + "\r\n" +
                    "\"" + "linkToPortIdProperty" + "\"" + ":" + "\"" + "toPort" + "\"" + "," + "\r\n" +
                    "\"" + "nodeDataArray" + "\"" + ":" + "[" + "\r\n" +
                    S_Node + "\r\n" +
                    "]," + "\r\n" +
                    "\"" + "linkDataArray" + "\"" + ":[" + "\r\n" +
                    S_Link + "\r\n" +
                    "]" + "\r\n" +
                    "}"
                    ;

                string S_RouteXml2_PWD = "";
                string[] List_XmlV2 = S_RouteXml2.Split('\r');

                foreach (var item in List_XmlV2)
                {
                    string S_Pwd = RSASFC.RsaEncrypt(item);
                    if (S_RouteXml2_PWD == "")
                    {
                        S_RouteXml2_PWD = S_Pwd;
                    }
                    else
                    {
                        S_RouteXml2_PWD += "," + "\r\n" + S_Pwd;
                    }
                }



                S_Sql =
                @"INSERT INTO mesRouteHistory (RouteID,XMLRoute,EmployeeID,XMLRouteV2) VALUES
                (
                    '" + List_mesRoute[0].ID + @"'
                    ,'" + List_mesRoute[0].XMLRoute + @"'
                    ,'" + List_Login.EmployeeID + @"'
                    ,'" + List_mesRoute[0].XMLRouteV2 + @"'
                )
                Update mesRoute set XMLRouteV2='" + S_RouteXml2_PWD + "' where Id='" + Id + "'";

                ExecSql(S_Sql);

            }
            catch(Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> ShowXMLRouteV2(string Id, IDbTransaction trans = null)
        {
            string S_Result = "";
            try
            {
                string S_Sql = "";
                S_Sql = "select * from mesRoute where Id='" + Id + "'";
                var v_query = await DapperConnRead2.QueryAsync<SC_mesRouteDto>(S_Sql, null, null, I_DBTimeout, null);
                List<SC_mesRouteDto> list = v_query.ToList();

                string S_XMLRouteV2 = list[0].XMLRouteV2;
                S_Result = S_XMLRouteV2;


                //string[] List_XMLRouteV2 = S_XMLRouteV2.Split(',');
                //string S_PWD_XMLRouteV2 = null;

                //string SSS = "";
                //foreach (var item in List_XMLRouteV2)
                //{
                //    S_PWD_XMLRouteV2 = RSASFC.RsaDecrypt(item);
                //    if (SSS == "")
                //    {
                //        SSS = S_PWD_XMLRouteV2;
                //    }
                //    else
                //    {
                //        SSS += S_PWD_XMLRouteV2;
                //    }
                //}

                //string S = SSS;

            }
            catch (Exception ex)
            {
                S_Result = "";
            }
            return S_Result;
        }

        public async Task<string> SaveXMLRouteV2(SC_XMLRouteV2 v_XMLRouteV2, IDbTransaction trans = null) 
        {
            string S_Result = "OK";
            string S_RouteID = v_XMLRouteV2.ID;

            try 
            {
                ////string SSS=await SetXMLToXMLV2(S_RouteID, trans);
                ////string SSS = await SetXMLToXMLV2("3", trans);

                //if (SSS != "OK")
                //{
                //    return SSS;
                //}

                string S_Check= SaveRouteDataV2Check(S_RouteID, v_XMLRouteV2.XMLRouteV2);
                if (S_Check != "OK") 
                {
                    return S_Check;
                }


                string S_Sql = "";
                S_Sql = "select * from mesRoute where Id='"+ v_XMLRouteV2.ID + "'";
                DataTable DT_Old = Data_Table(S_Sql);

                S_Sql = "";
                if (DT_Old.Rows.Count > 0)
                {
                    if (DT_Old.Rows[0]["XMLRouteV2"].ToString() != "")
                    {
                        S_Sql = @"INSERT INTO mesRouteHistory (RouteID,XMLRoute,EmployeeID,XMLRouteV2) VALUES
                        (
                            '" + v_XMLRouteV2.ID + @"'
                            ,'" + DT_Old.Rows[0]["XMLRoute"].ToString() + @"'
                            ,'" + List_Login.EmployeeID + @"'
                            ,'" + v_XMLRouteV2.XMLRouteV2 + @"'
                       )";
                    }
                }
                S_Sql += "Update mesRoute set XMLRouteV2='" + v_XMLRouteV2.XMLRouteV2 + "' where Id='"+ v_XMLRouteV2.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                S_Result = SaveRouteDataV2(S_RouteID, v_XMLRouteV2.XMLRouteV2);

            }
            catch (Exception ex)
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }
       
        private string SaveRouteDataV2Check(string S_RouteID, string v_XMLRouteV2)
        {
            string S_Result = "OK";
            try
            {
                //Task<string> ss =await SetXMLToXMLV2(S_RouteID,null);


                string[] List_XMLRouteV2 = v_XMLRouteV2.Split(',');
                string S_PWD_XMLRouteV2 = null;
                string S_XMLRouteV2 = null;

                foreach (var item in List_XMLRouteV2)
                {
                    S_PWD_XMLRouteV2 = RSASFC.RsaDecrypt(item);
                    S_XMLRouteV2 += S_PWD_XMLRouteV2;
                }

                JObject MyJO = (JObject)JsonConvert.DeserializeObject(S_XMLRouteV2.ToString());
                List<SC_nodeData> List_SC_nodeData = new List<SC_nodeData>();
                List<SC_linkData> List_SC_linkData = new List<SC_linkData>();

                string text = "";
                string key = "";
                string loc = "";
                string stationTypeID = "";

                string category = "";

                string from = "";
                string to = "";
                string fromPort = "";
                string toPort = "";
                string unitStatusID = "";
                string unitStateID = "";

                string fromUnitStateID = "";
                string toUnitStateID = "";
                string looperCount = "";

                for (int i = 0; i < MyJO["nodeDataArray"].Count(); i++)
                {
                    object obj = MyJO["nodeDataArray"][i];

                    text = MyJO["nodeDataArray"][i]["text"].ToString();
                    key = MyJO["nodeDataArray"][i]["key"].ToString();
                    loc = MyJO["nodeDataArray"][i]["loc"].ToString();

                    if (text != "Start" && text != "End")
                    {
                        if (MyJO["nodeDataArray"][i]["stationTypeID"] == null)
                        {
                            return S_Result = text + ": StationTypeID Cannot be empty";
                        }


                        stationTypeID = MyJO["nodeDataArray"][i]["stationTypeID"].ToString();

                        category = "";
                        try
                        {
                            if (MyJO["nodeDataArray"][i]["category"] != null)
                            {
                                category = MyJO["nodeDataArray"][i]["category"].ToString();
                            }
                        }
                        catch
                        { }

                        TBLR leftArray = new TBLR();
                        try
                        {
                            if (MyJO["nodeDataArray"][i]["leftArray"] != null)
                            {
                                string S_leftArray = MyJO["nodeDataArray"][i]["leftArray"].ToString();
                                if (S_leftArray != "")
                                {
                                    var v_JArray = JArray.Parse(S_leftArray);
                                    var JO_leftArray = v_JArray.ToObject<List<TBLR>>();

                                    leftArray.portId = JO_leftArray[0].portId;
                                    leftArray.portColor = JO_leftArray[0].portColor;
                                }
                            }
                        }
                        catch
                        { }

                        TBLR rightArray = new TBLR();
                        try
                        {
                            if (MyJO["nodeDataArray"][i]["rightArray"] != null)
                            {
                                string S_rightArray = MyJO["nodeDataArray"][i]["rightArray"].ToString();
                                if (S_rightArray != "")
                                {
                                    var v_JArray = JArray.Parse(S_rightArray);
                                    var JO_rightArray = v_JArray.ToObject<List<TBLR>>();

                                    rightArray.portId = JO_rightArray[0].portId;
                                    rightArray.portColor = JO_rightArray[0].portColor;
                                }
                            }
                        }
                        catch
                        { }

                        TBLR topArray = new TBLR();
                        try
                        {
                            if (MyJO["nodeDataArray"][i]["topArray"] != null)
                            {
                                string S_topArray = MyJO["nodeDataArray"][i]["topArray"].ToString();
                                if (S_topArray != "")
                                {
                                    var v_JArray = JArray.Parse(S_topArray);
                                    var JO_topArray = v_JArray.ToObject<List<TBLR>>();

                                    topArray.portId = JO_topArray[0].portId;
                                    topArray.portColor = JO_topArray[0].portColor;
                                }
                            }
                        }
                        catch
                        { }

                        TBLR bottomArray = new TBLR();
                        try
                        {
                            if (MyJO["nodeDataArray"][i]["bottomArray"] != null)
                            {
                                string S_bottomArray = MyJO["nodeDataArray"][i]["bottomArray"].ToString().Trim();
                                if (S_bottomArray != "")
                                {
                                    var v_JArray = JArray.Parse(S_bottomArray);
                                    var JO_bottomArray = v_JArray.ToObject<List<TBLR>>();

                                    bottomArray.portId = JO_bottomArray[0].portId;
                                    bottomArray.portColor = JO_bottomArray[0].portColor;
                                }
                            }
                        }
                        catch
                        { }

                        SC_nodeData v_SC_nodeData = new SC_nodeData();

                        v_SC_nodeData.text = text;
                        v_SC_nodeData.key = key;
                        v_SC_nodeData.loc = loc;
                        v_SC_nodeData.stationTypeID = stationTypeID;

                        v_SC_nodeData.category = category;
                        v_SC_nodeData.leftArray = leftArray;
                        v_SC_nodeData.rightArray = rightArray;
                        v_SC_nodeData.topArray = topArray;
                        v_SC_nodeData.bottomArray = bottomArray;

                        List_SC_nodeData.Add(v_SC_nodeData);
                    }
                }

                for (int i = 0; i < MyJO["linkDataArray"].Count(); i++)
                {
                    object obj = MyJO["linkDataArray"][i];

                    from = MyJO["linkDataArray"][i]["from"].ToString();
                    to = MyJO["linkDataArray"][i]["to"].ToString();
                    fromPort = MyJO["linkDataArray"][i]["fromPort"].ToString();
                    toPort = MyJO["linkDataArray"][i]["toPort"].ToString();

                    text = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["text"] != null)
                        {
                            text = MyJO["linkDataArray"][i]["text"].ToString();
                        }
                    }
                    catch
                    { }

                    unitStatusID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["unitStatusID"] != null)
                        {
                            unitStatusID = MyJO["linkDataArray"][i]["unitStatusID"].ToString();
                        }
                    }
                    catch
                    { }

                    unitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["unitStateID"] != null)
                        {
                            unitStateID = MyJO["linkDataArray"][i]["unitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    category = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["category"] != null)
                        {
                            category = MyJO["linkDataArray"][i]["category"].ToString();
                        }
                    }
                    catch
                    { }

                    fromUnitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["fromUnitStateID"] != null)
                        {
                            fromUnitStateID = MyJO["linkDataArray"][i]["fromUnitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    toUnitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["toUnitStateID"] != null)
                        {
                            toUnitStateID = MyJO["linkDataArray"][i]["toUnitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    looperCount = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["looperCount"] != null)
                        {
                            looperCount = MyJO["linkDataArray"][i]["looperCount"].ToString();
                        }
                    }
                    catch
                    { }


                    SC_linkData v_SC_linkData = new SC_linkData();

                    v_SC_linkData.from = from;
                    v_SC_linkData.to = to;
                    v_SC_linkData.fromPort = fromPort;
                    v_SC_linkData.toPort = toPort;
                    v_SC_linkData.unitStatusID = unitStatusID;
                    v_SC_linkData.unitStateID = unitStateID;

                    v_SC_linkData.text = text;
                    v_SC_linkData.category = category;
                    v_SC_linkData.fromUnitStateID = fromUnitStateID;
                    v_SC_linkData.toUnitStateID = toUnitStateID;
                    v_SC_linkData.looperCount = looperCount;

                    List_SC_linkData.Add(v_SC_linkData);
                }

                string S_Error = "";
                foreach (var item in List_SC_nodeData)
                {
                    if (item.text != "Start")
                    {
                        var query_ToFrom = from c in List_SC_linkData
                                           where c.@from == item.key && c.@from != c.to
                                           select c;
                        List<SC_linkData> List_query_ToFrom = query_ToFrom.ToList();
                        //分组类别
                        if (List_query_ToFrom.Count > 0)
                        {
                            //分组类别1
                            var query_Group = from c in List_query_ToFrom
                                              group c by new { c.unitStatusID,c.unitStateID,c.to } into g
                                              select new
                                              {
                                                  g.Key,
                                                  OutputCount = g.Count()
                                              }
                                              ;

                            var query_Group_Count = from c in query_Group where c.OutputCount > 1 select c;
                            foreach (var item_Count in query_Group_Count)
                            {
                                if (item_Count.OutputCount > 1)
                                {
                                    S_Error += "Output StationType:" + item.text + " UnitStatusID:" +
                                        item_Count.Key.unitStatusID + " non-uniqueness" + "\r\n";
                                }
                            }

                            //分组类别2
                            var query_Group2 = from c in List_query_ToFrom
                                              group c by new { c.unitStatusID} into g
                                              select new
                                              {
                                                  g.Key,
                                                  OutputCount = g.Count()
                                              }
                                              ;
                            var query_Group_Count2 = from c in query_Group2 where c.OutputCount > 1 select c;
                            foreach (var item_Count in query_Group_Count2)
                            {
                                if (item_Count.OutputCount > 1)
                                {
                                    var query_unitStateID=from c in List_query_ToFrom 
                                                          where c.unitStatusID== item_Count.Key.unitStatusID select c;

                                    string S_unitStateID = "";
                                    foreach (var item_unitStateID in query_unitStateID) 
                                    {
                                        if (S_unitStateID == "")
                                        {
                                            S_unitStateID = item_unitStateID.unitStateID;
                                        }
                                        else 
                                        {
                                            if (S_unitStateID != item_unitStateID.unitStateID) 
                                            {
                                                S_Error += "Output StationType:" + item.text + " UnitStatusID:" +
                                                    item_Count.Key.unitStatusID + " non-uniqueness" + "\r\n";
                                            }
                                        }
                                    }
                                }
                            }


                            var query_NotLink = from c in List_query_ToFrom where c.unitStateID == "" select c;
                            foreach (var item_NotLink in query_NotLink)
                            {
                                S_Error += "Output StationType:" + item.text + " unitStateID:" +
                                    item_NotLink.unitStateID + " is null " + "\r\n";
                            }

                            query_NotLink = from c in List_query_ToFrom where c.unitStatusID == "" select c;
                            foreach (var item_NotLink in query_NotLink)
                            {
                                S_Error += "Output StationType:" + item.text + " unitStatusID:" +
                                    item_NotLink.unitStatusID + " is null " + "\r\n";
                            }

                        }


                        //////////////////////////////////////////////////////////////////////////////////////
                        var query_MeToMe = from c in List_SC_linkData
                                           where c.@from == item.key && c.@from == c.to
                                               && c.fromPort == c.toPort
                                           select c;
                        List<SC_linkData> List_query_MeToMe = query_MeToMe.ToList();

                        foreach (var item_query_MeToMe in List_query_MeToMe) 
                        {
                            //本站输入的线条 
                            var query_Pre_StationType_Link = from c in List_SC_linkData
                                                             where c.to == item.key && c.toPort == item_query_MeToMe.fromPort
                                                                && c.unitStateID != ""
                                                             select c;
                            List<SC_linkData> List_Pre_StationType_Link = query_Pre_StationType_Link.ToList();
                            if (List_Pre_StationType_Link.Count > 0)
                            {
                                S_Error += "Input StationType:" + item.text + " toProt:" +
                                    List_Pre_StationType_Link[0].toPort + " from->to error" + "\r\n";
                            }
                        }

                    }
                }

                if (S_Error != "OK" && S_Error!="") 
                {
                    return S_Error;
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        private string SaveRouteDataV2(string S_RouteID, string v_XMLRouteV2)
        {
            string S_Result = "OK";
            string S_Exec_Sql = "OK";
            try
            {
                string[] List_XMLRouteV2 = v_XMLRouteV2.Split(',');
                string S_PWD_XMLRouteV2 = null;
                string S_XMLRouteV2 = null;
                string S_Sql = "";

                foreach (var item in List_XMLRouteV2)
                {
                    S_PWD_XMLRouteV2 = RSASFC.RsaDecrypt(item);
                    S_XMLRouteV2 += S_PWD_XMLRouteV2;
                }

                JObject MyJO = (JObject)JsonConvert.DeserializeObject(S_XMLRouteV2.ToString());
                List<SC_nodeData> List_SC_nodeData = new List<SC_nodeData>();
                List<SC_linkData> List_SC_linkData = new List<SC_linkData>();

                string text = "";
                string key = "";
                string loc = "";
                string stationTypeID = "";

                string category = "";

                string from = "";
                string to = "";
                string fromPort = "";
                string toPort = "";
                string unitStatusID = "";
                string unitStateID = "";

                string fromUnitStateID = "";
                string toUnitStateID = "";
                string looperCount = "";

                for (int i = 0; i < MyJO["nodeDataArray"].Count(); i++)
                {
                    object obj = MyJO["nodeDataArray"][i];

                    text = MyJO["nodeDataArray"][i]["text"].ToString();
                    key = MyJO["nodeDataArray"][i]["key"].ToString();
                    loc = MyJO["nodeDataArray"][i]["loc"].ToString();

                    //if (text == "Start") { continue; }


                    if (text == "Start" || text == "End")
                    {
                        stationTypeID = "";
                    }
                    else 
                    { 
                        stationTypeID = MyJO["nodeDataArray"][i]["stationTypeID"].ToString();
                    }

                    category = "";
                    try
                    {
                        if (MyJO["nodeDataArray"][i]["category"] != null)
                        {
                            category = MyJO["nodeDataArray"][i]["category"].ToString();
                        }
                    }
                    catch
                    { }

                    TBLR leftArray = new TBLR();
                    try
                    {
                        if (MyJO["nodeDataArray"][i]["leftArray"] != null)
                        {
                            string S_leftArray = MyJO["nodeDataArray"][i]["leftArray"].ToString();
                            if (S_leftArray != "")
                            {
                                var v_JArray = JArray.Parse(S_leftArray);
                                var JO_leftArray = v_JArray.ToObject<List<TBLR>>();

                                leftArray.portId = JO_leftArray[0].portId;
                                leftArray.portColor = JO_leftArray[0].portColor;
                            }
                        }
                    }
                    catch
                    { }

                    TBLR rightArray = new TBLR();
                    try
                    {
                        if (MyJO["nodeDataArray"][i]["rightArray"] != null)
                        {
                            string S_rightArray = MyJO["nodeDataArray"][i]["rightArray"].ToString();
                            if (S_rightArray != "")
                            {
                                var v_JArray = JArray.Parse(S_rightArray);
                                var JO_rightArray = v_JArray.ToObject<List<TBLR>>();

                                rightArray.portId = JO_rightArray[0].portId;
                                rightArray.portColor = JO_rightArray[0].portColor;
                            }
                        }
                    }
                    catch
                    { }

                    TBLR topArray = new TBLR();
                    try
                    {
                        if (MyJO["nodeDataArray"][i]["topArray"] != null)
                        {
                            string S_topArray = MyJO["nodeDataArray"][i]["topArray"].ToString();
                            if (S_topArray != "")
                            {
                                var v_JArray = JArray.Parse(S_topArray);
                                var JO_topArray = v_JArray.ToObject<List<TBLR>>();

                                topArray.portId = JO_topArray[0].portId;
                                topArray.portColor = JO_topArray[0].portColor;
                            }
                        }
                    }
                    catch
                    { }

                    TBLR bottomArray = new TBLR();
                    try
                    {
                        if (MyJO["nodeDataArray"][i]["bottomArray"] != null)
                        {
                            string S_bottomArray = MyJO["nodeDataArray"][i]["bottomArray"].ToString().Trim();
                            if (S_bottomArray != "")
                            {
                                var v_JArray = JArray.Parse(S_bottomArray);
                                var JO_bottomArray = v_JArray.ToObject<List<TBLR>>();

                                bottomArray.portId = JO_bottomArray[0].portId;
                                bottomArray.portColor = JO_bottomArray[0].portColor;
                            }
                        }
                    }
                    catch
                    { }

                    SC_nodeData v_SC_nodeData = new SC_nodeData();

                    v_SC_nodeData.text = text;
                    v_SC_nodeData.key = key;
                    v_SC_nodeData.loc = loc;
                    v_SC_nodeData.stationTypeID = stationTypeID;

                    v_SC_nodeData.category = category;
                    v_SC_nodeData.leftArray = leftArray;
                    v_SC_nodeData.rightArray = rightArray;
                    v_SC_nodeData.topArray = topArray;
                    v_SC_nodeData.bottomArray = bottomArray;

                    List_SC_nodeData.Add(v_SC_nodeData);
                }

                for (int i = 0; i < MyJO["linkDataArray"].Count(); i++)
                {
                    object obj = MyJO["linkDataArray"][i];

                    from = MyJO["linkDataArray"][i]["from"].ToString();
                    to = MyJO["linkDataArray"][i]["to"].ToString();
                    fromPort = MyJO["linkDataArray"][i]["fromPort"].ToString();
                    toPort = MyJO["linkDataArray"][i]["toPort"].ToString();

                    text = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["text"] != null)
                        {
                            text = MyJO["linkDataArray"][i]["text"].ToString();
                        }
                    }
                    catch
                    { }

                    unitStatusID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["unitStatusID"] != null)
                        {
                            unitStatusID = MyJO["linkDataArray"][i]["unitStatusID"].ToString();
                        }
                    }
                    catch
                    { }

                    unitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["unitStateID"] != null)
                        {
                            unitStateID = MyJO["linkDataArray"][i]["unitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    category = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["category"] != null)
                        {
                            category = MyJO["linkDataArray"][i]["category"].ToString();
                        }
                    }
                    catch
                    { }

                    fromUnitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["fromUnitStateID"] != null)
                        {
                            fromUnitStateID = MyJO["linkDataArray"][i]["fromUnitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    toUnitStateID = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["toUnitStateID"] != null)
                        {
                            toUnitStateID = MyJO["linkDataArray"][i]["toUnitStateID"].ToString();
                        }
                    }
                    catch
                    { }

                    looperCount = "";
                    try
                    {
                        if (MyJO["linkDataArray"][i]["looperCount"] != null)
                        {
                            looperCount = MyJO["linkDataArray"][i]["looperCount"].ToString();
                        }
                    }
                    catch
                    { }


                    SC_linkData v_SC_linkData = new SC_linkData();

                    v_SC_linkData.from = from;
                    v_SC_linkData.to = to;
                    v_SC_linkData.fromPort = fromPort;
                    v_SC_linkData.toPort = toPort;
                    v_SC_linkData.unitStatusID = unitStatusID;
                    v_SC_linkData.unitStateID = unitStateID;

                    v_SC_linkData.text = text;
                    v_SC_linkData.category = category;
                    v_SC_linkData.fromUnitStateID = fromUnitStateID;
                    v_SC_linkData.toUnitStateID = toUnitStateID;
                    v_SC_linkData.looperCount=looperCount;

                    List_SC_linkData.Add(v_SC_linkData);
                }

                S_Sql = "delete mesUnitInputState where RouteID=" + S_RouteID + "\r\n" +
                        "delete mesUnitOutputState where RouteID=" + S_RouteID + "\r\n" ;
                // "insert into mesRouteHistory(RouteID,XMLRoute,XMLRouteV2) select ID RouteID,XMLRoute,XMLRouteV2 from mesRoute where ID=" + S_RouteID;
                S_Exec_Sql =ExecSql(S_Sql);
                if (S_Exec_Sql != "OK") 
                {
                    return S_Exec_Sql;
                }

                foreach (var item in List_SC_nodeData)
                {
                    if (item.text != "Start" && item.text != "End")
                    {
                        var query_To = from c in List_SC_linkData
                                       where c.to == item.key
                                       select c;

                        if (query_To.Count() > 0) 
                        {
                            var  query_To_Start = (from c in List_SC_linkData
                                        where c.to == item.key
                                        select c).FirstOrDefault();

                            //上一个是 Start
                            var query_From_Start = (from c in List_SC_nodeData
                                                    where c.key== query_To_Start.@from
                                                    select c).FirstOrDefault();
                            if (query_From_Start.text == "Start")
                            {
                                query_To = from c in List_SC_linkData
                                           where c.to == "StartKey"
                                           select c;
                            }
                        }


                        var query_From = from c in List_SC_linkData
                                         where c.@from == item.key
                                         select c;

                        if (query_To.Count() == 0 && query_From.Count() > 0)
                        {
                            foreach (var v_From in query_From)
                            {
                                var qurty_Station_To = (from c in List_SC_nodeData where c.key == v_From.to select c).FirstOrDefault();
                                if (v_From.from == v_From.to)
                                {
                                    continue;
                                }

                                string S_OutputStatusID = "1";
                                if (v_From.unitStatusID.Trim() != "")
                                {
                                    S_OutputStatusID = v_From.unitStatusID.Trim();
                                }


                                var query_Path_To = (from c in List_SC_linkData where c.to == v_From.to select c).FirstOrDefault();
                                var query_Path_From = (from c in List_SC_linkData where c.@from == v_From.@from select c).FirstOrDefault();

                                if (CheckInput(S_RouteID, item.stationTypeID, "0") == "1")
                                {
                                    S_Sql = "insert into mesUnitInputState(RouteID,StationTypeID,CurrStateID) Values(" +
                                           "'" + S_RouteID + "'," +
                                           "'" + item.stationTypeID + "'," +
                                            "'0'" +
                                            ")";
                                    S_Exec_Sql = ExecSql(S_Sql);
                                    if (S_Exec_Sql != "OK")
                                    {
                                        return S_Exec_Sql;
                                    }
                                }


                                if (CheckOutput(S_RouteID, item.stationTypeID, qurty_Station_To.stationTypeID,
                                    v_From.unitStateID, S_OutputStatusID) == "1")
                                {
                                    S_Sql = "insert into mesUnitOutputState(RouteID,StationTypeID,CurrStateID,OutputStateID,OutputStateDefID) Values(" +
                                              "'" + S_RouteID + "'," +
                                              "'" + item.stationTypeID + "'," +
                                              "'" + qurty_Station_To.stationTypeID + "'," +
                                              "'" + v_From.unitStateID + "'," +
                                              "'" + S_OutputStatusID + "'" +
                                        ")";
                                    S_Exec_Sql = ExecSql(S_Sql);
                                    if (S_Exec_Sql != "OK")
                                    {
                                        return S_Exec_Sql;
                                    }
                                }
                            }
                        }
                        else if (query_To.Count() > 0 && query_From.Count() > 0)
                        {
                            query_To = from c in List_SC_linkData
                                       where c.@from == item.key
                                       select c;

                            query_From = from c in List_SC_linkData
                                         where c.to == item.key
                                         select c;

                            foreach (var v_From in query_From)
                            {
                                var query_StationTypeFrom = (from c in List_SC_nodeData where c.key == v_From.@from select c).FirstOrDefault();
                                var query_StationTypeTo = (from c in List_SC_nodeData where c.key == v_From.to select c).FirstOrDefault();

                                var query_Path_To = (from c in List_SC_linkData where c.to == v_From.to select c).FirstOrDefault();
                                var query_Path_From = (from c in List_SC_linkData where c.@from == v_From.@from select c).FirstOrDefault();

                                if (v_From.from == v_From.to)
                                {
                                    var query_LooperCount = from c in List_SC_linkData where c.@from == v_From.to || c.to == v_From.to select c;
                                    if (query_LooperCount.Count() == 0)
                                    {

                                    }

                                    continue;
                                }


                                if (CheckInput(S_RouteID, item.stationTypeID, v_From.unitStateID) == "1")
                                {
                                    if (query_StationTypeTo.text != null && v_From.unitStateID != "")
                                    {
                                        S_Sql = "insert into mesUnitInputState(RouteID,StationTypeID,CurrStateID) Values(" +
                                               "'" + S_RouteID + "'," +
                                               "'" + item.stationTypeID + "'," +
                                                "'" + v_From.unitStateID + "'" +
                                                ")";
                                        S_Exec_Sql = ExecSql(S_Sql);
                                        if (S_Exec_Sql != "OK")
                                        {
                                            return S_Exec_Sql;
                                        }
                                    }
                                }
                            }

                            foreach (var v_From in query_To)
                            {
                                string S_OutputState = "1";
                                if (v_From.unitStatusID.Trim() != "")
                                {
                                    S_OutputState = v_From.unitStatusID.Trim();
                                }

                                var query_StationTypeFrom = (from c in List_SC_nodeData where c.key == v_From.@from select c).FirstOrDefault();
                                var query_StationTypeTo = (from c in List_SC_nodeData where c.key == v_From.to select c).FirstOrDefault();

                                var query_Path_To = (from c in List_SC_linkData where c.to == v_From.@from select c).FirstOrDefault();
                                var query_Path_From = (from c in List_SC_linkData where c.@from == v_From.to select c).FirstOrDefault();

                                if (v_From.from == v_From.to)
                                {
                                    continue;
                                }

                                if (query_StationTypeTo.text != null)
                                {
                                    var qurty_Station_To = (from c in List_SC_nodeData where c.key == v_From.to select c).FirstOrDefault();

                                    qurty_Station_To.stationTypeID = qurty_Station_To.stationTypeID == "" ? "0" : qurty_Station_To.stationTypeID;
                                    if (CheckOutput(S_RouteID, item.stationTypeID, qurty_Station_To.stationTypeID,
                                        v_From.unitStateID, S_OutputState) == "1")
                                    {                                        
                                        S_Sql = "insert into mesUnitOutputState(RouteID,StationTypeID,CurrStateID,OutputStateID,OutputStateDefID) Values(" +
                                              "'" + S_RouteID + "'," +
                                              "'" + item.stationTypeID + "'," +
                                              "'" + qurty_Station_To.stationTypeID + "'," +
                                              "'" + v_From.unitStateID + "'," +
                                              "'" + S_OutputState + "'" +
                                        ")";
                                        S_Exec_Sql = ExecSql(S_Sql);
                                        if (S_Exec_Sql != "OK")
                                        {
                                            return S_Exec_Sql;
                                        }
                                    }
                                }
                            }

                        }
                        else if (query_To.Count() > 0 && query_From.Count() == 0)
                        {
                            foreach (var v_From in query_To)
                            {
                                string S_OutputState = "1";
                                if (v_From.unitStateID.Trim() != "")
                                {
                                    S_OutputState = v_From.unitStateID.Trim();
                                }

                                var query_StationTypeFrom = (from c in List_SC_nodeData where c.key == v_From.to select c).FirstOrDefault();
                                var query_StationTypeTo = (from c in List_SC_nodeData where c.key == v_From.@from select c).FirstOrDefault();

                                var query_Path_To = (from c in List_SC_linkData where c.to == v_From.to select c).FirstOrDefault();
                                var query_Path_From = (from c in List_SC_linkData where c.@from == v_From.@from select c).FirstOrDefault();

                                if (v_From.from == v_From.to)
                                {
                                    continue;
                                }

                                //if (CheckInput(S_RouteID, item.stationTypeID, query_Path_To.unitStateID) == "1")
                                if (CheckInput(S_RouteID, item.stationTypeID, v_From.unitStateID) == "1")
                                {
                                    S_Sql = "insert into mesUnitInputState(RouteID,StationTypeID,CurrStateID) Values(" +
                                       "'" + S_RouteID + "'," +
                                       "'" + item.stationTypeID + "'," +
                                        //  "'" + query_Path_To.unitStateID + "'" +
                                        "'" + v_From.unitStateID + "'" +
                                        ")";
                                    S_Exec_Sql = ExecSql(S_Sql);
                                    if (S_Exec_Sql != "OK")
                                    {
                                        return S_Exec_Sql;
                                    }
                                }
                            }
                        }
                        else if (query_To.Count() == 0 && query_From.Count() == 0)
                        {
                            if (CheckInput(S_RouteID, item.stationTypeID, "0") == "1")
                            {
                                S_Sql = "insert into mesUnitInputState(RouteID,StationTypeID,CurrStateID) Values(" +
                                   "'" + S_RouteID + "'," +
                                   "'" + item.stationTypeID + "'," +
                                    "'0'" +
                                    ")";
                                S_Exec_Sql = ExecSql(S_Sql);
                                if (S_Exec_Sql != "OK")
                                {
                                    return S_Exec_Sql;
                                }
                            }
                        }
                    }
                }
                //内部 短连接线处理
                foreach (var item in List_SC_nodeData) 
                {
                    if (item.text != "Start" && item.text != "End")
                    //if (item.text != "Start")
                    {
                        var query_ToFrom = from c in List_SC_linkData
                                           where c.to == item.key && c.@from == item.key
                                           select c;

                        //int I_DeleteOne = 0;
                        if (query_ToFrom.Count() > 0)
                        {
                            List<SC_linkData> List_query_ToFrom = query_ToFrom.ToList();
                            
                            foreach (var item_query_ToFrom in List_query_ToFrom)
                            {
                                if (item_query_ToFrom.looperCount == "")
                                {
                                    //本站输入的线条 
                                    var query_Pre_StationType_Link=from c in List_SC_linkData
                                                                   where c.to== item.key && c.toPort== item_query_ToFrom.fromPort
                                                                   select c;
                                    List<SC_linkData> List_Pre_StationType_Link = query_Pre_StationType_Link.ToList();

                                    // 本站节点
                                    var query_StationType_Node = from c in List_SC_nodeData
                                                                 where c.key == item_query_ToFrom.to
                                                                 select c;
                                    List<SC_nodeData> List_StationType_Node = query_StationType_Node.ToList();
                                    string S_StationType_Node = List_StationType_Node[0].stationTypeID;


                                    //本站输出的下一站线条
                                    var query_Next_StationType_Link = from c in List_SC_linkData
                                                                      where c.@from == item.key && c.fromPort == item_query_ToFrom.toPort
                                                                      select c;
                                    List<SC_linkData> List_Next_StationType_Link = query_Next_StationType_Link.ToList();

                                    //下一站节点
                                    var query_Next_Station_Node = from c in List_SC_nodeData
                                                                  where c.key == List_Next_StationType_Link[0].to
                                                                  select c;

                                    List<SC_nodeData> List_Next_Station_Node = query_Next_Station_Node.ToList();
                                    string S_Next_Station_Node = List_Next_Station_Node[0].stationTypeID;


 
                                    //if (I_DeleteOne == 0) 
                                    //{
                                    //    S_Sql = "delete mesUnitOutputState  " +
                                    //        " where RouteID='" + S_RouteID + "' and StationTypeID='" + S_StationType_Node +
                                    //        "' and  CurrStateID='" + S_Next_Station_Node + "'";

                                    //    S_Exec_Sql = ExecSql(S_Sql);
                                    //    if (S_Exec_Sql != "OK")
                                    //    {
                                    //        return S_Exec_Sql;
                                    //    }
                                    //    I_DeleteOne += 1;
                                    //}

 
                                    
                                    //S_Sql = "insert into mesUnitOutputState(RouteID,StationTypeID,CurrStateID,OutputStateID,OutputStateDefID,InputStateID) Values(" +
                                    //        "'" + S_RouteID + "'," +
                                    //        "'" + S_StationType_Node + "'," +
                                    //        "'" + S_Next_Station_Node + "'," +
                                    //        "'" + List_Next_StationType_Link[0].unitStateID + "'," +
                                    //        "'" + List_Next_StationType_Link[0].unitStatusID + "'," +
                                    //        "'" + List_Pre_StationType_Link[0].unitStateID + "'" +
                                    //")";




                                    if (List_Pre_StationType_Link[0].unitStateID != "")
                                    {
                                        S_Sql = $@"SELECT 1 FROM dbo.mesUnitOutputState WHERE RouteID = {S_RouteID} AND StationTypeID = {S_StationType_Node} AND CurrStateID = {S_Next_Station_Node} AND OutputStateID = {List_Next_StationType_Link[0].unitStateID} AND OutputStateDefID = {List_Next_StationType_Link[0].unitStatusID} AND ISNULL(InputStateID,0) = 0";

                                        S_Exec_Sql = ExecFirstSql(S_Sql);
                                        if (S_Exec_Sql == "1")
                                        {
                                            S_Sql =
                                            "Update mesUnitOutputState set InputStateID='" + List_Pre_StationType_Link[0].unitStateID + "'" +
                                            " where RouteID='" + S_RouteID + "' " +
                                            " and StationTypeID='" + S_StationType_Node + "'" +
                                            " and CurrStateID='" + S_Next_Station_Node + "'" +
                                            " and OutputStateID='" + List_Next_StationType_Link[0].unitStateID + "'" +
                                            " and OutputStateDefID='" + List_Next_StationType_Link[0].unitStatusID + "'"
                                            ;
                                        }
                                        else
                                        {
                                            S_Sql = "insert into mesUnitOutputState(RouteID,StationTypeID,CurrStateID,OutputStateID,OutputStateDefID,InputStateID) Values(" +
                                                    "'" + S_RouteID + "'," +
                                                    "'" + S_StationType_Node + "'," +
                                                    "'" + S_Next_Station_Node + "'," +
                                                    "'" + List_Next_StationType_Link[0].unitStateID + "'," +
                                                    "'" + List_Next_StationType_Link[0].unitStatusID + "'," +
                                                    "'" + List_Pre_StationType_Link[0].unitStateID + "'" +
                                            ")";
                                        }
                                    }




                                    S_Exec_Sql = ExecSql(S_Sql);
                                    if (S_Exec_Sql != "OK")
                                    {
                                        return S_Exec_Sql;
                                    }
                                }
                                else
                                {
                                    // 本站节点
                                    var query_StationType_Node = from c in List_SC_nodeData
                                                                 where c.key == item_query_ToFrom.to
                                                                 select c;
                                    List<SC_nodeData> List_StationType_Node = query_StationType_Node.ToList();
                                    string S_StationType_Node = List_StationType_Node[0].stationTypeID;
                                    string S_looperCount = item_query_ToFrom.looperCount;

                                    //本站输出的下一站线条
                                    var query_Next_StationType_Link = from c in List_SC_linkData
                                                                      where c.@from == item.key && c.fromPort == item_query_ToFrom.toPort
                                                                            && c.unitStateID!=""
                                                                      select c;
                                    List<SC_linkData> List_Next_StationType_Link = query_Next_StationType_Link.ToList();
                                    string S_Next_unitStateID = List_Next_StationType_Link[0].unitStateID;
                                    ///////////////////////////////
                                    S_Sql = "insert into mesUnitInputState(RouteID,StationTypeID,CurrStateID,LooperCount) Values(" + "\r\n" +
                                           "'" + S_RouteID + "'," + "\r\n" +
                                           "'" + S_StationType_Node + "'," + "\r\n" +
                                            "'" + S_Next_unitStateID + "'," + "\r\n" +
                                            "'" + S_looperCount + "'" + "\r\n" +
                                            ")";

                                    //+ "\r\n" +
                                    //自己到自己不插入 mesUnitOutputState
                                    //"insert into mesUnitOutputState(RouteID,StationTypeID,CurrStateID,OutputStateID,OutputStateDefID) Values(" + "\r\n" +
                                    //   "'" + S_RouteID + "'," + "\r\n" +
                                    //   "'" + S_StationType_Node + "'," + "\r\n" +
                                    //   "'" + S_StationType_Node + "'," + "\r\n" +
                                    //   "'" + item_query_ToFrom.fromUnitStateID + "'," + "\r\n" +
                                    //   "'1'" + "\r\n" +
                                    //")";

                                    S_Exec_Sql = ExecSql(S_Sql);
                                    if (S_Exec_Sql != "OK")
                                    {
                                        return S_Exec_Sql;
                                    }

                                }
                            }



                        }
                    }
                }

                try
                {
                    string S_SysPath = Directory.GetCurrentDirectory();

                    string S_Data100 = S_SysPath + "\\Data100.dll";
                    string S_Data101 = S_SysPath + "\\Data101.dll";

                    string S_Sql100 = "select * from mesUnitInputState";
                    DataTable DT_Sql100 = Data_Table(S_Sql100);
                    string json100 = PublicF.DataTableToJson(DT_Sql100);
                    string S_MI100 = EncryptHelper2023.EncryptString(json100);
                    File.WriteAllText(S_Data100, S_MI100);

                    string S_Sql101 = "select * from mesUnitOutputState";
                    DataTable DT_Sql101 = Data_Table(S_Sql101);
                    string json101 = PublicF.DataTableToJson(DT_Sql101);
                    string S_MI101 = EncryptHelper2023.EncryptString(json101);
                    File.WriteAllText(S_Data101, S_MI101);


                    try
                    {

                        string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                        string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                        foreach (var item in List_WinformWebDIR)
                        {
                            //string S_WinformWebDIR_Data100 = item + "\\Data100.dll";
                            //File.WriteAllText(S_WinformWebDIR_Data100, S_MI100);

                            //string S_WinformWebDIR_Data101 = item + "\\Data101.dll";
                            //File.WriteAllText(S_WinformWebDIR_Data101, S_MI101);


                            try
                            {
                                string S_FTPIP = item;
                                string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                                string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                                FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                                FTP.GotoDirectory("", true);
                                FTP.Upload(S_Data100, "Data100.dll");
                                FTP.Upload(S_Data101, "Data101.dll");
                            }
                            catch (Exception ex)
                            {
                                Log4NetHelper.Info(ex.Message);
                                throw new Exception(ex.Message);
                            }
                        }

                    }
                    catch { }
                }
                catch 
                { }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }
        private string CheckInput(string RouteID, string StationTypeID, string CurrStateID)
        {
            string S_Result = "1";
            string S_Sql = "select * from mesUnitInputState where RouteID='" + RouteID + "' and " +
                   " StationTypeID='" + StationTypeID + "' and CurrStateID='" + CurrStateID + "'";
            DataTable DT = Data_Table(S_Sql);
            if (DT.Rows.Count > 0)
            {
                S_Result = "0";
            }
            return S_Result;
        }

        private string CheckOutput(string RouteID, string StationTypeID, string CurrStateID, string OutputStateID, string OutputStateDefID)
        {
            string S_Result = "1";
            string S_Sql = "select * from mesUnitOutputState where RouteID='" + RouteID + "' and " +
                   " StationTypeID='" + StationTypeID + "' and CurrStateID='" + CurrStateID + "' and " +
                   " OutputStateID='" + OutputStateID + "' and OutputStateDefID='" + OutputStateDefID + "'";
            DataTable DT = Data_Table(S_Sql);
            if (DT.Rows.Count > 0)
            {
                S_Result = "0";
            }

            //S_Sql =
            //   @"
            //    SELECT COUNT(*) OutputCount FROM mesUnitOutputState 
	           //     WHERE RouteID='"+ RouteID + @"' AND StationTypeID='" + StationTypeID + @"' AND OutputStateDefID='"+ OutputStateDefID + @"
	           //     GROUP BY RouteID, StationTypeID, OutputStateDefID, OutputStateID
            //    HAVING COUNT(*)>0
            //    ";
            //DT = Data_Table(S_Sql);
            //if (DT.Rows.Count > 0)
            //{
            //    S_Result = "0";
            //}

            return S_Result;
        }


    }
}



