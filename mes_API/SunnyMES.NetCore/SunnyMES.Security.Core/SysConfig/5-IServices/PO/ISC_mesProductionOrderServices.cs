using NPOI.SS.Formula.Functions;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IServices.PO
{
    public interface ISC_mesProductionOrderServices : ICustomService<SC_mesProductionOrder, SC_mesProductionOrder, string>
    {
        /// <summary>
        /// 查询工单及对应的详细属性
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<PageResult<SC_mesProductionOrder>> FindWithPagerSearchAsync(SearchPOInputDto search);
        /// <summary>
        /// 查询工单和对应的详细属性及分线信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<PageResult<SC_mesProductionOrder>> FindWithLinePagerSearchAsync(SearchPOInputDto search);
        Task<PageResult<SC_mesProductionOrder>> FindWithLinePagerSearchPMCAsync(SearchPOInputDto search);
        Task<CommonResult> CloneDataAsync(SC_mesProductionOrder inputDto);
        Task<CommonResult> DeleteDataAsync(SC_mesProductionOrder inputDto);

        /// <summary>
        /// 异步新增PMC实体。
        /// 自动添加分线及特殊属性
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<CommonResult> InsertPMCAsync(PMCPOInsertInputDto entity);
    }
}
