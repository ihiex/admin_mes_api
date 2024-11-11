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
    public class SC_mesLineRepository : BaseRepositoryReport<string>, ISC_mesLineRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesLineRepository()
        {
        }
        public SC_mesLineRepository(IDbContextCore context) : base(context)
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

            string S_Sql= "select Lastname+'.'+Firstname ValStr1  from mesEmployee where ID='" + I_EmployeeID + "'";
            var v_Query_Employee =await DapperConnRead2.QueryFirstAsync<TabVal> (S_Sql, null, null, I_DBTimeout, null);
            if (v_Query_Employee != null) 
            {
                List_Login.UserName = v_Query_Employee.ValStr1;
            }


            if (P_PublicSCRepository == null)
            {
                P_PublicSCRepository = new PublicSCRepository(DB_Context,I_Language);
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
    
        public async Task<string> Insert(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLineDto.Location= v_SC_mesLineDto.Location ?? "";
            if (v_SC_mesLineDto.ServerID == null) { v_SC_mesLineDto.ServerID = 0; }

            try
            {
                string S_Sql = "select * from mesLine where Description='" + v_SC_mesLineDto.Description.Trim() + "'";
                var v_Exists=await DapperConnRead2.QueryAsync<SC_mesLineDto>(S_Sql);
                if (v_Exists.Count()>0) 
                {
                    return S_Result = "Line:"+v_SC_mesLineDto.Description+ " Data already exists.";
                }


                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesLine";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                v_SC_mesLineDto.ID = v_MaxID.LastID.ToString();

                S_Sql = "insert into mesLine(Id,Description,Location,ServerID,StatusID) Values(" +
                     "'" + v_SC_mesLineDto.ID + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.Description.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.Location + "'" + "\r\n" +
                      ",'" + v_SC_mesLineDto.ServerID + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.StatusID + "'"+"\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result = I_Result.ToString();
            
                string S_MSG= v_SC_mesLineDto.ToJson();
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
                SC_mesLineDto v_SC_mesLineDto = new SC_mesLineDto();
                string S_Sql = "select * from mesLine where Id='" + Id + "'";
                v_SC_mesLineDto=await DapperConnRead2.QueryFirstAsync<SC_mesLineDto>(S_Sql);

                List<SC_mesLineDetailDto> List_DetailDto = new List<SC_mesLineDetailDto>(); 
                S_Sql = "select * from mesLineDetail where LineID='" + Id + "'";
                var q_Query_Detail = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_Sql);
                List_DetailDto = q_Query_Detail.ToList();


                S_Sql = "delete mesLine where Id='"+Id+"' "+"\r\n"+
                        "delete mesLineDetail where LineID='"+Id+"'"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesLineDto != null)
                {            
                    string S_MSG = v_SC_mesLineDto.ToJson()+"\r\n"+
                           List_DetailDto.ToJson()
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesLine", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLineDto.Location = v_SC_mesLineDto.Location ?? "";
            if (v_SC_mesLineDto.ServerID == null) { v_SC_mesLineDto.ServerID = 0; }

            try
            {
                string S_Sql = "select * from mesLine where Description='" + v_SC_mesLineDto.Description.Trim() + "'"+
                                " and ID<>'"+ v_SC_mesLineDto.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineDto>(S_Sql);
                if (v_Exists.Count()>0)
                {
                    return S_Result = "Line:" + v_SC_mesLineDto.Description + " Data already exists.";
                }


                SC_mesLineDto v_mesLineDto_before = new SC_mesLineDto();
                S_Sql = "select * from mesLine where Id='" + v_SC_mesLineDto.ID + "'";
                v_mesLineDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLineDto>(S_Sql);
                
                S_Sql = "Update mesLine set " +
                        " Description='" + v_SC_mesLineDto.Description + "'" +
                        " ,ServerID='" + v_SC_mesLineDto.ServerID + "'" +
                        " ,Location='" + v_SC_mesLineDto.Location + "'" +
                        " ,StatusID='" + v_SC_mesLineDto.StatusID + "'" +
                        " where Id='" + v_SC_mesLineDto.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result = I_Result.ToString();

                if (v_mesLineDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesLineDto_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesLineDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesLine", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLineDto.Location = v_SC_mesLineDto.Location ?? "";
            if (v_SC_mesLineDto.ServerID == null) { v_SC_mesLineDto.ServerID = 0; }

            try
            {
                string S_Sql = "select * from mesLine where Description='" + v_SC_mesLineDto.Description.Trim() + "'"; 
                    
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = "Line:" + v_SC_mesLineDto.Description + " Data already exists.";
                }


                SC_mesLineDto v_mesLineDto_before = new SC_mesLineDto();
                S_Sql = "select * from mesLine where Id='" + v_SC_mesLineDto.ID + "'";
                v_mesLineDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLineDto>(S_Sql);

                S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesLine";
                MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                
                S_Sql =
                    "insert into mesLine(Id,Description,Location,ServerID,StatusID) Values(" +"\r\n"+
                     "'" + v_MaxID.LastID.ToString() + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.Description.Trim() + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.Location + "'" + "\r\n" +
                      ",'" + v_SC_mesLineDto.ServerID + "'" + "\r\n" +
                     ",'" + v_SC_mesLineDto.StatusID + "'" + "\r\n" +
                    ")" + "\r\n"
                    ;

                List<SC_mesLineDetailDto> List_DetailDto = new List<SC_mesLineDetailDto>();
                string S_SqlDetasil = "select * from mesLineDetail where LineID='" + v_SC_mesLineDto.ID + "'";
                var v_QueryDetail = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_SqlDetasil);
                List_DetailDto = v_QueryDetail.ToList();

                foreach (var item in List_DetailDto) 
                {
                    S_Sql += 
                        "insert into mesLineDetail(LineID,LineTypeDefID,Content) Values(" + "\r\n" +
                        "'" + v_MaxID.LastID.ToString() + "'" + "\r\n" +
                        ",'" + item.LineTypeDefID + "'" + "\r\n" +
                        ",'" + item.Content + "'" + "\r\n" +
                        ")" + "\r\n" ;
                }
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesLineDto_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesLineDto_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesLineDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesLine", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesLineALLDto>> FindWithPagerMyAsync(SC_mesLineSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.Description = search.Description ?? "";
            search.StatusValue = search.StatusValue ?? "";
            search.LineTypeDefID = search.LineTypeDefID ?? "";
            search.LineTypeDefValue = search.LineTypeDefValue ?? "";
            search.Content = search.Content ?? "";
            search.StatusID = search.StatusID ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesLineALLDto> List_Result = new List<SC_mesLineALLDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count= "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
	            FROM mesLine A
		            left join mesLineDetail B on A.id=B.LineID
		            left JOIN luLineTypeDef C on B.LineTypeDefID=C.ID  
		            LEFT JOIN sysStatus D ON A.StatusID=D.ID              
	            where 1=1
                   and ('"+ search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('"+ search.ID + @"',',')))
                   and ('"+ search.LineTypeDefID + @"'='' or C.ID  IN (SELECT value FROM dbo.F_Split('"+ search.LineTypeDefID + @"',',')))
                   and ('"+ search.StatusID + @"'='' or A.StatusID  IN (SELECT value FROM dbo.F_Split('" + search.StatusID + @"',',')))
                   and ('"+ search.Content + @"'='' or B.Content Like '%" + search.Content +@"%' )
                   and ('"+ search.LineTypeDefValue + @"'='' or C.Description  Like '%" + search.LineTypeDefValue + @"%'  )
                   and ('"+ search.Description + @"'='' or A.Description Like '%" + search.Description +@"%' )

                   and ( '"+ search.LikeQuery + @"'='' or A.Description Like '%" + search.LikeQuery + @"%'
                            or C.Description Like '%" + search.LikeQuery + @"%'
                            or B.Content Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.*,B.[Description]  StatusValue FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only "+"\r\n"+
                    ")A JOIN sysStatus B ON A.StatusID = B.ID"

                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesLineALLDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesLineALLDto>  list = v_query.ToList();

                foreach(var MainData in list) 
                {
                    List<SC_mesLineDetailDto> Children = new List<SC_mesLineDetailDto>();
                    SC_mesLineALLDto v_SC_mesLineALLDto=new SC_mesLineALLDto ();
                    
                    v_SC_mesLineALLDto = MainData;

                   string  S_SqlDetail =
                    @"
                     if exists(select 1 from mesLine where Description like '%"+ search.LikeQuery + @"%' and id='" + MainData.ID + @"')
                     begin
                         SELECT D1.ID,D1.LineID,D1.LineTypeDefID, D2.[Description] LineTypeDefValue,D1.[Content] FROM mesLineDetail D1 
                           Left JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID
                         WHERE 1=1 and LineID='" + MainData.ID + @"'
                            and ('"+ search.Content + @"'='' or D1.Content Like '%" + search.Content +@"%' )
                            and ('"+ search.LineTypeDefValue + @"'='' or D2.Description Like '%" + search.LineTypeDefValue + @"%' )
                            and ('"+ search.LineTypeDefID + @"'='' or D1.LineTypeDefID  IN (SELECT value FROM dbo.F_Split('" + search.LineTypeDefID + @"',',')))
                     end
                     else
                     begin
                         SELECT D1.ID,D1.LineID,D1.LineTypeDefID, D2.[Description] LineTypeDefValue,D1.[Content] FROM mesLineDetail D1 
                           Left JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID
                         WHERE 1=1 and LineID='" + MainData.ID + @"'
                            and ('"+ search.Content + @"'='' or D1.Content Like '%" + search.Content +@"%' )
                            and ('"+ search.LineTypeDefValue + @"'='' or D2.Description Like '%" + search.LineTypeDefValue + @"%' )
                            and ('"+ search.LineTypeDefID + @"'='' or D1.LineTypeDefID  IN (SELECT value FROM dbo.F_Split('" + search.LineTypeDefID + @"',',')))

                           and ('"+ search.LikeQuery + @"'='' or D1.Content Like '%" + search.LikeQuery + @"%'
                                    or D2.Description Like '%" + search.LikeQuery + @"%'
                                )
                     end
                    ";
                    var list_detail =await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_SqlDetail, null, null, I_DBTimeout, null);
                    Children= list_detail.ToList();

                    v_SC_mesLineALLDto.Children= Children;
                    List_Result.Add(v_SC_mesLineALLDto);
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

        //public async Task<List<SC_mesLineALLDto>> FindWithPagerMyAsync(string S_SqlPage, string S_SqlCount, PagerInfo info)
        //{
        //    List<SC_mesLineALLDto> List_Result = new List<SC_mesLineALLDto>();
        //    try
        //    {
        //        string S_Sql =
        //                @"if OBJECT_ID('tempdb..#Tab1') is not null 
        //                begin
	       //                 DROP TABLE #Tab1
        //                end " +
        //                S_SqlPage + " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
        //                ") rows fetch next " + info.PageSize + " rows only ";

        //        var v_query = await DapperConnRead2.QueryAsync<SC_mesLineALLDto>(S_Sql, null, null, I_DBTimeout, null);
        //        List<SC_mesLineALLDto> list = v_query.ToList();

        //        foreach (var MainData in list)
        //        {
        //            List<SC_mesLineDetailDto> Children = new List<SC_mesLineDetailDto>();
        //            SC_mesLineALLDto v_SC_mesLineALLDto = new SC_mesLineALLDto();

        //            v_SC_mesLineALLDto = MainData;

        //            S_Sql =
        //            @"
        //             SELECT D1.ID,D1.LineID,D1.LineTypeDefID, D2.[Description] LineTypeDefValue,D1.[Content] FROM mesLineDetail D1 
        //                JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID
        //             WHERE 1=1 and LineID='" + MainData.ID + @"'
        //            ";
        //            var list_detail = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_Sql, null, null, I_DBTimeout, null);
        //            Children = list_detail.ToList();

        //            v_SC_mesLineALLDto.Children = Children;
        //            List_Result.Add(v_SC_mesLineALLDto);
        //        }

        //        RefAsync<int> totalCount = 0;
        //        //S_Sql = "select Count(*) Valint1  from mesLine Where 1=1";
        //        S_SqlCount =
        //        @"if OBJECT_ID('tempdb..#Tab1') is not null 
        //        begin
	       //         DROP TABLE #Tab1
        //        end " +
        //        S_SqlCount;
        //        var v_Count = await DapperConnRead2.QueryAsync<TabVal>(S_SqlCount, null, null, I_DBTimeout, null);
        //        List<TabVal> List_TabVal = v_Count.ToList();
        //        totalCount = List_TabVal[0].Valint1;
        //        info.RecordCount = totalCount;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    return List_Result;
        //}



        ////////////////////////////////////////////// Detail ///////////////////////////////////////////////////////////////////////

        public async Task<string> InsertDetail(string ParentId, SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                //string S_Sql = "select * from mesLineDetail where LineID='" + ParentId + "'"+
                //                " and LineTypeDefID='"+ v_DetailDto.LineTypeDefID + "'" +
                //                " and Content='" + v_DetailDto.Content + "'";                                
                //var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_Sql);
                //if (v_Exists.Count()>0)
                //{
                //    return S_Result = " Data already exists.";
                //}

                string
                S_Sql = "insert into mesLineDetail(LineID,LineTypeDefID,Content) Values(" +"\r\n"+
                    "'"+ParentId+"'"+ "\r\n" +
                    ",'" + v_DetailDto.LineTypeDefID + "'" + "\r\n" +
                    ",'" + v_DetailDto.Content + "'"+ "\r\n" +
                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result=I_Result.ToString();

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesLineDetail", S_MSG);
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
                SC_mesLineDetailDto v_DetailDto = new SC_mesLineDetailDto();
                string S_Sql = "select * from mesLineDetail where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesLineDetailDto>(S_Sql);

                S_Sql = "delete  mesLineDetail where Id='" + Id + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesLineDetail", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> UpdateDetail(SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "";
                //string S_Sql = "select * from mesLineDetail where LineID='" + v_DetailDto.LineID + "'" +
                //               " and LineTypeDefID='" + v_DetailDto.LineTypeDefID + "'" +
                //               " and Content='" + v_DetailDto.Content + "'"+
                //               " and ID<>'" + v_DetailDto.ID + "'";
                //var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_Sql);
                //if (v_Exists.Count()>0)
                //{
                //    return S_Result = " Data already exists.";
                //}

                SC_mesLineDetailDto v_DetailDto_before = new SC_mesLineDetailDto();
                S_Sql = "select * from mesLineDetail where Id='" + v_DetailDto.ID + "'";
                v_DetailDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLineDetailDto>(S_Sql);
                
                S_Sql = "Update mesLineDetail set " +
                        " LineID='" + v_DetailDto.LineID + "'" +
                        " ,LineTypeDefID='" + v_DetailDto.LineTypeDefID + "'" +
                        " ,Content='" + v_DetailDto.Content + "'" +
                        " where Id='" + v_DetailDto.ID + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_DetailDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_DetailDto_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_DetailDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesLine", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<SC_mesLineDetailList> List_Detail(string ParentId) 
        {
            SC_mesLineDetailList v_SC_mesLineDetailList = new SC_mesLineDetailList();
            v_SC_mesLineDetailList.MSG = "OK";
            try
            {
                string S_Sql = @"SELECT A.*,B.[Description] LineTypeDefValue FROM  mesLineDetail A  
                                    JOIN luLineTypeDef B ON A.LineTypeDefID=B.ID where A.LineID='" + ParentId + "'";
                var v_Detail = await DapperConnRead2.QueryAsync<SC_mesLineDetailDto>(S_Sql);
                v_SC_mesLineDetailList.List_mesLineDetail = v_Detail.ToList();
            }
            catch (Exception ex) 
            {
                v_SC_mesLineDetailList.MSG = ex.ToString();
            }

            return v_SC_mesLineDetailList;
        }


    }
}