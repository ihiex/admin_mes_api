using System.Threading.Tasks;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.IServices.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.Services.MES.SAP
{
    public class CO_WH_ShipmentNewServices : BaseCustomService<CO_WH_ShipmentNew_T, CO_WH_ShipmentNew_T, string>, ICO_WH_ShipmentNewServices
    {
        private readonly ICO_WH_ShipmentNewRepository iRepository;

        public CO_WH_ShipmentNewServices(ICO_WH_ShipmentNewRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<PageResult<CO_WH_ShipmentNew_T>> GetListSapByHAWBAsync(SearchShipmentNewDataModel search)
        {
            string where = string.Empty;
            bool order = search.Order.ToUpper().Trim() == "DESC";
            where = GetDataPrivilege(false);
            where += $" and HAWB= '{search.HAWB}'";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await iRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<CO_WH_ShipmentNew_T> pageResult = new PageResult<CO_WH_ShipmentNew_T>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<CO_WH_ShipmentNew_T>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }

        public async Task<PageResult<CO_WH_ShipmentNew_T>> GetListSapByShippingTimeAsync(SearchShipmentNewDataModel search)
        {
            string where = string.Empty;
            bool order = search.Order.ToUpper().Trim() == "DESC";
            where = GetDataPrivilege(false);
            where += $" and FDate >= '{search.StartTime}' AND FDate < '{search.EndTime}'";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await iRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<CO_WH_ShipmentNew_T> pageResult = new PageResult<CO_WH_ShipmentNew_T>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<CO_WH_ShipmentNew_T>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
