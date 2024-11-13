using Dapper;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using static Dapper.SqlMapper;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Commons.Extensions;

namespace SunnyMES.Security.Repositories
{
    public class ReportRepository : BaseRepositoryReport<string>, IReportRepository
    {
        //List<BOMPart> List_BOMALL = new List<BOMPart>();
        //IEnumerable<WIP> List_WIP = null;
        //int I_MaxLevel = 1;

        public ReportRepository()
        {
        }

        public ReportRepository(IDbContextCore dbContext) : base(dbContext)
        {
            //string S_Sql = "SELECT ID UnitStateID, [Description] ,0 QTY,'' PType  FROM mesUnitState ";

            //var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            //if (!Query_Multiple.IsConsumed)
            //{
            //    List_WIP = Query_Multiple.Read<WIP>().AsList();
            //}
        }


        public async Task<List<dynamic>> GetOutputSum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";

            try
            {
                string S_Sql = "exec rptDashboardGetOutputSum '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_Shift + "'," +
                                                           "'" + S_DataType + "'," +
                                                           "'" + S_DataLevel + "',"+
                                                           "'" + YieldLevel + "'," +
                                                           "'" + IsCombineYield + "'" 
                                                          ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);

                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL.ToList<dynamic>();
        }


        public async Task<IEnumerable<dynamic>> GetPartFamilyType()
        {
            string S_Sql = "exec rptDashboardGetPartFamilyType ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetPartFamily(string PartFamilyTypeID)
        {
            if (PartFamilyTypeID == null) { PartFamilyTypeID = ""; }
            string S_Sql = "exec rptDashboardGetPartFamily  '" + PartFamilyTypeID + "' ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<IEnumerable<dynamic>> GetPart(string PartFamilyID)
        {
            if (PartFamilyID == null) { PartFamilyID = ""; }
            string S_Sql = "exec rptDashboardGetPartNumber '" + PartFamilyID + "' ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<IEnumerable<dynamic>> GetProductionOrder(string PartID)
        {
            if (PartID == null) { PartID = ""; }
            string S_Sql = "exec rptDashboardGetProductionOrder '" + PartID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<IEnumerable<dynamic>> GetStationType()
        {
            string S_Sql = "exec rptDashboardGetStationType";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<IEnumerable<dynamic>> GetStation(string StationTypeID)
        {
            if (StationTypeID == null) { StationTypeID = ""; }
            string S_Sql = "exec rptDashboardGetStation '" + StationTypeID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetShift()
        {
            string S_Sql = "exec rptDashboardGetShift";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetLine()
        {
            string S_Sql = "exec rptDashboardGetLine";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<dynamic>> GetUPHYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptDashboardGetUPHYield '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_Shift + "'," +
                                                           "'" + S_DataType + "'," +
                                                           "'" + S_DataLevel + "',"+
                                                           "'" + YieldLevel + "'," +
                                                           "'" + IsCombineYield + "'"
                                                           ;


                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }


        public async Task<List<dynamic>> GetUPH(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel,string YieldLevel,string IsCombineYield)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptDashboardGetUPH '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_Shift + "'," +
                                                           "'" + S_DataType + "'," +
                                                           "'" + S_DataLevel + "'," +
                                                           "'" + YieldLevel + "'," +
                                                           "'" + IsCombineYield + "'"
                                                          ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }

        public async Task<List<dynamic>> GetUPHCum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptDashboardGetUPHCum '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_Shift + "'," +
                                                           "'" + S_DataType + "'," +
                                                           "'" + S_DataLevel + "'," +
                                                           "'" + YieldLevel + "'," +
                                                           "'" + IsCombineYield + "'"
                                                          ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }


        public async Task<List<dynamic>> GetDefectYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string S_TopQTY, string YieldLevel, string IsCombineYield)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptDashboardGetDefectYield '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_Shift + "'," +
                                                           "'" + S_DataType + "'," +
                                                           "'" + S_DataLevel + "'," +
                                                           "'" + S_TopQTY + "',"+
                                                           "'" + YieldLevel + "'," +
                                                           "'" + IsCombineYield + "'"
                                                          ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }



        public async Task<IEnumerable<dynamic>> GetDataLevel()
        {
            string S_Sql = "exec rptDashboardGetDataLevel";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<List<dynamic>> GetReportGeneral(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
          //  string Shift,string YieldLevel,string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";

            IEnumerable<TabVal> List_Temp = await DapperConnRead2.QueryAsync<TabVal>
                    ("select '' ValStr1 ", null, null, I_DBTimeout, null);

            if (S_Type != "")
            {
                try
                {
                    string v_Sql = "SELECT DataType ValStr1,CMDSql ValStr2 FROM rptCMDConfig where " +
                    " ReportType='ReporList' and DataType='" + S_Type + "'";
                    IEnumerable<TabVal> List_ExportConfig = await DapperConnRead2.QueryAsync<TabVal>(v_Sql, null, null, I_DBTimeout, null);

                    int I_Count = 0;
                    if (List_ExportConfig != null)
                    {
                        if (List_ExportConfig.Count() > 0)
                        {
                            I_Count = List_ExportConfig.Count();
                        }
                    }

                    if (I_Count == 0)
                    {
                        List_Temp.First().ValStr1 = "ERROR";
                        List_Temp.First().ValStr2 = S_Type + " data type does not exist";
                        List_Temp.First().ValStr3 = S_DataStatus;

                        List_ALL.Add(List_Temp);
                        return List_ALL;
                    }

                    string S_PName = List_ExportConfig.First().ValStr2;
                    string S_Sql = S_PName + "  '" + S_StartDateTime + "', " +
                                                "'" + S_EndDateTime + "'," +
                                                "'" + S_PartFamilyTypeID + "'," +
                                                "'" + S_PartFamilyID + "'," +
                                                "'" + S_PartID + "'," +
                                                "'" + S_ProductionOrderID + "'," +
                                                "'" + S_StationTypeID + "'," +
                                                "'" + S_StationID + "'," +
                                                "'" + S_LineID + "'," +
                                                "'" + S_UnitStateID + "'," +
                                                "'" + S_UnitStatusID + "'," +

                                                //"'" + Shift + "'," +
                                                //"'" + YieldLevel + "'," +
                                                //"'" + IsCombineYield + "'," +
                                                
                                                "'" + S_SNs + "'," +
                                                "'" + S_PageIndex + "'," +
                                                "'" + S_PageQTY + "'"
                                                ;

                    var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);

                    try
                    {
                        while (!Query_Multiple.IsConsumed)
                        {
                            List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                            List_ALL.Add(v_List);
                        }
                        S_DataStatus = "1";
                    }
                    catch
                    {

                    }
                }
                catch (Exception ex)
                {
                    List_ALL = List_ERROR(ex, S_DataStatus);
                }
            }
            else
            {
                List_Temp.First().ValStr1 = "ERROR";
                List_Temp.First().ValStr2 = S_Type + " data type Can't be empty";
                List_Temp.First().ValStr3 = S_DataStatus;

                List_ALL.Add(List_Temp);
                return List_ALL;
            }

            return List_ALL;
        }

        public async Task<List<dynamic>> GetSearchCenter(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_Type)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptGetSearchData '" + S_StartDateTime + "', " +
                                                           "'" + S_EndDateTime + "'," +
                                                           "'" + S_PartFamilyTypeID + "'," +
                                                           "'" + S_PartFamilyID + "'," +
                                                           "'" + S_PartID + "'," +
                                                           "'" + S_ProductionOrderID + "'," +
                                                           "'" + S_StationTypeID + "'," +
                                                           "'" + S_StationID + "'," +
                                                           "'" + S_LineID + "'," +
                                                           "'" + S_UnitStateID + "'," +
                                                           "'" + S_UnitStatusID + "'," +
                                                           "'" + S_SNs + "'," +
                                                           "'" + S_Type + "'," +
                                                           "''";
                ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }



        public async Task<IEnumerable<TabVal>> GetExportExcel(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            IEnumerable<TabVal> List_WIPExcel = new List<TabVal>();
            try
            {
                string S_WIP_Report_URL = Configs.GetConfigurationValue("AppSetting", "WIP_Report_URL");
                string S_WIP_Report_Path = Configs.GetConfigurationValue("AppSetting", "WIP_Report_Path");
                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>
                        ("select '' ValStr1 ", null, null, I_DBTimeout, null);

                string S_Parameter = "''" + S_StartDateTime + "'', " +
                                "''" + S_EndDateTime + "''," +
                                "''" + S_PartFamilyTypeID + "''," +
                                "''" + S_PartFamilyID + "''," +
                                "''" + S_PartID + "''," +
                                "''" + S_ProductionOrderID + "''," +
                                "''" + S_StationTypeID + "''," +
                                "''" + S_StationID + "''," +
                                "''" + S_LineID + "''," +
                                "''" + S_UnitStateID + "''," +
                                "''" + S_UnitStatusID + "''," +
                                "''" + S_SNs + "''," +
                                "''" + S_PageIndex + "''," +
                                "''" + S_PageQTY + "''"
                                ;


                if (S_Type != "")
                {
                    string v_Sql = "SELECT DataType ValStr1,CMDSql ValStr2 FROM rptCMDConfig where " +
                        " ReportType='ExportExcel' and DataType='" + S_Type + "'";
                    IEnumerable<TabVal> List_ExportConfig = await DapperConnRead2.QueryAsync<TabVal>(v_Sql, null, null, I_DBTimeout, null);
                    int I_Count = 0;
                    if (List_ExportConfig != null)
                    {
                        if (List_ExportConfig.Count() > 0)
                        {
                            I_Count = List_ExportConfig.Count();
                        }
                    }

                    if (I_Count > 0)
                    {
                        string S_FileName = List_ExportConfig.First().ValStr1 + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".csv";
                        string S_Sql = List_ExportConfig.First().ValStr2 + " " + S_Parameter;

                        string F_Sql = @"DECLARE	@S_Result nvarchar(max)
                            SELECT	@S_Result = '0'
                            EXEC	[dbo].[TTWIP_List]
		                            @S_Sql = '" + S_Sql + @" ',
		                            @S_Path = '" + S_WIP_Report_Path + @" ',
		                            @S_FileName = '" + S_FileName + @"',
		                            @S_Result = @S_Result OUTPUT
                            SELECT	@S_Result as ValStr1";

                        List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>(F_Sql, null, null, I_DBTimeout, null);
                        string S_Result = List_WIPExcel.First().ValStr1 ?? "";

                        if (S_Result == "OK")
                        {
                            List_WIPExcel.First().ValStr2 = "http://" + S_WIP_Report_URL + "/" + S_FileName;
                        }
                        else
                        {
                            List_WIPExcel.First().ValStr2 = "The is no data";
                        }
                    }
                    else
                    {
                        List_WIPExcel.First().ValStr1 = "ERROR";
                        List_WIPExcel.First().ValStr2 = "DataType:" + S_Type + "  is not exists";
                    }
                }
                else
                {
                    List_WIPExcel.First().ValStr1 = "ERROR";
                    List_WIPExcel.First().ValStr2 = "DataType:" + S_Type + "  Can't be empty";
                }
            }
            catch (Exception ex)
            {
                TabVal v_TabVal = List_ERROR(ex, "1")[0][0] as TabVal;

                List_WIPExcel.First().ValStr1 = v_TabVal.ValStr1;
                List_WIPExcel.First().ValStr2 = v_TabVal.ValStr2;
            }

            return List_WIPExcel;
        }


        public async Task<List<dynamic>> GetWIPComponentList(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";

            try
            {
                string S_PName = "exec rptWIPComponentList";
                string S_Sql = S_PName + "  '" + S_StartDateTime + "', " +
                                            "'" + S_EndDateTime + "'," +
                                            "'" + S_PartFamilyTypeID + "'," +
                                            "'" + S_PartFamilyID + "'," +
                                            "'" + S_PartID + "'," +
                                            "'" + S_ProductionOrderID + "'," +
                                            "'" + S_StationTypeID + "'," +
                                            "'" + S_StationID + "'," +
                                            "'" + S_LineID + "'," +
                                            "'" + S_UnitStateID + "'," +
                                            "'" + S_UnitStatusID + "'," +

                                            "'" + S_SNs + "'," +
                                            "'" + S_PageIndex + "'," +
                                            "'" + S_PageQTY + "'"
                                            ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);

                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }

            return List_ALL;
        }


        public async Task<List<dynamic>> GetRawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";

            try
            {
                string S_PName = "[rptGetRawData]";
                string S_Sql = S_PName + "  '" + S_StartDateTime + "', " +
                                            "'" + S_EndDateTime + "'," +
                                            "'" + S_PartFamilyTypeID + "'," +
                                            "'" + S_PartFamilyID + "'," +
                                            "'" + S_PartID + "'," +
                                            "'" + S_ProductionOrderID + "'," +
                                            "'" + S_StationTypeID + "'," +
                                            "'" + S_StationID + "'," +
                                            "'" + S_LineID + "'," +
                                            "'" + S_UnitStateID + "'," +
                                            "'" + S_UnitStatusID + "'," +

                                            "'" + Shift + "'," +
                                            "'" + YieldLevel + "'," +
                                            "'" + IsCombineYield + "'," +

                                            "'" + S_SNs + "'," +
                                            "'" + S_PageIndex + "'," +
                                            "'" + S_PageQTY + "'"
                                            ;

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);

                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }

