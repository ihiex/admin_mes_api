using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_luPartDetailDefServices : BaseCustomService<SC_luPartDetailDef, SC_luPartDetailDef, string>, ISC_luPartDetailDefServices
    {
        public SC_luPartDetailDefServices(ISC_luPartDetailDefRepositories detailDefRepositories) : base(detailDefRepositories)
        {
            
        }
        public override async Task<PageResult<SC_luPartDetailDef>> FindWithPagerAsync(SearchInputDto<SC_luPartDetailDef> search)
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
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where = $"  Description LIKE '%{search.Keywords}%'";
            }
            List<SC_luPartDetailDef> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SC_luPartDetailDef> pageResult = new PageResult<SC_luPartDetailDef>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_luPartDetailDef>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
