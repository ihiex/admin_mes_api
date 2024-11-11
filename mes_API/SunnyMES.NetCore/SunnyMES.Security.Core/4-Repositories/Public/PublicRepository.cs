using API_MSG;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using static Dapper.SqlMapper;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.MSGCode;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Quartz.Impl.Triggers;
using SunnyMES.Commons.Helpers;

namespace SunnyMES.Security.Repositories
{
    public class PublicRepository : BaseRepositoryReport<string>, IPublicRepository
    {
        public PublicRepository()
        {
        }

        protected MSG_Public P_MSG_Public;
        protected MSG_Sys msgSys;
        protected static string S_Path = Directory.GetCurrentDirectory();
        protected string S_Path_NG = "NG";
        protected string S_Path_OK = "OK";
        protected string S_Path_RE = "RE";
        protected string S_DayLog = "";

        public PublicRepository(IDbContextCore dbContext, int I_Language) : base(dbContext)
        {
            P_MSG_Public = new MSG_Public(I_Language);
            msgSys = new MSG_Sys(I_Language);
        }

        public List<TabVal> SetMesLog(string S_MSG, string S_Type, LoginList List_Login)
        {
            List<TabVal> List_TabVal = new List<TabVal>();
            string S_Sql = "";
            try
            {
                string S_IsMesLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");
               
                if (S_Type == "Start" || S_Type == "1") { S_Type = "OK"; }
                if (S_Type == "RE" || S_Type == "ERROR") { S_Type = "NG"; }

                if (S_Type == "NG")
                {
                    S_Sql = "select 'NG' as ValStr1,'" + S_Path_NG + "' as ValStr2";
                }
                else if (S_Type == "OK")
                {
                    S_Sql = "select 'OK' as ValStr1,'" + S_Path_OK + "' as ValStr2";
                }
                else if (S_Type == "RE")
                {
                    S_Sql = "select 'RE' as ValStr1,'" + S_Path_RE + "' as ValStr2";
                }

                if (S_IsMesLog == "1")
                {
                    CreateDIR(List_Login);
                    File.AppendAllText(S_DayLog + S_Type + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH") + ".Log",
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + S_MSG + "\r\n");
                }

            }
            catch (Exception ex)
            {
                S_Sql = "select 'NG' as ValStr1,'" + ex.Message + "' as ValStr2";
            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
            }
            //List<dynamic> List_ALL = new List<dynamic>();
            //List_ALL.Add(List_TabVal);

            return List_TabVal;
        }

        private void CreateDIR(LoginList List_Login)
        {
            try
            {
                if (Directory.Exists(S_Path + "\\MesLog") == false)
                {
                    Directory.CreateDirectory(S_Path + "\\MesLog");
                }

                S_DayLog = S_Path + "\\MesLog\\" + List_Login.Station + "\\";
                if (Directory.Exists(S_DayLog) == false)
                {
                    Directory.CreateDirectory(S_DayLog);
                }

                S_DayLog += DateTime.Now.ToString("yyyy-MM-dd") + "\\";
                if (Directory.Exists(S_DayLog) == false)
                {
                    Directory.CreateDirectory(S_DayLog);
                }

                //S_DayLog = S_Path + "\\MesLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
                //if (Directory.Exists(S_DayLog) == false)
                //{
                //    Directory.CreateDirectory(S_DayLog);
                //}

                S_DayLog += List_Login.CurrentLoginIP + "\\";
                if (Directory.Exists(S_DayLog) == false)
                {
                    Directory.CreateDirectory(S_DayLog);
                }

                //S_DayLog += List_Login.Station + "\\";
                //if (Directory.Exists(S_DayLog) == false)
                //{
                //    Directory.CreateDirectory(S_DayLog);
                //}

                if (Directory.Exists(S_DayLog + "OK\\") == false)
                {
                    Directory.CreateDirectory(S_DayLog + "OK\\");
                }

                if (Directory.Exists(S_DayLog + "NG\\") == false)
                {
                    Directory.CreateDirectory(S_DayLog + "NG\\");
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<dynamic> List_ERROR(Exception ex, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            List<TabVal> List_TabVal = new List<TabVal>();
            List<TabVal> List_Sounds = new List<TabVal>();

            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
                List_TabVal.First().ValStr1 = "ERROR";
                List_TabVal.First().ValStr2 = GetSNDateTime(S_SN) + ex.Message;
                List_TabVal.First().ValStr3 = S_DataStatus;

                List_Sounds = SetMesLog(S_SN + "  " + ex.Message, "ERROR", List_Login);
                List_TabVal.First().ValStr4 = List_Sounds[0].ValStr2;
            }

            List<dynamic> List_ALL = new List<dynamic>();
            List_ALL.Add(List_TabVal);

            return List_ALL;
        }

        public List<dynamic> List_ERROR(string S_Error, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            List<TabVal> List_TabVal = new List<TabVal>();
            List<TabVal> List_Sounds = new List<TabVal>();

            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
                List_TabVal.First().ValStr1 = "ERROR";
                List_TabVal.First().ValStr2 = GetSNDateTime(S_SN) + S_Error;
                List_TabVal.First().ValStr3 = S_DataStatus;

                List_Sounds = SetMesLog(S_SN + "  " + S_Error, "ERROR", List_Login);
                List_TabVal.First().ValStr4 = List_Sounds[0].ValStr2;
            }

            List<dynamic> List_ALL = new List<dynamic>();
            List_ALL.Add(List_TabVal);

            return List_ALL;
        }

        public List<TabVal> List_ERROR_TabVal(Exception ex, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            List<TabVal> List_TabVal = new List<TabVal>();
            List<TabVal> List_Sounds = new List<TabVal>();

            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
                List_TabVal.First().ValStr1 = "ERROR";
                List_TabVal.First().ValStr2 = GetSNDateTime(S_SN) + ex.Message;
                List_TabVal.First().ValStr3 = S_DataStatus;

                List_Sounds = SetMesLog(S_SN + "  " + ex.Message, "ERROR", List_Login);
                List_TabVal.First().ValStr4 = List_Sounds[0].ValStr2;
            }

            List<TabVal> List_ALL = new List<TabVal>();
            List_ALL.Add(List_TabVal.First());

            return List_ALL;
        }

        public List<TabVal> List_ERROR_TabVal(string S_Error, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            List<TabVal> List_TabVal = new List<TabVal>();
            List<TabVal> List_Sounds = new List<TabVal>();

            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
                List_TabVal.First().ValStr1 = "ERROR";
                List_TabVal.First().ValStr2 = GetSNDateTime(S_SN) + S_Error;
                List_TabVal.First().ValStr3 = S_DataStatus;

                List_Sounds = SetMesLog(S_SN + "  " + S_Error, "ERROR", List_Login);
                List_TabVal.First().ValStr4 = List_Sounds[0].ValStr2;
            }

            List<TabVal> List_ALL = new List<TabVal>();
            List_ALL.Add(List_TabVal.First());

            return List_ALL;
        }


        public TabVal SetMesLogVal(string S_MSG, string S_Type, LoginList List_Login)
        {
            TabVal List_TabVal = new TabVal();
            try
            {
                string S_IsMesLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");
                if (S_IsMesLog == "1")
                {
                    CreateDIR(List_Login);

                    File.AppendAllText(S_DayLog + S_Type + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH") + ".Log",
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + S_MSG + "\r\n");
                }

                if (S_Type == "Start" || S_Type == "1") { S_Type = "OK"; }
                if (S_Type == "RE" || S_Type == "ERROR") { S_Type = "NG"; }

                if (S_Type == "NG")
                {
                    List_TabVal.ValStr1 = "NG";
                    List_TabVal.ValStr2 = S_Path_NG;
                }
                else if (S_Type == "OK")
                {
                    List_TabVal.ValStr1 = "OK";
                    List_TabVal.ValStr2 = S_Path_OK;
                }
                else if (S_Type == "RE")
                {
                    List_TabVal.ValStr1 = "RE";
                    List_TabVal.ValStr2 = S_Path_RE;
                }
            }
            catch (Exception ex)
            {
                List_TabVal.ValStr1 = "NG";
                List_TabVal.ValStr2 = ex.Message;
            }
            return List_TabVal;
        }

        public TabVal GetERROR(Exception ex, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            TabVal List_TabVal = new TabVal();
            TabVal List_Sounds = new TabVal();

            List_TabVal.ValStr1 = "ERROR";
            List_TabVal.ValStr2 = GetSNDateTime(S_SN) + ex.Message;
            List_TabVal.ValStr3 = S_DataStatus;

            List_Sounds = SetMesLogVal(S_SN + "  " + ex.Message, "ERROR", List_Login);
            List_TabVal.ValStr4 = List_Sounds.ValStr2;

            return List_TabVal;
        }

        public TabVal GetERROR(string S_Error, string S_DataStatus, LoginList List_Login, string S_SN)
        {
            TabVal List_TabVal = new TabVal();
            TabVal List_Sounds = new TabVal();

            List_TabVal.ValStr1 = "ERROR";
            List_TabVal.ValStr2 = GetSNDateTime(S_SN) + S_Error;
            List_TabVal.ValStr3 = S_DataStatus;

            List_Sounds = SetMesLogVal(S_SN + "  " + S_Error, "ERROR", List_Login);
            List_TabVal.ValStr4 = List_Sounds.ValStr2;

            return List_TabVal;
        }


        public async Task<List<TabVal>> List_ExecProc(string S_ProcName, string S_SN, string S_POID,
            string S_PartID, string xmlExtraData, string xmlStrSNbuf, LoginList List_Login)
        {
            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_POID + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + S_PartID + "\"> </Part>";
            string xmlStation = "<Station StationID=\"" + List_Login.StationID + "\"> </Station>";

            List<TabVal> List_Result = await GetTabVal("1", "1", "");

            // 这里暂时这样考虑，后续在做调整
            if (S_ProcName == "")
            {
                if (PublicF.StrLeft(S_ApplicationType, 2) == "QC" || PublicF.StrLeft(S_ApplicationType, 3) == "IQC") 
                {
                    S_ProcName = "uspQCCheck";
                }
                else if (PublicF.StrLeft(S_ApplicationType, 8) == "Assemble")
                {
                    S_ProcName = "uspAssembleCheck";
                }
                //else if (S_ApplicationType.IndexOf("TT-OverStation")>0)
                //{
                //    S_ProcName = "uspMachineToolingCheck";

                //    xmlStrSNbuf = List_Login.StationTypeID.ToString();
                //}
            }

            if (S_ProcName != "")
            {
                List<TabVal> List_ProCheck = await uspCallProcedure(S_ProcName,
                    S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, xmlStrSNbuf);

                string S_ProCheck = List_ProCheck[0].ValStr1;
                if (S_ProCheck != "1")
                {
                    List_Result = List_ERROR_TabVal(S_ProCheck, "0", List_Login, S_SN);
                    return List_Result;
                }
            }

            return List_Result;
        }


