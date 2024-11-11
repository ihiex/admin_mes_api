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

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_luPartFamilyDetailDefServices : BaseCustomService<SC_luPartFamilyDetailDef, SC_luPartFamilyDetailDef,string>, ISC_luPartFamilyDetailDefServices
    {
        public SC_luPartFamilyDetailDefServices(ISC_luPartFamilyDetailDefRepositories detailDefRepositories) : base(detailDefRepositories)
        {
            
        }

        public override async Task<PageResult<SC_luPartFamilyDetailDef>> FindWithPagerAsync(SearchInputDto<SC_luPartFamilyDetailDef> search)
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
            List<SC_luPartFamilyDetailDef> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SC_luPartFamilyDetailDef> pageResult = new PageResult<SC_luPartFamilyDetailDef>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_luPartFamilyDetailDef>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
