using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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

namespace SunnyMES.WebApi.SysConfig.PO
{
    /// <summary>
    ///工单消费信息
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    //[Obsolete]
    public class SC_mesProductionOrderConsumeInfoController : AreaApiControllerCustom<SC_mesProductionOrderConsumeInfo, SC_mesProductionOrderConsumeInfo, SC_mesProductionOrderConsumeInfo, ISC_mesProductionOrderConsumeInfoServices, string>
    {
        public SC_mesProductionOrderConsumeInfoController(ISC_mesProductionOrderConsumeInfoServices services) : base(services)
        {
            
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesProductionOrderConsumeInfo inInfo)
        {
            CommonResult commonResult = new CommonResult();
            OnBeforeUpdate(inInfo);
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
