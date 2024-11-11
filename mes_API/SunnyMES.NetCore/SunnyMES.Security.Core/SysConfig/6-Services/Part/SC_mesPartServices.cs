using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_mesPartServices : BaseCustomService<SC_mesPart, SC_mesPart, string>, ISC_mesPartServices
    {
        private readonly ISC_mesPartRepositories partRepositories;
        private readonly ISC_mesPartDetailRepositories detailRepositories;

        public SC_mesPartServices(ISC_mesPartRepositories partRepositories, ISC_mesPartDetailRepositories detailRepositories) : base(partRepositories)
        {
            this.partRepositories = partRepositories;
            this.detailRepositories = detailRepositories;
        }

        public async Task<CommonResult> CloneDataAsync(SC_mesPart inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var r = await partRepositories.CheckExistAsync(inputDto);
            if (!r)
            {   
                commonResult.ResultMsg = " Data already exists.";
                return commonResult;
            }

            var details = await detailRepositories.GetListWhereAsync($" PartID = {inputDto.ID}");
            var cr = await partRepositories.CloneDataAsync(inputDto, details);
            if (!cr)
            {
                commonResult.ResultMsg = " Clone Failed.";
            }
            return commonResult;
        }

        public async Task<CommonResult> DeleteDataAsync(SC_mesPart inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var beforeT = await partRepositories.GetSingleOrDefaultAsync(x => x.ID == inputDto.ID);
            if (beforeT is null)
            {
                commonResult.ResultMsg = "no found delete data.";
                return commonResult;
            }
            var details = await detailRepositories.GetListWhereAsync($" PartID = {inputDto.ID}");
            var r = await partRepositories.DeleteDataAsync(beforeT, details);
            if (!r)
            {
                commonResult.ResultMsg =  " Delete data failed.";
            }
            return commonResult;
        }

        public async Task<PageResult<SC_mesPart>> FindWithPagerSearchAsync(SearchPartInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT DISTINCT a.*,ss.Description StatusDesc, fp.Name PartFamilyName
                                FROM dbo.mesPart a
                                LEFT JOIN dbo.mesPartDetail b ON b.PartID = a.ID
                                LEFT JOIN dbo.luPartDetailDef c ON c.ID = b.PartDetailDefID
                                LEFT JOIN dbo.sysStatus ss ON ss.ID = a.Status
                                LEFT JOIN dbo.luPartFamily fp ON fp.ID = a.PartFamilyID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.PartNumber LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	c.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.PartName}' = '' OR a.PartNumber = '{search.PartName}')
                                     AND ( '{search.PartDesc}' = '' OR	a.Description = '{search.PartDesc}')
                                     AND ( '{search.DetailValue}' = '' OR	b.Content = '{search.DetailValue}')
                                     AND ( '{search.DetailName}' = '' OR	c.Description = '{search.DetailName}')
                                     AND ( ISNULL({search.Status},0) = 0 OR	  a.Status  = '{search.Status}')
                                    {(search.DetailNameIds.Any() ? $" AND c.ID in ({string.Join(',', search.DetailNameIds)}) " : "")}
                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            //List<SC_mesPartListOutputDto> tmpItems = new List<SC_mesPartListOutputDto>();
            //list.ForEach(x => tmpItems.Add(TransExp<SC_mesPart, SC_mesPartListOutputDto>.Trans(x)));
            //var tmpItems = list.MapTo<SC_mesPartListOutputDto>();

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach(x =>
            {
                string Sql = $@"IF EXISTS(SELECT a.*,b.ID DefID, b.Description DefDescription
                                    FROM dbo.mesPartDetail a
                                    JOIN dbo.luPartDetailDef b ON b.ID = a.PartDetailDefID
                                    WHERE a.PartID = {x.ID} 
                                    AND ('{search.Keywords}' = '' 
                                            OR	a.Content LIKE '%{search.Keywords}%'
                                            OR	b.Description LIKE '%{search.Keywords}%')
                                    AND (
                                            ( '{search.DetailValue}' = '' OR	a.Content = '{search.DetailValue}')
                                            AND ( '{search.DetailName}' = '' OR	b.Description = '{search.DetailName}')
                                    ))
                                BEGIN
                                    SELECT a.*,b.ID DefID, b.Description DefDescription
                                    FROM dbo.mesPartDetail a
                                    JOIN dbo.luPartDetailDef b ON b.ID = a.PartDetailDefID
                                    WHERE a.PartID = {x.ID} 
                                                                AND ('{search.Keywords}' = '' 
                                                                        OR	a.Content LIKE '%{search.Keywords}%'
                                                                        OR	b.Description LIKE '%{search.Keywords}%')
                                                                AND (
                                                                        ( '{search.DetailValue}' = '' OR	a.Content = '{search.DetailValue}')
                                                                        AND ( '{search.DetailName}' = '' OR	b.Description = '{search.DetailName}')
                                                                )
                                END
                                ELSE
                                BEGIN
	                                SELECT a.*,b.ID DefID, b.Description DefDescription
                                    FROM dbo.mesPartDetail a
                                    JOIN dbo.luPartDetailDef b ON b.ID = a.PartDetailDefID
                                    WHERE a.PartID = {x.ID}
                                END";
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesPartDetail>(Sql);
                x.PartDetails = allChilds;
            });

            #endregion


            PageResult<SC_mesPart> pageResult = new PageResult<SC_mesPart>
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
