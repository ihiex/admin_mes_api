using Dapper;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Mapping;
using SunnyMES.Security._1_Models.MES.SAP;
using MySqlX.XDevAPI;
using SunnyMES.Commons;

namespace SunnyMES.Security.Repositories.MES.SAP
{
    public class CO_ShipManifestData_SNRepository : BaseCustomRepository<CO_ShipManifestData_SN, string>, ICO_ShipManifestData_SNRepository
    {
        public CO_ShipManifestData_SNRepository()
        {
            
        }
        public CO_ShipManifestData_SNRepository(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TabVal>> FindExportCSVAsync(SearchSAPManifestDataModel search)
        {

            IEnumerable<TabVal> List_WIPExcel = new List<TabVal>();
            try
            {
                string S_WIP_Report_URL = Configs.GetConfigurationValue("AppSetting", "WIP_Report_URL");
                string S_WIP_Report_Path = Configs.GetConfigurationValue("AppSetting", "WIP_Report_Path");
                List_WIPExcel = await DapperConn.QueryAsync<TabVal>
                        ("select '' ValStr1 ", null, null, I_DBTimeout, null);


                string S_FileName = "SAPManifestDataSN_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".csv";

                bool order = search.Order == "asc" ? false : true;
                string where = string.Empty;


                where += $@"SELECT a.*,(SELECT TOP 1 ISNULL( b.FMPNNO, '''')   FROM dbo.CO_WH_ShipmentNew c
                                    JOIN dbo.CO_WH_ShipmentEntryNew b ON b.FInterID = c.FInterID 
                                    WHERE c.HAWB = a.HAWB) MPN
                        FROM CO_ShipManifestData_SN a
                        where
                         (
                           ''{search.Keywords}'' = '''' or ( 
                                                        a.HAWB LIKE ''%{search.Keywords}%''
                                                      OR a.BillNO LIKE ''%{search.Keywords}%''
                                                      OR a.PurchaseOrderNumber LIKE ''%{search.Keywords}%''
                                                      OR a.PalletSN LIKE ''%{search.Keywords}%''
                                                      OR a.Box_SN LIKE ''%{search.Keywords}%''
                                                      OR a.FG_SN LIKE ''%{search.Keywords}%''
                                                        )
                         )   
                         and (

                                                        (''{search.HAWB}'' = '''' or a.HAWB = ''{search.HAWB}'')
                                                        and (''{search.BillNO}'' = '''' or a.BillNO = ''{search.BillNO}'')
                                                        and (''{search.PurchaseOrderNumber}''='''' OR a.PurchaseOrderNumber = ''{search.PurchaseOrderNumber}'')
                                                        and (''{search.PalletSN}'' = '''' OR a.PalletSN = ''{search.PalletSN}'')
                                                        and (''{search.Box_SN}'' = '''' OR a.Box_SN = ''{search.Box_SN}'')
                                                        and ( ''{search.FG_SN}'' = '''' OR a.FG_SN = ''{search.FG_SN}'')
                         )

            ";
                where += string.IsNullOrEmpty(search.StartTime?.ToString()) ? "" : $" AND CreateTime >= ''{search.StartTime.ToString()}'' ";
                where += string.IsNullOrEmpty(search.EndTime?.ToString()) ? "" : $" AND CreateTime < ''{search.EndTime.ToString()}'' ";
                string F_Sql = @"DECLARE	@S_Result nvarchar(max)
                    SELECT	@S_Result = '0'
                    EXEC	[dbo].[TTWIP_List]
		                    @S_Sql = '" + where + @" ',
		                    @S_Path = '" + S_WIP_Report_Path + @" ',
		                    @S_FileName = '" + S_FileName + @"',
		                    @S_Result = @S_Result OUTPUT
                    SELECT	@S_Result as ValStr1";

                List_WIPExcel = await DapperConn.QueryAsync<TabVal>(F_Sql, null, null, I_DBTimeout, null);
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

        public async Task<List<CO_ShipManifestData_SN>> FindWithPagerLikeAsync(string condition, PagerInfo info, string fieldToSort, bool desc, IDbTransaction trans = null)
        {
            
            if (true)
            {
                // 临时表方式，不知道性能如何
                List<CO_ShipManifestData_SN> list = new List<CO_ShipManifestData_SN>();

                //if (HasInjectionData(condition))
                //{
                //    Log4NetHelper.Info(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                //    throw new Exception("检测出SQL注入的恶意数据");
                //}
                //if (string.IsNullOrEmpty(condition))
                //{
                //    condition = "1=1";
                //}

                PagerCustomHelper pagerHelper = new PagerCustomHelper(this.tableName, this.selectedFields, string.IsNullOrEmpty(fieldToSort) ? primaryKey : fieldToSort, info.PageSize, info.CurrentPageIndex <= 0 ? 1 : info.CurrentPageIndex, desc, condition);

                string pageSql = pagerHelper.GetSqlServerCustomSql();
                var reader = await DapperConnRead.QueryMultipleAsync(pageSql);
                info.RecordCount = reader.ReadFirst<int>();
                list = reader.Read<CO_ShipManifestData_SN>().ToList();

                return list;
            }
            else
            {
                return await base.FindWithPagerAsync(condition, info, fieldToSort, desc, trans);
            }
        }
        public override async Task<bool> UpdateAsync(CO_ShipManifestData_SN entity, string primaryKey, IDbTransaction trans = null)
        {
            return await base.UpdateAsync(entity, primaryKey, trans);
        }
        private List<dynamic> List_ERROR(Exception ex, string S_DataStatus)
        {
            string S_Sql = "SELECT '' ValStr1,'' ValStr2,'' ValStr3 ";
            var Query_Multiple = DapperConnRead.QueryMultiple(S_Sql, null, null, I_DBTimeout, null);
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
    }
}
