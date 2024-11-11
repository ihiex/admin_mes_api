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
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    /// <summary>
    /// 中箱称重
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class BoxScalagePackageController : AreaApiControllerCommon<IBoxScalagePackageServices,string>
    {
        private readonly IBoxScalagePackageServices iService;

        public BoxScalagePackageController(IBoxScalagePackageServices _iService) : base(_iService)
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
        /// 确认工单
        /// </summary>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("SetConfirmPOAsync")]
        [YuebonAuthorize("SetConfirmPOAsync")]
        [CommonAuthorize]
        [AllowAnonymous]
        public async Task<IActionResult> SetConfirmPOAsync([FromQuery] MesInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService?.SetConfirmPOAsync(input);
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
        /// 箱码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MainSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> MainSnVerifyAsync(
            [FromBody] MesSnInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.MainSnVerifyAsync(input);
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
        /// 最终中箱重量提交
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("FinalWeightSubmitAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> FinalWeightSubmitAsync(
            [FromBody] BoxScalageInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.FinalWeightSubmitAsync(input);
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
