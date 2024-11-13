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
    public class SC_mesStationRepository : BaseRepositoryReport<string>, ISC_mesStationRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesStationRepository()
        {
        }
        public SC_mesStationRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesStation v_SC_mesStation, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStation.Description = v_SC_mesStation.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStation where Description='" + v_SC_mesStation.Description.Trim() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result =  v_SC_mesStation.Description + " Data already exists.";
                }


                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesStation";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                v_SC_mesStation.ID = v_MaxID.LastID.ToString();

                S_Sql = "insert into mesStation(ID, [Description], StationTypeID, LineID, [Status]) Values(" +
                     "'" + v_SC_mesStation.ID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.Status + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesStation.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStation", S_MSG);
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
                SC_mesStation v_SC_mesStation = new SC_mesStation();
                string S_Sql = "select * from mesStation where Id='" + Id + "'";
                v_SC_mesStation = await DapperConnRead2.QueryFirstAsync<SC_mesStation>(S_Sql);

                List<SC_mesStationConfigSetting> List_DetailDto = new List<SC_mesStationConfigSetting>();
                S_Sql = "select * from mesStationConfigSetting where StationID='" + Id + "'";
                var q_Query_Detail = await DapperConnRead2.QueryAsync<SC_mesStationConfigSetting>(S_Sql);
                List_DetailDto = q_Query_Detail.ToList();


                S_Sql = "delete mesStation where Id='" + Id + "' " + "\r\n" +
                        "delete mesStationConfigSetting where StationID='" + Id + "'"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesStation != null)
                {
                    string S_MSG = v_SC_mesStation.ToJson() + "\r\n" +
                           List_DetailDto.ToJson()
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStation", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesStation v_SC_mesStation, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStation.Description = v_SC_mesStation.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStation where Description='" + v_SC_mesStation.Description.Trim() + "'" +
                                " and ID<>'" + v_SC_mesStation.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStation>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesStation.Description + " Data already exists.";
                }


                SC_mesStation v_mesStation_before = new SC_mesStation();
                S_Sql = "select * from mesStation where Id='" + v_SC_mesStation.ID + "'";
                v_mesStation_before = await DapperConnRead2.QueryFirstAsync<SC_mesStation>(S_Sql);

                S_Sql = "Update mesStation set " +
                        " Description='" + v_SC_mesStation.Description + "'" +
                        " ,StationTypeID='" + v_SC_mesStation.StationTypeID + "'" +
                        " ,LineID='" + v_SC_mesStation.LineID + "'" +
                        " ,Status='" + v_SC_mesStation.Status + "'" +

                        " where Id='" + v_SC_mesStation.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStation_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesStation_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesStation.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStation", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesStation v_SC_mesStation, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStation.Description = v_SC_mesStation.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStation where Description='" + v_SC_mesStation.Description.Trim() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesStation.Description + " Data already exists.";
                }


                SC_mesStation v_mesStation_before = new SC_mesStation();
                S_Sql = "select * from mesStation where Id='" + v_SC_mesStation.ID + "'";
                v_mesStation_before = await DapperConnRead2.QueryFirstAsync<SC_mesStation>(S_Sql);

                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesStation";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                string S_MaxID= v_MaxID.LastID.ToString();

                S_Sql = "insert into mesStation(ID, [Description], StationTypeID, LineID, [Status]) Values(" +
                     "'" + S_MaxID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.StationTypeID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.LineID + "'" + "\r\n" +
                     ",'" + v_SC_mesStation.Status + "'" + "\r\n" +

                    ")";

                List<SC_mesStationConfigSetting> List_Detail = new List<SC_mesStationConfigSetting>();
                string S_SqlDetasil = "select Name DetailName, [Description] DetailDescription," +
                    " StationID, [Value] DetailValue from mesStationConfigSetting where StationID='" + v_SC_mesStation.ID + "'";
                var v_QueryDetail = await DapperConnRead2.QueryAsync<SC_mesStationConfigSetting>(S_SqlDetasil);
                List_Detail = v_QueryDetail.ToList();

                foreach (var item in List_Detail)
                {
                    S_Sql +=
                        "insert into mesStationConfigSetting(Name, [Description], StationID, [Value]) Values(" + "\r\n" +
                        "'" + item.DetailName + "'" + "\r\n" +
                        ",'" + item.DetailDescription + "'" + "\r\n" +
                        ",'" + S_MaxID + "'" + "\r\n" +
                        ",'" + item.DetailValue + "'" + "\r\n" +
                        ")" + "\r\n";
                }
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStation_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesStation_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesStation.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesStation", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesStationALLDto>> FindWithPagerMyAsync(SC_mesStationSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.Description = search.Description ?? "";
            search.StationType = search.StationType ?? "";
            search.LineID = search.LineID ?? "";
            search.Line = search.Line ?? "";
            search.Status = search.StatusValue ?? "";

            search.DetailName = search.DetailName ?? "";
            search.DetailDescription = search.DetailDescription ?? "";
            search.DetailValue = search.DetailValue ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesStationALLDto> List_Result = new List<SC_mesStationALLDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesStation A
                   LEFT JOIN mesStationType B ON A.StationTypeID=B.ID
                   LEFT JOIN mesLine  C ON A.LineID=C.ID
                   LEFT JOIN sysStatus  D ON A.[Status]=D.ID
                   LEFT JOIN mesStationConfigSetting E ON A.ID=E.StationID           
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )

                   and ('" + search.StationTypeID + @"'='' or A.StationTypeID  IN (SELECT value FROM dbo.F_Split('" + search.StationTypeID + @"',',')))
                   --and ('" + search.StationType + @"'='' or B.Description Like '%" + search.StationType + @"%' )

                   and ('" + search.LineID + @"'='' or A.LineID  IN (SELECT value FROM dbo.F_Split('" + search.LineID + @"',',')))
                   --and ('" + search.Line + @"'='' or C.Description Like '%" + search.Line + @"%' )

                   and ('" + search.Status + @"'='' or A.Status  IN (SELECT value FROM dbo.F_Split('" + search.Status + @"',',')))
                   --and ('" + search.StatusValue + @"'='' or D.Description Like '%" + search.StatusValue + @"%' )

                   and ('" + search.DetailName + @"'='' or E.Name Like '%" + search.DetailName + @"%' )
                   and ('" + search.DetailDescription + @"'='' or E.Description Like '%" + search.DetailDescription + @"%' )
                   and ('" + search.DetailValue + @"'='' or E.Value Like '%" + search.DetailValue + @"%' )

                  
                   and ( '" + search.LikeQuery + @"'='' or A.Description Like '%" + search.LikeQuery + @"%'
                            or B.Description Like '%" + search.LikeQuery + @"%'                            
                            or C.Description Like '%" + search.LikeQuery + @"%'
                            or D.Description Like '%" + search.LikeQuery + @"%'

                            or E.Name Like '%" + search.LikeQuery + @"%'
                            or E.Description Like '%" + search.LikeQuery + @"%'
                            or E.Value Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.ID,A.[Description], A.StationTypeID,B.[Description] StationType,
                          A.LineID,C.[Description] Line, A.[Status],D.[Description] StatusValue  
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
                       LEFT JOIN mesStationType B ON A.StationTypeID = B.ID
                       LEFT JOIN mesLine C ON A.LineID = C.ID
                       LEFT JOIN sysStatus D ON A.[Status]= D.ID
                    "

                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesStationALLDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesStationALLDto> list = v_query.ToList();

                foreach (var MainData in list)
                {
                    List<SC_mesStationConfigSetting> Children = new List<SC_mesStationConfigSetting>();
                    SC_mesStationALLDto v_SC_mesStationALLDto = new SC_mesStationALLDto();

                    v_SC_mesStationALLDto = MainData;

                    string S_SqlDetail =
                     @"
                     if exists(select 1 from mesStation where Description like '%" + search.LikeQuery + @"%' and id='" + MainData.ID + @"')
                     begin
                         SELECT ID,Name DetailName, [Description] DetailDescription, StationID, [Value] DetailValue
                          FROM mesStationConfigSetting D1
                         WHERE 1=1 and StationID='" + MainData.ID + @"'
                            and ('" + search.DetailDescription + @"'='' or D1.Description Like '%" + search.DetailDescription + @"%' )
                            and ('" + search.DetailName + @"'='' or D1.Name Like '%" + search.DetailName + @"%' )
                            and ('" + search.DetailValue + @"'='' or D1.Value Like '%" + search.DetailValue + @"%' )
 
                           and ('" + search.LikeQuery + @"'='' or D1.Description Like '%" + search.LikeQuery + @"%'
                                    or D1.Name Like '%" + search.LikeQuery + @"%'
                                    or D1.Value Like '%" + search.LikeQuery + @"%'
                                )
                     end
                     else
                     begin
                         SELECT ID,Name DetailName, [Description] DetailDescription, StationID, [Value] DetailValue
                          FROM mesStationConfigSetting D1
                         WHERE 1=1 and StationID='" + MainData.ID + @"'
                            and ('" + search.DetailDescription + @"'='' or D1.Description Like '%" + search.DetailDescription + @"%' )
                            and ('" + search.DetailName + @"'='' or D1.Name Like '%" + search.DetailName + @"%' )
                            and ('" + search.DetailValue + @"'='' or D1.Value Like '%" + search.DetailValue + @"%' )
                     end
                    ";
                    var list_detail = await DapperConnRead2.QueryAsync<SC_mesStationConfigSetting>(S_SqlDetail, null, null, I_DBTimeout, null);
                    Children = list_detail.ToList();

                    v_SC_mesStationALLDto.Children = Children;
                    List_Result.Add(v_SC_mesStationALLDto);
                }

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

        ////////////////////////////////////////////// Detail ///////////////////////////////////////////////////////////////////////

        public async Task<string> InsertDetail(string ParentId, SC_mesStationConfigSetting v_Detail, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesStationConfigSetting where StationID='" + ParentId + "'" +
                                " and NAME='" + v_Detail.DetailName + "'"+
                                //" and Description='" + v_Detail.DetailDescription + "'" +
                                " and Value='" + v_Detail.DetailValue + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationConfigSetting>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                S_Sql =
                        "insert into mesStationConfigSetting(Name, [Description], StationID, [Value]) Values(" + "\r\n" +
                        "'" + v_Detail.DetailName + "'" + "\r\n" +
                        ",'" + v_Detail.DetailDescription + "'" + "\r\n" +
                        ",'" + ParentId + "'" + "\r\n" +
                        ",'" + v_Detail.DetailValue + "'" + "\r\n" +
                        ")" + "\r\n";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result=I_Result.ToString();

                string S_MSG = v_Detail.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStationConfigSetting", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> DeleteDetail(string Id, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                SC_mesStationConfigSetting v_DetailDto = new SC_mesStationConfigSetting();
                string S_Sql = "select * from mesStationConfigSetting where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesStationConfigSetting>(S_Sql);

                S_Sql = "delete  mesStationConfigSetting where Id='" + Id + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStationConfigSetting", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> UpdateDetail(SC_mesStationConfigSetting v_Detail, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesStationConfigSetting where StationID='" + v_Detail.StationID + "'" +
                                " and NAME='" + v_Detail.DetailName + "'" +
                                //" and Description='" + v_Detail.DetailDescription + "'" +
                                " and Value='" + v_Detail.DetailValue + "'"+
                                " and ID<>'" + v_Detail.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationConfigSetting>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesStationConfigSetting v_Detail_before = new SC_mesStationConfigSetting();
                S_Sql = "select * from mesStationConfigSetting where Id='" + v_Detail.ID + "'";
                v_Detail_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationConfigSetting>(S_Sql);

                S_Sql = "Update mesStationConfigSetting set " +
                        "Description='" + v_Detail.DetailDescription + "'" +
                        " ,Name='" + v_Detail.DetailName + "'" +
                        " ,Value='" + v_Detail.DetailValue + "'" +
                        " ,StationID='" + v_Detail.StationID + "'" +
                        " where Id='" + v_Detail.ID + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_Detail_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_Detail_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_Detail.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStationConfigSetting", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

    }
}