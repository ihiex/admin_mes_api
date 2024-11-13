using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// 料号组类别属性
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_luPartFamilyTypeDetailDefController : AreaApiControllerCustom<SC_luPartFamilyTypeDetailDef, SC_luPartFamilyTypeDetailDef, SC_luPartFamilyTypeDetailDef, ISC_luPartFamilyTypeDetailDefServices, string>
    {
        public SC_luPartFamilyTypeDetailDefController(ISC_luPartFamilyTypeDetailDefServices _iService) : base(_iService)
        {
        }

        /// <summary>
        /// 异步更新数据，需要在业务模块控制器重写该方法,否则更新无效
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_luPartFamilyTypeDetailDef inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var isExists = await base.IsExistsT(inInfo);
            if (isExists)
                return ToJsonContent(base.FormatNGResult(commonResult, ShowMsg(ErrCode.err70001)));
            OnBeforeUpdate(inInfo);
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
