using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.IRepositories.PO;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Services.PO
{
    public class SC_luProductionOrderDetailDefServices : BaseCustomService<SC_luProductionOrderDetailDef, SC_luProductionOrderDetailDef,string>, ISC_luProductionOrderDetailDefServices
    {
        public SC_luProductionOrderDetailDefServices(ISC_luProductionOrderDetailDefRepositories repositories) : base(repositories)
        {
            
        }

        public override async Task<PageResult<SC_luProductionOrderDetailDef>> FindWithPagerAsync(SearchInputDto<SC_luProductionOrderDetailDef> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = string.Empty;
            //不需要权限管控
            where = GetDataPrivilege(false);
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            if(!string.IsNullOrEmpty(search.Keywords))
            {
                where = $"  Description LIKE '%{search.Keywords}%' or  Parameters LIKE '%{search.Keywords}%'";
            }
            List<SC_luProductionOrderDetailDef> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SC_luProductionOrderDetailDef> pageResult = new PageResult<SC_luProductionOrderDetailDef>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_luProductionOrderDetailDef>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
