using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_luDefectServices : BaseCustomService<SC_luDefect, SC_luDefect,string>, ISC_luDefectServices
    {
        public SC_luDefectServices(ISC_luDefectRepositories defectRepositories) : base(defectRepositories)
        {

        }
        public async Task<PageResult<SC_luDefect>> FindWithPagerSearchAsync(SearchDefectInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT b.*,a.Description DefectType 
                                FROM dbo.luDefectType a
                                JOIN dbo.luDefect b ON b.DefectTypeID = a.ID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR b.DefectCode LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                     OR	b.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (                                        
                                      (ISNULL({search.Status},0) = 0 OR	b.Status  = '{search.Status}')
                                     AND ('{search.DefectCode}' = '' OR	b.DefectCode = '{search.DefectCode}')
                                    AND ('{search.DefectDesc}' = '' OR	b.Description = '{search.DefectDesc}')
                                    {(search.DefectTypeIds.Any() ? $" AND b.DefectTypeID in ({string.Join(',', search.DefectTypeIds)}) " : "")}
                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            

            PageResult<SC_luDefect> pageResult = new PageResult<SC_luDefect>
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
