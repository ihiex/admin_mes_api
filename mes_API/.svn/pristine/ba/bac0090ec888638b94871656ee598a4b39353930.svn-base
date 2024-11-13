using NPOI.SS.Formula.Functions;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.IRepositories.PO;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Services.PO
{
    public class SC_mesProductionOrderServices : BaseCustomService<SC_mesProductionOrder, SC_mesProductionOrder, string>, ISC_mesProductionOrderServices
    {
        private readonly ISC_mesProductionOrderRepositories productionOrderRepositories;
        private readonly ISC_mesProductionOrderDetailRepositories detailRepositories;
        private readonly ISC_mesLineOrderRepositories lineOrderRepositories;

        public SC_mesProductionOrderServices(ISC_mesProductionOrderRepositories productionOrderRepositories, ISC_mesProductionOrderDetailRepositories detailRepositories, ISC_mesLineOrderRepositories lineOrderRepositories) : base(productionOrderRepositories)
        {
            this.productionOrderRepositories = productionOrderRepositories;
            this.detailRepositories = detailRepositories;
            this.lineOrderRepositories = lineOrderRepositories;
        }

        public async Task<CommonResult> CloneDataAsync(SC_mesProductionOrder inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var r = await productionOrderRepositories.CheckExistAsync(inputDto);
            if (!r)
            {
                commonResult.ResultMsg = " Data already exists.";
                return commonResult;
            }

            var details = await detailRepositories.GetListWhereAsync($" ProductionOrderID = {inputDto.ID}");
            var lineOrders = await lineOrderRepositories.GetListWhereAsync($" ProductionOrderID = {inputDto.ID}");
            var cr = await productionOrderRepositories.CloneDataAsync(inputDto, details, lineOrders);
            if (!cr)
            {
                commonResult.ResultMsg = " Clone Failed.";
            }
            return commonResult;
        }

        public async Task<CommonResult> DeleteDataAsync(SC_mesProductionOrder inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var beforeT = await productionOrderRepositories.GetSingleOrDefaultAsync(x => x.ID == inputDto.ID);
            if (beforeT is null)
            {
                commonResult.ResultMsg = "no found delete data.";
                return commonResult;
            }
            var details = await detailRepositories.GetListWhereAsync($" ProductionOrderID = {inputDto.ID}");
            var lineOrders = await lineOrderRepositories.GetListWhereAsync($" ProductionOrderID = {inputDto.ID}");
            var r = await productionOrderRepositories.DeleteDataAsync(beforeT, details, lineOrders);
            if (!r)
            {
                commonResult.ResultMsg = " Delete data failed.";
            }
            return commonResult;
        }

        public async Task<PageResult<SC_mesProductionOrder>> FindWithPagerSearchAsync(SearchPOInputDto search)
        {

            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT DISTINCT a.*, ss.Description StatusDesc, pp.PartNumber PartNumber , me.Lastname + '.' + me.Firstname EmployeeName,pp.PartFamilyID PartFamilyID, pf.PartFamilyTypeID PartFamilyTypeID
                                FROM dbo.mesProductionOrder a
                                LEFT JOIN dbo.mesProductionOrderDetail b ON b.ProductionOrderID = a.ID
                                LEFT JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
                                LEFT JOIN dbo.luProductionOrderStatus ss ON ss.ID = a.StatusID
                                LEFT JOIN dbo.mesPart pp ON pp.ID = a.PartID
								LEFT JOIN dbo.mesEmployee me ON me.ID = a.EmployeeID
								LEFT JOIN dbo.mesLineOrder lo ON lo.ProductionOrderID = a.ID
								LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								LEFT JOIN dbo.luPartFamily pf ON pf.ID = pp.PartFamilyID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.ProductionOrderNumber LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
									 OR	c.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	pp.PartNumber LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.POName}' = '' OR a.ProductionOrderNumber = '{search.POName}')
										AND ('{search.PODesc}' = '' OR a.Description = '{search.PODesc}')
										AND ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
										AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
										AND (ISNULL({search.Status}, 0) = 0 OR ss.ID  = {search.Status})
                                        {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}
                                        {(search.PartIds.Any() ? $" AND a.PartID in ({string.Join(',', search.PartIds)}) " : "")}
								 )
            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach(x =>
            {              
                string Sql = $@"
                            IF EXISTS(SELECT b.*, c.ID DefID, c.Description DefDescription
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'
			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
		                            )
                            BEGIN
                                SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'
			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
                            END
                            ELSE
                            BEGIN
                                    SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
                            END
                            ";
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesProductionOrderDetail>(Sql);
                x.productionOrderDetails = allChilds;
            });

            #endregion


            PageResult<SC_mesProductionOrder> pageResult = new PageResult<SC_mesProductionOrder>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,

            };
            return pageResult;
        }

        public async Task<PageResult<SC_mesProductionOrder>> FindWithLinePagerSearchAsync(SearchPOInputDto search)
        {

            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT DISTINCT a.*, ss.Description StatusDesc, pp.PartNumber PartNumber , me.Lastname + '.' + me.Firstname EmployeeName,pp.PartFamilyID PartFamilyID, pf.PartFamilyTypeID PartFamilyTypeID
                                FROM dbo.mesProductionOrder a
                                LEFT JOIN dbo.mesProductionOrderDetail b ON b.ProductionOrderID = a.ID
                                LEFT JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
                                LEFT JOIN dbo.luProductionOrderStatus ss ON ss.ID = a.StatusID
                                LEFT JOIN dbo.mesPart pp ON pp.ID = a.PartID
								LEFT JOIN dbo.mesEmployee me ON me.ID = a.EmployeeID
								LEFT JOIN dbo.mesLineOrder lo ON lo.ProductionOrderID = a.ID
								LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								LEFT JOIN dbo.luPartFamily pf ON pf.ID = pp.PartFamilyID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.ProductionOrderNumber LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
									 OR	c.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	pp.PartNumber LIKE '%{search.Keywords}%'
                                     OR	lo.Description LIKE '%{search.Keywords}%'
                                     OR	ml.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.POName}' = '' OR a.ProductionOrderNumber = '{search.POName}')
										AND ('{search.PODesc}' = '' OR a.Description = '{search.PODesc}')
										AND ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
										AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
										AND (ISNULL({search.Status}, 0) = 0 OR ss.ID  = {search.Status})
										AND ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                        {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}
                                        {(search.PartIds.Any() ? $" AND a.PartID in ({string.Join(',', search.PartIds)}) " : "")}
                                        {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
								 )
            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach(x =>
            {
                #region 详细属性值
                string Sql = $@"
                            IF EXISTS(SELECT b.*, c.ID DefID, c.Description DefDescription
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
		                            )
                            BEGIN
                                SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'
			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
                            END
                            ELSE
                            BEGIN
                                    SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
                            END
                            ";
                #endregion
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesProductionOrderDetail>(Sql);
                x.productionOrderDetails = allChilds;
                #region 分线详细
                string SqlLine = $@"
                            IF EXISTS(SELECT lo.*, ml.Description LineName
								    FROM dbo.mesLineOrder lo
								    LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								    WHERE lo.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
                                             OR	lo.Description LIKE '%{search.Keywords}%'
                                             OR	ml.Description LIKE '%{search.Keywords}%'
			                            )
			                            AND (
										     ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										    AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                             {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
			                            )
		                            )
                            BEGIN
								SELECT lo.*, ml.Description LineName
								FROM dbo.mesLineOrder lo
								LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								WHERE lo.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
                                             OR	lo.Description LIKE '%{search.Keywords}%'
                                             OR	ml.Description LIKE '%{search.Keywords}%'
			                            )
			                            AND (
										     ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										    AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                             {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
			                            )
                            END
                            ELSE
                            BEGIN
                                    SELECT lo.*, ml.Description LineName
								    FROM dbo.mesLineOrder lo
								    LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								    WHERE lo.ProductionOrderID = {x.ID}
                            END
                            ";
                #endregion

                var allLineChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesLineOrder>(SqlLine);
                x.productionOrderLines = allLineChilds;
            });

            #endregion


            PageResult<SC_mesProductionOrder> pageResult = new PageResult<SC_mesProductionOrder>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,

            };
            return pageResult;
        }

        public async Task<CommonResult> InsertPMCAsync(PMCPOInsertInputDto entity )
        {
            return await productionOrderRepositories.InsertPMCAsync(entity);
        }

        public async Task<PageResult<SC_mesProductionOrder>> FindWithLinePagerSearchPMCAsync(SearchPOInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT DISTINCT a.*, ss.Description StatusDesc, pp.PartNumber PartNumber , me.Lastname + '.' + me.Firstname EmployeeName,pp.PartFamilyID PartFamilyID, pf.PartFamilyTypeID PartFamilyTypeID
                                FROM dbo.mesProductionOrder a
                                LEFT JOIN dbo.mesProductionOrderDetail b ON b.ProductionOrderID = a.ID
                                LEFT JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
                                LEFT JOIN dbo.luProductionOrderStatus ss ON ss.ID = a.StatusID
                                LEFT JOIN dbo.mesPart pp ON pp.ID = a.PartID
								LEFT JOIN dbo.mesEmployee me ON me.ID = a.EmployeeID
								LEFT JOIN dbo.mesLineOrder lo ON lo.ProductionOrderID = a.ID
								LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								LEFT JOIN dbo.luPartFamily pf ON pf.ID = pp.PartFamilyID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.ProductionOrderNumber LIKE '%{search.Keywords}%'
                                     OR	a.Description LIKE '%{search.Keywords}%'
									 OR	c.Description LIKE '%{search.Keywords}%'
                                     OR	b.Content LIKE '%{search.Keywords}%'
                                     OR	pp.PartNumber LIKE '%{search.Keywords}%'
                                     OR	lo.Description LIKE '%{search.Keywords}%'
                                     OR	ml.Description LIKE '%{search.Keywords}%'
                                        )
                                 AND (
                                        ( '{search.POName}' = '' OR a.ProductionOrderNumber = '{search.POName}')
										AND ('{search.PODesc}' = '' OR a.Description = '{search.PODesc}')
										AND ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
										AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
										AND (ISNULL({search.Status}, 0) = 0 OR ss.ID  = {search.Status})
										AND ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                        {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}
                                        {(search.PartIds.Any() ? $" AND a.PartID in ({string.Join(',', search.PartIds)}) " : "")}
                                        {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
								 )
            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);

            #region 通过主ID查询所有属性后，再根据条件进行筛选
            list.ForEach(x =>
            {
                #region 详细属性值
                string Sql = $@"
                            IF EXISTS(SELECT b.*, c.ID DefID, c.Description DefDescription
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
		                            )
                            BEGIN
                                SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
				                            OR	c.Description LIKE '%{search.Keywords}%'
                                            OR	b.Content LIKE '%{search.Keywords}%'
			                            )
			                            AND (
				                            ('{search.DetailName}' = '' OR c.Description = '{search.DetailName}')
				                            AND ('{search.DetailValue}' = '' OR b.Content = '{search.DetailValue}')
				                            {(search.DetailNameIds.Any() ? $" AND b.ProductionOrderDetailDefID in ({string.Join(',', search.DetailNameIds)}) " : "")}

			                            )
                            END
                            ELSE
                            BEGIN
                                    SELECT b.*, c.ID DefID, c.Description DefDescription 
			                            FROM dbo.mesProductionOrderDetail b
			                            JOIN dbo.luProductionOrderDetailDef c ON c.ID = b.ProductionOrderDetailDefID
			                            WHERE b.ProductionOrderID = {x.ID}
                            END
                            ";
                #endregion
                var allChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesProductionOrderDetail>(Sql);
                string pmcParas = Configs.GetConfigurationValue("AppSetting", "PMC_Parameter");
                var checkChilds = allChilds.Where(x => pmcParas.Contains(x.DefDescription));
                x.productionOrderDetails = checkChilds?.ToList();

                #region 分线详细
                string SqlLine = $@"
                            IF EXISTS(SELECT lo.*, ml.Description LineName
								    FROM dbo.mesLineOrder lo
								    LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								    WHERE lo.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
                                             OR	lo.Description LIKE '%{search.Keywords}%'
                                             OR	ml.Description LIKE '%{search.Keywords}%'
			                            )
			                            AND (
										     ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										    AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                             {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
			                            )
		                            )
                            BEGIN
								SELECT lo.*, ml.Description LineName
								FROM dbo.mesLineOrder lo
								LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								WHERE lo.ProductionOrderID = {x.ID}
			                            AND (
				                            '{search.Keywords}' = ''
                                             OR	lo.Description LIKE '%{search.Keywords}%'
                                             OR	ml.Description LIKE '%{search.Keywords}%'
			                            )
			                            AND (
										     ('{search.PoLineName}' = '' OR lo.Description = '{search.PoLineName}')
										    AND ('{search.LineName}' = '' OR ml.Description = '{search.LineName}')
                                             {(search.LineIds.Any() ? $" AND lo.LineID in ({string.Join(',', search.LineIds)}) " : "")}
			                            )
                            END
                            ELSE
                            BEGIN
                                    SELECT lo.*, ml.Description LineName
								    FROM dbo.mesLineOrder lo
								    LEFT JOIN dbo.mesLine ml ON ml.ID = lo.LineID
								    WHERE lo.ProductionOrderID = {x.ID}
                            END
                            ";
                #endregion

                var allLineChilds = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesLineOrder>(SqlLine);
                x.productionOrderLines = allLineChilds;
            });

            #endregion


            PageResult<SC_mesProductionOrder> pageResult = new PageResult<SC_mesProductionOrder>
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
