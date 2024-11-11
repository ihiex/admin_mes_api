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
    public class SC_luPartFamilyTypeServices : BaseCustomService<SC_luPartFamilyType, SC_luPartFamilyType, string>, ISC_luPartFamilyTypeServices
    {
        private readonly ISC_luPartFamilyTypeRepositories familyTypeRepositories;
        private readonly ISC_mesPartFamilyTypeDetailRepositories detailRepositoris;

        public SC_luPartFamilyTypeServices(ISC_luPartFamilyTypeRepositories familyTypeRepositories, ISC_mesPartFamilyTypeDetailRepositories detailRepositoris): base(familyTypeRepositories)
        {
            this.familyTypeRepositories = familyTypeRepositories;
            this.detailRepositoris = detailRepositoris;
        }

        public async Task<CommonResult> CloneDataAsync(SC_luPartFamilyType inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var r = await familyTypeRepositories.CheckExistAsync(inputDto);
            if (!r)
            {
                commonResult.ResultMsg = " Data already exists.";
                return commonResult;
            }

            var details = await detailRepositoris.GetListWhereAsync($" PartFamilyTypeID = {inputDto.ID}");
            var cr = await familyTypeRepositories.CloneDataAsync(inputDto, details);
            if (!cr)
            {
                commonResult.ResultMsg = " Clone Failed.";
            }
            return commonResult;
        }

        public async Task<CommonResult> DeleteDataAsync(SC_luPartFamilyType inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var beforeT = await familyTypeRepositories.GetSingleOrDefaultAsync(x => x.ID == inputDto.ID);
            if (beforeT is null)
            {
                commonResult.ResultMsg = "no found delete data.";
                return commonResult;
            }
            var details = await detailRepositoris.GetListWhereAsync($" PartFamilyTypeID = {inputDto.ID}");
            var r = await familyTypeRepositories.DeleteDataAsync(beforeT, details);
            if (!r)
            {
                commonResult.ResultMsg = " Delete data failed.";
            }
            return commonResult;
        }

        public async Task<PageResult<SC_luPartFamilyType>> FindWithPagerSearchAsync(SearchPartFamilyTypeInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";
            

            string selectStr = $@"SELECT DISTINCT a.*,ss.Description  StatusDesc
                                FROM dbo.luPartFamilyType a
                                LEFT JOIN dbo.mesPartFamilyTypeDetail b ON b.PartFamilyTypeID = a.ID
                                LEFT JOIN dbo.luPartFamilyTypeDetailDef c ON c.ID = b.PartFamilyTypeDetailDefID
                                JOIN dbo.sysStatus ss ON ss.ID = a.Status
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.Name LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	c.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.PartFamilyTypeName}' = '' OR a.Name = '{search.PartFamilyTypeName}')
                                     AND ( '{search.PartFamilyTypeDesc}' = '' OR	a.Description = '{search.PartFamilyTypeDesc}')
                                     AND ( '{search.DetailValue}' = '' OR	b.Content = '{search.DetailValue}')
                                     AND ( '{search.DetailName}' = '' OR	c.Description = '{search.DetailName}')
                                     AND ( ISNULL({search.Status},0) = 0 OR	  a.Status  = '{search.Status}')
                                    {(search.DetailNameIds.Any() ? $" AND c.ID in ({string.Join(',', search.DetailNameIds)}) " : "")}
                                 )
            ";
            #region sub
            //string selectStr = $@"SELECT * 
            //                    FROM dbo.luPartFamilyType
            //                    WHERE (ID IN (
            //                     SELECT a.PartFamilyTypeID 
            //                     FROM dbo.mesPartFamilyTypeDetail a
            //                     JOIN dbo.luPartFamilyTypeDetailDef b ON b.ID = a.PartFamilyTypeDetailDefID
            //                     WHERE (
            //                                1 = 1 OR a.Content LIKE '%{search.Keywords}%' OR b.Description LIKE '%{search.Keywords}%'
            //                            ) 
            //                            OR 
            //                            (
            //                                 ('{search.DetailValue}' = '' OR a.Content = '{search.DetailValue}') 
            //                                AND ('{search.DetailName}' = '' OR b.Description = '{search.DetailName}')
            //                            )
            //                        )
            //                        OR Name LIKE '%{search.Keywords}%' OR Description LIKE '{search.Keywords}';
            //                    ) 
            //                    OR(
            //                        (ISNULL('{search.Status}', -1) = -1 OR Status = {search.Status}) 
            //                        AND ('{search.PartFamilyTypeName}' = '' OR Name = '{search.PartFamilyTypeName}') 
            //                        AND ('{search.PartFamilyTypeDesc}' = '' OR Description = '{search.PartFamilyTypeDesc}')
            //                    )";


            #endregion

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            //var tmpItems = list.MapTo<SC_luPartFamilyTypeListOutputDto>();

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach( x =>
            {
                //string Sql = $@"SELECT a.*,b.ID DefID, b.Description DefDescription
                //                FROM dbo.mesPartFamilyTypeDetail a
                //                JOIN dbo.luPartFamilyTypeDetailDef b ON b.ID = a.PartFamilyTypeDetailDefID
                //                WHERE a.PartFamilyTypeID = {x.ID} 
                //                AND ('{search.Keywords}' = '' 
                //                        OR	a.Content LIKE '%{search.Keywords}%'
                //                        OR	b.Description LIKE '%{search.Keywords}%')
                //                AND (
                //                        ( '{search.DetailValue}' = '' OR	a.Content = '{search.DetailValue}')
                //                        AND ( '{search.DetailName}' = '' OR	b.Description = '{search.DetailName}')
                //                )";
                //var allChilds = repository.FindWithPagerCustomSql<SC_PartFamilyTypeDetailContentOutputDto>(Sql, pagerInfo, search.Sort, order);

                //x.PartFamilyTypeDetails = allChilds;
                string Sql = $@"SELECT a.*,b.ID DefID, b.Description DefDescription
                                FROM dbo.mesPartFamilyTypeDetail a
                                JOIN dbo.luPartFamilyTypeDetailDef b ON b.ID = a.PartFamilyTypeDetailDefID
                                WHERE a.PartFamilyTypeID = {x.ID} ";
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesPartFamilyTypeDetail>(Sql);

                var childs = allChilds.Where(x => {
                    bool b1 = true; bool b2 = true; bool b3 = true;
                    if (!string.IsNullOrEmpty(search.Keywords))
                    {
                        b1 = x.Content.Contains(search.Keywords) || x.DefDescription.Contains(search.Keywords);
                    }
                    if (!string.IsNullOrEmpty(search.DetailValue))
                    {
                        b2 = x.Content == search.DetailValue;
                    }
                    if (!string.IsNullOrEmpty(search.DetailName))
                    {
                        b3 = x.DefDescription == search.DetailName;
                    }
                    return b1 & b2 & b3;
                });
                x.PartFamilyTypeDetails = childs?.ToList().Count() > 0 ? childs.ToList() : allChilds;
            });

            #endregion


            PageResult<SC_luPartFamilyType> pageResult = new PageResult<SC_luPartFamilyType>
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
