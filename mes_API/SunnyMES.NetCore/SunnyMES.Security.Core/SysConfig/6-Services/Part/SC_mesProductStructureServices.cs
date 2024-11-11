using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_mesProductStructureServices : BaseCustomService<SC_mesProductStructure, SC_mesProductStructure, string>, ISC_mesProductStructureServices
    {
        public SC_mesProductStructureServices(ISC_mesProductStructureRepositories iRepositories) : base(iRepositories)
        {

        }
        public async Task<PageResult<SC_mesProductStructure>> FindWithPagerSearchAsync(SearchBOMInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*, pp.PartNumber ParentName, cp.PartNumber ChildName, d.Description StationTypeName,ss.Description  StatusDesc
                                    FROM dbo.mesProductStructure a
                                    JOIN dbo.mesPart pp ON pp.ID = a.ParentPartID
                                    JOIN dbo.mesPart cp ON cp.ID = a.PartID
                                    JOIN  dbo.mesStationType d ON d.ID = a.StationTypeID
                                    JOIN dbo.sysStatus ss ON ss.ID = a.Status
                                    WHERE 1 = 1
                                        AND ('{search.Keywords}' = '' 
                                            OR pp.PartNumber LIKE '%{search.Keywords}%'
                                            OR	cp.PartNumber LIKE '%{search.Keywords}%'
                                            OR	d.Description LIKE '%{search.Keywords}%')
                                        AND (
                                            (ISNULL({search.Status},0) = 0 OR	a.Status  = '{search.Status}')
                                            {(search.ParentPartIDs.Any() ? $" AND a.ParentPartID in ({string.Join(',', search.ParentPartIDs)}) " : "")}
                                            {(search.PartIDs.Any() ? $" AND a.PartID in ({string.Join(',', search.PartIDs)}) " : "")}
                                            {(search.StationTypeIDs.Any() ? $" AND d.ID in ({string.Join(',', search.StationTypeIDs)}) " : "")}

                                         )";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            

            PageResult<SC_mesProductStructure> pageResult = new PageResult<SC_mesProductStructure>
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
