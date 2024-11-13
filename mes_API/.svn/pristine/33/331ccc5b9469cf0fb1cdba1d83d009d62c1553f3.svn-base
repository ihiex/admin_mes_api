using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_luVendoServices : BaseCustomService<SC_luVendo, SC_luVendo, string>, ISC_luVendoServices
    {
        public SC_luVendoServices(ISC_luVendoRepositories iluVendoRepositories) : base(iluVendoRepositories)
        {
            
        }

        public async Task<PageResult<SC_luVendo>> FindWithPagerSearchAsync(SearchVendoInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*,b.PartNumber PartName, c.ID PartFamilyID, c.Name AS PartFamilyName, d.ID PartFamilyTypeID, d.Name PartFamilyTypeName
                                FROM dbo.luVendor a
                                JOIN dbo.mesPart b ON b.ID = a.PartID
                                JOIN dbo.luPartFamily c ON c.ID = b.PartFamilyID
                                JOIN dbo.luPartFamilyType d ON d.ID = c.PartFamilyTypeID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR b.PartNumber LIKE '%{search.Keywords}%'
                                     OR	a.Code LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.PartName}' = '' OR b.PartNumber = '{search.PartName}')
                                     AND ( '{search.Code}' = '' OR	a.Code = '{search.Code}')
                                    {(search.PartIds.Any() ? $" AND b.ID in ({string.Join(',', search.PartIds)}) " : "")}
                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);



            PageResult<SC_luVendo> pageResult = new PageResult<SC_luVendo>
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
