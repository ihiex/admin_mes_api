using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Models;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Services;
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
    ///工单分线
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesLineOrderController : AreaApiControllerCustom<SC_mesLineOrder, SC_mesLineOrder, SC_mesLineOrder, ISC_mesLineOrderServices, string>
    {

        public SC_mesLineOrderController(ISC_mesLineOrderServices services) : base(services)
        {
            base.ExistsWhere = new System.Collections.Generic.List<string> { "Description", "LineID", "ProductionOrderID" };
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesLineOrder inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            if (beforData.ProductionOrderID != inInfo.ProductionOrderID)
            {
                commonResult = base.FormatNGResult(commonResult, ShowMsg(ErrCode.err70002));
                return ToJsonContent(commonResult);
            }
            if (beforData.LineID != inInfo.LineID)
                commonResult = await base.UpdateBeforeCheckAsync(inInfo);

            if (!string.IsNullOrEmpty(commonResult.ResultMsg))
                return ToJsonContent(commonResult);

            OnBeforeUpdate(inInfo);
            await FormatUpdateMsg(beforData, inInfo);

            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