            return List_ALL;
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel_RawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
             string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            IEnumerable<TabVal> List_WIPExcel = new List<TabVal>();
            try
            {
                string S_WIP_Report_URL = Configs.GetConfigurationValue("AppSetting", "WIP_Report_URL");
                string S_WIP_Report_Path = Configs.GetConfigurationValue("AppSetting", "WIP_Report_Path");
                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>
                        ("select '' ValStr1 ", null, null, I_DBTimeout, null);

                string S_Parameter = "''" + S_StartDateTime + "'', " +
                                "''" + S_EndDateTime + "''," +
                                "''" + S_PartFamilyTypeID + "''," +
                                "''" + S_PartFamilyID + "''," +
                                "''" + S_PartID + "''," +
                                "''" + S_ProductionOrderID + "''," +
                                "''" + S_StationTypeID + "''," +
                                "''" + S_StationID + "''," +
                                "''" + S_LineID + "''," +
                                "''" + S_UnitStateID + "''," +
                                "''" + S_UnitStatusID + "''," +

                                "''" + Shift + "''," +
                                "''" + YieldLevel + "''," +
                                "''" + IsCombineYield + "''," +

                                "''" + S_SNs + "''," +
                                "''" + S_PageIndex + "''," +
                                "''" + S_PageQTY + "''"
                                ;

                string S_FileName = "RawData_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".csv";
                string S_Sql = "exec rptGetRawData " + S_Parameter;

                string F_Sql = @"DECLARE	@S_Result nvarchar(max)
                    SELECT	@S_Result = '0'
                    EXEC	[dbo].[TTWIP_List]
		                    @S_Sql = '" + S_Sql + @" ',
		                    @S_Path = '" + S_WIP_Report_Path + @" ',
		                    @S_FileName = '" + S_FileName + @"',
		                    @S_Result = @S_Result OUTPUT
                    SELECT	@S_Result as ValStr1";

                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>(F_Sql, null, null, I_DBTimeout, null);
                string S_Result = List_WIPExcel.First().ValStr1 ?? "";

                if (S_Result == "OK")
                {
                    List_WIPExcel.First().ValStr2 = "http://" + S_WIP_Report_URL + "/" + S_FileName;
                }
                else
                {
                    List_WIPExcel.First().ValStr2 = "The is no data";
                }
            }
            catch (Exception ex)
            {
                TabVal v_TabVal = List_ERROR(ex, "1")[0][0] as TabVal;

                List_WIPExcel.First().ValStr1 = v_TabVal.ValStr1;
                List_WIPExcel.First().ValStr2 = v_TabVal.ValStr2;
            }

