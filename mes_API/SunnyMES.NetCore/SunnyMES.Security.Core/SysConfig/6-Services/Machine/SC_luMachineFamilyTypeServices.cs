using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.IRepositories.Machine;
using SunnyMES.Security.SysConfig.IServices.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;

namespace SunnyMES.Security.SysConfig.Services.Machine
{
    public class SC_luMachineFamilyTypeServices : BaseCustomService<SC_luMachineFamilyType, SC_luMachineFamilyType, string>, ISC_luMachineFamilyTypeServices
    {
        public SC_luMachineFamilyTypeServices(ISC_luMachineFamilyTypeRepositories repositories) : base(repositories)
        {
            
        }

        public async Task<PageResult<SC_luMachineFamilyType>> FindWithPagerSearchAsync(SearchMachineFamilyTypeInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*, b.Description StatusDesc
                                FROM dbo.luMachineFamilyType a
                                JOIN dbo.sysStatus b ON b.ID = a.Status
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.Name LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (                                        
                                     ( ISNULL({search.Status},0) = 0 OR	  a.Status  = '{search.Status}')
                                    AND ('{search.MachineFamilyTypeName}' = '' OR	a.Name = '{search.MachineFamilyTypeName}')
                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            PageResult<SC_luMachineFamilyType> pageResult = new PageResult<SC_luMachineFamilyType>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,

            };
            return pageResult;
        }
    }
}
