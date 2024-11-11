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
    public class SC_mesRouteMachineMapServices : BaseCustomService<SC_mesRouteMachineMap, SC_mesRouteMachineMap, string>, ISC_mesRouteMachineMapServices
    {
        public SC_mesRouteMachineMapServices(ISC_mesRouteMachineMapRepositories repositories) : base(repositories)
        {
            
        }

        public async Task<PageResult<SC_mesRouteMachineMap>> FindWithPagerSearchAsync(SearchMachineMapInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*,b.Name  RouteName, c.Description StationTypeName, d.SN SN,e.PartNumber PartNumber,f.PartNumber MachinePartNumber,g.Name                    MachineFamilyName
                                FROM dbo.mesRouteMachineMap a
                                LEFT  JOIN dbo.mesRoute b ON b.ID = a.RouteID
                                LEFT  JOIN dbo.mesStationType c ON c.ID = a.StationTypeID
                                LEFT JOIN dbo.mesMachine d ON d.ID = a.MachineID
                                LEFT JOIN dbo.mesPart e ON e.ID = a.PartID
                                LEFT JOIN dbo.mesPart f ON f.ID = a.MachinePartID
                                LEFT JOIN dbo.luMachineFamily g ON g.ID = a.MachineFamilyID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR d.SN LIKE '%{search.Keywords}%'
                                     OR	b.Name LIKE '%{search.Keywords}%'
                                     OR	c.Description LIKE '%{search.Keywords}%'
                                     OR	e.PartNumber LIKE '%{search.Keywords}%'
                                     OR	f.PartNumber LIKE '%{search.Keywords}%'
                                     OR	g.Name LIKE '%{search.Keywords}%'
                                        )
                                 AND (                                        
                                      ('{search.SN}' = '' OR d.SN  = '{search.SN}')
                                     AND ('{search.StationTypeName}' = '' OR c.Description = '{search.StationTypeName}')
                                    AND ('{search.PartNumber}' = '' OR	e.PartNumber = '{search.PartNumber}')
                                    AND ('{search.MachinePartNumber}' = '' OR	f.PartNumber = '{search.MachinePartNumber}')
                                    AND ('{search.MachineFamilyName}' = '' OR g.Name  = '{search.MachineFamilyName}')
                                    {(search.RouteIds.Any() ? $" AND a.RouteID in ({string.Join(',', search.RouteIds)}) " : "")}
                                    {(search.PartIds.Any() ? $" AND a.PartID in ({string.Join(',', search.PartIds)}) " : "")}
                                    {(search.MachinePartIds.Any() ? $" AND a.MachinePartID in ({string.Join(',', search.MachinePartIds)}) " : "")}
                                    {(search.MachineFamilyIds.Any() ? $" AND a.MachineFamilyID in ({string.Join(',', search.MachineFamilyIds)}) " : "")}
                                    {(search.StationTypeIds.Any() ? $" AND a.StationTypeID in ({string.Join(',', search.StationTypeIds)}) " : "")}
                                 )
            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            PageResult<SC_mesRouteMachineMap> pageResult = new PageResult<SC_mesRouteMachineMap>
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
