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
    public class SC_luPartFamilyTypeDetailDefServices : BaseCustomService<SC_luPartFamilyTypeDetailDef, SC_luPartFamilyTypeDetailDef, string>, ISC_luPartFamilyTypeDetailDefServices
    {
        public SC_luPartFamilyTypeDetailDefServices(ISC_luPartFamilyTypeDetailDefRepositories detailDefRepositories) : base(detailDefRepositories)
        {
            
        }

        public override async Task<PageResult<SC_luPartFamilyTypeDetailDef>> FindWithPagerAsync(SearchInputDto<SC_luPartFamilyTypeDetailDef> search)
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
            List<SC_luPartFamilyTypeDetailDef> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SC_luPartFamilyTypeDetailDef> pageResult = new PageResult<SC_luPartFamilyTypeDetailDef>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_luPartFamilyTypeDetailDef>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
