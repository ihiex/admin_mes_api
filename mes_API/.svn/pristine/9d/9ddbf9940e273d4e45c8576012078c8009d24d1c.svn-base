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
    public class SC_mesStationTypeRepository : BaseRepositoryReport<string>, ISC_mesStationTypeRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesStationTypeRepository()
        {
        }
        public SC_mesStationTypeRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesStationType v_SC_mesStationTypeDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationTypeDto.ApplicationTypeID = v_SC_mesStationTypeDto.ApplicationTypeID ?? "";
            v_SC_mesStationTypeDto.Description = v_SC_mesStationTypeDto.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStationType where Description='" + v_SC_mesStationTypeDto.Description.Trim() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesStationTypeDto.Description + " Data already exists.";
                }


                //S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesStationType";
                //MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                //v_SC_mesStationTypeDto.ID = v_MaxID.LastID.ToString();

                S_Sql = "insert into mesStationType(Description,ApplicationTypeID) Values(" +                    
                     "'" + v_SC_mesStationTypeDto.Description.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesStationTypeDto.ApplicationTypeID + "'" + "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result = I_Result.ToString();

                string S_MSG = v_SC_mesStationTypeDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStationType", S_MSG);
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
                SC_mesStationType v_SC_mesStationType = new SC_mesStationType();
                string S_Sql = "select * from mesStationType where Id='" + Id + "'";
                v_SC_mesStationType = await DapperConnRead2.QueryFirstAsync<SC_mesStationType>(S_Sql);

                List<SC_mesStationTypeDetail> List_DetailDto = new List<SC_mesStationTypeDetail>();
                S_Sql = "select * from mesStationTypeDetail where StationTypeID='" + Id + "'";
                var q_Query_Detail = await DapperConnRead2.QueryAsync<SC_mesStationTypeDetail>(S_Sql);
                List_DetailDto = q_Query_Detail.ToList();


                S_Sql = "delete mesStationType where Id='" + Id + "' " + "\r\n" +
                        "delete mesStationTypeDetail where StationTypeID='" + Id + "'"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesStationType != null)
                {
                    string S_MSG = v_SC_mesStationType.ToJson() + "\r\n" +
                           List_DetailDto.ToJson()
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStationType", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesStationType v_SC_mesStationType, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationType.ApplicationTypeID = v_SC_mesStationType.ApplicationTypeID ?? "";
            v_SC_mesStationType.Description = v_SC_mesStationType.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStationType where Description='" + v_SC_mesStationType.Description.Trim() + "'" +
                                " and ID<>'" + v_SC_mesStationType.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationType>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesStationType.Description + " Data already exists.";
                }


                SC_mesStationType v_mesStationType_before = new SC_mesStationType();
                S_Sql = "select * from mesStationType where Id='" + v_SC_mesStationType.ID + "'";
                v_mesStationType_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationType>(S_Sql);

                S_Sql = "Update mesStationType set " +
                        " Description='" + v_SC_mesStationType.Description + "'" +

                        " ,ApplicationTypeID='" + v_SC_mesStationType.ApplicationTypeID + "'" +
   
                        " where Id='" + v_SC_mesStationType.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result = I_Result.ToString();

                if (v_mesStationType_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesStationType_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesStationType.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStationType", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesStationType v_SC_mesStationType, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesStationType.ApplicationTypeID = v_SC_mesStationType.ApplicationTypeID ?? "";
            v_SC_mesStationType.Description = v_SC_mesStationType.Description ?? "";

            try
            {
                string S_Sql = "select * from mesStationType where Description='" + v_SC_mesStationType.Description.Trim() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesStationType.Description + " Data already exists.";
                }


                SC_mesStationType v_mesStationType_before = new SC_mesStationType();
                S_Sql = "select * from mesStationType where Id='" + v_SC_mesStationType.ID + "'";
                v_mesStationType_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationType>(S_Sql);

                //S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesStationType";
                //MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);

                S_Sql = "DECLARE @ParentID INT "+"\r\n"+
                    "insert into mesStationType(Description,ApplicationTypeID) Values(" +
                     " '" + v_SC_mesStationType.Description.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesStationType.ApplicationTypeID + "'" + "\r\n" +
                    ")  SELECT @ParentID=@@IDENTITY FROM mesStationType" + "\r\n"
                    ;

                List<SC_mesStationTypeDetail> List_Detail = new List<SC_mesStationTypeDetail>();
                string S_SqlDetasil = "select * from mesStationTypeDetail where StationTypeID='" + v_SC_mesStationType.ID + "'";
                var v_QueryDetail = await DapperConnRead2.QueryAsync<SC_mesStationTypeDetail>(S_SqlDetasil);
                List_Detail = v_QueryDetail.ToList();

                foreach (var item in List_Detail)
                {
                    S_Sql +=                        
                        "insert into mesStationTypeDetail(StationTypeID,StationTypeDetailDefID,Content,Description) Values(" + "\r\n" +
                        "@ParentID" + "\r\n" +
                        ",'" + item.StationTypeDetailDefID + "'" + "\r\n" +
                        ",'" + item.Content + "'" + "\r\n" +
                        ",'" + item.Description + "'" + "\r\n" +
                        ")" + "\r\n";
                }
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesStationType_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesStationType_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesStationType.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesStationType", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesStationTypeALLDto>> FindWithPagerMyAsync(SC_mesStationTypeSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.Description = search.Description ?? "";
            search.ApplicationTypeID = search.ApplicationTypeID ?? "";
            search.ApplicationType = search.ApplicationType ?? "";
            //search.StationTypeID = search.StationTypeID ?? "";
            //search.StationType = search.StationType ?? "";
            search.StationTypeDetailDefID = search.StationTypeDetailDefID ?? "";
            search.StationTypeDetailDef = search.StationTypeDetailDefID ?? "";
            search.Content = search.Content ?? "";
            search.DetailDescription = search.DetailDescription ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesStationTypeALLDto> List_Result = new List<SC_mesStationTypeALLDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesStationType A
                   LEFT JOIN  mesStationTypeDetail B ON  A.ID=B.StationTypeID
                   LEFT JOIN luStationTypeDetailDef C ON B.StationTypeDetailDefID=C.ID
                   LEFT JOIN luApplicationType D ON A.ApplicationTypeID=D.ID            
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )

                   and ('" + search.ApplicationTypeID + @"'='' or A.ApplicationTypeID  IN (SELECT value FROM dbo.F_Split('" + search.ApplicationTypeID + @"',',')))
                   --and ('" + search.ApplicationType + @"'='' or D.Description Like '%" + search.ApplicationType + @"%' )

                   and ('" + search.StationTypeDetailDefID + @"'='' or B.StationTypeDetailDefID  IN (SELECT value FROM dbo.F_Split('" + search.StationTypeDetailDefID + @"',',')))
                   --and ('" + search.StationTypeDetailDef + @"'='' or C.Description Like '%" + search.StationTypeDetailDef + @"%' )

                   and ('" + search.Content + @"'='' or B.Content Like '%" + search.Content + @"%' )
                   and ('" + search.DetailDescription + @"'='' or B.Description Like '%" + search.DetailDescription + @"%' )

                  
                   and ( '" + search.LikeQuery + @"'='' or A.Description Like '%" + search.LikeQuery + @"%'
                            or B.Description Like '%" + search.LikeQuery + @"%'
                            or B.Content Like '%" + search.LikeQuery + @"%'
                            or C.Description Like '%" + search.LikeQuery + @"%'
                            or D.Description Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.*,B.[Description]  ApplicationType FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    ")A LEFT JOIN luApplicationType B ON A.ApplicationTypeID=B.ID"

                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesStationTypeALLDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesStationTypeALLDto> list = v_query.ToList();

                foreach (var MainData in list)
                {
                    List<SC_mesStationTypeDetailDto> Children = new List<SC_mesStationTypeDetailDto>();
                    SC_mesStationTypeALLDto v_SC_mesStationTypeALLDto = new SC_mesStationTypeALLDto();

                    v_SC_mesStationTypeALLDto = MainData;

                    string S_SqlDetail =
                     @"
                     if exists(select 1 from mesStationType where Description like '%" + search.LikeQuery + @"%' and id='" + MainData.ID + @"')
                     begin
                        SELECT D1.ID, D1.StationTypeDetailDefID,D2.[Description] StationTypeDetailDef,
                              D1.[Content],D1.[Description],D1.StationTypeID
                         FROM mesStationTypeDetail D1 
                             Left JOIN luStationTypeDetailDef D2 on D1.StationTypeDetailDefID=D2.ID 
                         WHERE 1=1 and StationTypeID='" + MainData.ID + @"'
                            and ('" + search.Content + @"'='' or D1.Content Like '%" + search.Content + @"%' )
                            --and ('" + search.StationTypeDetailDef + @"'='' or D2.Description Like '%" + search.StationTypeDetailDef + @"%' )
                            and ('" + search.StationTypeDetailDefID + @"'='' or D1.StationTypeDetailDefID  IN (SELECT value FROM dbo.F_Split('" + search.StationTypeDetailDefID + @"',',')))
                     end
                     else
                     begin
                        SELECT D1.ID, D1.StationTypeDetailDefID,D2.[Description] StationTypeDetailDef,
                              D1.[Content],D1.[Description],D1.StationTypeID
                         FROM mesStationTypeDetail D1 
                             Left JOIN luStationTypeDetailDef D2 on D1.StationTypeDetailDefID=D2.ID 
                         WHERE 1=1 and StationTypeID='" + MainData.ID + @"'
                            and ('" + search.Content + @"'='' or D1.Content Like '%" + search.Content + @"%' )
                            --and ('" + search.StationTypeDetailDef + @"'='' or D2.Description Like '%" + search.StationTypeDetailDef + @"%' )
                            and ('" + search.StationTypeDetailDefID + @"'='' or D1.StationTypeDetailDefID  IN (SELECT value FROM dbo.F_Split('" + search.StationTypeDetailDefID + @"',',')))

                           and ('" + search.LikeQuery + @"'='' or D1.Content Like '%" + search.LikeQuery + @"%'
                                    or D1.Description Like '%" + search.LikeQuery + @"%'
                                    or D2.Description Like '%" + search.LikeQuery + @"%'
                                )
                     end
                    ";
                    var list_detail = await DapperConnRead2.QueryAsync<SC_mesStationTypeDetailDto>(S_SqlDetail, null, null, I_DBTimeout, null);
                    Children = list_detail.ToList();

                    v_SC_mesStationTypeALLDto.Children = Children;
                    List_Result.Add(v_SC_mesStationTypeALLDto);
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

        public async Task<string> InsertDetail(string ParentId, SC_mesStationTypeDetail v_Detail, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesStationTypeDetail where StationTypeID='" + ParentId + "'" +
                                " and StationTypeDetailDefID='" + v_Detail.StationTypeDetailDefID + "'";
                                //" and Description='" + v_Detail.Description + "'" +
                                //" and Content='" + v_Detail.Content + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeDetailDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                S_Sql =
                    "insert into mesStationTypeDetail(StationTypeID,StationTypeDetailDefID,Content,Description) Values(" + "\r\n" +
                    "'" + ParentId + "'" + "\r\n" +
                    ",'" + v_Detail.StationTypeDetailDefID + "'" + "\r\n" +
                    ",'" + v_Detail.Content + "'" + "\r\n" +
                    ",'" + v_Detail.Description + "'" + "\r\n" +
                    ")" + "\r\n";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result=I_Result.ToString();

                string S_MSG = v_Detail.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesStationTypeDetail", S_MSG);
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
                SC_mesStationTypeDetailDto v_DetailDto = new SC_mesStationTypeDetailDto();
                string S_Sql = "select * from mesStationTypeDetail where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeDetailDto>(S_Sql);

                S_Sql = "delete  mesStationTypeDetail where Id='" + Id + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesStationTypeDetail", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> UpdateDetail(SC_mesStationTypeDetail v_Detail, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesStationTypeDetail where StationTypeID='" + v_Detail.StationTypeID + "'" +
                                " and StationTypeDetailDefID='" + v_Detail.StationTypeDetailDefID + "'" +
                                //" and Description='" + v_Detail.Description + "'" +
                                //" and Content='" + v_Detail.Content + "'"+
                                " and ID<>'" + v_Detail.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesStationTypeDetail>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                SC_mesStationTypeDetail v_Detail_before = new SC_mesStationTypeDetail();
                S_Sql = "select * from mesStationTypeDetail where Id='" + v_Detail.ID + "'";
                v_Detail_before = await DapperConnRead2.QueryFirstAsync<SC_mesStationTypeDetail>(S_Sql);

                S_Sql = "Update mesStationTypeDetail set " +
                       // " StationTypeID='" + v_Detail.StationTypeID + "'" +
                        " StationTypeDetailDefID='" + v_Detail.StationTypeDetailDefID + "'" +
                        " ,Content='" + v_Detail.Content + "'" +
                        " ,Description='" + v_Detail.Description + "'" +
                        " where Id='" + v_Detail.ID + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_Detail_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_Detail_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_Detail.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesStationType", S_MSG);
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