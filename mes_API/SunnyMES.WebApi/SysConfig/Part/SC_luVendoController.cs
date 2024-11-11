using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// 供应商
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_luVendoController : AreaApiControllerCustom<SC_luVendo, SC_luVendo, SC_luVendo, ISC_luVendoServices, string>
    {
        public SC_luVendoController(ISC_luVendoServices _iService) : base(_iService)
        {
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_luVendo inInfo)
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

        /// <summary>
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchVendoInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerSearchAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
