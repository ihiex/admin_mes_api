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
using System.Configuration;
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
    public class SC_mesSNFormatRepository : BaseRepositoryReport<string>, ISC_mesSNFormatRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesSNFormatRepository()
        {
        }
        public SC_mesSNFormatRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormat.Name = v_SC_mesSNFormat.Name ?? "";
            v_SC_mesSNFormat.Description = v_SC_mesSNFormat.Description ?? "";

            try
            {
                string S_Sql = "select * from mesSNFormat where Name='" + v_SC_mesSNFormat.Name.Trim() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormatDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesSNFormat.Name + " Data already exists.";
                }

                //S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesSNFormat";
                //MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                //v_SC_mesSNFormat.ID = v_MaxID.LastID.ToString();

                S_Sql = "insert into mesSNFormat(Name, [Description], SNFamilyID, Format) Values(" +
                     "'" + v_SC_mesSNFormat.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.SNFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.Format + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesSNFormat.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesSNFormat", S_MSG);
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
                SC_mesSNFormat v_SC_mesSNFormat = new SC_mesSNFormat();
                string S_Sql = "select * from mesSNFormat where Id='" + Id + "'";
                v_SC_mesSNFormat = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormat>(S_Sql);

                List<SC_mesSNSection> List_DetailDto = new List<SC_mesSNSection>();
                S_Sql = "select * from mesSNSection where SNFormatID='" + Id + "'";
                var q_Query_Detail = await DapperConnRead2.QueryAsync<SC_mesSNSection>(S_Sql);
                List_DetailDto = q_Query_Detail.ToList();

                S_Sql = "";                                               
                string S_MSG_mesSNRange = "";
                foreach (var item in List_DetailDto) 
                {
                    if (item.SectionType == "4")
                    {
                        try
                        {
                            List<SC_mesSNRange> List_mesSNRange = new List<SC_mesSNRange>();
                            string S_Sql_mesSNRange = "select * from mesSNRange where SNSectionID='" + item.ID + "'";
                            var v_mesSNRange = await DapperConnRead2.QueryFirstAsync<SC_mesSNRange>(S_Sql_mesSNRange);

                            S_MSG_mesSNRange += v_mesSNRange.ToJson() + "\r\n";
                        }
                        catch { }

                        S_Sql += " delete mesSNRange where SNSectionID='" + item.ID + "'"+"\r\n";
                    }
                }

                S_Sql = S_Sql + "\r\n" +
                    "delete mesSNSection where SNFormatID='" + Id + "'" + "\r\n"+
                    "delete mesSNFormat where Id='" + Id + "' ";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesSNFormat != null)
                {
                    string S_MSG = v_SC_mesSNFormat.ToJson() + "\r\n" +
                           List_DetailDto.ToJson()+ "\r\n" +
                           S_MSG_mesSNRange
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesSNFormat", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormat.Name = v_SC_mesSNFormat.Name ?? "";
            v_SC_mesSNFormat.Description = v_SC_mesSNFormat.Description ?? "";

            try
            {
                string S_Sql = "select * from mesSNFormat where Name='" + v_SC_mesSNFormat.Name.Trim() + "'" +
                                " and ID<>'" + v_SC_mesSNFormat.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormat>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesSNFormat.Name + " Data already exists.";
                }

                SC_mesSNFormat v_mesSNFormat_before = new SC_mesSNFormat();
                S_Sql = "select * from mesSNFormat where Id='" + v_SC_mesSNFormat.ID + "'";
                v_mesSNFormat_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormat>(S_Sql);

                S_Sql = "Update mesSNFormat set " +
                        " Name='" + v_SC_mesSNFormat.Name + "'" +
                        " ,Description='" + v_SC_mesSNFormat.Description + "'" +
                        " ,SNFamilyID='" + v_SC_mesSNFormat.SNFamilyID + "'" +
                        " ,Format='" + v_SC_mesSNFormat.Format + "'" +

                        " where Id='" + v_SC_mesSNFormat.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesSNFormat_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesSNFormat_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesSNFormat.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesSNFormat", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesSNFormat.Description = v_SC_mesSNFormat.Description ?? "";

            try
            {
                string S_Sql = "select * from mesSNFormat where Name='" + v_SC_mesSNFormat.Name.Trim() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesSNFormatDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesSNFormat.Name + " Data already exists.";
                }


                SC_mesSNFormat v_mesSNFormat_before = new SC_mesSNFormat();
                S_Sql = "select * from mesSNFormat where Id='" + v_SC_mesSNFormat.ID + "'";
                v_mesSNFormat_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNFormat>(S_Sql);

                //S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesSNFormat";
                //MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
                //string S_MaxID = v_MaxID.LastID.ToString();

                S_Sql = "DECLARE @SNFormatMaxID int " + "\r\n" +
                    "DECLARE @SNSectionMaxID int " + "\r\n" +
                    "insert into mesSNFormat(Name, [Description], SNFamilyID, Format) Values(" + "\r\n" +
                     "'" + v_SC_mesSNFormat.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.SNFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesSNFormat.Format + "'" + "\r\n" +

                    ")" + "\r\n" +
                    "SELECT @SNFormatMaxID=@@IDENTITY" + "\r\n"
                ;

                List<SC_mesSNSectionDto> List_Detail = new List<SC_mesSNSectionDto>();
                string S_SqlDetail = "select ID, SNFormatID, SectionType SectionTypeID, SectionParam, Increment, InvalidChar, LastUsed," +
                    " [Order], AllowReset from mesSNSection where SNFormatID='" + v_SC_mesSNFormat.ID + "'";
                var v_QueryDetail = await DapperConnRead2.QueryAsync<SC_mesSNSectionDto>(S_SqlDetail);
                List_Detail = v_QueryDetail.ToList();

                foreach (var item in List_Detail)
                {
                    S_Sql +=
                        "insert into mesSNSection(SNFormatID, SectionType, SectionParam," +
                            " Increment, InvalidChar, LastUsed,[Order], AllowReset) Values(" + "\r\n" +
                            "@SNFormatMaxID" + "\r\n" +
                            ",'" + item.SectionTypeID + "'" + "\r\n" +
                            ",'" + item.SectionParam + "'" + "\r\n" +
                            ",'" + item.Increment + "'" + "\r\n" +
                            ",'" + item.InvalidChar + "'" + "\r\n" +
                            ",'" + item.LastUsed + "'" + "\r\n" +
                            ",'" + item.Order + "'" + "\r\n" +
                            ",'" + item.AllowReset + "'" + "\r\n" +

                        ")" + "\r\n" +
                        "SELECT @SNSectionMaxID=@@IDENTITY" + "\r\n";

                    if (item.SectionTypeID == "4") 
                    {
                        try
                        {
                            string S_Sql_mesSNRange = "SELECT * FROM mesSNRange WHERE SNSectionID='" + item.ID + "'";
                            var v_mesSNRange= await DapperConnRead2.QueryAsync<SC_mesSNRange>(S_Sql_mesSNRange);
                            List<SC_mesSNRange> List_SNRange= v_mesSNRange.ToList();

                            foreach (var item_SNRange in List_SNRange)
                            {
                                S_Sql +=
                                    "insert into mesSNRange(SNSectionID, [Start], [End],[Order], StatusID) Values(" + "\r\n" +
                                    "@SNSectionMaxID" + "\r\n" +
                                    ",'" + item_SNRange.Start + "'" + "\r\n" +
                                    ",'" + item_SNRange.End + "'" + "\r\n" +
                                    ",'" + item_SNRange.Order + "'" + "\r\n" +
                                    ",'" + item_SNRange.StatusID + "'" + "\r\n" +

                                    ")" + "\r\n";
                            }
                        }
                        catch { }
                    }
                }
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesSNFormat_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesSNFormat_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesSNFormat.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesSNFormat", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesSNFormatALLDto>> FindWithPagerMyAsync(SC_mesSNFormatSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.Name = search.Name ?? "";
            search.Description = search.Description ?? "";
            search.SNFamilyID = search.SNFamilyID ?? "";
            search.SNFamily = search.SNFamily ?? "";

            search.SectionTypeID = search.SectionTypeID ?? "";
            search.SectionParam = search.SectionParam ?? "";
            search.Increment = search.Increment ?? "";
            search.InvalidChar = search.InvalidChar ?? "";
            search.LastUsed = search.LastUsed ?? "";

            search.AllowReset = search.AllowReset ?? false;
            if (search.AllowReset.ToString() == "") 
            {
                search.AllowReset = false;
            }

            search.LikeQuery = search.LikeQuery ?? "";

            string S_AllowReset = "0";
            if ( search.AllowReset.ToBool()==true) { S_AllowReset = "1"; }


            List<SC_mesSNFormatALLDto> List_Result = new List<SC_mesSNFormatALLDto>();
            try
            {
                string S_Page = "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";
                
                string S_Sql =
                @"
                FROM mesSNFormat A 
	                LEFT JOIN mesSNSection B ON A.ID=B.SNFormatID
	                LEFT JOIN luSNFamily C ON A.SNFamilyID=C.ID         
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.Name + @"'='' or A.Name  Like '%" + search.Name + @"%' )
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )
                   and ('" + search.SNFamilyID + @"'='' or A.SNFamilyID   IN (SELECT value FROM dbo.F_Split('" + search.SNFamilyID + @"',',')))

                   and ('" + search.SectionTypeID + @"'='' or B.SectionType   IN (SELECT value FROM dbo.F_Split('" + search.SectionTypeID + @"',',')))
                     
                   and ('" + search.SectionParam + @"'='' or B.SectionParam Like '%" + search.SectionParam + @"%' )
                   and ('" + search.Increment + @"'='' or B.Increment Like '%" + search.Increment + @"%' )
                   and ('" + search.InvalidChar + @"'='' or B.InvalidChar Like '%" + search.InvalidChar + @"%' )
                   and ('" + search.LastUsed + @"'='' or B.LastUsed  Like '%" + search.LastUsed + @"%' )

                   and ('" + S_AllowReset + @"'='0' or B.AllowReset=" + S_AllowReset + @" )
                  
                   and ( '" + search.LikeQuery + @"'='' or A.Name Like '%" + search.LikeQuery + @"%'
                            or A.Description Like '%" + search.LikeQuery + @"%'                            
                            or A.SNFamilyID Like '%" + search.LikeQuery + @"%'
                            or C.Name Like '%" + search.LikeQuery + @"%'

                            or B.SectionType Like '%" + search.LikeQuery + @"%'
                            or B.SectionParam Like '%" + search.LikeQuery + @"%'
                            or B.Increment Like '%" + search.LikeQuery + @"%'
                            or B.InvalidChar Like '%" + search.LikeQuery + @"%'
                            or B.LastUsed Like '%" + search.LikeQuery + @"%'
                        )"
                    ;
                string S_SqlPage =
                    @"
                    SELECT A.ID, A.Name, A.[Description], A.SNFamilyID, A.[Format],C.Name SNFamily
                           --,B.SectionType, B.SectionParam, B.Increment, B.InvalidChar, B.LastUsed,
                           --B.[Order], B.AllowReset 
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
	                  --LEFT  JOIN mesSNSection B ON A.ID=B.SNFormatID
	                  LEFT  JOIN luSNFamily C ON A.SNFamilyID=C.ID
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesSNFormatALLDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesSNFormatALLDto> list = v_query.ToList();

                foreach (var MainData in list)
                {
                    List<SC_mesSNSectionDto> Children = new List<SC_mesSNSectionDto>();
                    SC_mesSNFormatALLDto v_SC_mesSNFormatALLDto = new SC_mesSNFormatALLDto();

                    v_SC_mesSNFormatALLDto = MainData;

                    string S_SqlDetail =
                     @"
                     if exists(select 1 from mesSNFormat where Name like '%" + search.LikeQuery + @"%' and id='" + MainData.ID + @"')
                     begin
                        SELECT D1.ID, SNFormatID,B.Name SNFormat, SectionType SectionTypeID, SectionParam, Increment, InvalidChar, LastUsed,
                               [Order], AllowReset,
                                   (CASE WHEN SectionType=1 THEN '固定值(Fixed)'
                                        WHEN SectionType=2 THEN '存储过程(Procedure)'
                                        WHEN SectionType=3 THEN '日期时间(DateTime)'
                                        WHEN SectionType=4 THEN '计数(Counter)'
                                    END) SectionType 
                          FROM mesSNSection D1 left join mesSNFormat B on D1.SNFormatID=B.ID
                         WHERE 1=1 and SNFormatID='" + MainData.ID + @"'
                            and ('" + search.SectionTypeID + @"'='' or D1.SectionType Like '%" + search.SectionTypeID + @"%' )
                            and ('" + search.SectionParam + @"'='' or D1.SectionParam Like '%" + search.SectionParam + @"%' )
                            and ('" + search.Increment + @"'='' or D1.Increment Like '%" + search.Increment + @"%' )
                            and ('" + search.InvalidChar + @"'='' or D1.InvalidChar Like '%" + search.InvalidChar + @"%' )
                            and ('" + search.LastUsed + @"'='' or D1.LastUsed Like '%" + search.LastUsed + @"%' )

 
                           --and ('" + search.LikeQuery + @"'='' or D1.SectionType Like '%" + search.LikeQuery + @"%'
                           --         or D1.SectionParam Like '%" + search.LikeQuery + @"%'
                           --         or D1.Increment Like '%" + search.LikeQuery + @"%'
                           --         or D1.InvalidChar Like '%" + search.LikeQuery + @"%'
                           --         or D1.LastUsed Like '%" + search.LikeQuery + @"%'
                           --     )
                           Order By [Order]
                     end
                     else
                     begin
                        SELECT D1.ID, SNFormatID,B.Name SNFormat, SectionType SectionTypeID, SectionParam, Increment, InvalidChar, LastUsed,
                               [Order], AllowReset,
                                   (CASE WHEN SectionType=1 THEN '固定值(Fixed)'
                                        WHEN SectionType=2 THEN '存储过程(Procedure)'
                                        WHEN SectionType=3 THEN '日期时间(DateTime)'
                                        WHEN SectionType=4 THEN '计数(Counter)'
                                    END) SectionType 
                          FROM mesSNSection D1 left join mesSNFormat B on D1.SNFormatID=B.ID
                         WHERE 1=1 and SNFormatID='" + MainData.ID + @"'
                            and ('" + search.SectionTypeID + @"'='' or D1.SectionType Like '%" + search.SectionTypeID + @"%' )
                            and ('" + search.SectionParam + @"'='' or D1.SectionParam Like '%" + search.SectionParam + @"%' )
                            and ('" + search.Increment + @"'='' or D1.Increment Like '%" + search.Increment + @"%' )
                            and ('" + search.InvalidChar + @"'='' or D1.InvalidChar Like '%" + search.InvalidChar + @"%' )
                            and ('" + search.LastUsed + @"'='' or D1.LastUsed Like '%" + search.LastUsed + @"%' )

                        Order By [Order]
                     end
                    ";
                    var list_detail = await DapperConnRead2.QueryAsync<SC_mesSNSectionDto>(S_SqlDetail, null, null, I_DBTimeout, null);                    
                    Children = list_detail.ToList();

                    for (int i=0; i< Children.Count; i++)
                    {
                        if (Children[i].SectionTypeID == "4")
                        {
                            try
                            {
                                List<SC_mesSNRangeDto> List_SC_mesSNRange = new List<SC_mesSNRangeDto>();
                                string S_SqlSNRange = @"select *,                                   
                                    (CASE WHEN StatusID=0 THEN '新增(New)'
                                        WHEN StatusID=1 THEN '运行中(Running)'
                                        WHEN StatusID=2 THEN '完成(Completed)'
                                        WHEN StatusID=3 THEN 'F-完成(F-Completed)'
                                    END) StatusValue from mesSNRange  
                                    where SNSectionID='" + Children[i].ID + "'";
                                var v_SC_mesSNRange = await DapperConnRead2.QueryAsync<SC_mesSNRangeDto>(S_SqlSNRange, null, null, I_DBTimeout, null);
                                List_SC_mesSNRange=v_SC_mesSNRange.ToList();

                                Children[i].List_SNRange = List_SC_mesSNRange;
                            }
                            catch { }
                        }
                    }
                   
                    v_SC_mesSNFormatALLDto.Children = Children;
                    List_Result.Add(v_SC_mesSNFormatALLDto);
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


        public async Task<string> InsertDetail(string ParentId, SC_mesSNSectionEdit v_DetailDto, IDbTransaction trans = null) 
        {
            string S_Result = "OK";
            List<SC_mesSNRange> List_SNRange = v_DetailDto.List_SNRange;

            v_DetailDto.Increment = v_DetailDto.Increment ?? "";
            v_DetailDto.InvalidChar = v_DetailDto.Increment ?? "";

            if (v_DetailDto.AllowReset.ToString() == "" || v_DetailDto.AllowReset.ToString() == "null")
            {
                v_DetailDto.AllowReset = false;
            }
            v_DetailDto.AllowReset = v_DetailDto.AllowReset ?? false;


            try
            {
                string
                S_Sql =
                    "DECLARE @SNSectionMaxID int" + "\r\n"+
                    "insert into mesSNSection(SNFormatID, SectionType, SectionParam," +
                        " Increment, InvalidChar, [Order], AllowReset) Values(" + "\r\n" +
                    "'" + ParentId + "'" + "\r\n" +
                    ",'" + v_DetailDto.SectionTypeID + "'" + "\r\n" +
                    ",'" + v_DetailDto.SectionParam + "'" + "\r\n" +
                    ",'" + v_DetailDto.Increment + "'" + "\r\n" +
                    ",'" + v_DetailDto.InvalidChar + "'" + "\r\n" +                   
                    ",'" + v_DetailDto.Order + "'" + "\r\n" +
                    ",'" + v_DetailDto.AllowReset + "'" + "\r\n" +

                    ")" + "\r\n"+
                    "SELECT @SNSectionMaxID=@@IDENTITY" + "\r\n"
                    ;

                if (v_DetailDto.SectionTypeID == "4") 
                {
                    foreach (var v_mesSNRange in List_SNRange)
                    {
                        S_Sql +=
                            "insert into mesSNRange(SNSectionID, [Start], [End],[Order], StatusID) Values(" + "\r\n" +
                            "@SNSectionMaxID" + "\r\n" +
                            ",'" + v_mesSNRange.Start + "'" + "\r\n" +
                            ",'" + v_mesSNRange.End + "'" + "\r\n" +
                            ",'" + v_mesSNRange.Order + "'" + "\r\n" +
                            ",'" + v_mesSNRange.StatusID + "'" + "\r\n" +

                            ")" + "\r\n";
                    }
                }

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                //S_Result=I_Result.ToString();

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesSNSection", S_MSG);
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
                SC_mesSNSection v_DetailDto = new SC_mesSNSection();
                string S_Sql = "select * from mesSNSection where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesSNSection>(S_Sql);
                string S_MSG = v_DetailDto.ToJson();

                S_Sql = "";
                if (v_DetailDto.SectionType == "4")
                {                    
                    string S_Sql_mesSNRange = "select * from mesSNRange where SNSectionID='"+Id+"'";
                    var v_mesSNRange = await DapperConnRead2.QueryFirstAsync<SC_mesSNRange>(S_Sql_mesSNRange);

                    S_Sql += "delete  mesSNRange where SNSectionID='" + Id + "'";
                    S_MSG = v_DetailDto.ToJson()+"\r\n"+
                        v_mesSNRange.ToJson()
                        ;
                }
                S_Sql = S_Sql + "\r\n" +
                    "delete  mesSNSection where Id='" + Id + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesSNSection", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }
        public async Task<string> UpdateDetail(SC_mesSNSectionEdit v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            List<SC_mesSNRange> List_SNRange = v_DetailDto.List_SNRange;

            v_DetailDto.Increment = v_DetailDto.Increment ?? "";
            v_DetailDto.InvalidChar = v_DetailDto.InvalidChar ?? "";

            if (v_DetailDto.AllowReset.ToString() == "" || v_DetailDto.AllowReset.ToString() == "null")
            {
                v_DetailDto.AllowReset = false;
            }
            v_DetailDto.AllowReset = v_DetailDto.AllowReset ?? false;
            if (v_DetailDto.AllowReset.ToString() == "")
            {
                v_DetailDto.AllowReset = false;
            }

            try
            {
                SC_mesSNSection v_DetailDto_before = new SC_mesSNSection();
                string
                S_Sql = "select * from mesSNSection where Id='" + v_DetailDto.ID + "'";
                v_DetailDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNSection>(S_Sql);

                List<SC_mesSNRange> List_mesSNRange_before = new List<SC_mesSNRange>();
                try
                {
                    S_Sql = "select * from mesSNRange where SNSectionID='" + v_DetailDto.ID + "'";
                    var v_mesSNRange_before = await DapperConnRead2.QueryAsync<SC_mesSNRange>(S_Sql);
                    List_mesSNRange_before= v_mesSNRange_before.ToList();
                }
                catch { }

                S_Sql = "Update mesSNSection set " +
                        " SectionType='" + v_DetailDto.SectionTypeID + "'" +"\r\n"+
                        " ,SectionParam='" + v_DetailDto.SectionParam + "'" +"\r\n"+
                        " ,Increment='" + v_DetailDto.Increment + "'" +"\r\n"+
                        " ,InvalidChar='" + v_DetailDto.InvalidChar + "'" +"\r\n"+
                       // " ,LastUsed='" + v_DetailDto.LastUsed + "'" + "\r\n" +
                        " ,[Order]='" + v_DetailDto.Order + "'" + "\r\n" +
                        " ,AllowReset='" + v_DetailDto.AllowReset + "'" + "\r\n" +

                        " where Id='" + v_DetailDto.ID + "'"+ "\r\n" ;

                
                if (v_DetailDto.SectionTypeID == "4") 
                {

                    if (v_DetailDto.SectionTypeID == "4")
                    {
                        foreach (var v_mesSNRange in List_SNRange)
                        {
                            var v_Query=from C in List_mesSNRange_before where C.ID == v_mesSNRange.ID select C;

                            if (v_Query.Count() == 0)
                            {
                                S_Sql +=
                                    "insert into mesSNRange(SNSectionID, [Start], [End],[Order], StatusID) Values(" + "\r\n" +
                                    "'" + v_DetailDto.ID + "'" + "\r\n" +
                                    ",'" + v_mesSNRange.Start + "'" + "\r\n" +
                                    ",'" + v_mesSNRange.End + "'" + "\r\n" +
                                    ",'" + v_mesSNRange.Order + "'" + "\r\n" +
                                    ",'" + v_mesSNRange.StatusID + "'" + "\r\n" +

                                    ")" + "\r\n";
                            }
                            else 
                            {
                                S_Sql += "Update mesSNRange set " + "\r\n" +
                                        "  [Start]='" + v_mesSNRange.Start + "'" + "\r\n" +
                                        " ,[End]='" + v_mesSNRange.End + "'" + "\r\n" +
                                        " ,StatusID='" + v_mesSNRange.StatusID + "'" + "\r\n" +
                                        " ,[Order]='" + v_mesSNRange.Order + "'" + "\r\n" +

                                        " where ID='" + v_mesSNRange.ID + "'";
                            }
                        }
                    }

                }

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_DetailDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_DetailDto_before.ToJson() + "\r\n" +                                  
                                    "after-----" + "\r\n" +
                                   v_DetailDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesSNSection", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<SC_mesSNFormatDetailList> List_Detail(string ParentId)
        {
            SC_mesSNFormatDetailList v_SC_mesSNFormatlList = new SC_mesSNFormatDetailList();
            v_SC_mesSNFormatlList.MSG = "OK";
            try
            {
                string S_Sql = @"SELECT ID, SNFormatID, SectionType SectionTypeID, SectionParam, Increment, InvalidChar, LastUsed,
                                   [Order], AllowReset,
                                   (CASE WHEN SectionType=1 THEN '固定值(Fixed)'
                                        WHEN SectionType=2 THEN '存储过程(Procedure)'
                                        WHEN SectionType=3 THEN '日期时间(DateTime)'
                                        WHEN SectionType=4 THEN '计数(Counter)'
                                    END) SectionType
                              FROM mesSNSection where SNFormatID='" + ParentId + "'";
                var v_Detail = await DapperConnRead2.QueryAsync<SC_mesSNSectionDto>(S_Sql);
                v_SC_mesSNFormatlList.List_mesSNFormatDetail = v_Detail.ToList();
            }
            catch (Exception ex)
            {
                v_SC_mesSNFormatlList.MSG = ex.ToString();
            }

            return v_SC_mesSNFormatlList;
        }



        public async Task<string> InsertSNRange(string ParentId, SC_mesSNRange v_DetailDto, IDbTransaction trans = null) 
        {
            string S_Result = "OK";

            try
            {
                string
                S_Sql =
                    "insert into mesSNRange(SNSectionID, [Start], [End], StatusID) Values(" + "\r\n" +
                    "'" + ParentId + "'" + "\r\n" +
                    ",'" + v_DetailDto.Start + "'" + "\r\n" +
                    ",'" + v_DetailDto.End + "'" + "\r\n" +
                    ",'" + v_DetailDto.StatusID + "'" + "\r\n" +

                    ")" + "\r\n";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "insert", "mesSNRange", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        public async Task<string> DeleteSNRange(string Id, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                SC_mesSNRange v_DetailDto = new SC_mesSNRange();
                string S_Sql = "select * from mesSNRange where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesSNRange>(S_Sql);

                S_Sql = "delete  mesSNRange where Id='" + Id + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesSNRange", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }
        public async Task<string> UpdateSNRange(SC_mesSNRange v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {

                SC_mesSNRange v_DetailDto_before = new SC_mesSNRange();
                string
                S_Sql = "select * from mesSNRange where Id='" + v_DetailDto.ID + "'";
                v_DetailDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesSNRange>(S_Sql);

                S_Sql = "Update mesSNRange set " +
                        " SNSectionID='" + v_DetailDto.SNSectionID + "'" +
                        " ,Start='" + v_DetailDto.Start + "'" +
                        " ,End='" + v_DetailDto.End + "'" +
                        " ,StatusID='" + v_DetailDto.StatusID + "'" +

                        " where Id='" + v_DetailDto.ID + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_DetailDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_DetailDto_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_DetailDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesSNRange", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        public async Task<SC_mesSNRangeList> List_SNRange(string ParentId)
        {
            SC_mesSNRangeList v_SC_mesSNRangeList = new SC_mesSNRangeList();
            v_SC_mesSNRangeList.MSG = "OK";
            try
            {
                string S_Sql = @"SELECT ID, SNSectionID, [Start], [End],[Order], StatusID,
                                   (CASE WHEN StatusID=0 THEN '新增(New)'
                                        WHEN StatusID=1 THEN '运行中(Running)'
                                        WHEN StatusID=2 THEN '完成(Completed)'
                                        WHEN StatusID=3 THEN 'F-完成(F-Completed)'
                                    END) StatusValue
                              FROM mesSNRange where SNSectionID='" + ParentId + "'";
                var v_Detail = await DapperConnRead2.QueryAsync<SC_mesSNRangeDto>(S_Sql);
                v_SC_mesSNRangeList.List_mesSNRange = v_Detail.ToList();
            }
            catch (Exception ex)
            {
                v_SC_mesSNRangeList.MSG = ex.ToString();
            }

            return v_SC_mesSNRangeList;
        }

    }
}

