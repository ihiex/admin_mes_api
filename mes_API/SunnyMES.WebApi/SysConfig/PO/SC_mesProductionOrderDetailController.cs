using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.WebApi.SysConfig.PO
{
    /// <summary>
    ///工单详细
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesProductionOrderDetailController : AreaApiControllerCustom<SC_mesProductionOrderDetail, SC_mesProductionOrderDetail, SC_mesProductionOrderDetail, ISC_mesProductionOrderDetailServices, string>
    {
        public SC_mesProductionOrderDetailController(ISC_mesProductionOrderDetailServices services) : base(services)
        {
            base.ExistsWhere.Add("ProductionOrderDetailDefID");
            base.ExistsWhere.Add("ProductionOrderID");
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesProductionOrderDetail inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());

            if (beforData.ProductionOrderID != inInfo.ProductionOrderID)
            {
                commonResult = base.FormatNGResult(commonResult, ErrCode.err70002);
                return ToJsonContent(commonResult);
            }
            if (beforData.ProductionOrderDetailDefID != inInfo.ProductionOrderDetailDefID) 
            {
                var whereSql = OutputExtensions.FormartWhere<SC_mesProductionOrderDetail>(inInfo, null, "ID");
                var tmpCount = await iService.GetCountByWhereAsync(whereSql);
                if (tmpCount > 0)
                {
                    commonResult = base.FormatNGResult(commonResult, ErrCode.err70001);
                    return ToJsonContent(commonResult);
                }
            }
            OnBeforeUpdate(inInfo);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
