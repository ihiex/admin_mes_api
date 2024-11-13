using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.IServices.MES.SAP;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Commons.Helpers;
using System.Linq;
using SunnyMES.Security.Models;
using SunnyMES.Security._1_Models.MES.SAP;

namespace SunnyMES.Security.Services.MES.SAP
{
    public class CO_ShipManifestData_SNServices : BaseCustomService<CO_ShipManifestData_SN, CO_ShipManifestData_SN, string>, ICO_ShipManifestData_SNServices
    {
        private readonly ICO_ShipManifestData_SNRepository iRepository;

        public CO_ShipManifestData_SNServices(ICO_ShipManifestData_SNRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<IEnumerable<TabVal>> FindExportCSVAsync(SearchSAPManifestDataModel search)
        {
            return await iRepository.FindExportCSVAsync(search);
        }

        public async Task<PageResult<CO_ShipManifestData_SN>> FindWithPagerLikeAsync(SearchSAPManifestDataModel search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = string.Empty;
            //不需要权限管控
            //where = GetDataPrivilege(false);

            if (false)
            {
                where += $@"SELECT *
                        FROM CO_ShipManifestData_SN
                        where
                         (
                           '{search.Keywords}' = '' or ( 
                                                        HAWB LIKE '%{search.Keywords}%'
                                                      OR BillNO LIKE '%{search.Keywords}%'
                                                      OR PurchaseOrderNumber LIKE '%{search.Keywords}%'
                                                      OR PalletSN LIKE '%{search.Keywords}%'
                                                      OR Box_SN LIKE '%{search.Keywords}%'
                                                      OR FG_SN LIKE '%{search.Keywords}%'
                                                        )
                         )   
                         and (

                                                        ('{search.HAWB}' = '' or HAWB = '{search.HAWB}')
                                                        and ('{search.BillNO}' = '' or BillNO = '{search.BillNO}')
                                                        and ('{search.PurchaseOrderNumber}'='' OR PurchaseOrderNumber = '{search.PurchaseOrderNumber}')
                                                        and ('{search.PalletSN}' = '' OR PalletSN = '{search.PalletSN}')
                                                        and ('{search.Box_SN}' = '' OR Box_SN = '{search.Box_SN}')
                                                        and ( '{search.FG_SN}' = '' OR FG_SN = '{search.FG_SN}')
                         )

            ";
                where += string.IsNullOrEmpty(search.StartTime?.ToString()) ? "" : $" AND CreateTime >= '{search.StartTime.ToString()}' ";
                where += string.IsNullOrEmpty(search.EndTime?.ToString()) ? "" : $" AND CreateTime < '{search.EndTime.ToString()}' ";


                PagerInfo pagerInfo = new PagerInfo
                {
                    CurrentPageIndex = search.CurrentPageIndex,
                    PageSize = search.PageSize
                };
                List<CO_ShipManifestData_SN> list = await iRepository.FindWithPagerCustomSqlAsync(where, pagerInfo, search.Sort, order);
                var hawbs = list.Select(y => y.HAWB).Distinct();
                var hawbs2 = string.Join("','", hawbs.ToList());
                var MPNs = SqlSugarHelper.Db.SqlQueryable<HAWBMPN>($"SELECT a.HAWB HAWB,b.FMPNNO MPN FROM dbo.CO_WH_ShipmentNew a JOIN dbo.CO_WH_ShipmentEntryNew b ON b.FInterID = a.FInterID WHERE a.HAWB IN ('{hawbs2}') GROUP BY a.HAWB,b.FMPNNO").ToList();


                list.ForEach(x => x.MPN = MPNs.First(y => y.HAWB == x.HAWB).MPN);
                PageResult<CO_ShipManifestData_SN> pageResult = new PageResult<CO_ShipManifestData_SN>
                {
                    CurrentPage = pagerInfo.CurrentPageIndex,
                    Items = list.MapTo<CO_ShipManifestData_SN>(),
                    ItemsPerPage = pagerInfo.PageSize,
                    TotalItems = pagerInfo.RecordCount
                };
                return pageResult;
            }
            else
            {
                where += $@"SELECT s.*,(SELECT TOP 1 ISNULL( b.FMPNNO, '''')   FROM dbo.CO_WH_ShipmentNew a
                                    JOIN dbo.CO_WH_ShipmentEntryNew b ON b.FInterID = a.FInterID 
                                    WHERE a.HAWB = s.HAWB) MPN
                        FROM CO_ShipManifestData_SN s
                        where
                         (
                           '{search.Keywords}' = '' or ( 
                                                        s.HAWB LIKE '%{search.Keywords}%'
                                                      OR s.BillNO LIKE '%{search.Keywords}%'
                                                      OR s.PurchaseOrderNumber LIKE '%{search.Keywords}%'
                                                      OR s.PalletSN LIKE '%{search.Keywords}%'
                                                      OR s.Box_SN LIKE '%{search.Keywords}%'
                                                      OR s.FG_SN LIKE '%{search.Keywords}%'
                                                        )
                         )   
                         and (

                                                        ('{search.HAWB}' = '' or s.HAWB = '{search.HAWB}')
                                                        and ('{search.BillNO}' = '' or s.BillNO = '{search.BillNO}')
                                                        and ('{search.PurchaseOrderNumber}'='' OR s.PurchaseOrderNumber = '{search.PurchaseOrderNumber}')
                                                        and ('{search.PalletSN}' = '' OR s.PalletSN = '{search.PalletSN}')
                                                        and ('{search.Box_SN}' = '' OR s.Box_SN = '{search.Box_SN}')
                                                        and ( '{search.FG_SN}' = '' OR s.FG_SN = '{search.FG_SN}')
                         )

            ";
                where += string.IsNullOrEmpty(search.StartTime?.ToString()) ? "" : $" AND s.CreateTime >= '{search.StartTime.ToString()}' ";
                where += string.IsNullOrEmpty(search.EndTime?.ToString()) ? "" : $" AND s.CreateTime < '{search.EndTime.ToString()}' ";


                PagerInfo pagerInfo = new PagerInfo
                {
                    CurrentPageIndex = search.CurrentPageIndex,
                    PageSize = search.PageSize
                };
                List<CO_ShipManifestData_SN> list = await iRepository.FindWithPagerCustomSqlAsync(where, pagerInfo, search.Sort, order);

                PageResult<CO_ShipManifestData_SN> pageResult = new PageResult<CO_ShipManifestData_SN>
                {
                    CurrentPage = pagerInfo.CurrentPageIndex,
                    Items = list.MapTo<CO_ShipManifestData_SN>(),
                    ItemsPerPage = pagerInfo.PageSize,
                    TotalItems = pagerInfo.RecordCount
                };
                return pageResult;
            }


        }

        public override async Task<bool> UpdateAsync(CO_ShipManifestData_SN entity, string id, IDbTransaction trans = null)
        {
            return await base.UpdateAsync(entity, id, trans);
        }
    }
}
