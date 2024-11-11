using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// 不良代码组
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_luDefectTypeController : AreaApiControllerCustom<SC_luDefectType, SC_luDefectType, SC_luDefectType, ISC_luDefectTypeServices, string>
    {
        public SC_luDefectTypeController(ISC_luDefectTypeServices _iService) : base(_iService)
        {

        }

        /// <summary>
        /// 异步更新数据，需要在业务模块控制器重写该方法,否则更新无效
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_luDefectType inInfo)
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