            return List_WIPExcel;
        }

        public IEnumerable<APP> GetAPP()
        {
            string S_Sql = "SELECT * FROM API_APP";
            IEnumerable<APP> v_APP = DapperConnRead2.Query<APP>(S_Sql);
            return v_APP;
        }

        public async Task<IEnumerable<dynamic>> GetPartFamilyAll(string S_PartFamilyTypeID)
        {
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            string S_Sql = "exec rptDashboardGetPartFamilyAll '" + S_PartFamilyTypeID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetPartFamilyTypeAll()
        {
            string S_Sql = "exec rptDashboardGetPartFamilyTypeAll ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetPartNumberAll(string S_PartFamilyID)
        {
            S_PartFamilyID = S_PartFamilyID ?? "";
            string S_Sql = "exec rptDashboardGetPartNumberAll '" + S_PartFamilyID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetProductionOrderAll(string S_PartID)
        {
            S_PartID = S_PartID ?? "";
            string S_Sql = "exec rptDashboardGetProductionOrderAll '" + S_PartID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<IEnumerable<dynamic>> GetStationAll(string S_StationTypeID)
        {
            S_StationTypeID = S_StationTypeID ?? "";
            string S_Sql = "exec rptDashboardGetStationAll '" + S_StationTypeID + "'";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetStationTypeAll()
        {
            string S_Sql = "exec rptDashboardGetStationTypeAll ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetLineAll()
        {
            string S_Sql = "exec rptDashboardGetLineAll ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetUnitStatusAll()
        {
            string S_Sql = "exec rptDashboardGetUnitStatusAll ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetUnitStateAll()
        {
            string S_Sql = "exec rptDashboardGetUnitStateAll ";

            var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        private List<dynamic> List_ERROR(Exception ex, string S_DataStatus)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            var Query_Multiple = DapperConnRead2.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
            IEnumerable<TabVal> List_TabVal = new List<TabVal>();
            if (!Query_Multiple.IsConsumed)
            {
                List_TabVal = Query_Multiple.Read<TabVal>().AsList();
                List_TabVal.First().ValStr1 = "ERROR";
                List_TabVal.First().ValStr2 = ex.Message;
                List_TabVal.First().ValStr3 = S_DataStatus;
            }

            List<dynamic> List_ALL = new List<dynamic>();
            List_ALL.Add(List_TabVal);

            return List_ALL;
        }

        public async Task<List<dynamic>> GetSAPStock(int plant, string version, string material, int PageIndex, int PageQTY)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string S_Sql = $"exec rptSAPStock {plant}, '{version}', '{material}', {(PageIndex <= 0?1 : PageIndex)}, {PageQTY}";

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);
                while (!Query_Multiple.IsConsumed)
                {
                    var v_List = Query_Multiple.Read<dynamic>().AsList();
                    list.Add(v_List);
                }
            }
            catch (Exception ex)
            {
                list = List_ERROR(ex, ex.Message);
            }
            return list;
        }

        public async Task<List<dynamic>> GetSAPAllPlant()
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string S_Sql = $"SELECT DISTINCT Plant FROM dbo.SapBaseInfo";

                var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                list = v_Query.ToList();
            }
            catch (Exception ex)
            {
                list = List_ERROR(ex, ex.Message);
            }
            return list;
        }

        public async Task<List<dynamic>> GetSAPAllVersion(int plant)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string S_Sql = $"SELECT DISTINCT Version FROM dbo.SapDetailInfo WHERE Plant = {plant}  ORDER BY Version DESC";

                var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                list = v_Query.ToList();
            }
            catch (Exception ex)
            {
                list = List_ERROR(ex, ex.Message);
            }
            return list;
        }

        public async Task<List<dynamic>> GetSAPStockMaterial(int plant)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                //string S_Sql = $"SELECT DISTINCT Material FROM dbo.SapBaseInfo WHERE Plant = {plant} ";

                string S_Sql = $"SELECT DISTINCT Material FROM dbo.SapBaseInfo WHERE Plant = {plant}  ORDER BY Material";
                var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                list = v_Query.ToList();
            }
            catch (Exception ex)
            {
                list = List_ERROR(ex, ex.Message);
            }
            return list;
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel_SapStock(int plant, string version, string material)
        {
            IEnumerable<TabVal> List_WIPExcel = new List<TabVal>();
            try
            {
                string S_WIP_Report_URL = Configs.GetConfigurationValue("AppSetting", "WIP_Report_URL");
                string S_WIP_Report_Path = Configs.GetConfigurationValue("AppSetting", "WIP_Report_Path");
                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>
                        ("select '' ValStr1 ", null, null, I_DBTimeout, null);

                string S_Parameter = $" {plant},''{version}'', ''{material}'',0,0";

                string S_FileName = "SapStock_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".csv";
                string S_Sql = "exec rptSAPStock " + S_Parameter;

                string F_Sql = @"DECLARE	@S_Result nvarchar(max)
                    SELECT	@S_Result = '0'
                    EXEC	[dbo].[TTWIP_List]
		                    @S_Sql = '" + S_Sql + @" ',
		                    @S_Path = '" + S_WIP_Report_Path + @" ',
		                    @S_FileName = '" + S_FileName + @"',
		                    @S_Result = @S_Result OUTPUT
                    SELECT	@S_Result as ValStr1";

                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>(F_Sql, null, null, I_DBTimeout, null);
                string S_Result = List_WIPExcel.First().ValStr1 ?? "";

                if (S_Result == "OK")
                {
                    List_WIPExcel.First().ValStr2 = "http://" + S_WIP_Report_URL + "/" + S_FileName;
                }
                else
                {
                    List_WIPExcel.First().ValStr2 = "The is no data";
                }
            }
            catch (Exception ex)
            {
                TabVal v_TabVal = List_ERROR(ex, "1")[0][0] as TabVal;

                List_WIPExcel.First().ValStr1 = v_TabVal.ValStr1;
                List_WIPExcel.First().ValStr2 = v_TabVal.ValStr2;
            }

            return List_WIPExcel;
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel_SapStockPro(int plant, string version, string material, SapDateType sapDateType)
        {
            IEnumerable<TabVal> List_WIPExcel = new List<TabVal>();
            try
            {
                string S_WIP_Report_URL = Configs.GetConfigurationValue("AppSetting", "WIP_Report_URL");
                string S_WIP_Report_Path = Configs.GetConfigurationValue("AppSetting", "WIP_Report_Path");
                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>
                        ("select '' ValStr1 ", null, null, I_DBTimeout, null);

                string S_Parameter = $" {plant},''{version}'', ''{material}'',0,0,{sapDateType.ToInt()}";

                string S_FileName = "SapStockPro_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".csv";
                string S_Sql = "exec rptSAPStockPro " + S_Parameter;

                string F_Sql = @"DECLARE	@S_Result nvarchar(max)
                    SELECT	@S_Result = '0'
                    EXEC	[dbo].[TTWIP_List]
		                    @S_Sql = '" + S_Sql + @" ',
		                    @S_Path = '" + S_WIP_Report_Path + @" ',
		                    @S_FileName = '" + S_FileName + @"',
		                    @S_Result = @S_Result OUTPUT
                    SELECT	@S_Result as ValStr1";

                List_WIPExcel = await DapperConnRead2.QueryAsync<TabVal>(F_Sql, null, null, I_DBTimeout, null);
                string S_Result = List_WIPExcel.First().ValStr1 ?? "";

                if (S_Result == "OK")
                {
                    List_WIPExcel.First().ValStr2 = "http://" + S_WIP_Report_URL + "/" + S_FileName;
                }
                else
                {
                    List_WIPExcel.First().ValStr2 = "The is no data";
                }
            }
            catch (Exception ex)
            {
                TabVal v_TabVal = List_ERROR(ex, "1")[0][0] as TabVal;

                List_WIPExcel.First().ValStr1 = v_TabVal.ValStr1;
                List_WIPExcel.First().ValStr2 = v_TabVal.ValStr2;
            }

            return List_WIPExcel;
        }

        public async Task<List<dynamic>> GetSAPStockPro(int plant, string version, string material, int PageIndex, int PageQTY, SapDateType sapDateType)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string S_Sql = $"exec rptSAPStockPro {plant}, '{version}', '{material}', {(PageIndex <= 0 ? 1 : PageIndex)}, {PageQTY},{(int)sapDateType}";

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(S_Sql, null, null, I_DBTimeout);
                while (!Query_Multiple.IsConsumed)
                {
                    var v_List = Query_Multiple.Read<dynamic>().AsList();
                    list.Add(v_List);
                }
            }
            catch (Exception ex)
            {
                list = List_ERROR(ex, ex.Message);
            }
            return list;
        }

        public async Task<List<dynamic>> GetAutoAnalysisAlarmDashboard()
        {
            List<dynamic> List_ALL = new List<dynamic>();
            string S_DataStatus = "0";
            try
            {
                string sql = "exec rptAutoAnalysisAlarmDashboard ";

                var Query_Multiple = await DapperConnRead2.QueryMultipleAsync(sql, null, null, I_DBTimeout);
                try
                {
                    while (!Query_Multiple.IsConsumed)
                    {
                        List<dynamic> v_List = Query_Multiple.Read<dynamic>().AsList();
                        List_ALL.Add(v_List);
                    }
                    S_DataStatus = "1";
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                List_ALL = List_ERROR(ex, S_DataStatus);
            }
            return List_ALL;
        }


    }
}