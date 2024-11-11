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
    public class SC_mesLabelRepository : BaseRepositoryReport<string>, ISC_mesLabelRepository
    {
        PublicSCRepository P_PublicSCRepository;
        PublicMiniRepository Public_Repository;
        MSG_Sys P_MSG_Sys;

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public SC_mesLabelRepository()
        {
        }
        public SC_mesLabelRepository(IDbContextCore context) : base(context)
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

        public async Task<string> Insert(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLabel.Name = v_SC_mesLabel.Name ?? "";
            v_SC_mesLabel.Description = v_SC_mesLabel.Description ?? "";

            try
            {
                string S_Sql = "select * from mesLabel where Name='" + v_SC_mesLabel.Name.Trim() + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLabelDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesLabel.Name + " Data already exists.";
                }

                S_Sql = "insert into mesLabel(Name, Description, LabelFamilyID, LabelType, TargetPath," +
                    " OutputType,PrintCMD, SourcePath, PageCapacity) Values(" + "\r\n" +
                     "'" + v_SC_mesLabel.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.LabelFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.LabelType + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.TargetPath + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.OutputType + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.PrintCMD + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.SourcePath + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.PageCapacity + "'" + "\r\n" +

                    ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_SC_mesLabel.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesLabel", S_MSG);
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
                SC_mesLabel v_SC_mesLabel = new SC_mesLabel();
                string S_Sql = "select * from mesLabel where Id='" + Id + "'";
                v_SC_mesLabel = await DapperConnRead2.QueryFirstAsync<SC_mesLabel>(S_Sql);

                List<SC_mesLabelField> List_DetailDto = new List<SC_mesLabelField>();
                S_Sql = "select * from mesLabelField where LabelID='" + Id + "'";
                var q_Query_Detail = await DapperConnRead2.QueryAsync<SC_mesLabelField>(S_Sql);
                List_DetailDto = q_Query_Detail.ToList();

                S_Sql = "delete mesLabel where Id='" + Id + "' " + "\r\n" +
                        "delete mesLabelFormat where LabelID='" + Id + "' " + "\r\n" +
                        "delete mesLabelField where LabelID='" + Id + "'"
                       ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_SC_mesLabel != null)
                {
                    string S_MSG = v_SC_mesLabel.ToJson() + "\r\n" +
                           List_DetailDto.ToJson()
                          ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesLabel", S_MSG);
                }

            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> Update(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLabel.Name = v_SC_mesLabel.Name ?? "";
            v_SC_mesLabel.Description = v_SC_mesLabel.Description ?? "";

            try
            {
                string S_Sql = "select * from mesLabel where Name='" + v_SC_mesLabel.Name.Trim() + "'" +
                                " and ID<>'" + v_SC_mesLabel.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLabel>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesLabel.Name + " Data already exists.";
                }

                SC_mesLabel v_mesLabel_before = new SC_mesLabel();
                S_Sql = "select * from mesLabel where Id='" + v_SC_mesLabel.ID + "'";
                v_mesLabel_before = await DapperConnRead2.QueryFirstAsync<SC_mesLabel>(S_Sql);

                S_Sql = "Update mesLabel set " +
                        " Name='" + v_SC_mesLabel.Name + "'" +
                        " ,Description='" + v_SC_mesLabel.Description + "'" +
                        " ,LabelFamilyID='" + v_SC_mesLabel.LabelFamilyID + "'" +
                        " ,LabelType='" + v_SC_mesLabel.LabelType + "'" +
                        " ,TargetPath='" + v_SC_mesLabel.TargetPath + "'" +
                        " ,OutputType='" + v_SC_mesLabel.OutputType + "'" +
                        " ,PrintCMD='" + v_SC_mesLabel.PrintCMD + "'" +
                        " ,SourcePath='" + v_SC_mesLabel.SourcePath + "'" +
                        " ,PageCapacity='" + v_SC_mesLabel.PageCapacity + "'" +

                        " where Id='" + v_SC_mesLabel.ID + "'";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesLabel_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_mesLabel_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_SC_mesLabel.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesLabel", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<string> Clone(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            v_SC_mesLabel.Description = v_SC_mesLabel.Description ?? "";

            try
            {
                string S_Sql = "select * from mesLabel where Name='" + v_SC_mesLabel.Name.Trim() + "'";

                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLabelDto>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = v_SC_mesLabel.Name + " Data already exists.";
                }

                SC_mesLabel v_mesLabel_before = new SC_mesLabel();
                S_Sql = "select * from mesLabel where Id='" + v_SC_mesLabel.ID + "'";
                v_mesLabel_before = await DapperConnRead2.QueryFirstAsync<SC_mesLabel>(S_Sql);

                S_Sql = "DECLARE @MaxLabelID int" + "\r\n" +
                     "insert into mesLabel(Name, Description, LabelFamilyID, LabelType, TargetPath," + "\r\n" +
                    " OutputType,PrintCMD, SourcePath, PageCapacity) Values(" + "\r\n" +
                     "'" + v_SC_mesLabel.Name + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.Description + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.LabelFamilyID + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.LabelType + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.TargetPath + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.OutputType + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.PrintCMD + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.SourcePath + "'" + "\r\n" +
                     ",'" + v_SC_mesLabel.PageCapacity + "'" + "\r\n" +

                    ")"+ "\r\n"+
                    "SELECT @MaxLabelID=@@IDENTITY" + "\r\n" 
                    ;

                List<SC_mesLabelField> List_Detail = new List<SC_mesLabelField>();
                string S_SqlDetasil = "select *" +
                    "  from mesLabelField where LabelID='" + v_SC_mesLabel.ID + "'";
                var v_QueryDetail = await DapperConnRead2.QueryAsync<SC_mesLabelField>(S_SqlDetasil);
                List_Detail = v_QueryDetail.ToList();

                S_Sql += @"
                     DECLARE @LabelFieldID int
                     DECLARE @Order int  
                     "
                    ;

                foreach (var item in List_Detail)
                {
                    string S_Description = "Header";
                    if (item.LabelFormatPos == "1")
                    {
                        S_Description = "Header";
                    }
                    else if (item.LabelFormatPos == "2")
                    {
                        S_Description = "Line";
                    }

                    S_Sql += @"
                        select @LabelFieldID=ISNULL(max(LabelFieldID)+1,1)  from mesLabelField where LabelID=@MaxLabelID" +
                            @" and LabelFormatPos=" + item.LabelFormatPos + "\r\n" +

                            @"select @Order = ISNULL(max([Order]) + 1,1)  from mesLabelField where LabelID =@MaxLabelID " + "\r\n" +
                            @" and LabelFormatPos=" + item.LabelFormatPos + "\r\n" +

                        "insert into mesLabelField(LabelID, LabelFormatPos, LabelFieldID,[Description]," + "\r\n" +
                            " FieldDefinitionID,[Order]) Values(" + "\r\n" +
                        "@MaxLabelID" + "\r\n" +
                        ",'" + item.LabelFormatPos + "'" + "\r\n" +
                        ",@LabelFieldID" + "\r\n" +
                        ",'" + item.Description + "'" + "\r\n" +
                        ",'" + item.FieldDefinitionID + "'" + "\r\n" +
                        ",@Order" + "\r\n" +

                        ")" + "\r\n";

                        //"insert into mesLabelFormat(LabelID,LabelFormatPos,Description) values(" + "\r\n" +
                        //"@MaxLabelID," + "\r\n" +
                        //"'" + item.LabelFormatPos + "'," + "\r\n" +
                        //"'" + S_Description + "')"+ "\r\n";
                }

                S_Sql+=
                "insert into mesLabelFormat(LabelID,LabelFormatPos,Description) values(" + "\r\n" +
                "@MaxLabelID," + "\r\n" +
                "'1'," + "\r\n" +
                "'Header')" + "\r\n";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_mesLabel_before != null)
                {
                    string S_MSG = "from-----" + "\r\n" +
                                   v_mesLabel_before.ToJson() + "\r\n" +
                                    "clone-----" + "\r\n" +
                                   v_SC_mesLabel.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Clone", "mesLabel", S_MSG);
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }

        public async Task<List<SC_mesLabelALLDto>> FindWithPagerMyAsync(SC_mesLabelSearch search, PagerInfo info)
        {
            search.ID = search.ID ?? "";
            search.Name = search.Name ?? "";
            search.Description = search.Description ?? "";
            search.LabelFamilyID = search.LabelFamilyID ?? "";
            search.LabelFamily = search.LabelFamily ?? "";

            search.LabelType = search.LabelType ?? "";
            search.LabelTypeName = search.LabelTypeName ?? "";
            search.TargetPath = search.TargetPath ?? "";
            search.OutputType = search.OutputType ?? "";
            search.OutputTypeName = search.OutputTypeName ?? "";
            search.PrintCMD = search.PrintCMD ?? "";
            search.SourcePath = search.SourcePath ?? "";
            search.PageCapacity = search.PageCapacity ?? "";

            search.LikeQuery = search.LikeQuery ?? "";

            List<SC_mesLabelALLDto> List_Result = new List<SC_mesLabelALLDto>();
            try
            {
                string S_Page =
                                "SELECT distinct A.*" + "\r\n";
                string S_Count = "SELECT COUNT( DISTINCT A.Id) Valint1" + "\r\n";

                string S_Sql =
                @"
                FROM mesLabel A
                    LEFT JOIN luLabelFamily B ON A.LabelFamilyID=B.ID
                    LEFT JOIN V_LabelType C ON A.LabelType=C.ID
                    LEFT JOIN V_OutputType D ON A.OutputType=D.ID
    
                    LEFT JOIN mesLabelField E ON A.ID=E.LabelID
                    LEFT JOIN mesLabelFieldDefinition F ON E.FieldDefinitionID=F.ID           
	            where 1=1
                   and ('" + search.ID + @"'='' or A.id  IN (SELECT value FROM dbo.F_Split('" + search.ID + @"',',')))
                   and ('" + search.Name + @"'='' or A.Name  Like '%" + search.Name + @"%' )
                   and ('" + search.Description + @"'='' or A.Description Like '%" + search.Description + @"%' )
                   and ('" + search.LabelFamilyID + @"'='' or A.LabelFamilyID   IN (SELECT value FROM dbo.F_Split('" + search.LabelFamilyID + @"',',')))
                   and ('" + search.LabelType + @"'='' or A.LabelType   IN (SELECT value FROM dbo.F_Split('" + search.LabelType + @"',',')))
                     
                   and ('" + search.TargetPath + @"'='' or A.TargetPath Like '%" + search.TargetPath + @"%' )
                   and ('" + search.OutputType + @"'='' or A.OutputType   IN (SELECT value FROM dbo.F_Split('" + search.OutputType + @"',',')))
                   and ('" + search.PrintCMD + @"'='' or A.PrintCMD Like '%" + search.PrintCMD + @"%' )
                   and ('" + search.SourcePath + @"'='' or A.SourcePath Like '%" + search.SourcePath + @"%' )
                   and ('" + search.PageCapacity + @"'='' or A.PageCapacity Like '%" + search.PageCapacity + @"%' )

                   and ('" + search.LabelFieldDefName + @"'='' or F.Name Like '%" + search.LabelFieldDefName + @"%' )

                   and ( '" + search.LikeQuery + @"'='' or A.Name Like '%" + search.LikeQuery + @"%'
                            or A.Description Like '%" + search.LikeQuery + @"%'                            
                            or B.Name Like '%" + search.LikeQuery + @"%'
                            or C.LabelTypeName Like '%" + search.LikeQuery + @"%'

                            or D.OutputTypeName Like '%" + search.LikeQuery + @"%'
                            OR E.[Description] Like '%" + search.LikeQuery + @"%'
                            OR F.Name Like '%" + search.LikeQuery + @"%'
                            OR F.[Description] Like '%" + search.LikeQuery + @"%'


                        )"
                    ; 
                string S_SqlPage =
                    @"
                SELECT distinct AA.* FROM 
                (
                    SELECT A.*,B.Name LabelFamily,C.LabelTypeName,D.OutputTypeName
                    FROM 
                    (
                    " +
                    S_Page +
                    S_Sql +
                    " order by A.ID  offset ((" + info.CurrentPageIndex + " - 1) * " + info.PageSize +
                        ") rows fetch next " + info.PageSize + " rows only " + "\r\n" +
                    @")A 
                        LEFT JOIN luLabelFamily B ON A.LabelFamilyID=B.ID
                        LEFT JOIN V_LabelType C ON A.LabelType=C.ID
                        LEFT JOIN V_OutputType D ON A.OutputType=D.ID
    
                        LEFT JOIN mesLabelField E ON A.ID=E.LabelID
                        LEFT JOIN mesLabelFieldDefinition F ON E.FieldDefinitionID=F.ID  
                    )AA
                    "
                    ;

                var v_query = await DapperConnRead2.QueryAsync<SC_mesLabelALLDto>(S_SqlPage, null, null, I_DBTimeout, null);
                List<SC_mesLabelALLDto> list = v_query.ToList();

                foreach (var MainData in list)
                {
                    List<SC_mesLabelFieldDto> Children = new List<SC_mesLabelFieldDto>();
                    SC_mesLabelALLDto v_SC_mesLabelALLDto = new SC_mesLabelALLDto();

                    v_SC_mesLabelALLDto = MainData;

                    string S_SqlDetail =
                     @"
                     if exists(select 1 from mesLabel where Name like '%" + search.LikeQuery + @"%' and id='" + MainData.ID + @"')
                     begin
                        SELECT A.*,B.Description  AS LabelFormatPosKey,C.Name as LabelName,D.Name DefName, D.Definition DefDefinition
	                    FROM mesLabelField A 	
		                    Left join mesLabelFormat B ON A.LabelID = B.LabelID AND A.LabelFormatPos = B.LabelFormatPos
		                    Left join mesLabel C on A.LabelID=C.ID
		                    Left JOIN mesLabelFieldDefinition D ON A.FieldDefinitionID=D.ID 

                         WHERE 1=1 and A.LabelID='" + MainData.ID + @"'
                            and ('" + search.LabelFieldDefName + @"'='' or D.Name Like '%" + search.LabelFieldDefName + @"%' )

 
                           and ('" + search.LikeQuery + @"'='' or D.Name Like '%" + search.LikeQuery + @"%'
     
                                )
                     end
                     else
                     begin
                        SELECT A.*,B.Description  AS LabelFormatPosKey,C.Name as LabelName,D.Name DefName, D.Definition DefDefinition
	                    FROM mesLabelField A 	
		                    Left join mesLabelFormat B ON A.LabelID = B.LabelID AND A.LabelFormatPos = B.LabelFormatPos
		                    Left join mesLabel C on A.LabelID=C.ID
		                    Left JOIN mesLabelFieldDefinition D ON A.FieldDefinitionID=D.ID 

                         WHERE 1=1 and A.LabelID='" + MainData.ID + @"'                             
                            and ('" + search.LabelFieldDefName + @"'='' or D.Name Like '%" + search.LabelFieldDefName + @"%' )
                     end   
                    ";
                    var list_detail = await DapperConnRead2.QueryAsync<SC_mesLabelFieldDto>(S_SqlDetail, null, null, I_DBTimeout, null);
                    Children = list_detail.ToList();

                    v_SC_mesLabelALLDto.Children = Children;
                    List_Result.Add(v_SC_mesLabelALLDto);
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


        public async Task<string> InsertDetail(string ParentId, SC_mesLabelField v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesLabelField where LabelID='" + v_DetailDto.LabelID +
                    "' and LabelFormatPos='" + v_DetailDto.LabelFormatPos + "' and FieldDefinitionID='"+ v_DetailDto.FieldDefinitionID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLabelField>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }

                string S_Description = "Header";
                if (v_DetailDto.LabelFormatPos == "1")
                {
                    S_Description = "Header";
                }
                else if (v_DetailDto.LabelFormatPos == "2")
                {
                    S_Description = "Line";
                }

                S_Sql = @"
                     DECLARE @LabelFieldID int
                     DECLARE @Order int
                        select @LabelFieldID=ISNULL( max(LabelFieldID),0)+1  from mesLabelField where LabelID=" + v_DetailDto.LabelID + "\r\n" +
                        " and LabelFormatPos=" + v_DetailDto.LabelFormatPos+ "\r\n" +

                        //"select @Order = max([Order]) + 1  from mesLabelField where LabelID = " + v_DetailDto.LabelID + "\r\n" +
                        //" and LabelFormatPos=" + v_DetailDto.LabelFormatPos + "\r\n" +

                    "insert into mesLabelField(LabelID, LabelFormatPos, LabelFieldID,[Description],FieldDefinitionID,[Order]) Values(" + "\r\n" +
                    "'" + ParentId + "'" + "\r\n" +
                    ",'" + v_DetailDto.LabelFormatPos + "'" + "\r\n" +
                    ",@LabelFieldID" + "\r\n" +
                    ",'" + v_DetailDto.Description + "'" + "\r\n" +
                    ",'" + v_DetailDto.FieldDefinitionID + "'" + "\r\n" +
                    ",'"+ v_DetailDto.Order + "'" + "\r\n" +

                    ")" + "\r\n"
                ;

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                try
                {
                    S_Sql =
                    "insert into mesLabelFormat(LabelID,LabelFormatPos,Description) values(" + "\r\n" +
                    "'" + v_DetailDto.LabelID + "'," + "\r\n" +
                    "'" + v_DetailDto.LabelFormatPos + "'," + "\r\n" +
                    "'" + S_Description + "')";

                    await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);
                }
                catch { }

                //S_Result=I_Result.ToString();

                string S_MSG = v_DetailDto.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Insert", "mesLabelField", S_MSG);
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
                SC_mesLabelFieldDto v_DetailDto = new SC_mesLabelFieldDto();
                string S_Sql = "select * from mesLabelField where Id='" + Id + "'";
                v_DetailDto = await DapperConnRead2.QueryFirstAsync<SC_mesLabelFieldDto>(S_Sql);

                List<SC_mesLabelFormat> List_SC_mesLabelFormat = new List<SC_mesLabelFormat>(); 
                S_Sql = "select * from mesLabelFormat where LabelID='" + Id + "'";
                var v_mesLabelFormat =await DapperConnRead2.QueryAsync<SC_mesLabelFormat>(S_Sql);
                List_SC_mesLabelFormat = v_mesLabelFormat.ToList();

                S_Sql = "delete  mesLabelField where Id='" + Id + "'"+"\r\n"+
                        "delete mesLabelFormat where LabelID='" + Id + "'"
                        ;
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                string S_MSG = v_DetailDto.ToJson()+"\r\n"+ List_SC_mesLabelFormat.ToJson();
                await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Delete", "mesLabelField", S_MSG);
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }
        public async Task<string> UpdateDetail(SC_mesLabelField v_DetailDto, IDbTransaction trans = null)
        {
            string S_Result = "OK";

            try
            {
                string S_Sql = "select * from mesLabelField where LabelID='" + v_DetailDto.LabelID +
                    "' and LabelFormatPos='" + v_DetailDto.LabelFormatPos + "' and FieldDefinitionID='" +
                    v_DetailDto.FieldDefinitionID + "' and ID<>'"+ v_DetailDto.ID + "'";
                var v_Exists = await DapperConnRead2.QueryAsync<SC_mesLabelField>(S_Sql);
                if (v_Exists.Count() > 0)
                {
                    return S_Result = " Data already exists.";
                }


                SC_mesLabelField v_DetailDto_before = new SC_mesLabelField();                
                S_Sql = "select * from mesLabelField where Id='" + v_DetailDto.ID + "'";
                v_DetailDto_before = await DapperConnRead2.QueryFirstAsync<SC_mesLabelField>(S_Sql);

                S_Sql = "Update mesLabelField set " +
                        " LabelID='" + v_DetailDto.LabelID + "'" +
                        " ,LabelFormatPos='" + v_DetailDto.LabelFormatPos + "'" +
                       // " ,LabelFieldID='" + v_DetailDto.LabelFieldID + "'" +
                        " ,Description='" + v_DetailDto.Description + "'" +
                        " ,FieldDefinitionID='" + v_DetailDto.FieldDefinitionID + "'" +
                        " ,[Order]='" + v_DetailDto.Order + "'" +

                        " where Id='" + v_DetailDto.ID + "'";

                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql, null, trans, I_DBTimeout, null);

                if (v_DetailDto_before != null)
                {
                    string S_MSG = "before-----" + "\r\n" +
                                   v_DetailDto_before.ToJson() + "\r\n" +
                                    "after-----" + "\r\n" +
                                   v_DetailDto.ToJson()
                                   ;
                    await P_PublicSCRepository.WriteSysConfigLog(List_Login, "Update", "mesLabelField", S_MSG);
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


