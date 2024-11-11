using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// 料号详细信息
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesPartDetailController : AreaApiControllerCustom<SC_mesPartDetail, SC_mesPartDetail, SC_mesPartDetail, ISC_mesPartDetailServices, string>
    {
        public SC_mesPartDetailController(ISC_mesPartDetailServices services): base(services)
        {
            
        }


        /// <summary>
        /// 异步更新数据，需要在业务模块控制器重写该方法,否则更新无效
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesPartDetail inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());

            if (beforData.PartID != inInfo.PartID)
            {
                commonResult = base.FormatNGResult(commonResult, ErrCode.err70002);
                return ToJsonContent(commonResult);
            }
            if (beforData.PartDetailDefID != inInfo.PartDetailDefID)
                commonResult = await base.UpdateBeforeCheckAsync(inInfo);

            if (!string.IsNullOrEmpty(commonResult.ResultMsg))
                return ToJsonContent(commonResult);

            await FormatUpdateMsg(beforData, inInfo);
            OnBeforeUpdate(inInfo);

            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

    }
}
