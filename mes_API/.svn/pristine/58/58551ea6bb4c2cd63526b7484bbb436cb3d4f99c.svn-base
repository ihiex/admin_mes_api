using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading.Tasks;
using System;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.IServices.MES.Package;
using SunnyMES.Security.Dtos.MES;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Package
{
    /// <summary>
    /// OOBA print and rework 
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class OOBAController : AreaApiControllerCommon<IOOBAServices, string>
    {
        private readonly IOOBAServices iService;

        public OOBAController(IOOBAServices _iService) : base(_iService)
        {
            iService = _iService;
        }

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("GetPageInitializeAsync")]
        [YuebonAuthorize("GetPageInitializeAsync")]
        [CommonAuthorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetPageInitializeAsync(string S_URL)
        {

            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService?.GetPageInitializeAsync(S_URL);
                commonResult = await FormatResultAsync(commonResult, listDyn);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 箱码检查，并输出打印内容
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("BoxSnCheckAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> BoxSnCheckAsync(
            [FromBody] MesSnInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.BoxSnCheckAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.S_SN);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }
        /// <summary>
        /// 箱码返工
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("BoxSnReworkAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> BoxSnReworkAsync(
            [FromBody] MesSnInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.BoxSnReworkAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.S_SN);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }

    }
}
