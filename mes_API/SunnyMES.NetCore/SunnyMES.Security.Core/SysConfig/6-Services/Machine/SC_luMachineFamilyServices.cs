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
    public class SC_luMachineFamilyServices : BaseCustomService<SC_luMachineFamily, SC_luMachineFamily,string>, ISC_luMachineFamilyServices
    {
        public SC_luMachineFamilyServices(ISC_luMachineFamilyRepositories repositories) : base(repositories)
        {
            
        }

        public async Task<PageResult<SC_luMachineFamily>> FindWithPagerSearchAsync(SearchMachineFamilyInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*, b.Description StatusDesc, c.Name MachineFamilyTypeName
                                FROM dbo.luMachineFamily a
                                JOIN dbo.sysStatus b ON b.ID = a.Status
                                JOIN dbo.luMachineFamilyType c ON c.ID = a.MachineFamilyTypeID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.Name LIKE '%{search.Keywords}%'
                                     OR	c.Name LIKE '%{search.Keywords}%'
                                     OR a.Description LIKE '%{search.Keywords}%'
                                     OR	c.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (                                        
                                     ( ISNULL({search.Status},0) = 0 OR	  a.Status  = '{search.Status}')
                                     AND ('{search.MachineFamilyName}' = '' OR	a.Name = '{search.MachineFamilyName}')
                                    AND ('{search.MachineFamilyTypeName}' = '' OR	c.Name = '{search.MachineFamilyTypeName}')
                                    {(search.MachineFamilyTypeIDs.Any() ? $" AND c.ID in ({string.Join(',', search.MachineFamilyTypeIDs)}) " : "")}

                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            PageResult<SC_luMachineFamily> pageResult = new PageResult<SC_luMachineFamily>
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
