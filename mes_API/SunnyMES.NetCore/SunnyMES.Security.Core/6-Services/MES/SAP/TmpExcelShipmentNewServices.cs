using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.IServices.MES.SAP;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Dtos;
using SqlSugar;
using SunnyMES.Commons.Mapping;
using NPOI.SS.Formula.Functions;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Commons.Extend;

namespace SunnyMES.Security.Services.MES.SAP
{
    public class TmpExcelShipmentNewServices : BaseCustomService<tmpExcelShipmentNew, tmpExcelShipmentNew, string>, ITmpExcelShipmentNewServices
    {
        private readonly ITmpExcelShipmentNewRepository iRepository;

        public TmpExcelShipmentNewServices(ITmpExcelShipmentNewRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<PageResult<tmpExcelShipmentNew>> FindWithPagerLikeAsync(SearchSAPDataModel search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";
            string selectStr = $@"SELECT  a.ShipDate,  a.Project,  a.HAWB# as HAWB,  a.HubCode,  a.Country,  a.Region,  a.PO# as PO,  a.PartNumber,  a.PartNumberDesc,  a.QTY,  b.CartonQTY,  b.PalletQTY,  a.LineItem,  a.TruckNo,  a.Reference# as Reference,  a.Carrier,  a.ShipID,  a.ReturnAddress,  a.DeliveryToName,  a.AdditionalDeliveryToName,  a.DeliveryStreetAddress,  a.DeliveryCityName,  a.DeliveryPostalCode,  a.DeliveryRegion,  a.DeliveryCountry,  a.TelNo,  a.MAWB_OceanContainerNumber,  a.TransportMethod,  a.TotalVolume,  a.VolumeUnit,  a.Origion,  a.POE_COC,  a.POE,  a.memo,  a.CTRY,  a.SHA,  a.Sales,  a.Web# as Web,  a.UUI,  a.DN# as DN,  a.Delivery# as Delivery,  a.Special,  a.SCAC,  a.OEMSpecificPO1,  a.OEMSpecificPO2,  a.import, a.[NO.] as NO,  a.CreateTime
            FROM dbo.tmpExcelShipmentNew a
            JOIN dbo.tmpExcelShipmentNew_History b ON b.[NO.] = a.[NO.]
            WHERE 
            (
                ('{search.Keywords}' = '' or ( a.HAWB# LIKE '%{search.Keywords}%'
                                                OR a.Project LIKE '%{search.Keywords}%'
                                                OR a.HubCode LIKE '%{search.Keywords}%'
                                                OR a.Region LIKE '%{search.Keywords}%'
                                                OR a.PO# LIKE '%{search.Keywords}%'
                                                OR a.PartNumber LIKE '%{search.Keywords}%'
                                                OR a.PartNumberDesc LIKE '%{search.Keywords}%'
                                                )
                )
                and (
                            ( '{search.HAWB}' = '' OR a.HAWB# = '{search.HAWB}')
                            AND ( '{search.Project}' = ''  OR a.Project = '{search.Project}')
                            AND ( '{search.HubCode}' = ''   OR a.HubCode = '{search.HubCode}')
                            AND ( '{search.Region}' = ''   OR a.Region = '{search.Region}')
                            AND ( '{search.PO}' = ''   OR a.PO# = '{search.PO}')
                            AND ( '{search.PartNumber}' = ''   OR a.PartNumber = '{search.PartNumber}')
                            AND ( '{search.PartNumberDesc}' = ''   OR a.PartNumberDesc = '{search.PartNumberDesc}')
                            AND ( ISNULL({search.import},-1) = -1   OR a.import = '{search.import}')
                )
            )
            ";
            selectStr += string.IsNullOrEmpty(search.StartTime?.ToString()) ? "" : $" AND a.ShipDate >= '{search.StartTime.ToString()}' ";
            selectStr += string.IsNullOrEmpty(search.EndTime?.ToString()) ? "" : $" AND a.ShipDate < '{search.EndTime.ToString()}' ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await iRepository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, string.IsNullOrEmpty(search.Sort)?"NO": search.Sort, order);
            PageResult<tmpExcelShipmentNew> pageResult = new PageResult<tmpExcelShipmentNew>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<tmpExcelShipmentNew>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,
                
            };
            return pageResult;
        }

        public async Task<PageResult<tmpExcelShipmentNew>> GetListSapByHAWBAsync(SearchSAPDataModel search)
        {
            string where = string.Empty;
            bool order = search.Order.ToUpper().Trim() == "DESC";
            where = GetDataPrivilege(false);
            where += $" and HAWB#= '{search.HAWB}'";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await iRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<tmpExcelShipmentNew> pageResult = new PageResult<tmpExcelShipmentNew>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<tmpExcelShipmentNew>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }

        public async Task<PageResult<tmpExcelShipmentNew>> GetListSapByShippingTimeAsync(SearchSAPDataModel search)
        {
            string where = string.Empty;
            bool order = search.Order.ToUpper().Trim() == "DESC";
            where = GetDataPrivilege(false);
            where += $" and ShipDate >= '{search.StartTime}' AND ShipDate < '{search.EndTime}'";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await iRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<tmpExcelShipmentNew> pageResult = new PageResult<tmpExcelShipmentNew>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<tmpExcelShipmentNew>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
