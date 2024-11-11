using System;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_luPartFamilyServices : BaseCustomService<SC_luPartFamily, SC_luPartFamily, string>, ISC_luPartFamilyServices
    {
        private readonly ISC_luPartFamilyRepositories familyRepositories;
        private readonly ISC_mesPartFamilyDetailRepositories detailRepositories;
        private readonly ISC_mesPartRepositories _partRepositories;

        public SC_luPartFamilyServices(ISC_luPartFamilyRepositories familyRepositories, ISC_mesPartFamilyDetailRepositories detailRepositories, ISC_mesPartRepositories partRepositories) : base(familyRepositories)
        {
            this.familyRepositories = familyRepositories;
            this.detailRepositories = detailRepositories;
            _partRepositories = partRepositories;
        }

        public async Task<CommonResult> CloneDataAsync(SC_luPartFamily inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var r = await familyRepositories.CheckExistAsync(inputDto);
            if (!r)
            {
                commonResult.ResultMsg = " Data already exists.";
                return commonResult;
            }

            var details = await detailRepositories.GetListWhereAsync($" PartFamilyID = {inputDto.ID}");
            var cr = await familyRepositories.CloneDataAsync(inputDto, details);
            if (!cr)
            {
                commonResult.ResultMsg = " Clone Failed.";
            }
            return commonResult;
        }

        public async Task<CommonResult> DeleteDataAsync(SC_luPartFamily inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var beforeT = await familyRepositories.GetSingleOrDefaultAsync(x => x.ID == inputDto.ID);
            if (beforeT is null)
            {
                commonResult.ResultMsg = "no found delete data.";
                return commonResult;
            }

            var partLinked = await _partRepositories.GetListWhereAsync($" PartFamilyID = {inputDto.ID}");
            if (partLinked.Any())
            {
                var parts = partLinked.ToList();
                commonResult.ResultMsg = $"At first, please delete part [{String.Join(',', parts.Select(x => x.PartNumber))}].";
                return commonResult;
            }    


            var details = await detailRepositories.GetListWhereAsync($" PartFamilyID = {inputDto.ID}");
            var r = await familyRepositories.DeleteDataAsync(beforeT, details);
            if (!r)
            {
                commonResult.ResultMsg = " Delete data failed.";
            }
            return commonResult;
        }

        public async Task<PageResult<SC_luPartFamily>> FindWithPagerSearchAsync(SearchPartFamilyInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";


            string selectStr = $@"SELECT DISTINCT a.*,ss.Description  StatusDesc,  ft.Name PartFamilyTypeName
                                FROM dbo.luPartFamily a
                                LEFT JOIN dbo.mesPartFamilyDetail b ON b.PartFamilyID = a.ID
                                LEFT JOIN dbo.luPartFamilyDetailDef c ON c.ID = b.PartFamilyDetailDefID
                                Left JOIN dbo.sysStatus ss ON ss.ID = a.Status
                                LEFT JOIN dbo.luPartFamilyType ft ON ft.ID = a.PartFamilyTypeID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.Name LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	c.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.PartFamilyName}' = '' OR a.Name = '{search.PartFamilyName}')
                                     AND ( '{search.PartFamilyDesc}' = '' OR	a.Description = '{search.PartFamilyDesc}')
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

            //var tmpItems = list.MapTo<SC_luPartFamilyListOutputDto>();

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach(x =>
            {
                string Sql = $@"IF EXISTS(SELECT a.*,b.ID DefID, b.Description DefDescription
                                    FROM dbo.mesPartFamilyDetail a
                                    JOIN dbo.luPartFamilyDetailDef b ON b.ID = a.PartFamilyDetailDefID
                                    WHERE a.PartFamilyID = {x.ID} 
                                    AND ('{search.Keywords}' = '' 
                                            OR	a.Content LIKE '%{search.Keywords}%'
                                            OR	b.Description LIKE '%{search.Keywords}%')
                                    AND (
                                            ( '{search.DetailValue}' = '' OR	a.Content = '{search.DetailValue}')
                                            AND ( '{search.DetailName}' = '' OR	b.Description = '{search.DetailName}')
                                    ))
                                BEGIN
                                    SELECT a.*,b.ID DefID, b.Description DefDescription
                                                                FROM dbo.mesPartFamilyDetail a
                                                                JOIN dbo.luPartFamilyDetailDef b ON b.ID = a.PartFamilyDetailDefID
                                                                WHERE a.PartFamilyID = {x.ID} 
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
									                                FROM dbo.mesPartFamilyDetail a
									                                JOIN dbo.luPartFamilyDetailDef b ON b.ID = a.PartFamilyDetailDefID
									                                WHERE a.PartFamilyID = {x.ID}
                                END";
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesPartFamilyDetail>(Sql);
                x.PartFamilyDetails = allChilds;
            });

            #endregion


            PageResult<SC_luPartFamily> pageResult = new PageResult<SC_luPartFamily>
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