        /// <summary>
        /// GetDictData 获取数据字典
        /// </summary>
        /// <returns></returns>
        public async Task<List<DictData>> GetDictData()
        {
            string S_Sql = "SELECT Id,ParentId,EnCode,FullName FROM API_Items  ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<DictData>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        /// <summary>
        /// GetDictData 获取数据字典明细
        /// </summary>
        /// <param name="S_DictDataID"></param>
        /// <param name="S_EnCode"></param>
        /// <returns></returns>
        public async Task<List<DictDataDetail>> GetDictDataDetail(string S_DictDataID, string S_EnCode)
        {
            string S_Sql = "";
            if (S_EnCode != "")
            {
                S_Sql = "SELECT * FROM API_Items WHERE EnCode='" + S_EnCode + "'";
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var Query_Item = await DapperConn.QueryAsync<DictDataDetail>(S_Sql, null, null, I_DBTimeout, null);
                if (Query_Item.Count() > 0)
                {
                    S_DictDataID = Query_Item.ToList().First().Id;
                }
            }

            S_Sql = @"SELECT Id,ItemId DictDataId, ParentId, ItemCode, ItemName,SortCode FROM API_ItemsDetail 
                       WHERE ItemId = '" + S_DictDataID + "'  ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<DictDataDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        /// <summary>
        /// 获取单张表数据，适合小数据查询
        /// </summary>
        /// <param name="S_TabName"></param>
        /// <param name="S_Where"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetData(string S_TabName, string S_Where)
        {
            S_TabName = S_TabName ?? "";
            S_Where = S_Where ?? "";

            if (S_TabName == "") { return null; }

            string S_Sql = "select * from " + S_TabName;
            if (S_Where != "") { S_Sql += " where " + S_Where; }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        /// <summary>
        /// 获取单张表数据，适合小数据查询
        /// </summary>
        /// <param name="S_TabName"></param>
        /// <param name="S_Where"></param>
        /// <returns></returns>
        public GridReader GetList(string S_TabName, string S_Where)
        {
            S_TabName = S_TabName ?? "";
            S_Where = S_Where ?? "";

            if (S_TabName == "") { return null; }

            string S_Sql = "select * from " + S_TabName;
            if (S_Where != "") { S_Sql += " where " + S_Where; }

            //var v_Query = DapperConnRead2.Query(S_Sql);
            var v_Query = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);

            return v_Query;
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 计算  IEnumerable<Object> 总数
        /// </summary>
        /// <param name="ListObj"></param>
        /// <returns></returns>
        public int GetListCount(IEnumerable<Object> ListObj)
        {
            int I_Result = 0;
            if (ListObj != null)
            {
                int I_Count = ListObj.Count();
                if (I_Count > 0)
                {
                    I_Result = I_Count;
                    if (ListObj is TabVal)
                    {
                        IEnumerable<TabVal> v_TabVal = (IEnumerable<TabVal>)ListObj;
                        I_Result = v_TabVal.First().Valint1 ?? 0;
                    }
                }
            }
            return I_Result;
        }
        /// <summary>
        /// 计算 List<Object> 总数
        /// </summary>
        /// <param name="ListObj"></param>
        /// <returns></returns>
        public int GetListCount(List<Object> ListObj)
        {
            int I_Result = 0;
            if (ListObj != null)
            {
                int I_Count = ListObj.Count();
                if (I_Count > 0)
                {
                    I_Result = I_Count;

                    if (ListObj[0] is TabVal)
                    {
                        IEnumerable<TabVal> v_TabVal = (IEnumerable<TabVal>)ListObj;
                        I_Result = v_TabVal.First().Valint1 ?? 0;
                    }
                }
            }
            return I_Result;
        }


        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <param name="I_LineID"></param>
        /// <param name="I_PartID"></param>
        /// <param name="I_PartFamilyID"></param>
        /// <param name="I_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<mesRoute>> GetmesRoute(int I_LineID, int I_PartID, int I_PartFamilyID, int I_ProductionOrderID)
        {
            //int I_Reult = 0;
            string S_Sql = "";
            IEnumerable<TabVal> v_TabVal = new List<TabVal>();

            if (I_LineID == 0 && I_PartID == 0 && I_PartFamilyID == 0 && I_ProductionOrderID == 0)
            {
                return null;
            }

            S_Sql =
                @"

	            declare @LineID int = '" + I_LineID + @"'
	            declare @PartID int = '" + I_PartID + @"'
	            declare @PartFamilyID int = '" + I_PartFamilyID + @"'
	            declare @ProductionOrderID int = '" + I_ProductionOrderID + @"'

	            declare @nRoutID int
	            declare @nPartID int
	            declare @nLineID int
	            declare @nPartFamilyID int

	            set @PartID= case when @PartID=0 then null else @PartID end 
	            set @LineID= case when @LineID=0 then null else @LineID end 
	            set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 

	            if @PartID is not NULL and @PartFamilyID is not NULL begin
		            if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID) 
		            begin
		                SELECT null ID, null Name,null Description,null RouteType
		                RETURN
		            end
	            end else if @PartFamilyID is NULL begin
		            select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	            end

	            select top 1 @nRoutID = RouteID, @nPartID = PartID, @nLineID = LineID, @nPartFamilyID = PartFamilyID
	            from mesRouteMap
	            where
		            (LineID = @LineID or LineID is NULL OR LineID=0) AND
		            (PartID = @PartID or PartID is NULL OR PartID=0) AND
		            (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		            (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
	            order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

                 select ID,Name,Description,RouteType from mesRoute where ID= @nRoutID 
                 ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_QueryResult = await DapperConn.QueryAsync<mesRoute>(S_Sql, null, null, I_DBTimeout, null);
            return v_QueryResult.ToList();

        }


        /// <summary>
        /// 获取路由类别
        /// </summary>
        /// <param name="I_RouteID">0 表格  1 画图</param>
        /// <returns></returns>
        public int GetRouteType(int I_RouteID)
        {
            if (I_RouteID < 1) { return -1; }
            string S_Sql = "select RouteType Valint1 from mesRoute where ID='" + I_RouteID + "'";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.Query<TabVal>(S_Sql, null, null, I_DBTimeout, null);

            if (v_Query.Count() > 0)
            {
                return Convert.ToInt32(v_Query.First().Valint1);
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 获取表格路径信息
        /// </summary>
        /// <param name="S_RouteSequence"></param>
        /// <param name="I_RouteID"></param>
        /// <returns></returns>
        public async Task<List<TableRouteData>> GetRouteDataTable(string S_RouteSequence, int I_RouteID)
        {
            string S_Sql = string.Format(@"select A.* from 
                            (select B.ID,B.Name,B.Description RouteName,B.RouteType,
			                            A.StationTypeID,A.Sequence SequenceMod,A.Description RouteDetailName,
			                            A.UnitStateID,                                
			                            E.Description ApplicationType,
			                            cast(ROW_NUMBER() over(order by A.Sequence) as int) as Sequence
		                            from 
			                            mesRouteDetail A
			                            join mesRoute B on A.RouteID=B.ID  		  	                             
			                            join mesStationType D on D.ID=A.StationTypeID
			                            join luApplicationType E on E.ID=D.ApplicationTypeID
	                            where B.ID={0}  
                            )A where 1=1", I_RouteID);
            if (!string.IsNullOrEmpty(S_RouteSequence))
            {
                S_Sql = S_Sql + " AND A.Sequence=" + S_RouteSequence;
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TableRouteData>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取画图路径信息
        /// </summary>
        /// <param name="S_StationTypeID"></param>
        /// <param name="I_RouteID"></param>
        /// <param name="S_DataType"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetRouteDataDiagram(string S_StationTypeID, int I_RouteID, string S_DataType)
        {
            IEnumerable<dynamic> List_Result = null;
            string S_Data1 =
            @"
		        SELECT A.*,C.Description StationType,
			        D.Description OutputStation,E.Description CurrentStation,B.Description Status FROM #tempRoute A
		        LEFT JOIN luUnitStatus B ON A.OutputStateDefID=B.ID
		        LEFT JOIN mesStationType C ON A.StationTypeID=C.ID
		        LEFT JOIN mesUnitState D ON A.CurrStateID=D.ID
		        LEFT JOIN mesUnitState E ON A.OutputStateID = E.ID
		        WHERE RouteType =(case when EXISTS(SELECT 1 FROM V_StationTypeInfo WHERE 
												        StationTypeID ='" + S_StationTypeID + @"' AND Content='TT') then 1 else 2 end)
            ";

            string S_Data2 =
            @"
		        SELECT A.*,C.Description StationType,
			        E.Description CurrentStation,D.Description OutputStation,B.Description Status FROM #tempRoute A
		        LEFT JOIN luUnitStatus B ON A.OutputStateDefID=B.ID
		        LEFT JOIN mesStationType C ON A.StationTypeID=C.ID
		        LEFT JOIN mesUnitState D ON A.CurrStateID=D.ID
		        LEFT JOIN mesUnitState E ON A.OutputStateID = E.ID
		        WHERE RouteType=0 AND EXISTS(SELECT 1 FROM #tempRoute T WHERE A.ParentID=T.ID AND 
										        T.RouteType=(case when EXISTS(SELECT 1 FROM V_StationTypeInfo WHERE 
										        StationTypeID ='" + S_StationTypeID + @"' AND Content='TT') then 1 else 2 end))
            ";

            string S_End =
            @"END TRY
	        BEGIN CATCH
		        SELECT @strOutput=ERROR_MESSAGE()
	        END CATCH";

            string S_SqlVal =
            @"
	        declare	@CurrStateID			VARCHAR(64),
			        @Count					int,
			        @Type					VARCHAR(64),
                    @strOutput              NVARCHAR(MAX)  

	        BEGIN TRY
		        SET @strOutput='1'

		        SELECT CAST(ROW_NUMBER() OVER(ORDER BY (SELECT 0)) AS int )ID,A.StationTypeID,a.RouteID,B.CurrStateID,A.OutputStateID,
				        A.OutputStateDefID,0 RouteType,0 ParentID INTO #tempRoute 
			        FROM (SELECT max(ID) as ID,'" + I_RouteID + @"' as RouteID,StationTypeID,OutputStateID,OutputStateDefID 
					        FROM mesUnitOutputState WHERE RouteID='" + I_RouteID + @"' AND StationTypeID>0
					        GROUP BY StationTypeID,OutputStateID,OutputStateDefID) A 
			        INNER JOIN mesUnitInputState B ON A.StationTypeID=B.StationTypeID
			        WHERE B.RouteID='" + I_RouteID + @"'

		        UPDATE A SET A.RouteType='1' FROM #tempRoute A 
		        INNER JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID AND B.DetailDef='StationTypeType'
		        INNER JOIN #tempRoute C ON A.CurrStateID=C.OutputStateID 
		        INNER JOIN V_StationTypeInfo D ON C.StationTypeID=D.StationTypeID AND D.DetailDef='StationTypeType'
		        WHERE (B.Content='TT' AND D.Content='TT') or (isnull(a.CurrStateID,'')='' AND D.Content='TT')
			        or (B.Content='TT'  AND isnull(a.CurrStateID,'')='')

		        UPDATE A SET A.RouteType='2' FROM #tempRoute A 
		        LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID AND B.DetailDef='StationTypeType'
		        LEFT JOIN #tempRoute C ON A.CurrStateID=C.OutputStateID 
		        LEFT JOIN V_StationTypeInfo D ON C.StationTypeID=D.StationTypeID AND D.DetailDef='StationTypeType'
		        WHERE ISNULL(B.Content,'')<>'TT' AND ISNULL(D.Content,'')<>'TT' or (isnull(a.CurrStateID,'')='' AND 
			        ISNULL(D.Content,'')<>'TT') OR (ISNULL(B.Content,'')<>'TT' AND isnull(a.CurrStateID,'')='')

		        UPDATE A SET A.ParentID=B.ID FROM #tempRoute A 
		        INNER JOIN (SELECT StationTypeID,RouteType,MAX(ID) ID,OutputStateDefID FROM #tempRoute 
			        WHERE ISNULL(RouteType,'')<>''  GROUP BY StationTypeID,RouteType,OutputStateDefID) B 
		        ON A.StationTypeID=B.StationTypeID AND A.OutputStateDefID=B.OutputStateDefID
		        WHERE ISNULL(A.RouteType,'')=0

            ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            string S_Sql = "";
            if (S_DataType == "1")
            {
                S_Sql = S_SqlVal + S_Data1 + S_End;
                var v_Query1 = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                List_Result = v_Query1;
            }
            else if (S_DataType == "2")
            {
                S_Sql = S_SqlVal + S_Data2 + S_End;
                var v_Query2 = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                List_Result = v_Query2;
            }

            return List_Result.ToList();
        }

        /// <summary>
        /// 获取路由详细信息
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetRouteDetail(string S_LineID, string S_PartID,
            string S_PartFamilyID, string S_ProductionOrderID)
        {
            List<mesRoute> List_Route = await GetmesRoute(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                            Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_ProductionOrderID));
            List_Route = List_Route ?? new List<mesRoute>();
            if (List_Route == null)
            {
                return null;
            }

            string S_RouteType = List_Route[0].RouteType.ToString();
            string S_RouteID = List_Route[0].ID.ToString();

            string S_Sql = @"select B.Description,ROW_NUMBER() OVER(ORDER BY A.Sequence) Sequence,a.ID,
                                RouteID,StationTypeID,UnitStateID from mesRouteDetail A
                                    inner join mesStationType B ON A.StationTypeID = B.ID
                             WHERE A.RouteID='" + S_RouteID + "'";


            if (S_RouteType == "1")
            {
                S_Sql = @"	select B.Description StationType
	                           ,C.Description  CurrentStation 
	                           ,D.Description  OutputStation 
                                ,E.Description StateDef
                                ,A.StationTypeID
                                ,(case F.Content when 'TT' then 'TT' else 'SFC' end) StationTypeType
	                            from mesUnitOutputState A 
			                     left join mesStationType B on A.StationTypeID=B.ID	
			                    left join mesUnitState C on A.CurrStateID=C.ID
			                    left join mesUnitState D on A.OutputStateID=D.ID
                                join luUnitStatus E on E.ID=A.OutputStateDefID
                                left join mesStationTypeDetail F on A.StationTypeID=F.StationTypeID	
	                    where A.RouteID=" + S_RouteID + @" 		
	                    order by A.StationTypeID	";
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 获取工单信息
        /// </summary>
        /// <param name="S_PartID"></param>
        /// <param name="S_LineID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> MesGetProductionOrder(string S_PartID, string S_LineID)
        {
            S_PartID = S_PartID ?? "";
            S_LineID = S_LineID ?? "";

            if (S_PartID == "" || S_LineID == "")
            {
                return null;
            }

            string S_Sql = @"SELECT A.ID, A.ProductionOrderNumber,C.PartNumber, A.Description, A.OrderQuantity,
		                        B.Lastname+B.Firstname AS Employee, A.CreationTime, A.LastUpdate,D.Description AS Status 
		                        FROM 
		                        (SELECT  * FROM mesProductionOrder where StatusID=1)AS A  
		                        LEFT JOIN (SELECT * from mesEmployee ) B ON A.EmployeeID=B.ID
		                        JOIN (SELECT * from mesPart)AS C ON A.PartID=C.ID 
		                        JOIN (select * from sysStatus ) AS D ON A.StatusID=D.ID";

            if (S_LineID != "")
            {
                S_Sql = S_Sql + " JOIN(select * from mesLineOrder ) AS F ON A.ID = F.ProductionOrderID";
            }
            S_Sql = S_Sql + " Where 1=1";
            if (S_PartID != "" && S_LineID == "")
            {
                S_Sql += "AND C.ID=" + S_PartID;
            }
            if (S_LineID != "" && S_PartID == "")
            {
                S_Sql += "AND F.LineID=" + S_LineID;
            }

            if (S_PartID != "" && S_LineID != "")
            {
                S_Sql += "AND C.ID=" + S_PartID + "  and F.LineID=" + S_LineID;
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }



        public async Task<List<dynamic>> MesGetLine(string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select * from mesLine";
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<dynamic>> MesGetStation(string ID, string LineID, string StationTypeID)
        {
            ID = ID ?? "";
            LineID = LineID ?? "";
            StationTypeID = StationTypeID ?? "";

            string S_Sql = "select * from mesStation where Status=1";
            string S1 = " and ID='" + ID + "'";
            string S2 = " and LineID='" + LineID + "'";
            string S3 = " and StationTypeID='" + StationTypeID + "'";


            string S_Tmp = "";
            if (ID != "") { S_Tmp += S1; }
            if (LineID != "") { S_Tmp += S2; }
            if (StationTypeID != "") { S_Tmp += S3; }

            if (S_Tmp != "") { S_Sql += S_Tmp; }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<TabVal>> MesGetStationNoTask(string ID, string LineID, string StationTypeID)
        {
            ID = ID ?? "";
            LineID = LineID ?? "";
            StationTypeID = StationTypeID ?? "";

            string S_Sql = "SELECT [Description] ValStr1, StationTypeID ValInt1 from mesStation where Status=1";
            string S1 = " and ID='" + ID + "'";
            string S2 = " and LineID='" + LineID + "'";
            string S3 = " and StationTypeID='" + StationTypeID + "'";


            string S_Tmp = "";
            if (ID != "") { S_Tmp += S1; }
            if (LineID != "") { S_Tmp += S2; }
            if (StationTypeID != "") { S_Tmp += S3; }

            if (S_Tmp != "") { S_Sql += S_Tmp; }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            IEnumerable<TabVal> v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<dynamic>> MesGetStationType(string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select * from mesStationType";
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<TabVal>> MesGetStationTypeNoTask(string ID)
        {
            ID = ID ?? "";
            string S_Sql = "select [Description] ValStr1 from mesStationType";
            if (ID != "") { S_Sql += " where ID='" + ID + "'"; }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<string> LuGetApplicationType(string StationID)
        {
            string S_Result = "";
            StationID = StationID ?? "";
            if (StationID == "") { return null; }

            string S_Sql = @"SELECT C.[Description] ValStr1
                              FROM mesStation A  JOIN mesStationType B ON A.StationTypeID=B.ID
                                   JOIN luApplicationType C ON B.ApplicationTypeID=C.ID
                            WHERE A.ID='" + StationID + "' ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            IEnumerable<TabVal> List_Reult = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            if (List_Reult.Count() > 0)
            {
                S_Result = List_Reult.First().ValStr1;
            }

            return S_Result;

        }

        public async Task<IEnumerable<dynamic>> MesGetPart(string ID, string PartFamilyID)
        {
            ID = ID ?? "";
            PartFamilyID = PartFamilyID ?? "";
            List<mesPart> List_mesPart = new List<mesPart>();

            string S_Where = " Status=1";
            if (ID != "")
            {
                S_Where += " and ID='" + ID + "'";
            }
            if (PartFamilyID != "")
            {
                S_Where += " and PartFamilyID='" + PartFamilyID + "'";
            }

            S_Where += " ORDER BY PartNumber";
            string S_Sql = "select * from mesPart where " + S_Where;
            IEnumerable<mesPart> v_Query = await DapperConn.QueryAsync<mesPart>(S_Sql, null, null, I_DBTimeout, null);

            foreach (var item in v_Query)
            {
                mesPart v_mesPart = item;
                S_Sql =
                    @"SELECT A.*,C.[Description] Color,B.[Content] ColorValue
                      FROM mesPart A
                      LEFT JOIN mesPartDetail B ON A.ID = B.PartID
                      LEFT JOIN luPartDetailDef C ON B.PartDetailDefID = C.ID
                    WHERE A.PartFamilyID = '" + item.PartFamilyID +
                         @"' AND A.ID ='" + item.ID + "' AND C.[Description]='Color'";

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                IEnumerable<mesPart> v_QueryColor = await DapperConn.QueryAsync<mesPart>(S_Sql, null, null, I_DBTimeout, null);
                if (GetListCount(v_QueryColor) > 0)
                {
                    v_mesPart.Color = v_QueryColor.First().ColorValue;
                }


                S_Sql =
                    @"SELECT A.*,C.[Description] Color,B.[Content] ColorValue
                      FROM mesPart A
                      LEFT JOIN mesPartDetail B ON A.ID = B.PartID
                      LEFT JOIN luPartDetailDef C ON B.PartDetailDefID = C.ID
                    WHERE A.PartFamilyID = '" + item.PartFamilyID +
                         @"' AND A.ID ='" + item.ID + "' AND C.[Description]='ColorValue'";

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                IEnumerable<mesPart> v_QueryColorValue = await DapperConn.QueryAsync<mesPart>(S_Sql, null, null, I_DBTimeout, null);
                if (GetListCount(v_QueryColorValue) > 0)
                {
                    v_mesPart.ColorValue = v_QueryColorValue.First().ColorValue;
                }

                List_mesPart.Add(v_mesPart);
            }
            return List_mesPart;
        }



        /// <summary>
        /// 路由检查
        /// </summary>
        /// <param name="Scan_StationTypeID"></param>
        /// <param name="Scan_StationID"></param>
        /// <param name="S_LineID"></param>
        /// <param name="DT_Unit"></param>
        /// <param name="S_Str"></param>
        /// <returns></returns>
        public async Task<string> GetRouteCheck(int Scan_StationTypeID, int Scan_StationID, string S_LineID,
            mesUnit DT_Unit, string S_Str)
        {
            string S_Result = "1";
            DateTime dateStart = DateTime.Now;

            try
            {
                string S_PartID = DT_Unit.PartID.ToString();
                //最后扫描工序 UnitStateID
                string S_UnitID = DT_Unit.ID.ToString();
                string S_UnitStateID = DT_Unit.UnitStateID.ToString();
                string S_UnitStationID = DT_Unit.StationID.ToString();
                string S_PartPamilyID = DT_Unit.PartFamilyID.ToString();
                string S_ProductionOrderID = DT_Unit.ProductionOrderID.ToString();
                string S_StatusID = DT_Unit.StatusID.ToString();

                //获取此料工序路径
                List<mesRoute> List_Route = await GetmesRoute(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                                Convert.ToInt32(S_PartPamilyID), Convert.ToInt32(S_ProductionOrderID));
                List_Route = List_Route ?? new List<mesRoute>();

                if (List_Route.Count == 0)
                {
                    return P_MSG_Public.MSG_Public_001; //料号未配置工艺流程路线.
                }

                int I_RouteID = List_Route[0].ID;
                int I_RouteType = List_Route[0].RouteType;  //GetRouteType(I_RouteID);
                if (I_RouteType == 0)
                {
                    IEnumerable<TableRouteData> DT_Route = await GetRouteDataTable("", I_RouteID);
                    var v_Route_Sacn = from c in DT_Route
                                       where c.StationTypeID == Scan_StationTypeID
                                       select c;
                    if (v_Route_Sacn.ToList().Count() > 0)
                    {
                        if (S_StatusID == "1")
                        {

                            int I_Sequence_Scan = v_Route_Sacn.First().Sequence;
                            if (I_Sequence_Scan > 1)
                            {
                                //最后扫描信息 (上一站扫描)
                                int I_UnitStateID = DT_Unit.UnitStateID;

                                try
                                {
                                    //最后 扫描路径信息
                                    var v_Route = from c in DT_Route
                                                  where c.UnitStateID == I_UnitStateID
                                                  select c;
                                    //最后 扫描顺序
                                    int I_Sequence = v_Route.First().Sequence;
                                    //最后扫描顺序  比 当前扫描顺序
                                    if (I_Sequence >= I_Sequence_Scan)
                                    {
                                        S_Result = P_MSG_Public.MSG_Public_005;  //此条码已过站.
                                    }
                                    else
                                    {
                                        //判断上一站是否扫描
                                        if (I_Sequence_Scan - 1 != I_Sequence)
                                        {
                                            S_Result = P_MSG_Public.MSG_Public_006; //上一站未扫描.
                                        }
                                    }
                                }
                                catch
                                {
                                    S_Result = P_MSG_Public.MSG_Public_006; //上一站未扫描.
                                }
                            }
                        }
                        else
                        {
                            S_Result = P_MSG_Public.MSG_Public_007;  //此条码已NG.
                        }
                    }
                    else
                    {
                        //没有配置 此工位工序
                        S_Result = P_MSG_Public.MSG_Public_008;
                    }
                }
                else if (I_RouteType == 1)
                {
                    string S_Sql =
                    @"	declare	
			            @UnitID				 int,
			            @StationTypeID		 int,
			            @OldStationTypeType	 nvarchar(64),
			            @NewStationTypeType	 nvarchar(64),
			            @OldUnitStateID		 int,
			            @strOutput           nvarchar(200)		
		
                    BEGIN TRY
		                SET @strOutput=1
		                SET @UnitID ='" + S_UnitID + @"'
                        SET @StationTypeID='" + Scan_StationTypeID + @"'

		                --判断是否为新流程  料号未配置工艺流程路线
		                IF NOT EXISTS(SELECT 1 FROM mesRoute WHERE ID='" + I_RouteID + @"' AND RouteType=1)
		                BEGIN
			                SET @strOutput='" + P_MSG_Public.MSG_Public_001 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
		                IF NOT EXISTS(SELECT 1 FROM mesUnitInputState WHERE RouteID='" + I_RouteID + @"' AND StationTypeID=@StationTypeID)
		                BEGIN
			                SET @strOutput='" + P_MSG_Public.MSG_Public_002 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --原流程校验
		                SELECT TOP 1 @OldUnitStateID=UnitStateID FROM mesUnit WHERE ID=@UnitID
		
		                IF not exists(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID 
					                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
		                BEGIN
			                DECLARE @ShowState varchar(200)
			                SELECT @ShowState=isnull(Description,null)+'-'+isnull(cast(ID as varchar),'') FROM mesUnitState WHERE ID=@OldUnitStateID
			                SET @strOutput='" + P_MSG_Public.MSG_Public_003 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --条码对应StationTypeType-旧
		                SELECT @OldStationTypeType = E.Content FROM mesUnit B
		                INNER JOIN mesStation C ON B.StationID=C.ID
		                INNER JOIN mesStationType D ON C.StationTypeID=D.ID
		                INNER JOIN mesStationTypeDetail E ON D.ID=E.StationTypeID
		                INNER JOIN luStationTypeDetailDef F ON E.StationTypeDetailDefID=F.ID AND F.Description='StationTypeType'
		                WHERE B.ID=@UnitID

		                --当前选择对应StationTypeType-新
		                SELECT @NewStationTypeType = A.Content FROM mesStationTypeDetail A
		                INNER JOIN luStationTypeDetailDef B ON A.StationTypeDetailDefID=B.ID AND B.Description='StationTypeType'
		                WHERE A.StationTypeID=@StationTypeID

		                --如果从TT房间出来,需要进行之前流程校验
		                IF ISNULL(@OldStationTypeType,'')<>ISNULL(@NewStationTypeType,'') AND ISNULL(@OldStationTypeType,'')='TT'
		                BEGIN
			                SELECT TOP 1 @OldUnitStateID=reserved_20 FROM mesUnitDetail WHERE UnitID=@UnitID

			                --判断旧状态是否为最后一站
			                IF EXISTS(SELECT 1 FROM mesUnitoUTputState A
						                LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID
						                WHERE A.RouteID='" + I_RouteID + @"' and A.StationTypeID=@StationTypeID 
						                AND ISNULL(B.Content,'')<>'TT' AND CurrStateID=0
						                 AND A.OutputStateDefID=1)
			                BEGIN
				                --最后一站可能是上一站过来或最后一站出去再进来
				                IF NOT EXISTS (SELECT 1 FROM mesUnitOutputState WHERE StationTypeID=@StationTypeID
							                AND RouteID='" + I_RouteID + @"' AND OutputStateID=@OldUnitStateID)
				                   AND NOT EXISTS(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID  
									                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
				                BEGIN
					                SET @strOutput='" + P_MSG_Public.MSG_Public_003 + @"'	
                                    Select  @strOutput as ValStr1
					                RETURN
				                END
			                END
			                ELSE
			                BEGIN
				                IF NOT EXISTS(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID 
					                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
				                BEGIN					            
					                SET @strOutput='" + P_MSG_Public.MSG_Public_003 + @"'	
                                    Select  @strOutput as ValStr1
					                RETURN
				                END
			                END
		                END
		
		                IF 	ISNULL(@NewStationTypeType,'')='TT'
		                BEGIN
			                --判断是否已经投入使用
			                IF EXISTS (SELECT 1 FROM mesUnitComponent WHERE ChildUnitID=@UnitID AND StatusID=1)
			                BEGIN
				                SET @strOutput='" + P_MSG_Public.MSG_Public_004 + @"'
                                Select  @strOutput as ValStr1
				                RETURN
			                END
			                --首次进入TT进行UnitDetail记录
			                IF ISNULL(@OldStationTypeType,'')<>ISNULL(@NewStationTypeType,'')
			                BEGIN
				                UPDATE A SET A.reserved_20=B.UnitStateID FROM mesUnitDetail A 
				                INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@UnitID
			                END
		                END
                        Select  @strOutput as ValStr1
	                END TRY
	                BEGIN CATCH
		                SELECT @strOutput=ERROR_MESSAGE()
                        Select  @strOutput as ValStr1
	                END CATCH                      
                    ";

                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    IEnumerable<TabVal> List_Reult = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                    if (List_Reult.Count() > 0)
                    {
                        S_Result = List_Reult.First().ValStr1;
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            if (S_Result != "1") 
            {
                string S_UnitStateID = DT_Unit.UnitStateID.ToString();
                string S_SqlUnitStateID = "select * from mesUnitState where ID='"+ S_UnitStateID + "'";

                var v_ListmesUnitState = await DapperConn.QueryAsync<mesUnitState>(S_SqlUnitStateID, null, null, I_DBTimeout, null);
                List<mesUnitState> List_S_SqlUnitStateID = v_ListmesUnitState.ToList();
                S_Result = S_Result+ "  Current UnitState: " + List_S_SqlUnitStateID[0].Description;

            }


            return S_Result;
        }


        /// <summary>
        /// 料号详细
        /// </summary>
        /// <param name="I_PartID"></param>
        /// <param name="I_PartDetailDefName"></param>
        /// <returns></returns>
        public async Task<List<mesPartDetail>> GetmesPartDetail(int I_PartID, string I_PartDetailDefName)
        {
            string S_Sql = @"select * from mesPartDetail a  WHERE  a.PartID=" + I_PartID +
                " and exists (select 1 from luPartDetailDef b where a.PartDetailDefID = b.ID and b.Description = '" +
                   I_PartDetailDefName + "')";

            var v_Query = await DapperConn.QueryAsync<mesPartDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// BOM
        /// </summary>
        /// <param name="S_ParentPartID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<List<mesProductStructure>> GetBOMStructure(string S_ParentPartID, string S_PartID, string S_StationTypeID)
        {
            string S_Sql = string.Format(@"select A.ParentPartID,C.PartNumber ParentPartNumber, C.Description ParentPartDescription,
                             B.PartNumber PartNumber, B.Description PartDescription, A.PartID,A.StationTypeID
                               from mesProductStructure A inner join mesPart B
                            ON A.PartID = B.ID INNER JOIN mesPart C ON A.ParentPartID = C.ID
                            WHERE A.Status = 1 AND('{0}' = '' or ParentPartID = '{0}')
                            and('{1}' = '' or PartID = '{1}')
                            and('{2}' = '' or StationTypeID = '{2}')", S_ParentPartID, S_PartID, S_StationTypeID);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesProductStructure>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 获取工单分线
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<mesLineOrder>> GetmesLineOrder(string S_LineID, string S_ProductionOrderID)
        {
            S_LineID = S_LineID ?? "";
            S_ProductionOrderID = S_ProductionOrderID ?? "";

            if (S_LineID == "")
            {
                return null;
            }
            if (S_LineID == "" && S_ProductionOrderID == "")
            {
                return null;
            }

            string S_Sql = "select * FROM mesLineOrder WHERE LineID='" + S_LineID +
                "' AND ProductionOrderID='" + S_ProductionOrderID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesLineOrder>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 获取工单分线,不检查工单
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<mesLineOrder>> GetmesLineOrder(string S_LineID, string S_PartID, string S_ProductionOrderID = "")
        {
            S_LineID = S_LineID ?? "";
            S_PartID = S_PartID ?? "";

            if (S_LineID == "")
            {
                return null;
            }
            if (S_LineID == "" && S_PartID == "")
            {
                return null;
            }

            string S_Sql =
                @"SELECT * FROM mesLineOrder WHERE LineID='" + S_LineID + @"' AND  ProductionOrderID IN 
                (
                   SELECT ID FROM mesProductionOrder WHERE PartID='" + S_PartID + @"'
                )";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesLineOrder>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }


        /// <summary>
        /// 打印格式ID
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<string> GetSNFormatName(string S_LineID, string S_PartID, string S_PartFamilyID,
            string S_ProductionOrderID, string S_StationTypeID)
        {
            string S_Result = "";

            try
            {
                string S_Sql =
                    @"
                   declare  @LineID int = '" + S_LineID + @"'
	               declare  @PartID int = '" + S_PartID + @"'
	               declare  @PartFamilyID int = '" + S_PartFamilyID + @"'
	               declare  @ProductionOrderID int = '" + S_ProductionOrderID + @"'
	               declare  @StationTypeID int = '" + S_StationTypeID + @"'   

                   declare  @SNFormatName varchar(500)=''

	            set @PartID= case when @PartID=0 then null else @PartID end 
	            set @LineID= case when @LineID=0 then null else @LineID end 
	            set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 
	            set @ProductionOrderID= case when @ProductionOrderID=0 then null else @ProductionOrderID end
	            set @StationTypeID= case when @StationTypeID=0 then null else @StationTypeID end

	            if @PartID is not NULL and @PartFamilyID is not NULL begin
		            if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID) SET @SNFormatName=null
	            end else if @PartFamilyID is NULL begin
		            select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	            end

	            select top 1 @SNFormatName = B.Name
	            from mesSNFormatMap A
	            INNER JOIN mesSNFormat B ON A.SNFormatID=B.ID
	            where
		            (LineID = @LineID or LineID is NULL OR LineID=0) AND
		            (PartID = @PartID or PartID is NULL OR PartID=0) AND
		            (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		            (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0) AND
		            (StationTypeID = @StationTypeID or StationTypeID is NULL OR StationTypeID=0)
	            order by StationTypeID desc,ProductionOrderID desc, PartID desc, LineID desc, PartFamilyID desc

                select @SNFormatName as ValStr1
                ";

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                S_Result = v_Query.ToList()[0].ValStr1;
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                return S_Result;
            }
            return S_Result;
        }

        public async Task<List<TabVal>> GetLabelID(string S_LineID, string S_PartID, string S_PartFamilyID,
            string S_ProductionOrderID, string S_StationTypeID)
        {
            string S_Sql =
                @"
	            DECLARE @LineID int =" + S_LineID + @" 
	            DECLARE @PartID int = " + S_PartID + @"
	            DECLARE @PartFamilyID int = " + S_PartFamilyID + @"  
	            DECLARE @ProductionOrderID int = " + S_ProductionOrderID + @" 
	            DECLARE @StationTypeID int = " + S_StationTypeID + @"                 

	            set @PartID= case when @PartID=0 then null else @PartID end 
	            set @LineID= case when @LineID=0 then null else @LineID end 
	            set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 
	            set @ProductionOrderID= case when @ProductionOrderID=0 then null else @ProductionOrderID end
	            set @StationTypeID= case when @StationTypeID=0 then null else @StationTypeID end

	            if @PartID is not NULL and @PartFamilyID is not NULL 
	            begin
		            if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID)
		            begin
				        select null Valint1,null Valint2
			            return
		            end
	            end 
	            else if @PartFamilyID is NULL begin
		            select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	            end

	            select LabelID Valint1,ID Valint2
	            from mesStationTypeLabelMap
	            where
		            (LineID = @LineID or LineID is NULL OR LineID=0) AND
		            (PartID = @PartID or PartID is NULL OR PartID=0) AND
		            (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		            (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0) AND
		            (StationTypeID = @StationTypeID or StationTypeID is NULL OR StationTypeID=0)
	            order by StationTypeID desc,ProductionOrderID desc, PartID desc, LineID desc, PartFamilyID desc
                ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        public async Task<(string, List<LabelMap>)> GetLabelMap(string S_StationTypeID, string S_PartFamilyID,
            string S_PartID, string S_ProductionOrderID, string S_LineID)
        {
            string S_LabelID = "";
            List<TabVal> List_LabelID = await GetLabelID(S_LineID, S_PartID, S_PartFamilyID, S_ProductionOrderID, S_StationTypeID);
            if (List_LabelID?.Count <= 0)
            {
                return ("ERROR:" + P_MSG_Public.MSG_Public_033, null);
            }

            foreach (var item in List_LabelID)
            {
                S_LabelID = (String.IsNullOrEmpty(S_LabelID)?"":S_LabelID + ",") + item.Valint2.ToString();
            }
            //S_LabelID += List_LabelID[0].Valint1.ToString() + "," + List_LabelID[0].Valint2.ToString();

            string S_Sql =
                @"
                select A.ID, A.StationTypeID,B.Description StationType,
		                A.LabelID,C.Name LabelName,C.SourcePath LabelPath,
		                A.PartFamilyID,E.Name PartFamily,
		                A.PartID,D.PartNumber,		
		                A.ProductionOrderID,F.ProductionOrderNumber,
		                A.LineID,G.Description Line,
                        C.OutputType,C.TargetPath,C.PageCapacity
                from mesStationTypeLabelMap A
		                    left join mesStationType B on A.StationTypeID=B.ID
		                    left join mesLabel C on A.LabelID=C.ID
		                    left join luPartFamily E on A.PartFamilyID=E.ID
		                    left join mesPart D on A.PartID=D.ID           
                            left join mesProductionOrder F on A.ProductionOrderID=F.ID
                            left join mesLine G on A.LineID=G.ID  
                where 1=1 and A.ID in(" + S_LabelID + @")
                ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<LabelMap>(S_Sql, null, null, I_DBTimeout, null);
            return ("1",v_Query.ToList());
        }

        public async Task<string> GetLabelName(string S_StationTypeID, string S_PartFamilyID,
            string S_PartID, string S_ProductionOrderID, string S_LineID)
        {
            string S_LabelName = "";
            try
            {
                 var tmpLableMap = await GetLabelMap(S_StationTypeID,
                    S_PartFamilyID, S_PartID, S_ProductionOrderID, S_LineID);
                if (tmpLableMap.Item1 != "1")
                    return tmpLableMap.Item1;

                List<LabelMap> List_LabelMap = tmpLableMap.Item2;
                int I_LengthPa = 0;
                string S_OutputType = string.Empty;
                string S_Suffix = string.Empty;
                string S_TargetPath = string.Empty;
                string S_PageCapacity = string.Empty;

                string S_Path = "";

                foreach (var item in List_LabelMap)
                {
                    S_Path = item.LabelPath;
                    S_OutputType = item.OutputType;
                    S_TargetPath = item.TargetPath;
                    S_PageCapacity = item.PageCapacity;

                    I_LengthPa = S_Path.LastIndexOf('.');
                    S_Suffix = S_Path.Substring(I_LengthPa + 1, S_Path.Length - I_LengthPa - 1);

                    if (string.IsNullOrEmpty(S_OutputType) || (S_OutputType != "5" && S_OutputType != "6" &&
                        S_OutputType != "7" && S_OutputType != "1" && S_OutputType != "2"))
                    {
                        return "ERROR:" + P_MSG_Public.MSG_Public_033;//未配置打印模板.
                    }

                    if (S_OutputType == "5" && S_Suffix.ToUpper() != "LAB")
                    {
                        return "ERROR:" + P_MSG_Public.MSG_Public_034;//CodeSoft 模板配置错误.
                    }
                    else if (S_OutputType == "6" && S_Suffix.ToUpper() != "BTW")
                    {
                        return "ERROR:" + P_MSG_Public.MSG_Public_035;//BarTender 模板配置错误.
                    }
                    else if (S_OutputType == "7" && S_Suffix.ToUpper() != "BTW")
                    {
                        return "ERROR:" + P_MSG_Public.MSG_Public_035;//BarTender 模板配置错误.
                    }
                    else if (S_OutputType == "2" && S_Suffix.ToUpper() != "PRN")
                    {
                        return "ERROR:" + P_MSG_Public.MSG_Public_036;//此工序没有配置打印标签.
                    }

                    S_LabelName = (string.IsNullOrEmpty(S_LabelName) ? "" : S_LabelName + ";") + item.LabelName +
                        "," + item.LabelPath + "," + S_OutputType + "," + S_TargetPath + "," + S_PageCapacity;
                }
            }
            catch (Exception ex)
            {
                S_LabelName = "ERROR:" + ex.Message;
            }

            return S_LabelName;
        }

        /// <summary>
        /// SN基础信息
        /// </summary>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public async Task<List<SNBaseData>> GetSNBaseData(string S_SN)
        {
            string S_Sql =
                @"DECLARE @SN varchar(200)
                  set @SN='" + S_SN + @"'   
                  IF EXISTS(SELECT 1 FROM mesMachine WHERE SN='" + S_SN + @"')  
                  BEGIN  
                   SELECT TOP 1 @SN =Value FROM mesSerialNumber A  
                            INNER JOIN mesUnitDetail B ON A.UnitID = B.UnitID  
                            WHERE B.reserved_03 = 1 AND B.reserved_01 = '" + S_SN + @"'  
                            ORDER BY B.ID DESC  
                  END  
  
                  IF EXISTS(SELECT 1 FROM mesSerialNumber WHERE Value =@SN)  
                  BEGIN  
                   SELECT top 1 B.ProductionOrderID,B.PartID,C.PartFamilyID,D.PartFamilyTypeID,C.PartNumber,
                        E.Description as LineName,A.SerialNumberTypeID,A.UnitID   
                   FROM mesSerialNumber A  
                   INNER JOIN mesUnit B ON A.UnitID=B.ID  
                   INNER JOIN mesPart C ON B.PartID=C.ID  
                   INNER JOIN luPartFamily D ON C.PartFamilyID=D.ID  
                   inner join mesLine E on B.LineID=E.ID
                   WHERE A.Value=@SN  
                  END";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<SNBaseData>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// Unit 信息
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_PartID"></param>
        /// <returns></returns>
        public async Task<List<mesUnit>> GetCheckUnitInfo(string S_SN, string S_POID, string S_PartID)
        {
            string S_Sql = @"SELECT b.*,d.PartFamilyID
                            FROM dbo.mesSerialNumber a
                            JOIN dbo.mesUnit b ON b.ID = a.UnitID
                            JOIN dbo.mesProductionOrder c ON b.ProductionOrderID = c.ID AND b.PartID = c.PartID
                            JOIN dbo.mesPart d ON d.ID = b.PartID
                            WHERE a.Value = '" + S_SN + "' AND b.PartID = '" + S_PartID +
                            "' AND b.ProductionOrderID = '" + S_POID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        ///  TT Unit 信息
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_PartID"></param>
        /// <returns></returns>
        public async Task<List<mesUnit>> GetCheckUnitInfoTT(string S_SN, string S_POID, string S_PartID)
        {
            S_POID = S_POID ?? "0";

            string S_Sql = @"SELECT b.*,d.PartFamilyID
                            FROM dbo.mesSerialNumber a
                            JOIN dbo.mesUnit b ON b.ID = a.UnitID
                            LEFT JOIN dbo.mesProductionOrder c ON b.ProductionOrderID = c.ID AND b.PartID = c.PartID
                            JOIN dbo.mesPart d ON d.ID = b.PartID
                            WHERE a.Value = '" + S_SN + "' AND b.PartID = '" + S_PartID +
                            "' AND (b.ProductionOrderID=0 or b.ProductionOrderID = '" + S_POID + "')";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }


        /// <summary>
        /// GetUnitStateID
        /// </summary>
        /// <param name="I_RouteID"></param>
        /// <param name="I_StationTypeID"></param>
        /// <param name="I_StatusID"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetUnitStateID(int I_RouteID, int I_StationTypeID, int I_StatusID)
        {
            string S_Sql = @"DECLARE @UnitStateID varchar(64)
	            IF EXISTS(SELECT 1 FROM mesRoute WHERE ID='" + I_RouteID + @"' AND RouteType=1)
	            BEGIN
		            SELECT TOP 1 @UnitStateID = OutputStateID FROM mesUnitOutputState 
		            WHERE RouteID ='" + I_RouteID + @"' AND StationTypeID='" + I_StationTypeID + @"' and OutputStateDefID='" + I_StatusID + @"'
	            END
	            ELSE
	            BEGIN
		            SELECT @UnitStateID=UnitStateID FROM mesRouteDetail  
		            WHERE RouteID='" + I_RouteID + @"' AND StationTypeID='" + I_StationTypeID + @"'
	            END
	            Select @UnitStateID ValInt1";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// GetmesUnitState
        /// </summary>
        /// <param name="I_PartID"></param>
        /// <param name="I_PartFamilyID"></param>
        /// <param name="I_LineID"></param>
        /// <param name="I_StationTypeID"></param>
        /// <param name="I_ProductionOrderID"></param>
        /// <param name="I_StatusID"></param>
        /// <returns></returns>
        public async Task<List<IdDescription>> GetmesUnitState(int I_PartID, int I_PartFamilyID, int I_LineID,
            int I_StationTypeID, int I_ProductionOrderID, int I_StatusID)
        {
            List<mesRoute> List_Route = await GetmesRoute(I_LineID, I_PartID, I_PartFamilyID, I_ProductionOrderID);
            if (List_Route == null)
            {
                return null;
            }

            int I_RouteID = List_Route[0].ID;
            List<TabVal> List_UnitStateID = await GetUnitStateID(I_RouteID, I_StationTypeID, I_StatusID);

            if (List_UnitStateID == null)
            {
                return null;
            }

            int I_UnitStateID = Convert.ToInt32(List_UnitStateID[0].Valint1);
            if (I_UnitStateID == -1)
            {
                return null;
            }

            string S_Sql = "select * from mesUnitState where ID='" + I_UnitStateID + "'";

            var v_Query = await DapperConn.QueryAsync<IdDescription>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 工单数量
        /// </summary>
        /// <param name="I_StationID"></param>
        /// <param name="I_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProductionOrderQTY(int I_StationID, int I_ProductionOrderID)
        {
            string S_Sql =
            @"
		        --根据工站类型统计工单数量
	            DECLARE @prodId				int,
			            @stationID			int,
			            @stationTypeID		int,
			            @prodCount			int,
			            @currentCount		int

		        SELECT @stationTypeID=StationTypeID FROM mesStation WITH(NOLOCK) WHERE ID='" + I_StationID + @"'

		        SELECT @prodCount=OrderQuantity FROM mesProductionOrder WITH(NOLOCK) WHERE ID='" + I_ProductionOrderID + @"'

		        SELECT @currentCount=SUM(ConsumeQTY) FROM mesProductionOrderConsumeInfo WITH(NOLOCK)
			        WHERE ProductionOrderID='" + I_ProductionOrderID + @"' AND StationTypeID=@stationTypeID

		        SELECT ISNULL(@currentCount,0) AS currentCount,ISNULL(@prodCount,0) AS prodCount
            ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 不良代码
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetDefect(string S_PartFamilyTypeID)
        {
            string S_Sql =
            @"
                 Declare @DefectTypeID int,
                         @PartFamilyType  nvarchar(100) 

                 SELECT  @PartFamilyType=Name FROM luPartFamilyType WHERE ID='" + S_PartFamilyTypeID + @"'
                 select @DefectTypeID=ID from luDefectType where Description=@PartFamilyType 

                 select                                
                        A.ID,  
	                    A.DefectCode DefectCode,                     
	                    A.Description DefectName,
	                    C.Description Location	                             
                from 
	                (select *  from  luDefect)A 
	                left join luDefectType B on A.DefectTypeID=B.ID
	                left join luLocaltion C on A.LocaltionID=C.ID
	                left join sysStatus D on A.Status=D.ID                            
                where A.DefectTypeID=@DefectTypeID
                order by ID

            ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 工站配置信息
        /// </summary>
        /// <param name="S_SetName"></param>
        /// <param name="S_StationID"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetStationConfigSetting(string S_SetName, string S_StationID)
        {
            string S_Sql = "select Name ValStr1,Value ValStr2 from mesStationConfigSetting where StationID='" + S_StationID + "'";

            if (S_SetName != "")
            {
                S_Sql = "select Name ValStr1,Value ValStr2 from mesStationConfigSetting where Name='" +
                   S_SetName + "' and StationID='" + S_StationID + "'";
            }
            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 工站类型详细
        /// </summary>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetStationTypeDetail(string S_StationTypeID)
        {
            string S_Sql = @"SELECT A.Content ValStr1,B.Description ValStr2,D.[Description]  ValStr3
                              FROM mesStationTypeDetail A
                                LEFT JOIN luStationTypeDetailDef B ON A.StationTypeDetailDefID = B.ID
                                LEFT JOIN mesStationType C ON A.StationTypeID=C.ID
                                LEFT JOIN luApplicationType D ON C.ApplicationTypeID=D.ID
                            WHERE A.StationTypeID ='" + S_StationTypeID + "' ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// GetDataSql
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetDataSql(string S_Sql)
        {
            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        /// <summary>
        /// 夹具检查
        /// </summary>
        /// <param name="S_MachineSN"></param>
        /// <param name="S_MainCode"></param>
        /// <param name="S_PartID"></param>
        /// <param name="Login_List"></param>
        /// <returns></returns>
        public async Task<string> GetMachineToolingCheck(string S_MachineSN, string S_MainCode, string S_PartID, LoginList Login_List)
        {
            string S_Result = "1";

            string S_Sql =
            @"
	            declare	@MachineSN          varchar(500)='" + S_MachineSN + @"',
			            @PartId				int='" + S_PartID + @"',
			            @S_StatusID			int,
			            @ValidFrom			varchar(500)='',
			            @ValidTo			varchar(500)='',
			            @ValidDistribution	varchar(500)='',
			            @ValidValue			varchar(64)='',
			            @MainCode			varchar(64)='" + S_MainCode + @"',
			            @WarningStatus		int,
			            @MachineFamilyID	int,
			            @MachinePartID		int,
			            @RuningQTY			int,
			            @MaxQTY				int,
			            @RuningStationTypeID    int,
			            @CheckStatus		int,

                        @strOutput        nvarchar(200)

            BEGIN TRY
		        --判断是否存在Machine
		        IF NOT EXISTS(SELECT 1 FROM mesMachine WHERE SN=@MachineSN)
		        BEGIN
                    --条码不存在 
			        SET @strOutput='" + P_MSG_Public.MSG_Public_018 + @"'
                    Select  @strOutput as ValStr1
			        RETURN
		        END

		        SELECT @S_StatusID=StatusID,@ValidFrom=ValidFrom,@ValidDistribution=ValidDistribution,@CheckStatus=CheckStatus,
			        @RuningQTY=RuningCapacityQuantity,@RuningStationTypeID=RuningStationTypeID,@ValidTo=ValidTo,@WarningStatus=WarningStatus, 
			        @MachineFamilyID=MachineFamilyID,@MachinePartID=PartID FROM mesMachine WHERE SN=@MachineSN

		        --检查工位是否配置
		        IF ISNULL(@ValidFrom,'')=''
		        BEGIN
                    --未配置ValidFrom参数,请确认.
			        SET @strOutput='" + P_MSG_Public.MSG_Public_046 + @"'
                    Select  @strOutput as ValStr1
			        RETURN
		        END
		        IF ISNULL(@ValidTo,'')=''
		        BEGIN
                    --未配置ValidTo参数,请确认.
			        SET @strOutput='" + P_MSG_Public.MSG_Public_047 + @"'
                    Select  @strOutput as ValStr1
			        RETURN
		        END

		        --料号检查
		        IF ISNULL(@PartId,0)>0
		        BEGIN 
			        IF NOT EXISTS(SELECT 1 from  mesRouteMachineMap where (MachineFamilyID = @MachineFamilyID  OR ISNULL(MachineFamilyID,0)=0)
	                        AND (MachinePartID = @MachinePartID OR ISNULL(MachinePartID,0)=0) AND PartID=@PartId)
			        BEGIN
                        --未分配当前料号,请确认.
			            SET @strOutput='" + P_MSG_Public.MSG_Public_048 + @"'
                        Select  @strOutput as ValStr1
			            RETURN
			        END
		        END


		        --校验夹具状态
		        IF @S_StatusID=3
		        BEGIN
                    --夹具已停用,请更换夹具.
			        SET @strOutput='" + P_MSG_Public.MSG_Public_049 + @"'
                    Select  @strOutput as ValStr1
			        RETURN
		        END

		        IF exists(SELECT 1 FROM [dbo].[F_Split](@ValidFrom,';') WHERE value='" + Login_List.StationTypeID + @"')
		        BEGIN
			        IF @S_StatusID<>1
			        BEGIN
			            if exists(select 1 from mesUnitDetail where reserved_01=@MachineSN) --夹具有虚拟条码
				        begin
					        if not exists(select 1 from mesUnitDetail where reserved_01=@MachineSN and reserved_04='1')
					        begin 
                                --夹具已与其他条码绑定,请更换夹具.    
			                    SET @strOutput='" + P_MSG_Public.MSG_Public_050 + @"'
                                Select  @strOutput as ValStr1
			                    RETURN
					        end
				        end
				        else   --夹具无虚拟条码
				        begin
                                --夹具已与其他条码绑定,请更换夹具.    
			                    SET @strOutput='" + P_MSG_Public.MSG_Public_050 + @"'
                                Select  @strOutput as ValStr1
			                    RETURN			    
				        end
			        END
		        END
		        ELSE
		        BEGIN
			        IF @S_StatusID<>2
			        BEGIN
                        --夹具未与产品绑定   
			            SET @strOutput='" + P_MSG_Public.MSG_Public_026 + @"'
                        Select  @strOutput as ValStr1
			            RETURN	
			        END
		        END

		        --校验ValidDistribution配置
		        IF @WarningStatus IN (1,3)
		        BEGIN
			        SELECT @ValidValue=value FROM [dbo].[F_Split](@ValidDistribution,';') WHERE LEFT(value,CHARINDEX(',',value)-1)=" + Login_List.StationTypeID + @"
			        IF ISNULL(@ValidValue,'')=''
			        BEGIN
				       --治具参数ValidDistribution未配置当前工序过站次数.				        
			            SET @strOutput='" + P_MSG_Public.MSG_Public_051 + @"'
                        Select  @strOutput as ValStr1
			            RETURN	
			        END

			        --校验使用次数
			        IF " + Login_List.StationTypeID + @"=ISNULL(@RuningStationTypeID,'')
			        BEGIN
				        SET @MaxQTY = RIGHT(@ValidValue,len(@ValidValue)-CHARINDEX(',',@ValidValue))
				        IF @MaxQTY<=isnull(@RuningQTY,0)
				        BEGIN
					        --夹具扫描次数已超过限制次数.
			                SET @strOutput='" + P_MSG_Public.MSG_Public_052 + @"'
                            Select  @strOutput as ValStr1
			                RETURN						        
				        END
			        END
		        END

		        --校验数量是否超过夹具使用次数(WarningStatus大于1的需要校验夹具使用次数)
		        IF @WarningStatus>1
		        BEGIN
			        IF EXISTS (SELECT 1 FROM mesMachine WHERE SN=@MachineSN AND isnull(RuningQuantity,0)>=MaxUseQuantity)
			        BEGIN
				        --使用次数已超过最大限制,请重置.
			            SET @strOutput='" + P_MSG_Public.MSG_Public_053 + @"'
                        Select  @strOutput as ValStr1
			            RETURN					        
			        END
		        END

		        --扫出工位扫入工位条码一致性校验
		        IF exists(SELECT 1 FROM [dbo].[F_Split](@ValidTo,';') WHERE value='" + Login_List.StationTypeID + @"') and @CheckStatus=1 
		        BEGIN
			        IF ISNULL(@MainCode,'')=''
			        BEGIN
				        --解绑夹具校验条码一致性必须先扫描主条码.				        
			            SET @strOutput='" + P_MSG_Public.MSG_Public_054 + @"'
                        Select  @strOutput as ValStr1
			            RETURN	
			        END
			        --主条在mesSerialNumber中, 主码不是扫夹具的情况并且和主SN是同一个工序组装，才判断子码是夹具是否和主SN不一致的情况
			        if exists(select 1 from mesMachine where SN=@MainCode)
			        begin
				        SELECT @MainCode=A.Value FROM  mesSerialNumber   A	
				        INNER JOIN mesUnitDetail uc ON uc.UnitID=A.UnitID   and uc.reserved_03=1 where uc.reserved_01 = @MachineSN and @MainCode=@MachineSN
			        end
			        declare @ActualAssembly int=0
			        SELECT @ActualAssembly=s.StationTypeID FROM mesSerialNumber   A
					        INNER JOIN mesUnitComponent B ON A.UnitID=B.UnitID AND B.StatusID=1
					        INNER JOIN mesUnitDetail C ON B.ChildUnitID=C.UnitID AND C.reserved_01 = @MachineSN
					        join mesStation s on s.ID=B.InsertedStationID
				        where A.Value = @MainCode
			
			        if   ( @ValidFrom=@ActualAssembly )
			        begin
			    
				        IF NOT EXISTS( SELECT 1 FROM (SELECT * FROM mesSerialNumber where Value = @MainCode)  A
					        INNER JOIN mesUnitComponent B ON A.UnitID=B.UnitID AND B.StatusID=1
					        INNER JOIN mesUnitDetail C ON B.ChildUnitID=C.UnitID AND C.reserved_01 = @MachineSN)
				        BEGIN
				            --绑定工位与解绑工位的主条码不一致，无法解绑.
			                SET @strOutput='" + P_MSG_Public.MSG_Public_055 + @"'
                            Select  @strOutput as ValStr1
			                RETURN					            
				        END
			        end 
		        END

		        SELECT @strOutput='1'
                Select  @strOutput as ValStr1
	        END TRY
	        BEGIN CATCH
		        SELECT @strOutput=ERROR_MESSAGE()
                Select  @strOutput as ValStr1
	        END CATCH

            ";

            IEnumerable<TabVal> List_Reult = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            if (List_Reult.Count() > 0)
            {
                S_Result = List_Reult.First().ValStr1;
            }

            return S_Result;
        }


        /// <summary>
        /// 夹具获取产品SN
        /// </summary>
        /// <param name="S_MachineSN"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> MesGetBatchIDByBarcodeSN(string S_MachineSN)
        {
            string S_Sql = string.Format("select * from mesMachine where SN='{0}' AND StatusID=2", S_MachineSN);
            var v_Query = await DapperConn.QueryAsync<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query != null && v_Query.Count() > 0)
            {
                S_Sql = string.Format("SELECT top 1 reserved_02 as ValStr1,UnitID as ValStr2 FROM mesUnitDetail " +
                    " WHERE reserved_01='{0}' and reserved_03=1 order by id desc", S_MachineSN);

                var v_SN = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                return v_SN.ToList();
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            return null;
        }
        /// <summary>
        /// 获取SN
        /// </summary>
        /// <param name="S_UnitID"></param>
        /// <param name="S_Value"></param>
        /// <returns></returns>
        public async Task<List<mesSerialNumber>> GetmesSerialNumber(string S_UnitID, string S_Value)
        {
            S_UnitID = S_UnitID ?? "";
            S_Value = S_Value ?? "";

            if (S_UnitID == "" && S_Value == "")
            {
                return null;
            }

            string S_Sql = "select * from mesSerialNumber ";
            if (S_UnitID != "") { S_Sql += " where UnitID='" + S_UnitID + "'"; }
            if (S_UnitID == "")
            {
                if (S_Value != "") { S_Sql += " where Value='" + S_Value + "'"; }
            }
            else
            {
                if (S_Value != "") { S_Sql += " and Value='" + S_Value + "'"; }
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesSerialNumber>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取 Unit 和 SerialNumber 关联信息
        /// </summary>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public async Task<List<mesUnitSerialNumber>> GetmesUnitSerialNumber(string S_SN)
        {
            string S_Sql = "select * from mesSerialNumber A join mesUnit B on A.UnitID=B.ID where A.Value='" + S_SN + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnitSerialNumber>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        public async Task<List<mesUnitSerialNumber>> GetmesUnitSerialNumber(string S_UnitID,string S_SN)
        {
            S_UnitID = S_UnitID ?? "";
            S_SN = S_SN ?? "";

            string S_Sql = "select * from mesSerialNumber A join mesUnit B on A.UnitID=B.ID ";

            if (S_UnitID != "") { S_Sql += " where B.ID='" + S_UnitID + "'"; }
            if (S_UnitID == "")
            {
                if (S_SN != "") { S_Sql += " where A.Value='" + S_SN + "'"; }
            }
            else
            {
                if (S_SN != "") { S_Sql += " and A.Value='" + S_SN + "'"; }
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnitSerialNumber>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }


        /// <summary>
        /// GetmesUnit
        /// </summary>
        /// <param name="S_ID"></param>
        /// <returns></returns>
        public async Task<List<mesUnit>> GetmesUnit(string S_ID)
        {
            string S_Sql = "select * from mesUnit Where ID='" + S_ID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// GetmesUnitTTBox
        /// </summary>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public async Task<List<mesUnitTTBox>> GetmesUnitTTBox(string S_SN)
        {
            string S_Sql = @"DECLARE @BoxUnitID INT
                DECLARE @ChildCount INT
                DECLARE @PartID INT
                DECLARE @FullNumber INT=-1

                select @BoxUnitID=UnitID FROM mesSerialNumber    WHERE [Value]='" + S_SN + @"' 


                SELECT @ChildCount=COUNT(*) FROM mesUnit WHERE PanelID=@BoxUnitID
                SELECT @PartID=PartID FROM mesUnit WHERE ID=@BoxUnitID

                select @FullNumber=a.[Content] from mesPartDetail a  WHERE  a.PartID=@PartID 
                and exists (select 1 from luPartDetailDef b where a.PartDetailDefID = b.ID and b.Description ='FullNumber')

                select A.*,B.Value SN, @ChildCount ChildCount,@FullNumber FullNumber,C.reserved_04 BoxSNStatus
                  FROM mesUnit A 
                    JOIN mesSerialNumber B ON A.ID=B.UnitID   
                    JOIN mesUnitDetail C ON A.ID=C.UnitID     
                WHERE B.[Value]='" + S_SN + @"' and C.reserved_02='" + S_SN + "' Order by A.ID";

            var v_Query = await DapperConn.QueryAsync<mesUnitTTBox>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query.Count() < 1)
            {
                S_Sql = "select -1 ID, 0 ChildCount,10 FullNumber,1 BoxSNStatus";
                v_Query = await DapperConn.QueryAsync<mesUnitTTBox>(S_Sql, null, null, I_DBTimeout, null);
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            return v_Query.ToList();
        }

        public async Task<List<mesUnitDetail>> GetmesUnitDetail(string UnitID) 
        {
            string S_Sql = "select * from mesUnitDetail A where A.UnitID='" + UnitID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnitDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }


        public async Task<List<TabVal>> GetPONumberCheck(string ProductionOrderID, string S_OperationNumber)
        {
            string S_Sql = "";
            S_Sql =
              @"
                    declare @strOutput nvarchar(200)  
                    SET @strOutput='1'

	                declare @OrderQuantity		int,
			                @NowNumber			int

		            IF NOT EXISTS(SELECT 1 FROM mesProductionOrder WHERE ID='" + ProductionOrderID + @"')
		            BEGIN
			            SET @strOutput='" + P_MSG_Public.MSG_Public_013 + @"'
                        select @strOutput ValStr1
			            RETURN
		            END

		            IF NOT EXISTS(SELECT 1 FROM mesProductionOrder WHERE ID='" + ProductionOrderID + @"' AND StatusID=1)
		            BEGIN
			            SET @strOutput='" + P_MSG_Public.MSG_Public_030 + @"'
                        select @strOutput ValStr1
			            RETURN
		            END

		            SELECT @OrderQuantity=OrderQuantity FROM mesProductionOrder WHERE ID='" + ProductionOrderID + @"'
		            SELECT @NowNumber=count(1) FROM mesUnit WHERE ProductionOrderID='" + ProductionOrderID + @"'

		            IF isnull(@OrderQuantity,0)<isnull(@NowNumber,0)+isnull('" + S_OperationNumber + @"',0)
		            BEGIN
			            SET @strOutput='" + P_MSG_Public.MSG_Public_031 + @"'
                        select @strOutput ValStr1
			            RETURN
		            END
                    
                    select @strOutput ValStr1
               ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //对象名称,属性名
        //返回该对象下的数据内容
        public object GetPropertyValue(object info, string field)
        {
            if (info == null) return null;
            Type t = info.GetType();
            IEnumerable<System.Reflection.PropertyInfo> property =
                from pi in t.GetProperties() where pi.Name == field select pi;
            return property.First().GetValue(info, null);
        }


        public async Task<string> MesModMachineBySNStationTypeID_Sql(string MachineSN, int StationTypeID)
        {
            string S_Result = "";
            string S_Sql = string.Empty;

            try
            {
                S_Sql = string.Format(@"select WarningStatus,ValidFrom,ValidTo,RuningStationTypeID,
                                    RuningCapacityQuantity,ValidDistribution from mesMachine where SN='{0}'", MachineSN);
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var Query_mesMachine = await DapperConn.QueryAsync<mesMachine>(S_Sql, null, null, I_DBTimeout, null);

                if (Query_mesMachine != null || Query_mesMachine.Count() > 0)
                {
                    mesMachine v_mesMachine = Query_mesMachine.AsList()[0];

                    int WarningStatus = Convert.ToInt32(v_mesMachine.WarningStatus);
                    int RuningCapacityQuantity = 0;
                    if (v_mesMachine.RuningCapacityQuantity.ToString() != "0")
                    {
                        RuningCapacityQuantity = Convert.ToInt32(v_mesMachine.RuningCapacityQuantity);
                    }
                    string[] StationFromList = v_mesMachine.ValidFrom.ToString().Split(';');
                    string[] StationToList = v_mesMachine.ValidTo.ToString().Split(';');
                    string[] ValidDistributionList = v_mesMachine.ValidDistribution.ToString().Split(';');
                    string RuningStationTypeID = v_mesMachine.RuningStationTypeID.ToString();

                    if (WarningStatus == 1 || WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (RuningStationTypeID != StationTypeID.ToString())
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningStationTypeID={1},RuningCapacityQuantity=1 where SN = '{0}'", MachineSN, StationTypeID);
                            RuningCapacityQuantity = 1;
                            RuningStationTypeID = StationTypeID.ToString();
                        }
                        else
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningCapacityQuantity=isnull(RuningCapacityQuantity,0)+1 where SN = '{0}'", MachineSN);
                            RuningCapacityQuantity = RuningCapacityQuantity + 1;
                        }
                        //SqlServerHelper.ExecSql(S_Sql);
                        S_Result += "\r\n" + S_Sql;
                    }

                    if (WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (StationFromList.Contains(StationTypeID.ToString()))
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningQuantity=isnull(RuningQuantity,0)+1 where SN = '{0}'", MachineSN);
                            //SqlServerHelper.ExecSql(S_Sql);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }

                    if (StationFromList.Contains(StationTypeID.ToString()))
                    {
                        S_Sql = string.Format("Update mesMachine set StartRuningTime=GETDATE(),StatusID='2' where SN = '{0}'", MachineSN);
                        //SqlServerHelper.ExecSql(S_Sql);
                        S_Result += "\r\n" + S_Sql;
                    }
                    if (StationToList.Contains(StationTypeID.ToString()))
                    {
                        if (WarningStatus == 1 || WarningStatus == 3)
                        {
                            int qty = 0;
                            foreach (string str in ValidDistributionList)
                            {
                                string[] strList = str.Split(',');
                                if (strList[0].ToString() == StationTypeID.ToString())
                                {
                                    qty = Convert.ToInt32(strList[1].ToString());
                                    break;
                                }
                            }

                            if (RuningCapacityQuantity >= qty && RuningStationTypeID == StationTypeID.ToString())
                            {
                                S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID=1 where SN = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                                S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                            }
                        }
                        else
                        {
                            S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID='1' where SN = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                            S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                return S_Result;
            }

            return S_Result;
        }


        /// <summary>
        /// GetPageInitialize 页面初始化
        /// </summary>
        /// <param name="S_StationID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<List<IsCheckPOPN>> GetPageInitialize(LoginList List_Login, string S_URL)
        {
            List<IsCheckPOPN> List_Result = null;

            try
            {
                List<TabVal> List_StationConfig = await GetStationConfigSetting("", List_Login.StationID.ToString());
                List<TabVal> List_StationTypeDetail = await GetStationTypeDetail(List_Login.StationTypeID.ToString());
                S_ApplicationType = await LuGetApplicationType(List_Login.StationID.ToString()); //List_StationTypeDetail[0].ValStr3;

                foreach (var item in List_StationTypeDetail)
                {
                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "IsCheckPO")
                        {
                            if (item.ValStr1.Trim() == "1")
                            {
                                S_IsCheckPO = "1";
                            }
                            else
                            {
                                S_IsCheckPO = "0";
                            }
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "IsCheckPN")
                        {
                            if (item.ValStr1.Trim() == "1")
                            {
                                S_IsCheckPN = "1";
                            }
                            else
                            {
                                S_IsCheckPN = "0";
                            }
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "IsCheckCardID")
                        {
                            if (item.ValStr1.Trim() == "1")
                            {
                                S_IsCheckCardID = "1";
                            }
                            else
                            {
                                S_IsCheckCardID = "0";
                            }
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "TTScanType")
                        {
                            S_TTScanType = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "TTRegistSN")
                        {
                            S_IsTTRegistSN = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "TTBoxType")
                        {
                            S_TTBoxType = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "IsChangePN")
                        {
                            S_IsChangePN = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "IsChangePO")
                        {
                            S_IsChangePO = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "ChangedUnitStateID")
                        {
                            S_ChangedUnitStateID = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "JumpFromUnitStateID")
                        {
                            S_JumpFromUnitStateID = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "JumpToUnitStateID")
                        {
                            S_JumpToUnitStateID = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "JumpStatusID")
                        {
                            S_JumpStatusID = item.ValStr1.Trim();
                        }
                    }

                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "JumpUnitStateID")
                        {
                            S_JumpUnitStateID = item.ValStr1.Trim();
                        }
                    }


                    if (item.ValStr2 != null)
                    {
                        if (item.ValStr2.Trim() == "CardIDPattern")
                        {
                            S_CardIDPattern = item.ValStr1.Trim();
                        }
                    }
                }
                /////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////
                foreach (var item_StationConfig in List_StationConfig)
                {

                    if (item_StationConfig.ValStr1 == "IsCheckPO")
                    {
                        if (item_StationConfig.ValStr2.Trim() == "1")
                        {
                            S_IsCheckPO = "1";
                        }
                        else
                        {
                            S_IsCheckPO = "0";
                        }
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "IsCheckPN")
                    {
                        if (item_StationConfig.ValStr2.Trim() == "1")
                        {
                            S_IsCheckPN = "1";
                        }
                        else
                        {
                            S_IsCheckPN = "0";
                        }
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "IsCheckCardID")
                    {
                        if (item_StationConfig.ValStr2.Trim() == "1")
                        {
                            S_IsCheckCardID = "1";
                        }
                        else
                        {
                            S_IsCheckCardID = "0";
                        }
                        continue;
                    }
                }




                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "TTScanType")
                    {
                        S_TTScanType = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "TTRegistSN")
                    {
                        S_IsTTRegistSN = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "TTBoxType")
                    {
                        S_TTBoxType = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }



                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "IsChangePN")
                    {
                        S_IsChangePN = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "IsChangePO")
                    {
                        S_IsChangePO = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }

                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "ChangedUnitStateID")
                    {
                        S_ChangedUnitStateID = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }

                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "JumpFromUnitStateID")
                    {
                        S_JumpFromUnitStateID = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "JumpToUnitStateID")
                    {
                        S_JumpToUnitStateID = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "JumpStatusID")
                    {
                        S_JumpStatusID = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "JumpUnitStateID")
                    {
                        S_JumpUnitStateID = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "OperationType")
                    {
                        S_OperationType = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }

                foreach (var item_StationConfig in List_StationConfig)
                {
                    if (item_StationConfig.ValStr1 == "CardIDPattern")
                    {
                        S_CardIDPattern = item_StationConfig.ValStr2.Trim();
                        continue;
                    }
                }


                S_IsLegalPage = "0";
                string S_Sql =
                  @"SELECT EnCode AS ValStr1,FullName AS ValStr2 FROM API_Menu  WHERE EnCode='" +
                        S_URL + "' AND FullName='" + S_ApplicationType + "'";
                var v_Query_Menu = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                if (v_Query_Menu != null && v_Query_Menu.Count() > 0)
                {
                    S_IsLegalPage = "1";
                }

                S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                        "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                        "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                        "' as IsTTRegistSN,'" + S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                        S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                        S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                        S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                        S_JumpStatusID + "' as JumpStatusID,'" +
                        S_JumpUnitStateID + "' as JumpUnitStateID,'" +
                        S_OperationType + "' as OperationType,'" +

                        S_IsCheckCardID + "' as IsCheckCardID,'" +
                        S_CardIDPattern + "' as CardIDPattern,'" +

                        S_TTBoxType + "' as TTBoxType";

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                var v_Query = await DapperConn.QueryAsync<IsCheckPOPN>(S_Sql, null, null, I_DBTimeout, null);
                List_Result = v_Query.ToList();
            }
            catch (Exception ex)
            {
                List_Result = new List<IsCheckPOPN>();
                List_Result.Add(new IsCheckPOPN());
                List_Result[0].TabVal = List_ERROR_TabVal(ex, "0", List_Login, "")[0];
            }
            return List_Result;
        }


        /// <summary>
        /// 处理 TabVal 
        /// </summary>
        /// <param name="S_Code">ERROR, OK, 1, 0 ...</param>
        /// <param name="S_MSG"></param>
        /// <param name="S_DataStatus"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetTabVal(string S_Code, string S_MSG, string S_DataStatus)
        {
            string S_Sql = "select '" + S_Code + "' as ValStr1,'" + S_MSG + "' ValStr2,'" + S_DataStatus + "' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<TabVal>> uspCallProcedure(string Pro_Name, string S_FormatName, string xmlProdOrder, string xmlPart,
                                        string xmlStation, string xmlExtraData, string strSNbuf)
        {
            string strOutput = "";
            try
            {
                if (string.IsNullOrEmpty(Pro_Name))
                {
                    //过程名称不能为空.
                    strOutput = P_MSG_Public.MSG_Public_023;
                    return null;
                }
                S_FormatName = string.IsNullOrEmpty(S_FormatName) ? "null" : S_FormatName;
                xmlProdOrder = string.IsNullOrEmpty(xmlProdOrder) ? "null" : xmlProdOrder;
                xmlPart = string.IsNullOrEmpty(xmlPart) ? "null" : xmlPart;
                xmlStation = string.IsNullOrEmpty(xmlStation) ? "null" : xmlStation;
                xmlExtraData = string.IsNullOrEmpty(xmlExtraData) ? "null" : xmlExtraData;
                strSNbuf = string.IsNullOrEmpty(strSNbuf) ? "null" : strSNbuf;

                DynamicParameters dp = new DynamicParameters();
                dp.Add("@strSNFormat", S_FormatName, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@xmlProdOrder", xmlProdOrder, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@xmlPart", xmlPart, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@xmlStation", xmlStation, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@xmlExtraData", xmlExtraData, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@strSNbuf", strSNbuf, DbType.String, ParameterDirection.Input, 100);
                dp.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 100);

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                var v_Query = await DapperConnRead2.ExecuteAsync(Pro_Name, dp, null, I_DBTimeout, CommandType.StoredProcedure);
                strOutput = dp.Get<string>("@strOutput");

            }
            catch (Exception ex)
            {
                strOutput = ex.Message.ToString();
            }

            List<TabVal> List_TabVal = await GetTabVal(strOutput, "", "1");
            return List_TabVal;
        }
        /// <summary>
        /// TimeCheck 时间检查
        /// </summary>
        /// <param name="StationID"></param>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> TimeCheck(string StationID, string S_SN)
        {
            string S_Sql = "exec [uspTimeCheck] '" + StationID + "','" + S_SN + "'";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            string S_Value = v_Query.ToList()[0].ValStr1;
            S_Value = S_Value ?? "1";

            List<TabVal> List_TabVal = await GetTabVal(S_Value, "", "1");

            return List_TabVal;
        }

        public async Task<List<TabVal>> GetPattern(string S_PartID, LoginList List_Login)
        {
            List<TabVal> List_Result = new List<TabVal>();
            string S_Batch_Pattern = "";
            string S_SN_Pattern = "";

            try
            {
                int I_PartID = Convert.ToInt32(S_PartID);
                List<mesPartDetail> List_mesPartDetail = await GetmesPartDetail(I_PartID, "Batch_Pattern");

                if (List_mesPartDetail.Count > 0)
                {
                    S_Batch_Pattern = List_mesPartDetail[0].Content.Trim();
                    S_Batch_Pattern = PublicF.EncryptPassword(S_Batch_Pattern, S_PwdKey);
                }
                else
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_014, "0", List_Login, "");//未配置校验批次正则表达式
                    return List_Result;
                }

                List_mesPartDetail = await GetmesPartDetail(I_PartID, "SN_Pattern");
                if (List_mesPartDetail.Count > 0)
                {
                    S_SN_Pattern = List_mesPartDetail[0].Content.Trim();
                    S_SN_Pattern = PublicF.EncryptPassword(S_SN_Pattern, S_PwdKey);
                }
                else
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_015, "0", List_Login, "");//未配置校验SN正则表达式
                    return List_Result;
                }

                string S_Sql = "select '" + S_Batch_Pattern + "' as ValStr1,'" + S_SN_Pattern + "' ValStr2";
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                return v_Query.AsList();
            }
            catch (Exception ex)
            {
                List_Result = List_ERROR_TabVal(ex, "0", List_Login, "");
            }

            return List_Result;
        }



        /// <summary>
        /// SetConfirmPO 确认工单 
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, LoginList List_Login)
        {
            string S_LineID = List_Login.LineID.ToString();

            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "0";
            S_PartFamilyID = S_PartFamilyID ?? "0";
            S_PartID = S_PartID ?? "0";
            S_POID = S_POID ?? "0";
            S_UnitStatus = S_UnitStatus ?? "0";

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            ConfirmPOOutputDto F_ConfirmPOOutputDto = new ConfirmPOOutputDto();
            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                if (S_POID == "0" && S_IsCheckPO == "1")
                {
                    TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_009, "0", List_Login, "");//工单不能为空,请确认
                    F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                    return F_ConfirmPOOutputDto;
                }
                else
                {
                    if (S_IsCheckPN == "1")
                    {
                        if (S_PartFamilyTypeID == "0")
                        {
                            TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_010, "0", List_Login, "");//未选择料号类别,请确认
                            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                            return F_ConfirmPOOutputDto;
                        }
                        if (S_PartFamilyID == "0")
                        {
                            TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_011, "0", List_Login, "");//未选择料号群,请确认.
                            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                            return F_ConfirmPOOutputDto;
                        }
                        if (S_PartID == "0")
                        {
                            TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_012, "0", List_Login, "");//未选择料号,请确认.
                            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                            return F_ConfirmPOOutputDto;
                        }
                    }

                    if (S_IsCheckPO == "0")
                    {
                        List<mesLineOrder> list_mesLineOrder = await GetmesLineOrder(List_Login.LineID.ToString(), S_PartID, "");
                        if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                        {
                            TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_024, "0", List_Login, "");//料号和线别不匹配.
                            F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                            return F_ConfirmPOOutputDto;
                        }
                    }

                    List<TabVal> List_Pattern = await GetPattern(S_PartID, List_Login);
                    if (List_Pattern[0].ValStr2 == "0")
                    {
                        TabVal_MSGERROR = GetERROR(List_Pattern[0].ValStr1, "0", List_Login, "");
                        F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                        return F_ConfirmPOOutputDto;
                    }


                    // BOM
                    List<mesProductStructure> List_mesProductStructure = await GetBOMStructure(S_PartID, "", "");
                    F_ConfirmPOOutputDto.mesProductStructures = List_mesProductStructure;

                    //Route
                    List<mesRoute> List_Route = await GetmesRoute(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                                    Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_POID));
                    List_Route = List_Route ?? new List<mesRoute>();

                    if (List_Route.Count == 0)
                    {
                        TabVal_MSGERROR = GetERROR(P_MSG_Public.MSG_Public_001, "0", List_Login, "");//料号未配置工艺流程路线.
                        F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
                        return F_ConfirmPOOutputDto;
                    }

                    int I_RouteID = List_Route[0].ID;
                    int I_RouteType = List_Route[0].RouteType;
                    if (I_RouteType == 1)
                    {
                        List<dynamic> List_RouteDataDiagram1 = await
                            GetRouteDataDiagram(List_Login.StationTypeID.ToString(), I_RouteID, "1");
                        F_ConfirmPOOutputDto.RouteDataDiagram1 = List_RouteDataDiagram1;

                        List<dynamic> List_RouteDataDiagram2 = await
                            GetRouteDataDiagram(List_Login.StationTypeID.ToString(), I_RouteID, "2");
                        F_ConfirmPOOutputDto.RouteDataDiagram2 = List_RouteDataDiagram2;
                    }
                    else
                    {
                        List<dynamic> List_RouteDetail = await
                            GetRouteDetail(S_LineID, S_PartID, S_PartFamilyID, S_POID);
                        F_ConfirmPOOutputDto.RouteDetail = List_RouteDetail;
                    }
                    //工单数量
                    List<dynamic> List_ProductionOrderQTY = await
                        GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                    F_ConfirmPOOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;
                }
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = GetERROR(ex, "0", List_Login, "");
                F_ConfirmPOOutputDto.MSG = TabVal_MSGERROR;
            }
            return F_ConfirmPOOutputDto;
        }

        /// <summary>
        /// GetScanSNCheck 扫描时的检查
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="S_DefectID"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetScanSNCheck(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, LoginList List_Login)
        {
            List<TabVal> List_Result = new List<TabVal>();

            try
            {
                if (S_SN == "")
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);//条码不能为空.                
                    return List_Result;
                }

                UnitStatus Enum_UnitStatus = PublicF.ToEnum<UnitStatus>(S_UnitStatus);
                if (Enum_UnitStatus == UnitStatus.FAIL || Enum_UnitStatus == UnitStatus.SCRAP)
                {
                    if (S_DefectID == "")
                    {
                        List_Result = List_ERROR_TabVal(
                            P_MSG_Public.MSG_Public_017, "0", List_Login, S_SN);//确认此物料NG,请设置NG原因.
                        return List_Result;
                    }
                }

                List<SNBaseData> List_SNBaseData = await GetSNBaseData(S_SN);
                if (List_SNBaseData == null || List_SNBaseData.Count < 1)
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_018, "0", List_Login, S_SN);//条码不存在或者状态不符.
                    return List_Result;
                }

                if (S_IsCheckPN == "0")
                {
                    S_PartID = List_SNBaseData[0].PartID.ToString();
                    S_PartFamilyID = List_SNBaseData[0].PartFamilyID.ToString();
                }
                if (S_IsCheckPO == "0")
                {
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();

                    List<mesLineOrder> list_mesLineOrder = await GetmesLineOrder(List_Login.LineID.ToString(), S_POID);
                    if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                    {
                        List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_024, "0", List_Login, S_SN);//料号和线别不匹配.
                        return List_Result;
                    }
                }

                List<mesUnit> List_mesUnit_Get = await GetCheckUnitInfo(S_SN, S_POID, S_PartID);
                if (List_mesUnit_Get.Count < 1)
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_019, "0", List_Login, S_SN);//此条码和选择的工单不一致.
                    return List_Result;
                }

                string S_StatusID = List_mesUnit_Get[0].StatusID.ToString();
                UnitStatus Enum_StatusID = PublicF.ToEnum<UnitStatus>(S_StatusID);
                if (Enum_StatusID != UnitStatus.PASS)
                {
                    //List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_020, "0", List_Login, S_SN);//此条码已NG
                    //return List_Result;
                }

                string S_RouteCheck = await GetRouteCheck(Convert.ToInt32(List_Login.StationTypeID),
                    List_Login.StationID, List_Login.LineID.ToString(), List_mesUnit_Get[0], S_SN);

                if (S_RouteCheck == "1")
                {
                    List_Result = await GetTabVal("1", "", "1");
                    List_Result[0].UnitID = List_mesUnit_Get[0].ID.ToString();
                    List_Result[0].PartID = S_PartID;
                    List_Result[0].PartFamilyID = S_PartFamilyID;
                    List_Result[0].ProductionOrderID = S_POID;
                    List_Result[0].StatusID = S_StatusID;

                    int I_PartID = Convert.ToInt32(S_PartID);
                    int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    int I_LineID = List_Login.LineID;
                    int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                    int I_ProductionOrderID = Convert.ToInt32(S_POID);
                    int I_StatusID = Convert.ToInt32(S_UnitStatus);

                    List<TabVal> List_TimeCheck = await TimeCheck(List_Login.StationID.ToString(), S_SN);
                    string S_IsTimeCheck = List_TimeCheck[0].ValStr1;
                    if (S_IsTimeCheck != "1")
                    {
                        List_Result = List_ERROR_TabVal(S_IsTimeCheck, "0", List_Login, S_SN);
                        return List_Result;
                    }

                    List<TabVal> List_ProCheck = await List_ExecProc("", S_SN, S_POID, S_PartID, null, null, List_Login);
                    string S_ProCheck = List_ProCheck[0].ValStr1;
                    if (S_ProCheck != "1")
                    {
                        List_Result = List_ERROR_TabVal(S_ProCheck, "0", List_Login, S_SN);
                        return List_Result;
                    }

                    List<IdDescription> List_mesUnitState = new List<IdDescription>();
                    List_mesUnitState = await GetmesUnitState(I_PartID, I_PartFamilyID,
                        I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);


                    if (List_mesUnitState == null || List_mesUnitState.Count == 0)
                    {
                        //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                        List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                        return List_Result;
                    }
                    int I_UnitStateID = List_mesUnitState[0].ID;

                    ///////////////////////////////////////////////////////////////////
                    //二次检查是否有指定线路  20231214

                    var specialUnitState = await GetmesUnitStateSecondAsync(I_PartID.ToString(), I_PartFamilyID.ToString(), "",
                        I_LineID.ToString(), I_StationTypeID, I_ProductionOrderID.ToString(), I_StatusID.ToString(), S_SN);

                    if (!string.IsNullOrEmpty(specialUnitState.errorCode))
                    {
                        List_Result = List_ERROR_TabVal(specialUnitState.errorCode, "0", List_Login, S_SN);
                        return List_Result;
                    }
                    I_UnitStateID = specialUnitState.unitStateId.ToInt();
                    ////////////////////////////////////////////////////////////////////
                    List_Result[0].UnitStateID = I_UnitStateID.ToString();
                }
                else
                {
                    List_Result = List_ERROR_TabVal(S_RouteCheck, "0", List_Login, S_SN);
                }
            }
            catch (Exception ex)
            {
                List_Result = List_ERROR_TabVal(ex.Message, "0", List_Login, S_SN);
                return List_Result;
            }

            return List_Result;
        }

        /// <summary>
        /// GetScanSNCheck TT 扫描时的检查
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="S_DefectID"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetScanSNCheckTT(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, LoginList List_Login)
        {
            List<TabVal> List_Result = new List<TabVal>();

            try
            {
                if (S_SN == "")
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);//条码不能为空.                
                    return List_Result;
                }

                UnitStatus Enum_UnitStatus = PublicF.ToEnum<UnitStatus>(S_UnitStatus);
                if (Enum_UnitStatus == UnitStatus.FAIL || Enum_UnitStatus == UnitStatus.SCRAP)
                {
                    if (S_DefectID == "")
                    {
                        List_Result = List_ERROR_TabVal(
                            P_MSG_Public.MSG_Public_017, "0", List_Login, S_SN);//确认此物料NG,请设置NG原因.
                        return List_Result;
                    }
                }

                List<SNBaseData> List_SNBaseData = await GetSNBaseData(S_SN);
                if (List_SNBaseData == null || List_SNBaseData.Count < 1)
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_018, "0", List_Login, S_SN);//条码不存在或者状态不符.
                    return List_Result;
                }


                string v_Sql = "select reserved_01 ValStr1,reserved_02 ValStr2,reserved_04 ValStr3 from mesUnitDetail where (reserved_01='" +
                    S_SN + "' or reserved_02='" + S_SN + "' )and reserved_04='1'";
                var v_Query = await DapperConn.QueryAsync<TabVal>(v_Sql, null, null, I_DBTimeout, null);

                if (v_Query.Count() > 0)
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_059, "0", List_Login, S_SN);//箱子没有关闭
                    return List_Result;
                }

                //if (S_IsCheckPN == "0")
                {
                    S_PartID = List_SNBaseData[0].PartID.ToString();
                    S_PartFamilyID = List_SNBaseData[0].PartFamilyID.ToString();
                }
                //if (S_IsCheckPO == "0")
                {
                    S_POID = List_SNBaseData[0].ProductionOrderID.ToString();

                    if (S_POID != "0" && S_POID != "")
                    {
                        List<mesLineOrder> list_mesLineOrder = await GetmesLineOrder(List_Login.LineID.ToString(), S_POID);
                        if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                        {
                            List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_024, "0", List_Login, S_SN);//料号和线别不匹配.
                            return List_Result;
                        }
                    }
                }

                List<mesUnit> List_mesUnit_Get = await GetCheckUnitInfoTT(S_SN, S_POID, S_PartID);
                if (List_mesUnit_Get.Count < 1)
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_019, "0", List_Login, S_SN);//此条码和选择的工单不一致.
                    return List_Result;
                }

                string S_StatusID = List_mesUnit_Get[0].StatusID.ToString();
                UnitStatus Enum_StatusID = PublicF.ToEnum<UnitStatus>(S_StatusID);
                if (Enum_StatusID != UnitStatus.PASS)
                {
                    //List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_020, "0", List_Login, S_SN);//此条码已NG
                    //return List_Result;
                }

                string S_RouteCheck = await GetRouteCheck(Convert.ToInt32(List_Login.StationTypeID),
                    List_Login.StationID, List_Login.LineID.ToString(), List_mesUnit_Get[0], S_SN);

                if (S_RouteCheck == "1")
                {
                    List_Result = await GetTabVal("1", "", "1");
                    List_Result[0].UnitID = List_mesUnit_Get[0].ID.ToString();
                    List_Result[0].PartID = S_PartID;
                    List_Result[0].PartFamilyID = S_PartFamilyID;
                    List_Result[0].ProductionOrderID = S_POID;
                    List_Result[0].StatusID = S_StatusID;

                    int I_PartID = Convert.ToInt32(S_PartID);
                    int I_PartFamilyID = Convert.ToInt32(S_PartFamilyID);
                    int I_LineID = List_Login.LineID;
                    int I_StationTypeID = Convert.ToInt32(List_Login.StationTypeID);
                    int I_ProductionOrderID = Convert.ToInt32(S_POID);
                    int I_StatusID = Convert.ToInt32(S_UnitStatus);

                    List<TabVal> List_TimeCheck = await TimeCheck(List_Login.StationID.ToString(), S_SN);
                    string S_IsTimeCheck = List_TimeCheck[0].ValStr1;
                    if (S_IsTimeCheck != "1")
                    {
                        List_Result = List_ERROR_TabVal(S_IsTimeCheck, "0", List_Login, S_SN);
                        return List_Result;
                    }

                    List<TabVal> List_ProCheck = await List_ExecProc("", S_SN, S_POID, S_PartID, null, null, List_Login);
                    string S_ProCheck = List_ProCheck[0].ValStr1;
                    if (S_ProCheck != "1")
                    {
                        List_Result = List_ERROR_TabVal(S_ProCheck, "0", List_Login, S_SN);
                        return List_Result;
                    }

                    List<IdDescription> List_mesUnitState = new List<IdDescription>();
                    List_mesUnitState = await GetmesUnitState(I_PartID, I_PartFamilyID,
                        I_LineID, I_StationTypeID, I_ProductionOrderID, I_StatusID);
                    if (List_mesUnitState == null)
                    {
                        //当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
                        List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_021, "0", List_Login, S_SN);
                        return List_Result;
                    }
                    int I_UnitStateID = List_mesUnitState[0].ID;
                    List_Result[0].UnitStateID = I_UnitStateID.ToString();
                }
                else
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_022, "0", List_Login, S_SN);
                }
            }
            catch (Exception ex)
            {
                List_Result = List_ERROR_TabVal(ex.Message, "0", List_Login, S_SN);
                return List_Result;
            }

            return List_Result;
        }


        public async Task<List<luVendor>> GetVendor(string PartID)
        {
            string S_Sql = "select * from luVendor where PartID=" + PartID;
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<luVendor>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public DataSet GetMesMaterialUnitByLotCode(string PartID, string LotCode)
        {
            string sql = string.Format(@"select * from mesMaterialUnit where SerialNumber='{0}' and partid={1}", LotCode, PartID);
            DataSet ds = Data_Set(sql);
            return ds;
        }

        public int GetMesMaterialUseQTY(string MaterialUnitID)
        {
            int UseQTY = 0;
            string sql = string.Format(@"SELECT isnull(SUM(isnull(B.Quantity,0))+SUM(isnull(B.BalanceQty,0)),0) UseQTY 
                    FROM mesMaterialUnit B WHERE B.ParentID={0}", MaterialUnitID);
            DataTable dt = Data_Table(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                UseQTY = Convert.ToInt32(dt.Rows[0]["UseQTY"].ToString());
            }
            return UseQTY;
        }

        public DataSet GetMesMaterialUnitData(string SerialNumber)
        {
            string sql = string.Format(@"select * from mesMaterialUnit where SerialNumber='{0}'", SerialNumber);
            DataSet ds = Data_Set(sql);
            return ds;
        }



        public async Task<List<MaterailBomData>> GetMaterailBomData(string PartID,string StationTypeID) 
        {
            string S_Sql = "SELECT A.PartID,B.PartNumber Description FROM mesProductStructure A " + "\r\n" +
                          "  INNER JOIN mesPart B ON A.PartID=B.ID " + "\r\n" +
                          "WHERE A.ParentPartID='"+ PartID + "' and A.Status=1 and A.StationTypeID='"+ StationTypeID + "' AND B.Status=1";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<MaterailBomData>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        /// <summary>
        /// UPC站选择的是区域工单，只检查料号
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="PartID"></param>
        /// <returns></returns>
        public DataSet GetAndCheckUnitInfoUPC(string barcode, string PartID)
        {
            string sql = @"SELECT b.*,d.PartFamilyID
                            FROM dbo.mesSerialNumber a
                            JOIN dbo.mesUnit b ON b.ID = a.UnitID
                            JOIN dbo.mesProductionOrder c ON b.ProductionOrderID = c.ID
                            JOIN dbo.mesPart d ON  d.ID = c.PartID
                            WHERE a.Value = '" + barcode + "' AND d.ID = '" + PartID + "'";
            DataSet ds = Data_Set(sql);
            return ds;
        }



        #region howard add

        public object throwMsg = "process error";

        public async Task<List<PoDetailConfig>> GetmesPoDetailConfigAsync(string poID)
        {
            string S_Sql = $"SELECT po.ProductionOrderNumber,pod.ProductionOrderID,po.PartID,podd.ID as PoDefID,podd.Description,pod.Content,pod.IsCheckList,pod.Sequence,podd.ParentValueID,podd.ParentCheckID,podd.ValueType,podd.CheckType,podd.Parameters,podd.Required FROM dbo.mesProductionOrder po JOIN dbo.mesProductionOrderDetail pod ON pod.ProductionOrderID =po.ID\r\nJOIN dbo.luProductionOrderDetailDef podd ON podd.ID = pod.ProductionOrderDetailDefID WHERE po.ID = {poID}";
            var vQuery = await DapperConn.QueryAsync<PoDetailConfig>(S_Sql, null, null, I_DBTimeout, null);
            return vQuery?.ToList();
        }

        public string uspKitBoxFGSNCheck(string FGSN, string UPCSN) =>
            FGSN.Substring(FGSN.Length - 12, 12) == UPCSN.Substring(UPCSN.Length - 12, 12) ? "1" : "21000";
        public async Task<mesRoute> GetmesRouteAsync(string routeId)
        {
            string S_Sql = $"SELECT * FROM mesRoute WHERE ID={routeId}";
            return await DapperConnRead2.QueryFirstOrDefaultAsync<mesRoute>(S_Sql, null, null, I_DBTimeout, null);
        }

        public async Task<List<mesRouteDetail>> GetmesRouteDetailAsync(string routeId)
        {
            string S_Sql = $"SELECT * FROM dbo.mesUnitOutputState  WHERE RouteID = {routeId}";
            var vDetails = await DapperConn.QueryAsync<mesRouteDetail>(S_Sql, null, null, I_DBTimeout, null);
            return vDetails?.ToList();
        }
        public async Task<List<mesUnitOutputState>> GetmesUnitOutputStateAsync(string routeId)
        {
            string S_Sql = $"SELECT * FROM mesRoute WHERE ID={routeId}";
            var vDetails = await DapperConn.QueryAsync<mesUnitOutputState>(S_Sql, null, null, I_DBTimeout, null);
            return vDetails?.ToList();
        }
        public async Task<string> uspFGLinkUPCUPCSNCheck(string UPCSN, string partId, string poId, LoginList loginList)
        {
            string result = "0";
            var mMesUnit = await GetMesUnitSerialAsync(UPCSN);
            if (mMesUnit is null)
                return result = "20012";

            if (mMesUnit.StatusID is not 1)
                return "20036";

            if (string.IsNullOrEmpty(mMesUnit.ProductionOrderID.ToString()))
                return "20014";

            if (mMesUnit.SerialNumberTypeID is not 6)
                return "20035";

            if (mMesUnit.PartID.ToString() != partId)
                return "20043";

            if (poId != mMesUnit.ProductionOrderID.ToString())
                return "20133";

            var mUnitDetail = await GetmesUnitDetailWithKitAsync(mMesUnit.Value);
            if (mUnitDetail?.ID > 0)
                return "20051";

            var mStation = await MesGetStationAsync(mMesUnit.StationID.ToString());
            if (mStation?.Count <= 0)
                return "70019";

            //var routeId = await GetRouteID(partId, "", mStation?[0].LineID.ToString(), poId);
            var routeId = await ufnRTEGetRouteIDAsync(mStation[0].LineID.ToInt(), partId.ToInt(), 0, poId.ToInt());
            if (string.IsNullOrEmpty( routeId) || routeId.ToInt() <= 0)
                return "20195";

            var mMesRoute = await GetmesRouteAsync(routeId.ToString());
            if (mMesRoute is not null && mMesRoute.RouteType == 1)
            {
                var mMesUnitOutputStates = await GetmesUnitOutputStateAsync(routeId.ToString());
                if (mMesUnitOutputStates is not null && mMesUnitOutputStates.Where(x => x.CurrStateID == mMesUnit.UnitStateID).Any())
                    return "20241";
            }
            else
            {
                var mMesRouteDetails = await GetmesRouteDetailAsync(routeId.ToString());
                var maxSeq = mMesRouteDetails?.Max(x => x.Sequence);
                var currentSeq = mMesRouteDetails.Where(x => x.UnitStateID == mMesUnit.UnitStateID)
                    .Select(x => x.Sequence).FirstOrDefault();

                if (maxSeq != currentSeq)
                    return "20241";

            }
            return result = "1";
        }
        public async Task<string> uspFGLinkUPCFGSNCheck(string FGSN, string partId, LoginList List_Login)
        {
            string result = "0";
            var mMesUnit = await GetMesUnitSerialAsync(FGSN);
            if (mMesUnit is null)
                return result = "20012";

            if (mMesUnit.StatusID is not 1)
                return "20036";

            if (string.IsNullOrEmpty(mMesUnit.ProductionOrderID.ToString()))
                return "20014";

            if (mMesUnit.SerialNumberTypeID is 6)
                return "20035";

            if (mMesUnit.PartID.ToString() != partId)
                return "20043";

            result = await GetRouteCheckAsync(List_Login.StationTypeID.ToInt(), List_Login.StationID, List_Login.LineID.ToString(), mMesUnit, FGSN);
            return result;
        }

        public async Task<mesUnitSerial> GetMesUnitSerialAsync(string sn)
        {
            string S_Sql = $"SELECT a.*,b.*,c.PartFamilyID\r\nFROM dbo.mesUnit a\r\nJOIN dbo.mesSerialNumber b ON a.ID = b.UnitID\r\nJOIN dbo.mesPart c ON c.ID = a.PartID\r\nWHERE b.Value = '{sn}'";
            return await DapperConnRead2.QueryFirstOrDefaultAsync<mesUnitSerial>(S_Sql, null, null, I_DBTimeout, null);
        }

        public async Task<mesUnitDetail> GetmesUnitDetailAsync(string unitID)
        {
            string S_Sql = $"SELECT * FROM dbo.mesUnitDetail WHERE UnitID = {unitID}";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnitDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.FirstOrDefault();
        }
        public async Task<mesUnitDetail> GetmesUnitDetailWithKitAsync(string kitSerialNumber)
        {
            string S_Sql = $"SELECT * FROM dbo.mesUnitDetail WHERE KitSerialNumber = '{kitSerialNumber}'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnitDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.FirstOrDefault();
        }
        /// <summary>
        /// 通过SN获取unit表的相关状态
        /// </summary>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public async Task<mesUnit> GetmesUnitSNAsync(string S_SN)
        {
            string S_Sql = $"select B.*,c.PartFamilyID from mesSerialNumber A " +
                           $"JOIN mesUnit B on A.UnitID = B.ID " +
                           $"JOIN dbo.mesPart c ON c.ID = B.PartID where A.Value = '{S_SN}'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.FirstOrDefault();
        }

        public async Task<DataTable> GetRoute(string S_RouteSequence, int I_RouteID)
        {
            DataTable dtRoute = new DataTable();
            string routeID = I_RouteID.ToString();

            string S_Sql = string.Format(@"select A.* from 
                            (select B.ID,B.Name,B.Description RouteName,B.RouteType,
			                            A.StationTypeID,A.Sequence SequenceMod,A.Description RouteDetailName,
			                            A.UnitStateID,                                
			                            E.Description ApplicationType,
			                            cast(ROW_NUMBER() over(order by A.Sequence) as int) as Sequence
		                            from 
			                            mesRouteDetail A
			                            join mesRoute B on A.RouteID=B.ID  		  	                             
			                            join mesStationType D on D.ID=A.StationTypeID
			                            join luApplicationType E on E.ID=D.ApplicationTypeID
	                            where B.ID={0}  
                            )A where 1=1", I_RouteID);

            if (!string.IsNullOrEmpty(S_RouteSequence))
            {
                S_Sql = S_Sql + " AND A.Sequence=" + S_RouteSequence;
            }

            var v = await DapperConnRead2.ExecuteReaderAsync(S_Sql, null, null, I_DBTimeout, null);
            dtRoute.Load(v);
            return dtRoute;
        }


        /// <summary>
        /// 检查解绑站点于当前站点是否匹配
        /// </summary>
        /// <param name="MachineSN"></param>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        public string MesToolingReleaseCheck(string MachineSN, string StationTypeID)
        {
            string result = "1";
            string S_Sql = string.Format("select ValidTo from mesMachine where SN='{0}'", MachineSN);
            var mValidTo = DapperConnRead2.ExecuteScalar(S_Sql, null, null, I_DBTimeout, null);

            if (mValidTo is null)
            {
                result = "20053";
            }
            else
            {
                string ValidTo = mValidTo.ToString();
                if (ValidTo != StationTypeID)
                {
                    result = "20161";
                }
            }
            return result;
        }
        public void ModMachine(string S_SN, string StatusID, Boolean IsUpUnitDetail)
        {
            string S_Sql = string.Empty;
            if (StatusID == "1")
            {
                S_Sql = "Update mesMachine set RuningCapacityQuantity=0 ,StatusID=" + StatusID + " where SN='" + S_SN + "'";
            }
            else
            {
                S_Sql = "Update mesMachine set StatusID=" + StatusID + " where SN='" + S_SN + "'";
            }

            S_Sql += "\r\n";

            if (IsUpUnitDetail == true)
            {
                S_Sql += @"Update  mesUnitDetail set reserved_03=2
	                    where reserved_01='" + S_SN + @"' and reserved_03=1 and 
	                    UnitID=(select max(unitid)  from mesUnitDetail where reserved_01='" + S_SN + @"' and reserved_03=1)";
            }

            DapperConn.Execute(S_Sql);
        }

        public void ModMachine2(string ID, string StatusID)
        {
            string S_Sql = "Update mesMachine set StatusID=" + StatusID + " where ID='" + ID + "'";
            DapperConn.Execute(S_Sql);
        }

        /// <summary>
        /// 包装工艺路线检查
        /// </summary>
        /// <param name="strSNFormat"></param>
        /// <param name="xmlProdOrder"></param>
        /// <param name="xmlPart"></param>
        /// <param name="xmlStation"></param>
        /// <param name="xmlExtraData"></param>
        /// <param name="strSNbuf"></param>
        /// <returns></returns>
        public async Task<string> uspPackageRouteCheck(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string outputStr = "0";
            string sql = $@"
                                                        declare	@prodID				int,
			                        @partID				int,
			                        @stationID			int,
			                        @routeID			int,
			                        @lineID				int,
			                        @stationTypeID		int,
			                        @UnitStateID		int,
			                        @StatusID			int,
			                        @Seq				int,
			                        @stage				int,
			                        @count				int,
			                        @idoc				int,
			                        @strOutput        nvarchar(200),
                                    @strSNFormat nvarchar(200)
	                        SET @strOutput='1'
                            set @strSNFormat = '{strSNFormat}'
	                        IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat)
	                        BEGIN
		                        SET @strOutput='20012'
                                select @strOutput
		                        RETURN
	                        END

	                        BEGIN TRY
                                declare @xmlPart nvarchar(200), 
                                        @xmlProdOrder nvarchar(200),
                                        @xmlStation nvarchar(200)
                                set @xmlPart = '{xmlPart}'
                                set @xmlProdOrder = '{xmlProdOrder}'
                                set @xmlStation = '{xmlStation}'
		                        --读取xmlPart参数值
		                        exec sp_xml_preparedocument @idoc output, @xmlPart
		                        SELECT @partID=PartId
		                        FROM   OPENXML (@idoc,			'/Part',2)
				                        WITH (PartId  int		'@PartID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@partID,'') = ''
		                        begin
			                        SET @strOutput='20077'
                                    select @strOutput
			                        RETURN
		                        end

		                        --读取ProdOrder参数值
		                        exec sp_xml_preparedocument @idoc output, @xmlProdOrder
		                        SELECT @prodID=ProdId
		                        FROM   OPENXML (@idoc,			'/ProdOrder',2)
				                        WITH (ProdId  int		'@ProdOrderID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@prodID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput
			                        RETURN
		                        END

		                        exec sp_xml_preparedocument @idoc output, @xmlStation
		                        SELECT @stationID=StationId
		                        FROM   OPENXML (@idoc, '/Station',2)
				                        WITH (StationId		  int			'@StationId')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@stationID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput
			                        RETURN
		                        END
	                        END TRY
	                        BEGIN CATCH
		                        SET @strOutput='20136'
                                select @strOutput
		                        RETURN
	                        END CATCH

	                        --校验
	                        SELECT TOP 1 @lineID = LineID,@stationTypeID=StationTypeID 
		                        FROM mesStation WHERE ID=@stationID
	                        IF ISNULL(@lineID,'')=''
	                        BEGIN
		                        SET @strOutput='20137'
                                select @strOutput
		                        RETURN
	                        END

	                        --栈板不校验流程
	                        IF @stationTypeID = 14
	                        BEGIN
                                select @strOutput
		                        RETURN
	                        END

	                        --SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@prodID)
							DECLARE @PartFamilyID INT = 0
							select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

							select top 1 @RouteID = RouteID
							from mesRouteMap
							where
								(LineID = @LineID or LineID is NULL OR LineID=0) AND
								(PartID = @PartID or PartID is NULL OR PartID=0) AND
								(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
								(ProductionOrderID = @prodID or ProductionOrderID is NULL OR ProductionOrderID=0)
							order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
	                        IF ISNULL(@routeID,'')=''
	                        BEGIN
		                        SET @strOutput='20195'
                                select @strOutput
		                        RETURN
	                        END

	                        CREATE TABLE #tempUnitState(UnitStateID VARCHAR(64),StatusID VARCHAR(64))

	                        --获取SN UnitStateID
	                        SELECT @stage=Stage FROM mesPackage WHERE SerialNumber=@strSNFormat
	                        IF ISNULL(@stage,'')=1
	                        BEGIN
		                        INSERT INTO #tempUnitState(UnitStateID,StatusID)
		                        SELECT A.UnitStateID,A.StatusID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
					                        INNER JOIN mesPackage C ON B.InmostPackageID=C.ID 
		                        WHERE C.SerialNumber=@strSNFormat GROUP BY A.UnitStateID,A.StatusID
	                        END
	                        ELSE
	                        BEGIN
		                        INSERT INTO #tempUnitState(UnitStateID,StatusID)
		                        SELECT A.UnitStateID,A.StatusID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
					                        INNER JOIN mesPackage C ON B.InmostPackageID=C.ID
					                        INNER JOIN mesPackage D ON C.ParentID=D.ID
		                        WHERE D.SerialNumber=@strSNFormat GROUP BY A.UnitStateID,A.StatusID
	                        END

	                        SELECT @count = COUNT(1) FROM #tempUnitState
	                        IF isnull(@count,0)<>1
	                        BEGIN
		                        SET @strOutput='20204'
                                select @strOutput
		                        RETURN
	                        END

	                        SELECT @UnitStateID=UnitStateID,@StatusID=StatusID FROM #tempUnitState

	                        ---------------------------------------新流程校验--------------------------------------
	                        IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND ISNULL(RouteType,'')=1)
	                        BEGIN
		                        IF NOT EXISTS (SELECT 1 FROM mesUnitInputState WHERE RouteID=@routeID 
			                        AND StationTypeID=ISNULL(@stationTypeID,'') AND CurrStateID=ISNULL(@UnitStateID,''))
		                        BEGIN
			                        SET @strOutput='20241'
                                    select @strOutput
			                        RETURN
		                        END
	                        END
	                        ---------------------------------------旧流程--------------------------------------
	                        ELSE
	                        BEGIN
		                        IF ISNULL(@StatusID,'')<>'1'
		                        BEGIN
			                        SET @strOutput='20204'
                                    select @strOutput
			                        RETURN			
		                        END

		                        --RouteDetail临时表
		                        SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 0)) ID ,StationTypeID,UnitStateID into #RouteDetail
		                        FROM mesRouteDetail where RouteID=@routeID order by cast(Sequence as int)

		                        SELECT @Seq=ID FROM #RouteDetail WHERE StationTypeID=@stationTypeID
		                        IF ISNULL(@Seq,'')=''
		                        BEGIN
			                        SET @strOutput='20203'
                                    select @strOutput
			                        RETURN			
		                        END

		                        IF NOT EXISTS(SELECT 1 FROM #RouteDetail WHERE ID=(@Seq-1) AND UnitStateID=@UnitStateID)
		                        BEGIN
			                        SET @strOutput='20241'
                                    select @strOutput
			                        RETURN
		                        END
	                        END
                            select @strOutput

                            ";

            var queryObj = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            outputStr = queryObj?.ToString();
            return outputStr;
        }
        /// <summary>
        /// 包装工艺路线检查
        /// </summary>
        /// <param name="strSNFormat"></param>
        /// <param name="xmlProdOrder"></param>
        /// <param name="xmlPart"></param>
        /// <param name="xmlStation"></param>
        /// <param name="xmlExtraData"></param>
        /// <param name="strSNbuf"></param>
        /// <returns></returns>
        public async Task<string> uspPackageRouteCheck_P(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string outputStr = "0";
            string sql = $@"
                            declare	@prodID				int,
			                        @partID				int,
			                        @stationID			int,
			                        @routeID			int,
			                        @lineID				int,
			                        @stationTypeID		int,
			                        @UnitStateID		int,
			                        @StatusID			int,
			                        @Seq				int,
			                        @stage				int,
			                        @count				int,
			                        @idoc				int,
			                        @strOutput        nvarchar(200),
                                    @strSNFormat nvarchar(200)
	                        SET @strOutput='1'
                            set @strSNFormat = '{strSNFormat}'
	                        IF NOT EXISTS(SELECT 1 FROM mesPackage WHERE SerialNumber=@strSNFormat)
	                        BEGIN
		                        SET @strOutput='20012'
                                select @strOutput
		                        RETURN
	                        END

	                        BEGIN TRY
                                declare @xmlPart nvarchar(200), 
                                        @xmlProdOrder nvarchar(200),
                                        @xmlStation nvarchar(200)
                                set @xmlPart = '{xmlPart}'
                                set @xmlProdOrder = '{xmlProdOrder}'
                                set @xmlStation = '{xmlStation}'
		                        --读取xmlPart参数值
		                        exec sp_xml_preparedocument @idoc output, @xmlPart
		                        SELECT @partID=PartId
		                        FROM   OPENXML (@idoc,			'/Part',2)
				                        WITH (PartId  int		'@PartID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@partID,'') = ''
		                        begin
			                        SET @strOutput='20077'
                                    select @strOutput
			                        RETURN
		                        end

		                        --读取ProdOrder参数值
		                        exec sp_xml_preparedocument @idoc output, @xmlProdOrder
		                        SELECT @prodID=ProdId
		                        FROM   OPENXML (@idoc,			'/ProdOrder',2)
				                        WITH (ProdId  int		'@ProdOrderID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@prodID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput
			                        RETURN
		                        END

		                        exec sp_xml_preparedocument @idoc output, @xmlStation
		                        SELECT @stationID=StationId
		                        FROM   OPENXML (@idoc, '/Station',2)
				                        WITH (StationId		  int			'@StationId')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@stationID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput
			                        RETURN
		                        END
	                        END TRY
	                        BEGIN CATCH
		                        SET @strOutput='20136'
                                select @strOutput
		                        RETURN
	                        END CATCH

	                        --校验
	                        SELECT TOP 1 @lineID = LineID,@stationTypeID=StationTypeID 
		                        FROM mesStation WHERE ID=@stationID
	                        IF ISNULL(@lineID,'')=''
	                        BEGIN
		                        SET @strOutput='20137'
                                select @strOutput
		                        RETURN
	                        END

	                        --栈板不校验流程
	                        IF @stationTypeID = 14
	                        BEGIN
                                select @strOutput
		                        RETURN
	                        END

	                        SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@prodID)
	                        IF ISNULL(@routeID,'')=''
	                        BEGIN
		                        SET @strOutput='20195'
                                select @strOutput
		                        RETURN
	                        END

	                        CREATE TABLE #tempUnitState(UnitStateID VARCHAR(64),StatusID VARCHAR(64))

	                        --获取SN UnitStateID
	                        SELECT @stage=Stage FROM mesPackage WHERE SerialNumber=@strSNFormat
	                        IF ISNULL(@stage,'')=1
	                        BEGIN
		                        INSERT INTO #tempUnitState(UnitStateID,StatusID)
		                        SELECT A.UnitStateID,A.StatusID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
					                        INNER JOIN mesPackage C ON B.InmostPackageID=C.ID 
		                        WHERE C.SerialNumber=@strSNFormat GROUP BY A.UnitStateID,A.StatusID
	                        END
	                        ELSE
	                        BEGIN
		                        INSERT INTO #tempUnitState(UnitStateID,StatusID)
		                        SELECT A.UnitStateID,A.StatusID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
					                        INNER JOIN mesPackage C ON B.InmostPackageID=C.ID
					                        INNER JOIN mesPackage D ON C.ParentID=D.ID
		                        WHERE D.SerialNumber=@strSNFormat GROUP BY A.UnitStateID,A.StatusID
	                        END

	                        SELECT @count = COUNT(1) FROM #tempUnitState
	                        IF isnull(@count,0)<>1
	                        BEGIN
		                        SET @strOutput='20204'
                                select @strOutput
		                        RETURN
	                        END

	                        SELECT @UnitStateID=UnitStateID,@StatusID=StatusID FROM #tempUnitState

	                        ---------------------------------------新流程校验--------------------------------------
	                        IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND ISNULL(RouteType,'')=1)
	                        BEGIN
		                        IF NOT EXISTS (SELECT 1 FROM mesUnitInputState WHERE RouteID=@routeID 
			                        AND StationTypeID=ISNULL(@stationTypeID,'') AND CurrStateID=ISNULL(@UnitStateID,''))
		                        BEGIN
			                        SET @strOutput='20241'
                                    select @strOutput
			                        RETURN
		                        END
	                        END
	                        ---------------------------------------旧流程--------------------------------------
	                        ELSE
	                        BEGIN
		                        IF ISNULL(@StatusID,'')<>'1'
		                        BEGIN
			                        SET @strOutput='20204'
                                    select @strOutput
			                        RETURN			
		                        END

		                        --RouteDetail临时表
		                        SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 0)) ID ,StationTypeID,UnitStateID into #RouteDetail
		                        FROM mesRouteDetail where RouteID=@routeID order by cast(Sequence as int)

		                        SELECT @Seq=ID FROM #RouteDetail WHERE StationTypeID=@stationTypeID
		                        IF ISNULL(@Seq,'')=''
		                        BEGIN
			                        SET @strOutput='20203'
                                    select @strOutput
			                        RETURN			
		                        END

		                        IF NOT EXISTS(SELECT 1 FROM #RouteDetail WHERE ID=(@Seq-1) AND UnitStateID=@UnitStateID)
		                        BEGIN
			                        SET @strOutput='20241'
                                    select @strOutput
			                        RETURN
		                        END
	                        END
                            select @strOutput
                            ";

            var queryObj = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            outputStr = queryObj?.ToString();
            return outputStr;
        }
        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="mesOutputDto"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public MesOutputDto SetMsgSys(MesOutputDto mesOutputDto, string errorCode)
        {
            try
            {
                mesOutputDto.ErrorMsg = msgSys.GetLanguage(errorCode);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                return mesOutputDto;
            }
            return mesOutputDto;
        }

        /// <summary>
        /// 设置新增提示信息
        /// </summary>
        /// <param name="mesOutputDto"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public MesOutputDto SetMsgPublic(MesOutputDto mesOutputDto, string errorCode)
        {
            try
            {
                mesOutputDto.ErrorMsg = errorCode;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                return mesOutputDto;
            }
            return mesOutputDto;
        }

        /// <summary>
        /// 根据条码及类型获取产品数据
        /// 0:FG/治具条码，1：箱码，2：栈板条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<DataTable> GetProductDataInfo(string barcode, int type)
        {
            DataTable dataTable = null;
            try
            {
                string sql = string.Empty;
                int tmptype = !new int[3] { 0, 1, 2 }.Contains(type) ? await GetBarcodeType(barcode) : type;

                if (tmptype == 0)
                {
                    //查询FG或者治具条码 
                    sql = @"DECLARE @mFGBarcode VARCHAR(50),@IBarcode VARCHAR(50) = '" + barcode + @"'
                    IF EXISTS(SELECT 1 FROM dbo.mesMachine WHERE SN = @IBarcode)
                    BEGIN
                        IF not EXISTS(SELECT 1 FROM dbo.mesUnitDetail WHERE reserved_01 = @IBarcode AND reserved_03 = '1')
                         BEGIN
                             PRINT '扫入的是治具条码，但是已经是解绑状态'
                          RETURN
                         END
                         SELECT @mFGBarcode = b.Value FROM dbo.mesUnitDetail a
                         JOIN dbo.mesSerialNumber b ON b.UnitID = a.UnitID
                         WHERE a.reserved_01 = @IBarcode AND a.reserved_03 = '1'
                    END
                    IF ISNULL(@mFGBarcode, '') = ''
                    BEGIN
                        SET @mFGBarcode = @IBarcode
                    END
                    SELECT a.*,b.Value FROM dbo.mesUnit a
                    JOIN dbo.mesSerialNumber b ON b.UnitID = a.ID
                    WHERE b.Value = @mFGBarcode
                    ";
                }
                else if (tmptype == 1)
                {
                    //查询箱码
                    sql = @"SELECT a.SerialNumber,c.UnitStateID,c.PartID,c.ProductionOrderID FROM dbo.mesPackage a
                            JOIN dbo.mesUnitDetail b ON a.ID = b.InmostPackageID
                            JOIN dbo.mesUnit c ON c.ID = b.UnitID
                            WHERE a.SerialNumber = '" + barcode + @"' AND a.Stage = 1
                            GROUP BY a.SerialNumber,c.UnitStateID,c.PartID,c.ProductionOrderID";
                }
                else if (tmptype == 2)
                {
                    //查询栈板条码
                    sql = @"SELECT  d.SerialNumber,a.UnitStateID, a.PartID,a.ProductionOrderID FROM dbo.mesUnit a
                            JOIN dbo.mesUnitDetail b ON b.UnitID = a.ID
                            JOIN dbo.mesPackage c ON c.ID = b.InmostPackageID
                            JOIN dbo.mesPackage d ON c.ParentID = d.ID
                            WHERE d.SerialNumber = '" + barcode + @"' AND d.Stage = '2'
                            GROUP BY d.SerialNumber,a.UnitStateID, a.PartID,a.ProductionOrderID";
                }
                else
                {
                    return dataTable;
                }

                var tmpReader = await DapperConnRead2.ExecuteReaderAsync(sql, null, null, I_DBTimeout, null);
                dataTable = new DataTable();
                dataTable.Load(tmpReader);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                return null;
            }

            return dataTable;
        }

        /// <summary>
        /// 0  FG barcode
        /// 1  Carton barcode
        /// 2  Pallet barcode
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<int> GetBarcodeType(string barcode)
        {
            int tmpType = -1;
            try
            {
                string sql = string.Empty;
                sql = @"IF	EXISTS(SELECT 1 FROM dbo.mesUnit a
			                            JOIN dbo.mesSerialNumber b ON b.UnitID = a.ID
			                            WHERE b.Value = '" + barcode + @"' )
                            BEGIN
	                            SELECT 0 AS mType		    
	                            RETURN
                            END
                            IF	EXISTS(SELECT 1 FROM dbo.mesPackage WHERE SerialNumber = '" + barcode + @"' AND Stage = 1)
                            BEGIN
                                SELECT 1 AS mType
	                            RETURN
                            END
                            IF	EXISTS(SELECT 1 FROM dbo.mesPackage WHERE SerialNumber = '" + barcode + @"' AND Stage = 2)
                            BEGIN
                                SELECT 2 AS mType
	                            RETURN
                            END
                            SELECT -1 AS mType";
                object tmpRes = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
                if (!new string[3] { "0", "1", "2" }.Contains(tmpRes?.ToString()))
                    return tmpType;
                tmpType = int.Parse(tmpRes.ToString());
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                return tmpType;
            }
            return tmpType;
        }

        public async Task<DataTable> GetPalletAllData(string palletSn)
        {
            DataTable dt = new DataTable();
            string sql = $"SELECT ROW_NUMBER() OVER(ORDER BY A.ID) SEQNO, B.SerialNumber KITSN,B.LastUpdate TIME,b.CurrPartID,b.CurrProductionOrderID,                 d.UnitStateID,d.PartID,d.ProductionOrderID,d.StatusID,e.Value " +
                         $"FROM mesPackage A                " +
                         $"INNER JOIN mesPackage B ON A.ID=B.ParentID               " +
                         $"JOIN dbo.mesUnitDetail c ON c.InmostPackageID = b.ID                " +
                         $"JOIN dbo.mesUnit d ON d.ID = c.UnitID                " +
                         $"JOIN dbo.mesSerialNumber e ON e.UnitID = d.ID               " +
                         $"WHERE A.Stage=2 AND A.SerialNumber= '{palletSn}'";

            var dr = await DapperConnRead2.ExecuteReaderAsync(sql, null, null, I_DBTimeout, null);
            dt.Load(dr);
            return dt;
        }



        /// <summary>
        /// 工站配置信息
        /// </summary>
        /// <param name="S_SetName"></param>
        /// <param name="S_StationID"></param>
        /// <returns></returns>
        public async Task<List<StationAttribute>> GetStationConfigSettingAsync(string S_StationID, string S_SetName = "")
        {
            string S_Sql = "select LTRIM(RTRIM(Name)) AttributeName,LTRIM(RTRIM(Value)) AttributeValue from mesStationConfigSetting where StationID='" + S_StationID + "'";

            if (S_SetName != "")
            {
                S_Sql = "select LTRIM(RTRIM(Name)) AttributeName,LTRIM(RTRIM(Value)) AttributeValue from mesStationConfigSetting where Name='" +
                        S_SetName + "' and StationID='" + S_StationID + "'";
            }
            var v_Query = await DapperConn.QueryAsync<StationAttribute>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToList();
        }

        /// <summary>
        /// 工站类型详细
        /// </summary>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<List<StationAttribute>> GetStationTypeDetailAsync(string S_StationTypeID, string S_SetName = "")
        {
            string S_Sql = @"SELECT LTRIM(RTRIM(B.Description))  AttributeName, LTRIM(RTRIM(A.Content)) AttributeValue
                              FROM mesStationTypeDetail A
                                JOIN luStationTypeDetailDef B ON A.StationTypeDetailDefID = B.ID
                                JOIN mesStationType C ON A.StationTypeID=C.ID
                            WHERE A.StationTypeID ='" + S_StationTypeID + "' ";

            if (S_SetName != "")
            {
                S_Sql += $" and b.Description = '{S_SetName}'";
            }


            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<StationAttribute>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToList();
        }

        /// <summary>
        /// 获取当前站点对应的程序类型
        /// 站点id和工序id任取其一，都可以获取到程序类型
        /// </summary>
        /// <param name="S_StationID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<string> GetStationAPPTypeAsync(string S_StationID = "", string S_StationTypeID = "")
        {
            if (S_StationTypeID == "" && S_StationID == "")
                return "";

            string S_Sql = $@"SELECT c.Description  ApplicationType
                                FROM  dbo.mesStation a
                                JOIN dbo.mesStationType b ON b.ID = a.StationTypeID
                                JOIN luApplicationType c ON b.ApplicationTypeID=c.ID
                             where {(string.IsNullOrEmpty(S_StationTypeID) ? "" : $"b.ID = '{S_StationTypeID}'")} {(string.IsNullOrEmpty(S_StationID) ? "" : $" {(string.IsNullOrEmpty(S_StationTypeID) ? " " : " and ")} a.ID = '{S_StationID}'")}";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSNFormat"></param>
        /// <param name="xmlProdOrder"></param>
        /// <param name="xmlPart"></param>
        /// <param name="xmlStation"></param>
        /// <param name="xmlExtraData"></param>
        /// <param name="strSNbuf">--扫描类型 1:夹具绑批次校验/扫描批次组装校验 2:扫描夹具组装校验</param>
        /// <returns></returns>
        public async Task<string> uspBatchDataCheckAsync(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string sql = $@"declare	@idoc				int,
			                        @PartId				int,
			                        @RemainQTY			int,
			                        @ConsumeQTY			int,
			                        @ScanType			int,
			                        @BatchConfig		nvarchar(200),
			                        @MaterialTypeID		int,
			                        @MaterialType		int,			--物料批次类型 2:系统未注册 7:系统注册 8:系统注册公共批次
			                        @StatusID			int,
			                        @tmpStrSNFormat		varchar(50),
                                    @strOutput          varchar(50),
                                    @tmpXmlPart         varchar(50)

	                        BEGIN TRY
		                        set @tmpXmlPart = '{@xmlPart}' 
		                        --读取XML参数值
		                        EXEC sp_xml_preparedocument @idoc output,  @tmpXmlPart
		                        SELECT @PartId=PartId
		                        FROM   OPENXML (@idoc,			'/Part',2)
				                        WITH (PartId  int		'@PartID'
					                        )   
		                        EXEC sp_xml_removedocument @idoc

		                        SELECT TOP 1 @MaterialType=CAST(A.Content AS int) FROM mesPartDetail A INNER JOIN luPartDetailDef B ON A.PartDetailDefID=B.ID
			                        WHERE B.Description='ScanType' AND A.PartID=@PartId
		                        IF ISNULL(@MaterialType,'')=''
		                        BEGIN
				                        SELECT @strOutput='20024'
                                    select @strOutput
		                        END
		                        
		                        IF '{@strSNbuf}'= '2'
		                        BEGIN
			                        SELECT @tmpStrSNFormat = reserved_02 FROM mesUnitDetail WHERE ID = (SELECT MAX(ID) 
				                        FROM mesUnitDetail WHERE reserved_01='{@strSNFormat}' AND reserved_03=1)
			                        IF ISNULL(@tmpStrSNFormat,'')=''
			                        BEGIN
				                        SELECT @strOutput='20054'
                                        select @strOutput
			                        END
		                        END

		                        --系统注册批次
		                        IF @MaterialType='7'
		                        BEGIN
			                        IF NOT EXISTS(SELECT 1 FROM mesMaterialUnit WHERE SerialNumber = '{@strSNFormat}')
			                        BEGIN
				                        SELECT @strOutput='20180'
                                        select @strOutput
			                        END

			                        IF NOT EXISTS(SELECT 1 FROM mesMaterialUnit where SerialNumber='{@strSNFormat}' and PartID=@PartId)
			                        BEGIN
				                        SELECT @strOutput='20043'
                                        select @strOutput
			                        END
			                        
			                        SELECT @RemainQTY=ISNULL(BalanceQty,0)+ISNULL(Quantity,0),@StatusID=StatusID,@MaterialTypeID=MaterialTypeID 
				                        FROM mesMaterialUnit WHERE SerialNumber = '{@strSNFormat}'
			                        IF @StatusID<>1
			                        BEGIN
				                        SELECT @strOutput='20175'
                                        select @strOutput
			                        END
			                        IF @MaterialTypeID<>1
			                        BEGIN
				                        SELECT @strOutput='20176'
                                        select @strOutput
			                        END
			                        IF @RemainQTY<=0
			                        BEGIN
				                        SELECT @strOutput='20177'
                                        select @strOutput
			                        END
			                        
		                        END
		                        --非系统注册批次
		                        ELSE IF @MaterialType='2'
		                        BEGIN
			                        SELECT @BatchConfig=A.Content FROM mesPartDetail A INNER JOIN luPartDetailDef B
				                        ON A.PartDetailDefID=B.ID AND B.Description='MaterialBatchQTY'
			                        WHERE A.PartID=@PartId

			                        IF ISNULL(@BatchConfig,'')=''
			                        BEGIN
				                        SELECT @strOutput='20178'
                                        select @strOutput
			                        END
			                        SET @RemainQTY=CAST(@BatchConfig AS int)
		                        END
		                        ELSE IF @MaterialType='8'
		                        BEGIN
			                        DECLARE	
			                        @tPartId				INT ,
			                        @tMaterialUnitID		INT,
			                        @tPartValue			VARCHAR(500),
			                        @tMaterialUnitIDList	VARCHAR(500)
			                        SET @tPartId = @PartId
			                        --获取所有数据(bom表对应料号数据)--20200907修改 增加联合索引/修改获取数据逻辑
			                        SELECT DISTINCT A.ID,A.SerialNumber,A.RollCode,A.Quantity,B.Reserved_01,ISNULL(A.BalanceQty,0) BalanceQty INTO #Material
			                        FROM mesMaterialUnit A 
					                        JOIN mesMaterialUnitDetail B ON A.ID=B.MaterialUnitID
					                        LEFT JOIN mesProductStructure C ON A.PartID=C.ParentPartID 
				                        WHERE (C.PartID=@tPartId OR A.PartID=@tPartId)  AND A.StatusID=1 AND A.ExpiringTime>GETDATE() AND MaterialTypeID=1  --and A.PartID=@tPartId
		
			                        SELECT M.ID,M.SerialNumber,M.RollCode,ISNULL(M.Quantity,'') Quantity,M.Reserved_01,M.BalanceQty,ISNULL(S.UsageQTY,0) UsageQTY
				                        INTO #MaterialBomTemp FROM #Material M 
			                        LEFT JOIN 
				                        (SELECT A.SerialNumber,ISNULL(SUM(ISNULL(B.Quantity,0))
				                        +SUM(ISNULL(B.BalanceQty,0)),0) UsageQTY FROM #Material A
				                        INNER JOIN mesMaterialUnit B ON B.ParentID=A.ID
				                        GROUP BY A.SerialNumber) S ON M.SerialNumber=S.SerialNumber
			                        WHERE M.Quantity-ISNULL(S.UsageQTY,0)+M.BalanceQty>0
		
			                        DECLARE cursorAc CURSOR FOR
			                        SELECT ID,Reserved_01 FROM #MaterialBomTemp WHERE ISNULL(Reserved_01,'')<>''
			                        OPEN cursorAc
			                        FETCH NEXT FROM cursorAc INTO @tMaterialUnitID,@tPartValue
				                        WHILE @@FETCH_STATUS = 0
				                        BEGIN
					                        IF EXISTS(SELECT 1 FROM dbo.F_Split(@tPartValue,',') WHERE value = @tPartId)
					                        BEGIN
						                        SET @tMaterialUnitIDList = ISNULL(@tMaterialUnitIDList,'')+','+CAST(@tMaterialUnitID AS VARCHAR);
					                        END
			                        FETCH NEXT FROM cursorAc INTO @tMaterialUnitID,@tPartValue
			                        END
			                        CLOSE cursorAc
			                        DEALLOCATE cursorAc

			                        SELECT * 
			                        INTO #MaterialBomTemp2
			                        FROM
				                        (
				                        SELECT * FROM #MaterialBomTemp WHERE ISNULL(Reserved_01,'')='' 
				                        UNION ALL
				                        SELECT * FROM #MaterialBomTemp WHERE ID IN (SELECT value FROM dbo.F_Split(@tMaterialUnitIDList,','))
			                        ) RS

			                        IF NOT EXISTS(SELECT * FROM #MaterialBomTemp2 WHERE SerialNumber = '{@strSNFormat}')
			                        BEGIN
				                        IF EXISTS(SELECT * FROM dbo.mesMaterialUnit WITH(NOLOCK) WHERE SerialNumber = '{@strSNFormat}')
				                        BEGIN
					                        DECLARE @tmpPartDesc NVARCHAR(256)
					                        SELECT @tmpPartDesc = b.PartNumber FROM dbo.mesMaterialUnit a WITH(NOLOCK) 
					                        JOIN dbo.mesPart b ON b.ID = a.PartID
					                        WHERE a.SerialNumber = '{@strSNFormat}'

					                        SET @strOutput='输入的批次与当前选择的料号不符，请选择 ['+@tmpPartDesc+'] 下的子料料号..'
				                        END
				                        ELSE
				                        BEGIN
					                        SELECT @strOutput='20180'
                                            select @strOutput
				                        END
				                        RETURN
			                        END

			                        DROP TABLE #MaterialBomTemp2
			                        DROP TABLE #Material
			                        DROP TABLE #MaterialBomTemp


			                        SELECT @RemainQTY=ISNULL(BalanceQty,0)+ISNULL(Quantity,0),@StatusID=StatusID,@MaterialTypeID=MaterialTypeID 
				                        FROM mesMaterialUnit WHERE SerialNumber = '{@strSNFormat}'
			                        IF @StatusID<>1
			                        BEGIN
				                        SELECT @strOutput='20175'
                                        select @strOutput
				                        RETURN
			                        END
			                        IF @MaterialTypeID<>1
			                        BEGIN
				                        SELECT @strOutput='20176'
                                        select @strOutput

				                        RETURN
			                        END
			                        IF @RemainQTY<=0
			                        BEGIN
				                        SELECT @strOutput='20177'
                                        select @strOutput
				                        RETURN
			                        END
		                        END
		                        --查询使用数量
		                        SELECT @ConsumeQTY=isnull(sum(ConsumeQTY),0) FROM mesMaterialConsumeInfo WITH(NOLOCK) 
			                        WHERE SN='{@strSNFormat}' and MaterialTypeID=1

		                        IF @RemainQTY - @ConsumeQTY <=0
		                        BEGIN
			                        SELECT @strOutput='20177'
                                    select @strOutput
		                        END

		                        SELECT @strOutput='1'
                                SELECT @strOutput
	                        END TRY
	                        BEGIN CATCH
		                        SELECT @strOutput=ERROR_MESSAGE()
                                select @strOutput
	                        END CATCH";

            var queryRes = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return queryRes == null ? "0" : queryRes.ToString();
        }
        public async Task<string> uspRouteLastSationCheckAsync(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string sql = $@"declare	@RouteType			int,
			                        @UnitID				int,
			                        @UnitStateID		int,
			                        @PartID				int,
			                        @LineID				int,
			                        @OrderID			int,
			                        @RouteID			int,
			                        @Sequence			int,
			                        @StationTypeID		int,
			                        @RouteNumber		int,
                                    @strOutput          varchar(50)

	                        BEGIN TRY
		                        SET @strOutput='1'

		                        --查找RouteID
		                        SELECT @UnitID=B.ID,@LineID=B.LineID,@PartID=B.PartID,@OrderID=B.ProductionOrderID,
				                        @UnitStateID=UnitStateID,@StationTypeID=C.StationTypeID
		                        FROM mesSerialNumber A
			                        INNER JOIN mesUnit B ON A.UnitID=B.ID
			                        INNER JOIN mesStation C ON B.StationID=C.ID
		                        WHERE A.Value='{@strSNFormat}'


		                        --SELECT @RouteID=DBO.ufnRTEGetRouteID(@LineID,@PartID,'',@OrderID)
								DECLARE @PartFamilyID INT = 0
								select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID

								select top 1 @RouteID = RouteID
								from mesRouteMap
								where
									(LineID = @LineID or LineID is NULL OR LineID=0) AND
									(PartID = @PartID or PartID is NULL OR PartID=0) AND
									(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
									(ProductionOrderID = @OrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
								order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
		                        IF ISNULL(@RouteID,'')=''
		                        BEGIN
			                        SET @strOutput='20195'
                                    select @strOutput
		                        END

		                        --1:画图流程
		                        SELECT @RouteType = RouteType FROM mesRoute WHERE ID=@RouteID
		                        IF @RouteType=1
		                        BEGIN 	
			                        IF NOT EXISTS(SELECT 1 FROM mesUnitoUTputState A
						                        LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID
						                        WHERE A.RouteID=@RouteID and A.StationTypeID=@StationTypeID 
						                        AND ISNULL(B.Content,'')<>'TT' AND CurrStateID=0
						                         AND A.OutputStateDefID=1)
			                        OR EXISTS(SELECT 1 FROM mesStationType A 
						                        INNER JOIN V_StationTypeInfo B ON A.ID=B.StationTypeID
							                        WHERE isnull(B.Content,'')='TT' and A.ID=@StationTypeID)
			                        BEGIN
				                        SET @strOutput='20038'
                                        select @strOutput
			                        END
		                        END
		                        ELSE
		                        --表格流程
		                        BEGIN
			                        SELECT @Sequence = A.Sequence FROM (SELECT ROW_NUMBER() OVER(ORDER BY Sequence) OrderID,* 
				                        FROM mesRouteDetail) A WHERE A.RouteID=@RouteID AND A.UnitStateID=@UnitStateID

			                        SELECT @RouteNumber=COUNT(1) FROM mesRouteDetail WHERE RouteID=@RouteID

			                        IF(ISNULL(@Sequence,-1)<>ISNULL(@RouteNumber,-2))
			                        BEGIN
				                        SET @strOutput='20038'
                                        select @strOutput
			                        END
		                        END
                                select @strOutput
	                        END TRY
	                        BEGIN CATCH
		                        SELECT @strOutput=ERROR_MESSAGE()
                                select @strOutput
	                        END CATCH";
            var queryRes = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return queryRes == null ? "0" : queryRes.ToString();
        }
        /// <summary>
        /// 最后一站校验
        /// </summary>
        /// <returns></returns>
        public async Task<string> uspRouteLastSationCheckAsync_P(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string sql = $@"declare	@RouteType			int,
			                        @UnitID				int,
			                        @UnitStateID		int,
			                        @PartID				int,
			                        @LineID				int,
			                        @OrderID			int,
			                        @RouteID			int,
			                        @Sequence			int,
			                        @StationTypeID		int,
			                        @RouteNumber		int,
                                    @strOutput          varchar(50)

	                        BEGIN TRY
		                        SET @strOutput='1'

		                        --查找RouteID
		                        SELECT @UnitID=B.ID,@LineID=B.LineID,@PartID=B.PartID,@OrderID=B.ProductionOrderID,
				                        @UnitStateID=UnitStateID,@StationTypeID=C.StationTypeID
		                        FROM mesSerialNumber A
			                        INNER JOIN mesUnit B ON A.UnitID=B.ID
			                        INNER JOIN mesStation C ON B.StationID=C.ID
		                        WHERE A.Value='{@strSNFormat}'

		                        SELECT @RouteID=DBO.ufnRTEGetRouteID(@LineID,@PartID,'',@OrderID)
		                        IF ISNULL(@RouteID,'')=''
		                        BEGIN
			                        SET @strOutput='20195'
                                    select @strOutput
		                        END

		                        --1:画图流程
		                        SELECT @RouteType = RouteType FROM mesRoute WHERE ID=@RouteID
		                        IF @RouteType=1
		                        BEGIN 	
			                        IF NOT EXISTS(SELECT 1 FROM mesUnitoUTputState A
						                        LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID
						                        WHERE A.RouteID=@RouteID and A.StationTypeID=@StationTypeID 
						                        AND ISNULL(B.Content,'')<>'TT' AND CurrStateID=0
						                         AND A.OutputStateDefID=1)
			                        OR EXISTS(SELECT 1 FROM mesStationType A 
						                        INNER JOIN V_StationTypeInfo B ON A.ID=B.StationTypeID
							                        WHERE isnull(B.Content,'')='TT' and A.ID=@StationTypeID)
			                        BEGIN
				                        SET @strOutput='20038'
                                        select @strOutput
			                        END
		                        END
		                        ELSE
		                        --表格流程
		                        BEGIN
			                        SELECT @Sequence = A.Sequence FROM (SELECT ROW_NUMBER() OVER(ORDER BY Sequence) OrderID,* 
				                        FROM mesRouteDetail) A WHERE A.RouteID=@RouteID AND A.UnitStateID=@UnitStateID

			                        SELECT @RouteNumber=COUNT(1) FROM mesRouteDetail WHERE RouteID=@RouteID

			                        IF(ISNULL(@Sequence,-1)<>ISNULL(@RouteNumber,-2))
			                        BEGIN
				                        SET @strOutput='20038'
                                        select @strOutput
			                        END
		                        END
                                select @strOutput
	                        END TRY
	                        BEGIN CATCH
		                        SELECT @strOutput=ERROR_MESSAGE()
                                select @strOutput
	                        END CATCH";
            var queryRes = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return queryRes == null ? "0" : queryRes.ToString();
        }

        /// <summary>
        /// 通过ID获取线别信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<List<mesLine>> MesGetLineAsync(string ID)
        {
            string S_Sql = "select * from mesLine where StatusID = 1";

            S_Sql += string.IsNullOrEmpty(ID) ? "" : "and ID = '" + ID + "'";

            var v_Query = await DapperConn.QueryAsync<mesLine>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        /// <summary>
        /// 通过ID获取站点信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public async Task<List<mesStation>> MesGetStationAsync(string ID)
        {
            string S_Sql = "select * from mesStation where Status=1";
            S_Sql += string.IsNullOrEmpty(ID) ? "" : " and ID='" + ID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesStation>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 获取表格路径信息
        /// </summary>
        /// <param name="S_RouteSequence"></param>
        /// <param name="I_RouteID"></param>
        /// <returns></returns>
        public async Task<List<TableRouteData>> GetRouteDataTableAsync(string S_RouteSequence, int I_RouteID)
        {
            string S_Sql = string.Format(@"select A.* from 
                            (select B.ID,B.Name,B.Description RouteName,B.RouteType,
			                            A.StationTypeID,A.Sequence SequenceMod,A.Description RouteDetailName,
			                            A.UnitStateID,                                
			                            E.Description ApplicationType,
			                            cast(ROW_NUMBER() over(order by A.Sequence) as int) as Sequence
		                            from 
			                            mesRouteDetail A
			                            join mesRoute B on A.RouteID=B.ID  		  	                             
			                            join mesStationType D on D.ID=A.StationTypeID
			                            join luApplicationType E on E.ID=D.ApplicationTypeID
	                            where B.ID={0}  
                            )A where 1=1", I_RouteID);
            if (!string.IsNullOrEmpty(S_RouteSequence))
            {
                S_Sql = S_Sql + " AND A.Sequence=" + S_RouteSequence;
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TableRouteData>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 路由检查
        /// </summary>
        /// <param name="Scan_StationTypeID"></param>
        /// <param name="Scan_StationID"></param>
        /// <param name="S_LineID"></param>
        /// <param name="DT_Unit"></param>
        /// <param name="S_Str"></param>
        /// <returns></returns>
        public async Task<string> GetRouteCheckAsync(int Scan_StationTypeID, int Scan_StationID, string S_LineID,
            mesUnit DT_Unit, string S_Str)
        {
            string S_Result = "1";
            DateTime dateStart = DateTime.Now;

            try
            {
                string S_PartID = DT_Unit.PartID.ToString();
                //最后扫描工序 UnitStateID
                string S_UnitID = DT_Unit.ID.ToString();
                string S_UnitStateID = DT_Unit.UnitStateID.ToString();
                string S_UnitStationID = DT_Unit.StationID.ToString();
                string S_PartPamilyID = DT_Unit.PartFamilyID.ToString();
                string S_ProductionOrderID = DT_Unit.ProductionOrderID.ToString();
                string S_StatusID = DT_Unit.StatusID.ToString();

                //获取此料工序路径
                List<mesRoute> List_Route = await GetmesRouteAsync(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                                Convert.ToInt32(S_PartPamilyID), Convert.ToInt32(S_ProductionOrderID));
                List_Route = List_Route ?? new List<mesRoute>();

                if (List_Route.Count == 0)
                {
                    //return P_MSG_Public.MSG_Public_001; //料号未配置工艺流程路线.
                    return msgSys.MSG_Sys_20195;
                }

                int I_RouteID = List_Route[0].ID;
                int I_RouteType = List_Route[0].RouteType;  //GetRouteType(I_RouteID);
                if (I_RouteType == 0)
                {
                    IEnumerable<TableRouteData> DT_Route = await GetRouteDataTableAsync("", I_RouteID);
                    var v_Route_Sacn = from c in DT_Route
                                       where c.StationTypeID == Scan_StationTypeID
                                       select c;
                    if (v_Route_Sacn.ToList().Count() > 0)
                    {
                        if (S_StatusID == "1")
                        {

                            int I_Sequence_Scan = v_Route_Sacn.First().Sequence;
                            if (I_Sequence_Scan > 1)
                            {
                                //最后扫描信息 (上一站扫描)
                                int I_UnitStateID = DT_Unit.UnitStateID;

                                try
                                {
                                    //最后 扫描路径信息
                                    var v_Route = from c in DT_Route
                                                  where c.UnitStateID == I_UnitStateID
                                                  select c;
                                    //最后 扫描顺序
                                    int I_Sequence = v_Route.First().Sequence;
                                    //最后扫描顺序  比 当前扫描顺序
                                    if (I_Sequence >= I_Sequence_Scan)
                                    {
                                        //S_Result = P_MSG_Public.MSG_Public_005;  //此条码已过站.
                                        S_Result = msgSys.MSG_Sys_20016;
                                    }
                                    else
                                    {
                                        //判断上一站是否扫描
                                        if (I_Sequence_Scan - 1 != I_Sequence)
                                        {
                                            //S_Result = P_MSG_Public.MSG_Public_006; //上一站未扫描.
                                            S_Result = msgSys.MSG_Sys_20017;
                                        }
                                    }
                                }
                                catch
                                {
                                    //S_Result = P_MSG_Public.MSG_Public_006; //上一站未扫描.
                                    S_Result = msgSys.MSG_Sys_20017;
                                }
                            }
                        }
                        else
                        {
                            //S_Result = P_MSG_Public.MSG_Public_007;  //此条码已NG.
                            S_Result = msgSys.MSG_Sys_20036;
                        }
                    }
                    else
                    {
                        //没有配置 此工位工序
                        S_Result = P_MSG_Public.MSG_Public_008;
                    }
                }
                else if (I_RouteType == 1)
                {
                    string S_Sql =
                    @"	declare	
			            @UnitID				 int,
			            @StationTypeID		 int,
			            @OldStationTypeType	 nvarchar(64),
			            @NewStationTypeType	 nvarchar(64),
			            @OldUnitStateID		 int,
			            @strOutput           nvarchar(200)		
		
                    BEGIN TRY
		                SET @strOutput=1
		                SET @UnitID ='" + S_UnitID + @"'
                        SET @StationTypeID='" + Scan_StationTypeID + @"'

		                --判断是否为新流程  料号未配置工艺流程路线
		                IF NOT EXISTS(SELECT 1 FROM mesRoute WHERE ID='" + I_RouteID + @"' AND RouteType=1)
		                BEGIN
			                SET @strOutput='" + msgSys.MSG_Sys_20195 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
		                IF NOT EXISTS(SELECT 1 FROM mesUnitInputState WHERE RouteID='" + I_RouteID + @"' AND StationTypeID=@StationTypeID)
		                BEGIN
			                SET @strOutput='" + msgSys.MSG_Sys_20203 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --原流程校验
		                SELECT TOP 1 @OldUnitStateID=UnitStateID FROM mesUnit WHERE ID=@UnitID
		
		                IF not exists(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID 
					                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
		                BEGIN
			                DECLARE @ShowState varchar(200)
			                SELECT @ShowState=isnull(Description,null)+'-'+isnull(cast(ID as varchar),'') FROM mesUnitState WHERE ID=@OldUnitStateID
			                SET @strOutput='" + msgSys.MSG_Sys_20241 + @"'
                            Select  @strOutput as ValStr1
			                RETURN
		                END

		                --条码对应StationTypeType-旧
		                SELECT @OldStationTypeType = E.Content FROM mesUnit B
		                INNER JOIN mesStation C ON B.StationID=C.ID
		                INNER JOIN mesStationType D ON C.StationTypeID=D.ID
		                INNER JOIN mesStationTypeDetail E ON D.ID=E.StationTypeID
		                INNER JOIN luStationTypeDetailDef F ON E.StationTypeDetailDefID=F.ID AND F.Description='StationTypeType'
		                WHERE B.ID=@UnitID

		                --当前选择对应StationTypeType-新
		                SELECT @NewStationTypeType = A.Content FROM mesStationTypeDetail A
		                INNER JOIN luStationTypeDetailDef B ON A.StationTypeDetailDefID=B.ID AND B.Description='StationTypeType'
		                WHERE A.StationTypeID=@StationTypeID

		                --如果从TT房间出来,需要进行之前流程校验
		                IF ISNULL(@OldStationTypeType,'')<>ISNULL(@NewStationTypeType,'') AND ISNULL(@OldStationTypeType,'')='TT'
		                BEGIN
			                SELECT TOP 1 @OldUnitStateID=reserved_20 FROM mesUnitDetail WHERE UnitID=@UnitID

			                --判断旧状态是否为最后一站
			                IF EXISTS(SELECT 1 FROM mesUnitoUTputState A
						                LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID
						                WHERE A.RouteID='" + I_RouteID + @"' and A.StationTypeID=@StationTypeID 
						                AND ISNULL(B.Content,'')<>'TT' AND CurrStateID=0
						                 AND A.OutputStateDefID=1)
			                BEGIN
				                --最后一站可能是上一站过来或最后一站出去再进来
				                IF NOT EXISTS (SELECT 1 FROM mesUnitOutputState WHERE StationTypeID=@StationTypeID
							                AND RouteID='" + I_RouteID + @"' AND OutputStateID=@OldUnitStateID)
				                   AND NOT EXISTS(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID  
									                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
				                BEGIN
					                SET @strOutput='" + msgSys.MSG_Sys_20241 + @"'	
                                    Select  @strOutput as ValStr1
					                RETURN
				                END
			                END
			                ELSE
			                BEGIN
				                IF NOT EXISTS(SELECT 1 FROM mesUnitInputState A WHERE A.StationTypeID=@StationTypeID 
					                AND CurrStateID = @OldUnitStateID AND RouteID='" + I_RouteID + @"')
				                BEGIN					            
					                SET @strOutput='" + msgSys.MSG_Sys_20241 + @"'	
                                    Select  @strOutput as ValStr1
					                RETURN
				                END
			                END
		                END
		
		                IF 	ISNULL(@NewStationTypeType,'')='TT'
		                BEGIN
			                --判断是否已经投入使用
			                IF EXISTS (SELECT 1 FROM mesUnitComponent WHERE ChildUnitID=@UnitID AND StatusID=1)
			                BEGIN
				                SET @strOutput='" + msgSys.MSG_Sys_20038 + @"'
                                Select  @strOutput as ValStr1
				                RETURN
			                END
			                --首次进入TT进行UnitDetail记录
			                IF ISNULL(@OldStationTypeType,'')<>ISNULL(@NewStationTypeType,'')
			                BEGIN
				                UPDATE A SET A.reserved_20=B.UnitStateID FROM mesUnitDetail A 
				                INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@UnitID
			                END
		                END
                        Select  @strOutput as ValStr1
	                END TRY
	                BEGIN CATCH
		                SELECT @strOutput=ERROR_MESSAGE()
                        Select  @strOutput as ValStr1
	                END CATCH                      
                    ";

                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    IEnumerable<TabVal> List_Reult = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                    if (List_Reult.Count() > 0)
                    {
                        S_Result = List_Reult.First().ValStr1;
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }

            return S_Result;
        }
        public async Task<mesPart> GetmesPartAsync(string sPartId)
        {
            string S_Sql = "select * from mesPart Where ID='" + sPartId + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            return await DapperConnRead2.QueryFirstAsync<mesPart>(S_Sql, null, null, I_DBTimeout, null);
        }
        public async Task<List<mesProductionOrder>> GetmesProductionOrderAsync(string PoId = "")
        {
            string S_Sql = "SELECT * FROM dbo.mesProductionOrder ";

            if (!string.IsNullOrEmpty(PoId))
            {
                S_Sql += $" WHERE ID = {PoId}";
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesProductionOrder>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// GetmesUnit
        /// </summary>
        /// <param name="S_ID"></param>
        /// <returns></returns>
        public async Task<List<mesUnit>> GetmesUnitAsync(string S_ID)
        {
            string S_Sql = "select * from mesUnit Where ID='" + S_ID + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取SN
        /// </summary>
        /// <param name="S_UnitID"></param>
        /// <param name="S_Value"></param>
        /// <returns></returns>
        public async Task<List<mesSerialNumber>> GetmesSerialNumberAsync(string S_UnitID, string S_Value)
        {
            S_UnitID = S_UnitID ?? "";
            S_Value = S_Value ?? "";
            string S_Sql = "select * from mesSerialNumber ";
            if (S_UnitID != "") { S_Sql += " where UnitID='" + S_UnitID + "'"; }
            if (S_UnitID == "")
            {
                if (S_Value != "") { S_Sql += " where Value='" + S_Value + "'"; }
            }
            else
            {
                if (S_Value != "") { S_Sql += " and Value='" + S_Value + "'"; }
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<mesSerialNumber>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 工单数量
        /// </summary>
        /// <param name="I_StationID"></param>
        /// <param name="I_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetProductionOrderQTYAsync(int I_StationID, int I_ProductionOrderID)
        {
            string S_Sql =
                @"
		        --根据工站类型统计工单数量
	            DECLARE @prodId				int,
			            @stationID			int,
			            @stationTypeID		int,
			            @prodCount			int,
			            @currentCount		int

		        SELECT @stationTypeID=StationTypeID FROM mesStation WITH(NOLOCK) WHERE ID='" + I_StationID + @"'

		        SELECT @prodCount=OrderQuantity FROM mesProductionOrder WITH(NOLOCK) WHERE ID='" + I_ProductionOrderID + @"'

		        SELECT @currentCount=SUM(ConsumeQTY) FROM mesProductionOrderConsumeInfo WITH(NOLOCK)
			        WHERE ProductionOrderID='" + I_ProductionOrderID + @"' AND StationTypeID=@stationTypeID

		        SELECT ISNULL(@currentCount,0) AS currentCount,ISNULL(@prodCount,0) AS prodCount
            ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取路由详细信息
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetRouteDetailAsync(string S_LineID, string S_PartID,
            string S_PartFamilyID, string S_ProductionOrderID)
        {
            List<mesRoute> List_Route = await GetmesRouteAsync(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                            Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_ProductionOrderID));
            List_Route = List_Route ?? new List<mesRoute>();
            if (List_Route == null)
            {
                return null;
            }

            string S_RouteType = List_Route[0].RouteType.ToString();
            string S_RouteID = List_Route[0].ID.ToString();

            string S_Sql = @"select B.Description,ROW_NUMBER() OVER(ORDER BY A.Sequence) Sequence,a.ID,
                                RouteID,StationTypeID,UnitStateID from mesRouteDetail A
                                    inner join mesStationType B ON A.StationTypeID = B.ID
                             WHERE A.RouteID='" + S_RouteID + "'";


            if (S_RouteType == "1")
            {
                S_Sql = @"	select B.Description StationType
	                           ,C.Description  CurrentStation 
	                           ,D.Description  OutputStation 
                                ,E.Description StateDef
                                ,A.StationTypeID
                                ,(case F.Content when 'TT' then 'TT' else 'SFC' end) StationTypeType
	                            from mesUnitOutputState A 
			                     left join mesStationType B on A.StationTypeID=B.ID	
			                    left join mesUnitState C on A.CurrStateID=C.ID
			                    left join mesUnitState D on A.OutputStateID=D.ID
                                join luUnitStatus E on E.ID=A.OutputStateDefID
                                left join mesStationTypeDetail F on A.StationTypeID=F.StationTypeID	
	                    where A.RouteID=" + S_RouteID + @" 		
	                    order by A.StationTypeID	";
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取画图路径信息
        /// </summary>
        /// <param name="S_StationTypeID"></param>
        /// <param name="I_RouteID"></param>
        /// <param name="S_DataType"></param>
        /// <returns></returns>
        public async Task<List<dynamic>> GetRouteDataDiagramAsync(string S_StationTypeID, int I_RouteID, string S_DataType)
        {
            IEnumerable<dynamic> List_Result = null;
            string S_Data1 =
            @"
		        SELECT A.*,C.Description StationType,
			        D.Description OutputStation,E.Description CurrentStation,B.Description Status FROM #tempRoute A
		        LEFT JOIN luUnitStatus B ON A.OutputStateDefID=B.ID
		        LEFT JOIN mesStationType C ON A.StationTypeID=C.ID
		        LEFT JOIN mesUnitState D ON A.CurrStateID=D.ID
		        LEFT JOIN mesUnitState E ON A.OutputStateID = E.ID
		        WHERE RouteType =(case when EXISTS(SELECT 1 FROM V_StationTypeInfo WHERE 
												        StationTypeID ='" + S_StationTypeID + @"' AND Content='TT') then 1 else 2 end)
            ";

            string S_Data2 =
            @"
		        SELECT A.*,C.Description StationType,
			        E.Description CurrentStation,D.Description OutputStation,B.Description Status FROM #tempRoute A
		        LEFT JOIN luUnitStatus B ON A.OutputStateDefID=B.ID
		        LEFT JOIN mesStationType C ON A.StationTypeID=C.ID
		        LEFT JOIN mesUnitState D ON A.CurrStateID=D.ID
		        LEFT JOIN mesUnitState E ON A.OutputStateID = E.ID
		        WHERE RouteType=0 AND EXISTS(SELECT 1 FROM #tempRoute T WHERE A.ParentID=T.ID AND 
										        T.RouteType=(case when EXISTS(SELECT 1 FROM V_StationTypeInfo WHERE 
										        StationTypeID ='" + S_StationTypeID + @"' AND Content='TT') then 1 else 2 end))
            ";

            string S_End =
            @"END TRY
	        BEGIN CATCH
		        SELECT @strOutput=ERROR_MESSAGE()
	        END CATCH";

            string S_SqlVal =
            @"
	        declare	@CurrStateID			VARCHAR(64),
			        @Count					int,
			        @Type					VARCHAR(64),
                    @strOutput              NVARCHAR(MAX)  

	        BEGIN TRY
		        SET @strOutput='1'

		        SELECT CAST(ROW_NUMBER() OVER(ORDER BY (SELECT 0)) AS int )ID,A.StationTypeID,a.RouteID,B.CurrStateID,A.OutputStateID,
				        A.OutputStateDefID,0 RouteType,0 ParentID INTO #tempRoute 
			        FROM (SELECT max(ID) as ID,'" + I_RouteID + @"' as RouteID,StationTypeID,OutputStateID,OutputStateDefID 
					        FROM mesUnitOutputState WHERE RouteID='" + I_RouteID + @"' AND StationTypeID>0
					        GROUP BY StationTypeID,OutputStateID,OutputStateDefID) A 
			        INNER JOIN mesUnitInputState B ON A.StationTypeID=B.StationTypeID
			        WHERE B.RouteID='" + I_RouteID + @"'

		        UPDATE A SET A.RouteType='1' FROM #tempRoute A 
		        INNER JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID AND B.DetailDef='StationTypeType'
		        INNER JOIN #tempRoute C ON A.CurrStateID=C.OutputStateID 
		        INNER JOIN V_StationTypeInfo D ON C.StationTypeID=D.StationTypeID AND D.DetailDef='StationTypeType'
		        WHERE (B.Content='TT' AND D.Content='TT') or (isnull(a.CurrStateID,'')='' AND D.Content='TT')
			        or (B.Content='TT'  AND isnull(a.CurrStateID,'')='')

		        UPDATE A SET A.RouteType='2' FROM #tempRoute A 
		        LEFT JOIN V_StationTypeInfo B ON A.StationTypeID=B.StationTypeID AND B.DetailDef='StationTypeType'
		        LEFT JOIN #tempRoute C ON A.CurrStateID=C.OutputStateID 
		        LEFT JOIN V_StationTypeInfo D ON C.StationTypeID=D.StationTypeID AND D.DetailDef='StationTypeType'
		        WHERE ISNULL(B.Content,'')<>'TT' AND ISNULL(D.Content,'')<>'TT' or (isnull(a.CurrStateID,'')='' AND 
			        ISNULL(D.Content,'')<>'TT') OR (ISNULL(B.Content,'')<>'TT' AND isnull(a.CurrStateID,'')='')

		        UPDATE A SET A.ParentID=B.ID FROM #tempRoute A 
		        INNER JOIN (SELECT StationTypeID,RouteType,MAX(ID) ID,OutputStateDefID FROM #tempRoute 
			        WHERE ISNULL(RouteType,'')<>''  GROUP BY StationTypeID,RouteType,OutputStateDefID) B 
		        ON A.StationTypeID=B.StationTypeID AND A.OutputStateDefID=B.OutputStateDefID
		        WHERE ISNULL(A.RouteType,'')=0

            ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            string S_Sql = "";
            if (S_DataType == "1")
            {
                S_Sql = S_SqlVal + S_Data1 + S_End;
                var v_Query1 = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                List_Result = v_Query1;
            }
            else if (S_DataType == "2")
            {
                S_Sql = S_SqlVal + S_Data2 + S_End;
                var v_Query2 = await DapperConn.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                List_Result = v_Query2;
            }

            return List_Result.ToList();
        }
        /// <summary>
        /// BOM
        /// </summary>
        /// <param name="S_ParentPartID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<List<mesProductStructure>> GetBOMStructureAsync(string S_ParentPartID, string S_PartID, string S_StationTypeID)
        {
            string S_Sql = string.Format(@"select A.ParentPartID,C.PartNumber ParentPartNumber, C.Description ParentPartDescription,
                             B.PartNumber PartNumber, B.Description PartDescription, A.PartID,A.StationTypeID
                               from mesProductStructure A inner join mesPart B
                            ON A.PartID = B.ID INNER JOIN mesPart C ON A.ParentPartID = C.ID
                            WHERE A.Status = 1 AND('{0}' = '' or ParentPartID = '{0}')
                            and('{1}' = '' or PartID = '{1}')
                            and('{2}' = '' or StationTypeID = '{2}')", S_ParentPartID, S_PartID, S_StationTypeID);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesProductStructure>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <param name="I_LineID"></param>
        /// <param name="I_PartID"></param>
        /// <param name="I_PartFamilyID"></param>
        /// <param name="I_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<mesRoute>> GetmesRouteAsync(int I_LineID, int I_PartID, int I_PartFamilyID, int I_ProductionOrderID)
        {
            //int I_Reult = 0;
            string S_Sql = "";
            IEnumerable<TabVal> v_TabVal = new List<TabVal>();

            if (I_LineID == 0 && I_PartID == 0 && I_PartFamilyID == 0 && I_ProductionOrderID == 0)
            {
                return null;
            }

            S_Sql =
                @"

	            declare @LineID int = '" + I_LineID + @"'
	            declare @PartID int = '" + I_PartID + @"'
	            declare @PartFamilyID int = '" + I_PartFamilyID + @"'
	            declare @ProductionOrderID int = '" + I_ProductionOrderID + @"'

	            declare @nRoutID int
	            declare @nPartID int
	            declare @nLineID int
	            declare @nPartFamilyID int

	            set @PartID= case when @PartID=0 then null else @PartID end 
	            set @LineID= case when @LineID=0 then null else @LineID end 
	            set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 

	            if @PartID is not NULL and @PartFamilyID is not NULL begin
		            if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID) 
		            begin
		                SELECT null ID, null Name,null Description,null RouteType
		                RETURN
		            end
	            end else if @PartFamilyID is NULL begin
		            select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	            end

	            select top 1 @nRoutID = RouteID, @nPartID = PartID, @nLineID = LineID, @nPartFamilyID = PartFamilyID
	            from mesRouteMap
	            where
		            (LineID = @LineID or LineID is NULL OR LineID=0) AND
		            (PartID = @PartID or PartID is NULL OR PartID=0) AND
		            (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		            (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
	            order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc

                 select ID,Name,Description,RouteType from mesRoute where ID= @nRoutID 
                 ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_QueryResult = await DapperConn.QueryAsync<mesRoute>(S_Sql, null, null, I_DBTimeout, null);
            return v_QueryResult.ToList();

        }
        /// <summary>
        /// 料号详细
        /// </summary>
        /// <param name="I_PartID"></param>
        /// <param name="I_PartDetailDefName"></param>
        /// <returns></returns>
        public async Task<List<mesPartDetail>> GetmesPartDetailAsync(int I_PartID, string I_PartDetailDefName)
        {
            string S_Sql = @"select * from mesPartDetail a  WHERE  a.PartID=" + I_PartID +
                           " and exists (select 1 from luPartDetailDef b where a.PartDetailDefID = b.ID and b.Description = '" +
                           I_PartDetailDefName + "')";

            var v_Query = await DapperConn.QueryAsync<mesPartDetail>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }
        public async Task<List<TabVal>> GetPatternAsync(string S_PartID, LoginList List_Login)
        {
            List<TabVal> List_Result = new List<TabVal>();
            string S_Batch_Pattern = "";
            string S_SN_Pattern = "";

            try
            {
                int I_PartID = Convert.ToInt32(S_PartID);
                List<mesPartDetail> List_mesPartDetail = await GetmesPartDetailAsync(I_PartID, "Batch_Pattern");

                if (List_mesPartDetail.Count > 0)
                {
                    S_Batch_Pattern = List_mesPartDetail[0].Content.Trim();
                    S_Batch_Pattern = PublicF.EncryptPassword(S_Batch_Pattern, S_PwdKey);
                }
                else
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_014, "0", List_Login, "");//未配置校验批次正则表达式
                    return List_Result;
                }

                List_mesPartDetail = await GetmesPartDetailAsync(I_PartID, "SN_Pattern");
                if (List_mesPartDetail.Count > 0)
                {
                    S_SN_Pattern = List_mesPartDetail[0].Content.Trim();
                    S_SN_Pattern = PublicF.EncryptPassword(S_SN_Pattern, S_PwdKey);
                }
                else
                {
                    List_Result = List_ERROR_TabVal(P_MSG_Public.MSG_Public_015, "0", List_Login, "");//未配置校验SN正则表达式
                    return List_Result;
                }

                string S_Sql = "select '" + S_Batch_Pattern + "' as ValStr1,'" + S_SN_Pattern + "' ValStr2";
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                return v_Query.AsList();
            }
            catch (Exception ex)
            {
                List_Result = List_ERROR_TabVal(ex, "0", List_Login, "");
            }

            return List_Result;
        }
        public async Task<(string, BarcodePattern)> GetPatternAsync(string S_PartID, string pattern = "")
        {
            BarcodePattern mBarcodePattern = new BarcodePattern();
            string S_Batch_Pattern = "";
            string S_SN_Pattern = "";

            try
            {
                int I_PartID = Convert.ToInt32(S_PartID);
                if (string.IsNullOrEmpty(pattern))
                {

                    List<mesPartDetail> List_mesPartDetail = await GetmesPartDetailAsync(I_PartID, "Batch_Pattern");

                    if (List_mesPartDetail.Count <= 0)
                        return (P_MSG_Public.MSG_Public_014, null);//未配置校验批次正则表达式


                    S_Batch_Pattern = List_mesPartDetail[0].Content.Trim();
                    mBarcodePattern.BatchPattern = PublicF.EncryptPassword(S_Batch_Pattern, S_PwdKey);

                    List_mesPartDetail = await GetmesPartDetailAsync(I_PartID, "SN_Pattern");
                    if (List_mesPartDetail.Count > 0)
                        return (P_MSG_Public.MSG_Public_015, null);//未配置校验SN正则表达式

                    S_SN_Pattern = List_mesPartDetail[0].Content.Trim();
                    mBarcodePattern.SNPattern = PublicF.EncryptPassword(S_SN_Pattern, S_PwdKey);
                }
                else
                {
                    var pattDetails = await GetmesPartDetailAsync(I_PartID, pattern);
                    if (pattDetails.Count > 0)
                    {
                        S_SN_Pattern = pattDetails[0].Content.Trim();
                        mBarcodePattern.AppointPattern = PublicF.EncryptPassword(S_SN_Pattern, S_PwdKey);
                    }
                    else
                    {
                        return (P_MSG_Public.MSG_Public_015, null);//未配置校验SN正则表达式
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                return (ex.Message, null);
            }
            return ("", mBarcodePattern);
        }
        /// <summary>
        /// 获取工单分线,不检查工单
        /// </summary>
        /// <param name="S_LineID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <returns></returns>
        public async Task<List<mesLineOrder>> GetmesLineOrderAsync(string S_LineID, string S_PartID, string S_ProductionOrderID = "")
        {
            S_LineID = S_LineID ?? "";
            S_PartID = S_PartID ?? "";

            if (S_LineID == "")
            {
                return null;
            }
            if (S_LineID == "" && S_PartID == "")
            {
                return null;
            }

            string S_Sql =
                @"SELECT * FROM mesLineOrder WHERE LineID='" + S_LineID + @"' AND  ProductionOrderID IN 
                (
                   SELECT ID FROM mesProductionOrder WHERE PartID='" + S_PartID + @"'
                )";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesLineOrder>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.ToList();
        }

        /// <summary>
        /// 处理 TabVal 
        /// </summary>
        /// <param name="S_Code">ERROR, OK, 1, 0 ...</param>
        /// <param name="S_MSG"></param>
        /// <param name="S_DataStatus"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> GetTabValAsync(string S_Code, string S_MSG, string S_DataStatus)
        {
            string S_Sql = "select '" + S_Code + "' as ValStr1,'" + S_MSG + "' ValStr2,'" + S_DataStatus + "' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<string> uspCallProcedureAsync(string Pro_Name, string S_FormatName, string xmlProdOrder, string xmlPart,
                                        string xmlStation, string xmlExtraData, string strSNbuf)
        {
            string strOutput = "";
            try
            {
                if (string.IsNullOrEmpty(Pro_Name))
                {
                    //过程名称不能为空.
                    strOutput = P_MSG_Public.MSG_Public_023;
                    return null;
                }
                //S_FormatName = string.IsNullOrEmpty(S_FormatName) ? "null" : S_FormatName;
                //xmlProdOrder = string.IsNullOrEmpty(xmlProdOrder) ? "null" : xmlProdOrder;
                //xmlPart = string.IsNullOrEmpty(xmlPart) ? "null" : xmlPart;
                //xmlStation = string.IsNullOrEmpty(xmlStation) ? "null" : xmlStation;
                //xmlExtraData = string.IsNullOrEmpty(xmlExtraData) ? "null" : xmlExtraData;
                //strSNbuf = string.IsNullOrEmpty(strSNbuf) ? "null" : strSNbuf;

                DynamicParameters dp = new DynamicParameters();
                dp.Add("@strSNFormat", S_FormatName, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@xmlProdOrder", xmlProdOrder, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@xmlPart", xmlPart, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@xmlStation", xmlStation, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@xmlExtraData", xmlExtraData, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@strSNbuf", strSNbuf, DbType.String, ParameterDirection.Input, 1024);
                dp.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 1024);

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                var v_Query = await DapperConnRead2.ExecuteAsync(Pro_Name, dp, null, I_DBTimeout, CommandType.StoredProcedure);
                strOutput = dp.Get<string>("@strOutput");

            }
            catch (Exception ex)
            {
                strOutput = ex.Message.ToString();
            }

            //List<TabVal> List_TabVal = await GetTabValAsync(strOutput, "", "1");
            return strOutput;
        }
        /// <summary>
        /// 夹具获取产品SN
        /// </summary>
        /// <param name="S_MachineSN"></param>
        /// <returns></returns>
        public async Task<List<TabVal>> MesGetBatchIDByBarcodeSNAsync(string S_MachineSN)
        {
            string S_Sql = string.Format("select * from mesMachine where SN='{0}' AND StatusID=2", S_MachineSN);
            var v_Query = await DapperConn.QueryAsync<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query != null && v_Query.Count() > 0)
            {
                S_Sql = string.Format("SELECT top 1 reserved_02 as ValStr1,UnitID as ValStr2 FROM mesUnitDetail " +
                                      " WHERE reserved_01='{0}' and reserved_03=1 order by id desc", S_MachineSN);

                var v_SN = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                return v_SN.ToList();
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            return null;
        }
        /// <summary>
        /// SetConfirmPOAsync 确认工单 
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<UniversalConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, LoginList List_Login)
        {
            string S_LineID = List_Login.LineID.ToString();

            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "0";
            S_PartFamilyID = S_PartFamilyID ?? "0";
            S_PartID = S_PartID ?? "0";
            S_POID = S_POID ?? "0";
            S_UnitStatus = S_UnitStatus ?? "0";


            UniversalConfirmPoOutput mUniversalConfirmPoOutput = new UniversalConfirmPoOutput();
            try
            {
                if (S_POID == "0" && S_IsCheckPO == "1")
                {
                    mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_009;//工单不能为空,请确认
                    return mUniversalConfirmPoOutput;
                }
                else
                {
                    if (S_IsCheckPN == "1")
                    {
                        if (S_PartFamilyTypeID == "0")
                        {
                            mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_010;//未选择料号类别,请确认
                            return mUniversalConfirmPoOutput;
                        }
                        if (S_PartFamilyID == "0")
                        {
                            mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_011;//未选择料号群,请确认.
                            return mUniversalConfirmPoOutput;
                        }
                        if (S_PartID == "0")
                        {
                            mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_012;//未选择料号,请确认.
                            return mUniversalConfirmPoOutput;
                        }
                    }

                    if (S_IsCheckPO == "0")
                    {
                        List<mesLineOrder> list_mesLineOrder = await GetmesLineOrderAsync(List_Login.LineID.ToString(), S_PartID, "");
                        if (list_mesLineOrder == null || list_mesLineOrder.Count < 1)
                        {
                            mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_024;//料号和线别不匹配.
                            return mUniversalConfirmPoOutput;
                        }
                    }

                    // BOM
                    List<mesProductStructure> List_mesProductStructure = await GetBOMStructureAsync(S_PartID, "", "");
                    mUniversalConfirmPoOutput.MesProductStructures = List_mesProductStructure;

                    //Route
                    List<mesRoute> List_Route = await GetmesRouteAsync(Convert.ToInt32(S_LineID), Convert.ToInt32(S_PartID),
                                    Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_POID));
                    List_Route = List_Route ?? new List<mesRoute>();

                    if (List_Route.Count == 0)
                    {
                        mUniversalConfirmPoOutput.ErrorMsg = P_MSG_Public.MSG_Public_001;//料号未配置工艺流程路线.
                        return mUniversalConfirmPoOutput;
                    }

                    int I_RouteID = List_Route[0].ID;
                    int I_RouteType = List_Route[0].RouteType;
                    if (I_RouteType == 1)
                    {
                        List<dynamic> List_RouteDataDiagram1 = await
                            GetRouteDataDiagramAsync(List_Login.StationTypeID.ToString(), I_RouteID, "1");
                        mUniversalConfirmPoOutput.RouteDataDiagram1 = List_RouteDataDiagram1;

                        List<dynamic> List_RouteDataDiagram2 = await
                            GetRouteDataDiagramAsync(List_Login.StationTypeID.ToString(), I_RouteID, "2");
                        mUniversalConfirmPoOutput.RouteDataDiagram2 = List_RouteDataDiagram2;
                    }
                    else
                    {
                        List<dynamic> List_RouteDetail = await
                            GetRouteDetailAsync(S_LineID, S_PartID, S_PartFamilyID, S_POID);
                        mUniversalConfirmPoOutput.RouteDetail = List_RouteDetail;
                    }
                    //工单数量
                    List<dynamic> List_ProductionOrderQTY = await
                        GetProductionOrderQTYAsync(List_Login.StationID, Convert.ToInt32(S_POID));
                    mUniversalConfirmPoOutput.ProductionOrderQTY = List_ProductionOrderQTY;



                    int I_PartID = Convert.ToInt32(S_PartID);
                    List<mesPartDetail> List_mesPartDetail = await GetmesPartDetail(I_PartID, "Batch_Pattern");

                    if (List_mesPartDetail.Count > 0)
                    {
                        mUniversalConfirmPoOutput.S_Batch_Pattern = List_mesPartDetail[0].Content.Trim();
                    }
                    //else
                    //{
                    //    List_Result = List_ERROR(P_MSG_Public.MSG_Public_014, "0", List_Login);//未配置校验批次正则表达式
                    //    return List_Result;
                    //}

                    List_mesPartDetail = await GetmesPartDetail(I_PartID, "SN_Pattern");
                    if (List_mesPartDetail.Count > 0)
                    {
                        mUniversalConfirmPoOutput.S_SN_Pattern = List_mesPartDetail[0].Content.Trim();
                    }
                    //else
                    //{
                    //    List_Result = List_ERROR(P_MSG_Public.MSG_Public_015, "0", List_Login);//未配置校验SN正则表达式
                    //    return List_Result;
                    //}
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                mUniversalConfirmPoOutput.ErrorMsg = ex;
            }

            return mUniversalConfirmPoOutput;
        }


        //public async Task<List<BomPartInfo>> MESGetBomPartInfoEntityAsync(int ParentPartID, int StationTypeID)
        //{
        //    string S_Sql =
        //        $"SELECT *,'' as 'Barcode',0 as 'IsMainSn' FROM DBO.ufnGetBomPartInfoByStationType({ParentPartID},{StationTypeID})";

        //    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
        //    {

        //    }
        //    var v_Query = await DapperConn.QueryAsync<BomPartInfo>(S_Sql, null, null, I_DBTimeout, null);
        //    return v_Query.ToList();
        //}



        /// <summary>
        /// GetPageStationInfoAsync 页面初始化
        /// </summary>
        /// <param name="S_StationID"></param>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        public async Task<InitPageInfo> GetPageStationInfoAsync(LoginList List_Login, string S_URL)
        {
            InitPageInfo List_Result = new InitPageInfo();
            try
            {
                StationAttributes oStationAttributes = new StationAttributes();

                #region 获取站点详细信息
                List<TabVal> List_StationConfig = await GetStationConfigSetting("", List_Login.StationID.ToString());
                List<TabVal> List_StationTypeDetail = await GetStationTypeDetail(List_Login.StationTypeID.ToString());

                //未配置详细 无法获取工站类型
                //if (List_StationTypeDetail.Count > 0)
                //{
                //    oStationAttributes.ApplicationType = S_ApplicationType = List_StationTypeDetail[0].ValStr3 ?? "";
                //}
                oStationAttributes.ApplicationType = S_ApplicationType =  await GetStationAPPTypeAsync(List_Login.StationID.ToString(), List_Login.StationTypeID.ToString());


                oStationAttributes.IsCheckPO = S_IsCheckPO = ConvertProperties(List_StationConfig, List_StationTypeDetail, "IsCheckPO") ?? oStationAttributes.IsCheckPO;
                oStationAttributes.IsCheckPN = S_IsCheckPN = ConvertProperties(List_StationConfig, List_StationTypeDetail, "IsCheckPN") ?? oStationAttributes.IsCheckPN;
                oStationAttributes.COF = ConvertProperties(List_StationConfig, List_StationTypeDetail, "COF") ?? oStationAttributes.COF;
                oStationAttributes.IsCheckVendor = ConvertProperties(List_StationConfig, List_StationTypeDetail, "IsCheckVendor") ?? oStationAttributes.IsCheckVendor;
                oStationAttributes.SNScanType = ConvertProperties(List_StationConfig, List_StationTypeDetail, "SNScanType") ?? oStationAttributes.SNScanType;
                oStationAttributes.IsDOEPrint = ConvertProperties(List_StationConfig, List_StationTypeDetail, "IsDOEPrint") ?? oStationAttributes.IsDOEPrint;

                string S_Sql =
                  @"SELECT EnCode AS ValStr1,FullName AS ValStr2 FROM API_Menu  WHERE EnCode='" +
                        S_URL + "' AND FullName='" + S_ApplicationType + "'";
                var v_Query_Menu = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                if (v_Query_Menu != null && v_Query_Menu.Any())
                {
                    List_Result.IsLegalPage = "1";
                }

                List_Result.stationAttribute = oStationAttributes;
                #endregion
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
            }
            return List_Result;
        }
        /// <summary>
        /// 获取站点全部配置信息
        /// </summary>
        /// <param name="List_Login"></param>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        public async Task<(string, InitPageInfo)> GetPageAllStationInfoAsync(LoginList List_Login, string S_URL)
        {
            InitPageInfo List_Result = new InitPageInfo();
            StationAttributes oStationAttributes = new StationAttributes();
            #region 获取站点详细信息
            List<StationAttribute> List_StationConfig = await GetStationConfigSettingAsync(List_Login.StationID.ToString());
            List<StationAttribute> List_StationTypeDetail = await GetStationTypeDetailAsync(List_Login.StationTypeID.ToString());

            string tmpAppType =
                await GetStationAPPTypeAsync(List_Login.StationID.ToString(), List_Login.StationTypeID.ToString());

            if (!string.IsNullOrEmpty(tmpAppType))
                oStationAttributes.ApplicationType = tmpAppType;

            string S_Sql =
                @"SELECT EnCode AS ValStr1,FullName AS ValStr2 FROM API_Menu  WHERE EnCode='" +
                S_URL + "' AND FullName='" + tmpAppType + "'";
            var v_Query_Menu = await DapperConn.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            if (v_Query_Menu != null && v_Query_Menu.Count() > 0)
            {
                List_Result.IsLegalPage = "1";
                var msg = ConvertProperties(List_StationConfig, List_StationTypeDetail, ref oStationAttributes);
                if (!string.IsNullOrEmpty(msg))
                {
                    return (msg, List_Result);
                }
            }
            List_Result.stationAttribute = oStationAttributes;
            #endregion

            return ("", List_Result);
        }

        public async Task<InitPageInfo> GetPagePoInfoAsync(string S_POID, InitPageInfo initPageInfo, LoginList List_Login)
        {
            InitPageInfo List_Result = initPageInfo;
            try
            {
                PoAttributes oPoAttributes = new PoAttributes();
                if (List_Result?.stationAttribute?.IsDOEPrint == "1")
                {
                    var DOEList = await GetluPODetailDef(int.Parse(S_POID), "DOE_Parameter1");
                    if (DOEList != null && DOEList.Count > 0)
                    {
                        oPoAttributes.DOE_Parameter1 = PublicCommonFun.ConvertDynamic(DOEList, "Content");

                        List_Result.poAttributes = oPoAttributes;
                        oPoAttributes.ColorCode = oPoAttributes.DOE_Parameter1.Split(',').ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                SetMesLog(ex.Message, "NG", List_Login);
            }
            return List_Result;
        }
        public async Task<InitPageInfo> GetAllPagePoInfoAsync(string S_POID, InitPageInfo initPageInfo)
        {
            InitPageInfo List_Result = initPageInfo;
            try
            {
                PoAttributes oPoAttributes = new PoAttributes();

                var poDetails = await GetluPODetailDefs(S_POID.ToInt());
                foreach (PoDetailDefs poDetail in poDetails)
                {
                    if (string.IsNullOrEmpty(poDetail.PODetailDef) || string.IsNullOrEmpty(poDetail.Content))
                        continue;

                    string key = poDetail.PODetailDef;
                    if (key.Length > 0)
                    {
                        if (char.IsDigit(key[0]))
                        {
                            key = "_" + key;
                        }

                        var tmpPropertyInfo =  oPoAttributes.GetType().GetProperty(key);
                        if (tmpPropertyInfo is null)
                        {
                            oPoAttributes.OtherProperty.Add(key, poDetail.Content);
                        }
                        else
                        {
                            tmpPropertyInfo.SetValue(oPoAttributes, poDetail.Content);
                        }
                    }
                }

                List_Result.poAttributes = oPoAttributes;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
            }
            return List_Result;
        }
        /// <summary>
        /// 通过指定属性名获取站点的属性值
        /// </summary>
        /// <param name="stations">站点属性列表</param>
        /// <param name="stationTypes">工序属性列表</param>
        /// <param name="keys">属性名</param>
        /// <returns></returns>
        public string ConvertProperties(List<TabVal> stations, List<TabVal> stationTypes, string keys)
        {
            string result = string.Empty;

            var stationType = stationTypes?.SingleOrDefault<TabVal>(x => x.ValStr2?.Trim() == keys.Trim());
            var station = stations?.SingleOrDefault<TabVal>(x => x.ValStr2?.Trim() == keys.Trim());

            string sStationTpye = stationType?.ValStr1?.Trim() ?? "";
            string sStation = station?.ValStr1?.Trim() ?? "";

            switch (sStationTpye)
            {
                case "":
                    result = sStation;
                    break;
                case "0":
                    result = sStationTpye;
                    break; ;
                case "1":
                    //查看站点信息是否存在
                    result = string.IsNullOrEmpty(sStation) ? sStationTpye : sStation;
                    break;
                default:
                    result = sStationTpye;
                    break;
            }
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public string ConvertProperties(List<StationAttribute> stationDetilList,
            List<StationAttribute> stationTypeDetilList, ref StationAttributes stationAttributes)
        {
            var stationTypeAtts = stationTypeDetilList.Where(x => !string.IsNullOrEmpty(x.AttributeName.Trim())).Select(x => x.AttributeName.Trim());
            var stationAtts = stationDetilList.Where(x => !string.IsNullOrEmpty(x.AttributeName.Trim())).Select(x => x.AttributeName.Trim());

            var attNames = stationAtts.Union(stationTypeAtts);

            StationAttribute mStationType;
            StationAttribute mStation;
            foreach (string name in attNames)
            {
                var property = stationAttributes.GetType().GetProperty(name);

                mStationType = stationTypeDetilList.FirstOrDefault(x => x.AttributeName == name);
                mStation = stationDetilList.FirstOrDefault(x => x.AttributeName == name);

                string tmpVal = string.Empty;
                if (mStationType is null)
                {
                    tmpVal = mStation.AttributeValue.ToString();
                }
                else
                {
                    switch (mStationType?.AttributeValue)
                    {
                        case "0":
                            tmpVal = mStationType.AttributeValue;
                            break; ;
                        case "1":
                            //查看站点信息是否存在
                            tmpVal = string.IsNullOrEmpty(mStation?.AttributeValue) ? mStationType.AttributeValue : mStation.AttributeValue;
                            break;
                        default:
                            //设置站点的优先级高于站点类型
                            tmpVal = string.IsNullOrEmpty(mStation?.AttributeValue) ? mStationType.AttributeValue : mStation.AttributeValue;
                            break;
                    }
                }



                if (property is null)
                {
                    stationAttributes.OtherProperty.Add(name,tmpVal);
                }
                else
                {
                    property.SetValue(stationAttributes, tmpVal);
                }
            }

            return "";
        }

        public string ConvertProperties2(List<StationAttribute> stationDetilList,
            List<StationAttribute> stationTypeDetilList, ref StationAttributes stationAttributes)
        {
            var stationTypeAtts = stationTypeDetilList.Where(x => !string.IsNullOrEmpty(x.AttributeName.Trim())).Select(x => x.AttributeName.Trim());
            var stationAtts = stationDetilList.Where(x => !string.IsNullOrEmpty(x.AttributeName.Trim())).Select(x => x.AttributeName.Trim());

            var attNames = stationAtts.Union(stationTypeAtts);

            StationAttribute mStationType;
            StationAttribute mStation;
            foreach (string name in attNames)
            {
                var property = stationAttributes.GetType().GetProperty(name);
                if (property is null)
                {
                    return $"please setup the station attribute of '{name}'";

                }

                mStationType = stationTypeDetilList.FirstOrDefault(x => x.AttributeName == name);
                mStation = stationDetilList.FirstOrDefault(x => x.AttributeName == name);



                if (mStationType?.AttributeValue == "")
                {
                    stationAttributes.GetType().GetProperty(name)?.SetValue(stationAttributes, mStation.AttributeName);
                    continue;
                }

                string result = string.Empty;
                switch (mStationType?.AttributeValue)
                {
                    case "0":
                        result = mStationType.AttributeValue;
                        break; ;
                    case "1":
                        //查看站点信息是否存在
                        result = string.IsNullOrEmpty(mStation?.AttributeValue) ? mStationType.AttributeValue : mStation.AttributeValue;
                        break;
                    default:
                        //设置站点的优先级高于站点类型
                        result = string.IsNullOrEmpty(mStation?.AttributeValue) ? mStationType.AttributeValue : mStation.AttributeValue;
                        break;
                }

                stationAttributes.GetType().GetProperty(name)?.SetValue(stationAttributes, result);
            }

            return "";
        }
        /// <summary>
        /// 通过属性名在指定列表中获取值
        /// </summary>
        /// <param name="allStationAttributes"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public async Task<string> ConvertAttributes(List<StationAttribute> allStationAttributes, string attributeName)
        {
            var v = allStationAttributes.SingleOrDefault(x => x.AttributeName == attributeName.Trim());
            return v?.AttributeValue ?? "";
        }

        /// <summary>
        /// 当attributeName为空时，通过站点stationID获取当前站点所有的详细属性值，
        /// 当attributeName不为空时，获取指定属性值
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public async Task<List<StationAttribute>> GetStationAttributes(string stationID, string attributeName = "")
        {

            string sql = @"
                        declare @StationID int=" + stationID + @"
                        declare @StationAttributeName nvarchar(500)='" + attributeName + @"'
                        select s.ID as StationID,case when scs.Name is not null then scs.Name else @StationAttributeName end StationDetailDef,scs.Value as StationDetailDefValue,s.StationTypeID
                        into #station
                        from  mesStation s 
                        left join mesStationConfigSetting scs on s.id=scs.StationID and (@StationAttributeName='' or scs.Name=@StationAttributeName)
                        where 
                        s.ID=@StationID

                        select 
                        std.StationTypeDetailDefID,case when stdd.Description is not null then stdd.Description else @StationAttributeName end StationTypeDetailDef,std.Content as StationTypeDetailValue,st.ID as StationTypeID
                        into #StationTypeDef
                        from mesStationType st
                        left join mesStationTypeDetail std on st.ID=std.StationTypeID and std.StationTypeDetailDefID in(select id from luStationTypeDetailDef where (@StationAttributeName='' or Description=@StationAttributeName))
                        left join luStationTypeDetailDef stdd on stdd.ID=std.StationTypeDetailDefID  
                        where st.ID=(select distinct StationTypeID from #station)
                        --select  *from #StationTypeDef

                        select 
                        s.StationID,s.StationDetailDef,s.StationDetailDefValue,std.*
                        into #final
                        from #station s
                        left join #StationTypeDef std on std.StationTypeDetailDef=s.StationDetailDef and s.StationTypeID=std.StationTypeID
                        union
                        select 
                        s.StationID,s.StationDetailDef,s.StationDetailDefValue,std1.*
                        from #station s
                        right join  #StationTypeDef std1 on std1.StationTypeDetailDef=s.StationDetailDef  and s.StationTypeID=std1.StationTypeID

                        select 
                        --*,
                        case when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and s.StationTypeDetailValue='0' then s.StationTypeDetailDef
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and s.StationTypeDetailValue='1' then s.StationDetailDef
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and (s.StationTypeDetailValue<>'1' or s.StationTypeDetailValue<>'0') then s.StationDetailDef
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is  null then s.StationDetailDef
                        when s.StationDetailDefValue is  null and s.StationTypeDetailValue is not null then s.StationTypeDetailDef
                        else @StationAttributeName
                        end 'AttributeName',
                        case when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and s.StationTypeDetailValue='0' then s.StationTypeDetailValue
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and s.StationTypeDetailValue='1' then s.StationDetailDefValue
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is not null and (s.StationTypeDetailValue<>'1' or s.StationTypeDetailValue<>'0') then s.StationDetailDefValue
                        when s.StationDetailDefValue is not null and s.StationTypeDetailValue is  null then s.StationDetailDefValue
                        when s.StationDetailDefValue is  null and s.StationTypeDetailValue is not null then s.StationTypeDetailValue
                        else null
                        end 'AttributeValue'
                        from #final s

                        drop table #station
                        drop table #StationTypeDef
                        drop table #final
                        ";

            var tmpList = await DapperConn.QueryAsync<StationAttribute>(sql, null, null, I_DBTimeout, null);
            return tmpList.AsList();
        }
        public async Task<List<dynamic>> GetDatatablesAsync(string sql, DynamicParameters par = null, DbTransaction tran = null, CommandType commandType = CommandType.Text)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            try
            {
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }
                var Querye = await DapperConn.QueryAsync(sql, par, tran, I_DBTimeout, commandType);
                List_ALL = Querye.AsList();
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, P_MSG_Public.MSG_Public_001, null, "");
            }
            return List_ALL;
        }
        public async Task<List<dynamic>> GetDatatablesMultipleAsync(string sql, DynamicParameters par = null, DbTransaction tran = null, CommandType commandType = CommandType.Text)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            try
            {
                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, par, tran, I_DBTimeout, commandType);

                while (!Query_Multiple.IsConsumed)
                {
                    List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                    List_ALL.Add(v_List);
                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, P_MSG_Public.MSG_Public_001, null, "");
            }
            return List_ALL;
        }
        public Tuple<string, List<dynamic>> uspPONumberCheck(string MainSn)
        {
            string strOutput = string.Empty;

            DynamicParameters dpc = new DynamicParameters();
            dpc.Add("@strSNFormat", MainSn, DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlProdOrder", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlPart", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlStation", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlExtraData", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strSNbuf", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 100);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = DapperConnRead2.Query<dynamic>("uspPONumberCheck", dpc, null, true, I_DBTimeout, CommandType.StoredProcedure);
            strOutput = dpc.Get<string>("@strOutput");
            return new Tuple<string, List<dynamic>>(strOutput, v_Query.AsList());
        }
        public async Task<Tuple<string, List<dynamic>>> uspPONumberCheckAsync(string MainSn)
        {
            string strOutput = string.Empty;

            DynamicParameters dpc = new DynamicParameters();
            dpc.Add("@strSNFormat", MainSn, DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlProdOrder", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlPart", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlStation", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlExtraData", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strSNbuf", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 100);
            await GetDatatablesAsync("uspPONumberCheck", dpc, null, CommandType.StoredProcedure);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>("uspPONumberCheck", dpc, null, I_DBTimeout, CommandType.StoredProcedure);
            strOutput = dpc.Get<string>("@strOutput");
            return new Tuple<string, List<dynamic>>(strOutput, v_Query.AsList());
        }
        public Tuple<string, List<dynamic>> uspGetBaseData(string MainSn)
        {
            string strOutput = string.Empty;

            DynamicParameters dpc = new DynamicParameters();
            dpc.Add("@strSNFormat", MainSn, DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlProdOrder", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlPart", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlStation", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlExtraData", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strSNbuf", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 100);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = DapperConnRead2.Query("uspGetBaseData", dpc, null, true, I_DBTimeout, CommandType.StoredProcedure);
            strOutput = dpc.Get<string>("@strOutput");
            return new Tuple<string, List<dynamic>>(strOutput, v_Query.AsList());
        }
        public async Task<Tuple<string, List<dynamic>>> uspGetBaseDataAsync(string MainSn)
        {
            string strOutput = string.Empty;

            DynamicParameters dpc = new DynamicParameters();
            dpc.Add("@strSNFormat", MainSn, DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlProdOrder", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlPart", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlStation", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@xmlExtraData", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strSNbuf", "", DbType.String, ParameterDirection.Input, 100);
            dpc.Add("@strOutput", strOutput, DbType.String, ParameterDirection.Output, 100);

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync("uspGetBaseData", dpc, null, I_DBTimeout, CommandType.StoredProcedure);
            strOutput = dpc.Get<string>("@strOutput");
            return new Tuple<string, List<dynamic>>(strOutput, v_Query.AsList());
        }

        /// <summary>
        /// 获取PO详细信息
        /// </summary>
        /// <param name="ProductionOrderID"></param>
        /// <param name="PODetailDef"></param>
        /// <returns></returns>
        public async Task<dynamic> GetluPODetailDef(int ProductionOrderID, string PODetailDef)
        {
            string strSql = @"select A.ID,
                                       A.ProductionOrderID,B.ProductionOrderNumber, 
                                    A.ProductionOrderDetailDefID,C.Description PODetailDef,
                                    A.Content 
                                from mesProductionOrderDetail A 
                                 join mesProductionOrder B on A.ProductionOrderID=B.ID
                                 join luProductionOrderDetailDef C on C.ID=A.ProductionOrderDetailDefID 
                              Where A.ProductionOrderID=" + ProductionOrderID +
                            "  and  C.Description='" + PODetailDef + "'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<dynamic>(strSql, null, null, I_DBTimeout, null);
            return v_Query;
        }

        /// <summary>
        ///获取PO详细信息
        /// </summary>
        /// <param name="ProductionOrderID"></param>
        /// <param name="PODetailDef"></param>
        /// <returns></returns>
        public async Task<List<PoDetailDefs>> GetluPODetailDefs(int ProductionOrderID, string PODetailDef = "")
        {
            string strSql = @"select A.ID,
                                       A.ProductionOrderID,B.ProductionOrderNumber, 
                                    A.ProductionOrderDetailDefID,C.Description PODetailDef,
                                    A.Content 
                                from mesProductionOrderDetail A 
                                 join mesProductionOrder B on A.ProductionOrderID=B.ID
                                 join luProductionOrderDetailDef C on C.ID=A.ProductionOrderDetailDefID 
                              Where A.ProductionOrderID=" + ProductionOrderID + " ";

            if (!string.IsNullOrEmpty(PODetailDef))
            {
                strSql += $" and  C.Description='{PODetailDef}'";
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync<PoDetailDefs>(strSql, null, null, I_DBTimeout, null);
            return v_Query?.ToList();
        }

        /// <summary>
        /// 校验主条码
        /// </summary>
        /// <param name="PartID"></param>
        /// <param name="LineID"></param>
        /// <param name="StationID"></param>
        /// <param name="StationTypeID"></param>
        /// <param name="SN"></param>
        /// <returns></returns>
        public async Task<string> MESAssembleCheckMianSNAsync(string ProductionOrderID, int LineID, int StationID, int StationTypeID, string SN, bool COF)
        {
            var DT_SN = await GetmesSerialNumberAsync("", SN);
            if (DT_SN?.Count <= 0)
                return "20012";

            string S_UnitID = DT_SN[0].UnitID.ToString();

            var DT_Unit = await GetmesUnitAsync(S_UnitID);
            if (DT_Unit?.Count <= 0)
                return "20012";

            if (!COF)
            {
                if (DT_Unit[0].StatusID.ToString() != "1")
                {
                    return "20036";
                }
            }

            string UnitProductionOrderID = DT_Unit[0].ProductionOrderID.ToString();

            if (UnitProductionOrderID != ProductionOrderID)
            {
                return "20133";
            }

            var mesPart = await GetmesPartAsync(DT_Unit[0].PartID.ToString());
            mesUnit mp = DT_Unit[0] ?? new mesUnit();
            mp.PartFamilyID = mesPart.PartFamilyID;
            string S_RouteCheck = await GetRouteCheckAsync(StationTypeID, StationID, LineID.ToString(), DT_Unit[0], SN);
            return S_RouteCheck;
        }

        /// <summary>
        /// 校验其他类型条码(存在系统中条码)
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public async Task<string> MESAssembleCheckOtherSNAsync(string SN, string PartID, bool COF, LoginList List_Login)
        {
            var DT_SN = await GetmesSerialNumberAsync("", SN);
            string S_UnitID = string.Empty;

            if (DT_SN?.Count <= 0)
                return "20012";

            S_UnitID = DT_SN[0].UnitID.ToString();
            var DT_Unit = await GetmesUnitAsync(S_UnitID);
            if (!COF)
            {
                if (DT_Unit[0].StatusID.ToString() != "1")
                {
                    return "20036";
                }
            }

            string UnitPart = DT_Unit[0].PartID.ToString();
            if (UnitPart != PartID)
            {
                //替代料逻辑预留
                return "20037";
            }

            string strOutput = await uspRouteLastSationCheckAsync(SN, "", "", "", "", "");
            //List<TabVal> tmpVals = await uspCallProcedureAsync("uspRouteLastSationCheck", SN, "", "", "", "", "");
            //strOutput = tmpVals[0].ValStr1;

            if (strOutput != "1")
            {
                return strOutput;
            }

            string sql = string.Format(@"select 1 from mesUnitComponent A INNER JOIN mesSerialNumber B
                            ON A.UnitID = B.UnitID AND A.ChildSerialNumber ='{0}' and A.StatusID=1", SN);
            object dt = DapperConnRead2.ExecuteScalar(sql);
            if (dt != null && dt.ToString() == "1")
                return "20039";

            return "1";
        }
        public List<mesMachine> GetmesMachine(string S_SN)
        {
            string S_Sql = "select * from mesMachine where SN='" + S_SN + "' ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.Query<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<mesMachine>> GetmesMachineAsync(string S_SN)
        {
            string S_Sql = "select * from mesMachine where SN='" + S_SN + "' ";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public string BuckToFGSN(string S_BuckSN)
        {
            string S_Sql = String.Format(@"SELECT TOP 1 Value AS FG_SN FROM mesSerialNumber A
                                            INNER JOIN mesUnitDetail B ON A.UnitID = B.UnitID
                                            WHERE B.reserved_03 = '1' AND B.reserved_01 = '{0}'
                                            ORDER BY B.ID DESC", S_BuckSN);
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.ExecuteScalar(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToString() ?? "";
        }
        public async Task<string> BuckToFGSNAsync(string S_BuckSN)
        {
            string S_Sql = String.Format(@"SELECT TOP 1 Value AS FG_SN FROM mesSerialNumber A
                                            INNER JOIN mesUnitDetail B ON A.UnitID = B.UnitID
                                            WHERE B.reserved_03 = '1' AND B.reserved_01 = '{0}'
                                            ORDER BY B.ID DESC", S_BuckSN);
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToString() ?? "";
        }
        public bool MESCheckChildSerialNumber(string ChildSerialNumber)
        {
            string S_Sql = string.Format("SELECT 1 FROM mesUnitComponent WHERE ChildSerialNumber='{0}' and StatusID=1", ChildSerialNumber);
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.ExecuteScalar(S_Sql, null, null, I_DBTimeout, null);
            return (v_Query?.ToString() ?? "") == "1";
        }
        public async Task<bool> MESCheckChildSerialNumberAsync(string ChildSerialNumber)
        {
            string S_Sql = string.Format("SELECT 1 FROM mesUnitComponent WHERE ChildSerialNumber='{0}' and StatusID=1", ChildSerialNumber);
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            return (v_Query?.ToString() ?? "") == "1";
        }
        public List<mesUnit> GetSerialNumber2(string S_SN)
        {
            string S_Sql = "select B.* from mesSerialNumber A join mesUnit B on A.UnitID=B.ID where A.Value='" + S_SN + "'";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.Query<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<List<mesUnit>> GetSerialNumber2Async(string S_SN)
        {
            string S_Sql = "select B.* from mesSerialNumber A join mesUnit B on A.UnitID=B.ID where A.Value='" + S_SN + "'";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync<mesUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }
        public async Task<string> uspAssembleCheckAsync(string S_SN, string S_ProductionOrderID, string S_CurrentPartID, LoginList List_Login, string mainPartID)
        {
            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_ProductionOrderID + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + mainPartID + "\"> </Part>";
            string xmlStation = "<Station StationID=\"" + List_Login.StationID + "\"> </Station>";
            string tmpList = await uspCallProcedureAsync("uspAssembleCheck", S_SN, xmlProdOrder, xmlPart, xmlStation, "", S_CurrentPartID);
            return tmpList;
        }

        public async Task<string> uspAssembleCheck2Async(string S_SN, string MainSN, string S_ProductionOrderID, string S_CurrentPartID, LoginList List_Login, string mainPartID)
        {
            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_ProductionOrderID + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + mainPartID + "\"> </Part>";
            string xmlStation = "<Station StationID=\"" + List_Login.StationID + "\"> </Station>";
            string xmlExtraDataAss = "<ExtraData MainCode=\"" + MainSN + "\"> </ExtraData>";
            string tmpList = await uspCallProcedureAsync("uspAssembleCheck", S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraDataAss, S_CurrentPartID);
            return tmpList;
        }

        public async Task<string> uspQCCheckAsync(string S_SN, string S_ProductionOrderID, string S_MainPartID, LoginList List_Login)
        {
            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_ProductionOrderID + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + S_MainPartID + "\"> </Part>";
            string xmlStation = "<Station StationID=\"" + List_Login.StationID + "\"> </Station>";
            string tmpList = await uspCallProcedureAsync("uspQCCheck", S_SN, xmlProdOrder, xmlPart, xmlStation, "", "");
            return tmpList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="S_MainSn"></param>
        /// <param name="ChildSns"></param>
        /// <param name="S_ProductionOrderID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<bool> uspCheckVendorAsync(string S_MainSn, string ChildSns, string S_ProductionOrderID, string S_PartID,
            LoginList List_Login)
        {
            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + S_ProductionOrderID + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + S_PartID + "\"> </Part>";
            string xmlStation = "<Station StationID='" + List_Login.StationID + "'> </Station>";
            string xmlExtra = "<ExtraData EmployeeID=\"" + List_Login.EmployeeID + "\" ChildSns=\"" + ChildSns + "\" > </ExtraData>";
            string lt = await uspCallProcedureAsync("Usp_CheckMaterialVendor", S_MainSn, xmlProdOrder, xmlPart, xmlStation, xmlExtra, null);
            return lt == "1";
        }
        /// <summary>
        /// 获取下一个状态
        /// </summary>
        /// <param name="S_PartID"></param>
        /// <param name="PartFamilyID"></param>
        /// <param name="LineID"></param>
        /// <param name="StationTypeID"></param>
        /// <param name="ProductionOrderID"></param>
        /// <param name="StatusID"></param>
        /// <param name="S_RouteSequence"></param>
        /// <returns></returns>
        public async Task<string> GetMesUnitState(string S_PartID, string PartFamilyID, string LineID, int StationTypeID, string ProductionOrderID, string StatusID, string S_RouteSequence = "")
        {
            //int routeId = await GetRouteID(S_PartID, PartFamilyID, LineID, ProductionOrderID);
            var RouteID = await ufnRTEGetRouteIDAsync(LineID.ToInt(), S_PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt());
            int routeId = RouteID.ToInt();

            if (routeId == 0)
                return "";

            //string S_Sql = string.Format("SELECT dbo.ufnGetUnitStateID({0},{1},{2}) AS UnitStateID",
            //    routeId, StationTypeID, StatusID);
            //if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            //{

            //}
            //var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            //return (v_Query ?? "").ToString();

            return await ufnGetUnitStateIDAsync(routeId, StationTypeID, StatusID.ToInt());

        }

        public async Task<string> ufnGetUnitStateIDAsync(int routeId, int stationTypeId, int StatusId)
        {
            string sql = $@"DECLARE @UnitStateID varchar(64),@RouteID INT = {routeId},@StationTypeID INT = {stationTypeId},@StatusID INT ={StatusId}
	                    IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@RouteID AND RouteType=1)
	                    BEGIN
		                    SELECT TOP 1 @UnitStateID = OutputStateID FROM mesUnitOutputState 
		                    WHERE RouteID =@RouteID AND StationTypeID=@StationTypeID and OutputStateDefID=@StatusID
	                    END
	                    ELSE
	                    BEGIN
		                    SELECT @UnitStateID=UnitStateID FROM mesRouteDetail  
		                    WHERE RouteID=@RouteID AND StationTypeID=@StationTypeID
	                    END
	                    SELECT @UnitStateID UnitStateID";
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return (v_Query ?? "").ToString();

        }
        public async Task<string> ufnRTEGetSNFormatIDAsync(int lineId, int partId, int partFaimilyId, int productionOrderId, int stationTypeId)
        {
            string sql = $@"declare @SNFormatID varchar(500),	@LineID int = {lineId},
	                        @PartID int = {partId},
	                        @PartFamilyID int = {partFaimilyId} ,
	                        @ProductionOrderID int = {productionOrderId},
	                        @StationTypeID int = {stationTypeId}

	                        set @PartID= case when @PartID=0 then null else @PartID end 
	                        set @LineID= case when @LineID=0 then null else @LineID end 
	                        set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 
	                        set @ProductionOrderID= case when @ProductionOrderID=0 then null else @ProductionOrderID end
	                        set @StationTypeID= case when @StationTypeID=0 then null else @StationTypeID end

	                        if @PartID is not NULL and @PartFamilyID is not NULL begin
		                        if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID) 
                                    select @SNFormatID SNFormatID
	                        end else if @PartFamilyID is NULL begin
		                        select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	                        end

	                        select top 1 @SNFormatID = B.Name
	                        from mesSNFormatMap A
	                        INNER JOIN mesSNFormat B ON A.SNFormatID=B.ID
	                        where
		                        (LineID = @LineID or LineID is NULL OR LineID=0) AND
		                        (PartID = @PartID or PartID is NULL OR PartID=0) AND
		                        (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		                        (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0) AND
		                        (StationTypeID = @StationTypeID or StationTypeID is NULL OR StationTypeID=0)
	                        order by StationTypeID desc,ProductionOrderID desc, PartID desc, LineID desc, PartFamilyID desc

	                        select @SNFormatID SNFormatID --could be NULL if no route is found";
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return (v_Query ?? "").ToString();

        }
        public async Task<string> ufnRTEGetRouteIDAsync(int lineId, int partId, int partFaimilyId, int productionOrderId)
        {
            string sql = $@"declare @nRoutID int
	                        declare @nPartID int
	                        declare @nLineID int
	                        declare @nPartFamilyID INT,	
			                        @LineID int = {lineId},
			                        @PartID int = {partId},
			                        @PartFamilyID int = {partFaimilyId},
			                        @ProductionOrderID int = {productionOrderId}

	                        set @PartID= case when @PartID=0 then null else @PartID end 
	                        set @LineID= case when @LineID=0 then null else @LineID end 
	                        set @PartFamilyID= case when @PartFamilyID=0 then null else @PartFamilyID end 

	                        if @PartID is not NULL and @PartFamilyID is not NULL begin
		                        if not exists (select 1 from mesPart where [ID] = @PartID and PartFamilyID = @PartFamilyID) 
                                    select @nRoutID RoutID
	                        end else if @PartFamilyID is NULL begin
		                        select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
	                        end

	                        select top 1 @nRoutID = RouteID, @nPartID = PartID, @nLineID = LineID, @nPartFamilyID = PartFamilyID
	                        from mesRouteMap
	                        where
		                        (LineID = @LineID or LineID is NULL OR LineID=0) AND
		                        (PartID = @PartID or PartID is NULL OR PartID=0) AND
		                        (PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
		                        (ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
	                        order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
	                        -- priority of select is PartID, LineID, ProdStruct, and PartFamilyID (non-NULL is listed ahead of NULL)

	                        select @nRoutID RoutID  --could be NULL if no route is found";
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return (v_Query ?? "").ToString();

        }
        public async Task<int> GetRouteID(string S_PartID, string PartFamilyID, string LineID, string ProductionOrderID)
        {
            LineID = string.IsNullOrEmpty(LineID) ? "''" : LineID;
            S_PartID = string.IsNullOrEmpty(S_PartID) ? "''" : S_PartID;
            PartFamilyID = string.IsNullOrEmpty(PartFamilyID) ? "''" : PartFamilyID;
            ProductionOrderID = string.IsNullOrEmpty(ProductionOrderID) ? "''" : ProductionOrderID;
            string S_Sql = string.Format("select dbo.ufnRTEGetRouteID({0},{1},{2},{3}) as RouteName",
                LineID, S_PartID, PartFamilyID, ProductionOrderID);
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            return Convert.ToInt32(v_Query ?? 0);
        }


        public string mesGetSNFormatIDByList(string PartID, string PartFamilyID, string LineID, string ProductionOrderID, string StationTypeID)
        {
            string SNFormatID = string.Empty;
            PartID = string.IsNullOrEmpty(PartID) ? "null" : PartID;
            PartFamilyID = string.IsNullOrEmpty(PartFamilyID) ? "null" : PartFamilyID;
            LineID = string.IsNullOrEmpty(LineID) ? "null" : LineID;
            ProductionOrderID = string.IsNullOrEmpty(ProductionOrderID) ? "null" : ProductionOrderID;
            StationTypeID = string.IsNullOrEmpty(StationTypeID) ? "null" : StationTypeID;
            //string S_Sql = string.Format(@"select dbo.ufnRTEGetSNFormatID({0},{1},{2},{3},{4}) as SNFormatID", LineID, PartID, PartFamilyID, ProductionOrderID, StationTypeID);

            //if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            //{

            //}
            //var v_Query = DapperConnRead2.ExecuteScalar(S_Sql, null, null, I_DBTimeout, null);
            //return Convert.ToString(v_Query ?? "");
            return ufnRTEGetSNFormatIDAsync(LineID.ToInt(), PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt(), StationTypeID.ToInt()).Result;
        }
        public async Task<string> mesGetSNFormatIDByListAsync(string PartID, string PartFamilyID, string LineID, string ProductionOrderID, string StationTypeID)
        {
            string SNFormatID = string.Empty;
            PartID = string.IsNullOrEmpty(PartID) ? "null" : PartID;
            PartFamilyID = string.IsNullOrEmpty(PartFamilyID) ? "null" : PartFamilyID;
            LineID = string.IsNullOrEmpty(LineID) ? "null" : LineID;
            ProductionOrderID = string.IsNullOrEmpty(ProductionOrderID) ? "null" : ProductionOrderID;
            //StationTypeID = string.IsNullOrEmpty(StationTypeID) ? "null" : StationTypeID;
            //string S_Sql = string.Format(@"select dbo.ufnRTEGetSNFormatID({0},{1},{2},{3},{4}) as SNFormatID", LineID, PartID, PartFamilyID, ProductionOrderID, StationTypeID);

            //if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            //{

            //}
            //var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            //return Convert.ToString(v_Query ?? "");

            return await ufnRTEGetSNFormatIDAsync(LineID.ToInt(), PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt(), StationTypeID.ToInt());

        }

        public string GetFirstStationType(string S_MachineSN)
        {
            string S_Sql = $"SELECT * FROM mesMachine WHERE SN='{S_MachineSN}'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = DapperConnRead2.QueryFirst<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            //DataTable DT = SqlServerHelper.Data_Table(S_Sql);
            string S_Result = "";

            if (v_Query != null)
            {
                string S_ValidDistribution = v_Query.ValidDistribution.ToString();

                string[] List_ValidDistribution = S_ValidDistribution.Split(';');
                if (List_ValidDistribution.Length > 0)
                {
                    string[] List_StationType = List_ValidDistribution[0].Split(',');
                    S_Result = List_StationType[0];
                }
            }
            return S_Result;
        }
        public async Task<string> GetFirstStationTypeAsync(string S_MachineSN)
        {
            string S_Sql = $"SELECT * FROM mesMachine WHERE SN='{S_MachineSN}'";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConnRead2.QueryFirstAsync<mesMachine>(S_Sql, null, null, I_DBTimeout, null);
            //DataTable DT = SqlServerHelper.Data_Table(S_Sql);
            string S_Result = "";

            if (v_Query != null)
            {
                string S_ValidDistribution = v_Query.ValidDistribution.ToString();

                string[] List_ValidDistribution = S_ValidDistribution.Split(';');
                if (List_ValidDistribution.Length > 0)
                {
                    string[] List_StationType = List_ValidDistribution[0].Split(',');
                    S_Result = List_StationType[0];
                }
            }
            return S_Result;
        }
        public async Task<(string, MBoxToBatch)> BoxSnToBatch(string S_BoxSN)
        {

            string S_Sql = string.Format(@"select StatusID from mesMachine WHERE  SN='" + S_BoxSN + "'");

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);

            if (v_Query != null)
            {
                string S_MachineStatusID = v_Query.ToString();
                if (S_MachineStatusID == "2")
                {
                    S_Sql = @"select B.partID,UnitID,reserved_02 HB_Batch from mesUnitDetail A inner join mesUnit B ON A.UnitID=B.ID
	                            where A.ID=(select max(ID)  from mesUnitDetail where reserved_01='" + S_BoxSN + @"' and reserved_03=1)";
                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    var v_Query2 = await DapperConnRead2.QueryFirstOrDefaultAsync<MBoxToBatch>(S_Sql, null, null, I_DBTimeout, null);
                    if (v_Query2 is null)
                    {
                        return ("", null);
                    }

                    string sResult = v_Query2.HB_Batch;
                    return (sResult, v_Query2);
                }
            }

            return ("", null);
        }
        public async Task<List<dynamic>> GetPO(string S_PartID, string S_StatusID)
        {
            string strSql = @"SELECT A.ID, A.ProductionOrderNumber,C.PartNumber, A.Description, A.OrderQuantity,
		                        B.Lastname+B.Firstname AS Employee, A.CreationTime, A.LastUpdate,D.Description AS Status 
		                        FROM 
		                        (SELECT  * FROM mesProductionOrder where StatusID=1)AS A  
		                        JOIN (SELECT * from mesEmployee ) B ON A.EmployeeID=B.ID
		                        JOIN (SELECT * from mesPart)AS C ON A.PartID=C.ID 
		                        JOIN (select * from sysStatus ) AS D ON A.StatusID=D.ID
                            WHERE A.PartID ='" + S_PartID + "'";

            if (S_StatusID != "")
            {
                strSql += " AND A.StatusID='" + S_StatusID + "'";
            }
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var v_Query = await DapperConn.QueryAsync(strSql, null, null, I_DBTimeout, null);
            return v_Query?.ToList();
        }

        public async Task<List<dynamic>> GetComponent(int I_ChildUnitID)
        {
            string S_Sql = @"select A.*,B.PartFamilyID,C.Value  from 
                                    mesUnit A join mesPart B on A.PartID=B.ID
                                    join mesSerialNumber C on A.ID=C.UnitID
                            where A.ID= " + I_ChildUnitID;
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConn.QueryAsync(S_Sql, null, null, I_DBTimeout, null);
            return v_Query?.ToList();
        }


        public async Task<string> GetSNRGetNextAsync(string S_SNFormat, string S_ReuseSNByStation,
            string S_ProdOrder, string S_Part, string S_Station, string S_ExtraData) =>
            await new SNFormatRepository().GetSNRGetNext(S_SNFormat, S_ReuseSNByStation, S_ProdOrder, S_Part,
                S_Station, S_ExtraData);

        public async Task<mesMaterialUnit> GetMaterialUnitBySnAsync(string S_SN)
        {
            string S_Sql = $@"SELECT  PartID
                                FROM dbo.mesMaterialUnit
                                WHERE SerialNumber= '{S_SN}'";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryFirstOrDefaultAsync<mesMaterialUnit>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query;
        }
        public async Task<int> GetPartIdByMachineSNAsync(string S_SN)
        {
            string S_Sql = $@"SELECT TOP 1 b.PartID
                            FROM dbo.mesMachine a
                            LEFT JOIN dbo.mesRouteMachineMap b ON  a.ID = b.MachineID or 
								                            a.PartID =b.MachinePartID or 
								                            a.MachineFamilyID =b.MachineFamilyID
                            WHERE a.SN ='{S_SN}'
                            ORDER BY b.MachineID DESC, b.MachineFamilyID DESC,b.MachinePartID DESC";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            return v_Query == null ? -1 : v_Query.ToInt();
        }

        public List<DNShift> GetrptDashboardGetDNShift() 
        {
            string S_Sql = "exec rptDashboardGetDNShift";
            List<DNShift> List_DNShift = new List<DNShift>();

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            if (!Query_Multiple.IsConsumed)
            {
                List_DNShift = Query_Multiple.Read<DNShift>().AsList();
            }

            return List_DNShift?.ToList();
        }

        public async void TestFn()
        {
            Console.WriteLine("Test");

            var nextState = await ufnGetUnitStateIDAsync(13, 12, 1);
            Console.WriteLine(nextState);
            var routeId = await ufnRTEGetRouteIDAsync(0, 0, 2, 0);
            await Console.Out.WriteLineAsync(routeId);


            var snFormatId = await ufnRTEGetSNFormatIDAsync(10, 76, 7, 0, 0);
            await Console.Out.WriteLineAsync(snFormatId);
        }
        public async Task<(string unitStateId, string errorCode)> GetmesUnitStateSecondAsync(string S_PartID, string PartFamilyID, string S_RouteSequence, string LineID, int StationTypeID, string ProductionOrderID, string StatusID, string S_SN)
        {
            var RouteID = await ufnRTEGetRouteIDAsync(LineID.ToInt(), S_PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt());
            int routeId = RouteID.ToInt();

            if (routeId <= 0)
                return ("", msgSys.MSG_Sys_70019);

            var oldUnitStateId = await ufnGetUnitStateIDAsync(routeId, StationTypeID, StatusID.ToInt());

            var mesunits = await GetmesUnitSerialNumber(S_SN);
            if (mesunits == null || mesunits.Count <= 0)
                return (oldUnitStateId, "");

            var unit = mesunits[0];

            var looperCountCheck = await GetmesUnitStateLooperAsync(routeId, StationTypeID, unit.UnitStateID.ToString(), S_SN);
            if (looperCountCheck != "1")
                return ("", looperCountCheck);

            string sql = $"SELECT * FROM dbo.mesUnitOutputState WHERE StationTypeID = {StationTypeID} AND RouteID = {routeId} AND InputStateID = {unit.UnitStateID}";
            var outputTable = await SqlSugarHelper.Db.Ado.SqlQueryAsync<mesUnitOutputState>(sql);

            if (outputTable == null || outputTable.Count() <= 0)
                return (oldUnitStateId, "");

            //一旦进来的工序状态是有内部线条的，则必须按照内部线条的选择来匹配，否则报错
            var specialStates = outputTable.Where(x => x.OutputStateDefID == StatusID.ToInt());
            if (specialStates == null || specialStates.Count() <= 0)
                return ("", P_MSG_Public.MSG_Public_6056);

            return (specialStates.ToList()[0].OutputStateID.ToString(), "");
        }
        public async Task<string> GetmesUnitStateLooperAsync(int RouteId, int StationTypeID, string currentUnitState, string SN)
        {
            string sql = $@"
                        DECLARE @looperCountCC INT,@looperCount INT, @historyCount INT, @sn VARCHAR(200) = '{SN}',  @strOutput VARCHAR(500)='1' 
                        declare @routeId int = {RouteId},@stationTypeId int = {StationTypeID}, @currentStateId INT = {currentUnitState}
                        IF EXISTS (
                        SELECT * 
                        FROM dbo.mesUnitInputState WHERE RouteID = @routeId AND StationTypeID = @stationTypeId AND CurrStateID = @currentStateId AND ISNULL(LooperCount, '')  <> '')
                        BEGIN
	                        select  @looperCountCC =count(*)
	                        from (
                            SELECT RouteID,StationTypeID,CurrStateID,LooperCount
	                        FROM dbo.mesUnitInputState WHERE RouteID = @routeId AND StationTypeID = @stationTypeId AND CurrStateID = @currentStateId AND ISNULL(LooperCount, '')  <> ''
	                        GROUP BY RouteID,StationTypeID,CurrStateID,LooperCount) ab
	                        IF @looperCountCC > 1
	                        BEGIN
		                        SET @strOutput = '{P_MSG_Public.MSG_Public_6057}'
		                        SELECT @strOutput strOutput
		                        RETURN 
	                        END
	                        SELECT TOP 1 @looperCount = LooperCount
	                        FROM dbo.mesUnitInputState WHERE RouteID = @routeId AND StationTypeID = @stationTypeId AND CurrStateID = @currentStateId AND ISNULL(LooperCount, '')  <> ''

	                        SELECT @historyCount = COUNT(*)
	                        FROM dbo.mesSerialNumber a
	                        JOIN dbo.mesHistory b ON b.UnitID = a.UnitID AND b.UnitStateID = @currentStateId
	                        WHERE a.Value = @sn

	                        IF @historyCount >= @looperCount
	                        BEGIN
		                        SET @strOutput = '{P_MSG_Public.MSG_Public_6058}'
		                        SELECT @strOutput strOutput
		                        RETURN 
	                        END
                        END
                        SELECT @strOutput strOutput";

            var tt = await SqlSugarHelper.Db.Ado.SqlQueryAsync<SqlOutputStr>(sql);

            var tmpLooperCount = await DapperConn.QueryFirstAsync<SqlOutputStr>(sql);

            return tmpLooperCount?.strOutput;
        }
        #endregion


    }
}
