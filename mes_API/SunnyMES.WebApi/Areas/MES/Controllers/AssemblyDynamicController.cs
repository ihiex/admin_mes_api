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
using SunnyMES.Commons.Core.Dtos.MesInputDtos.AssemblyDynamic;
using System.Linq;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class AssemblyDynamicController : AreaApiControllerCommon<IAssemblyDynamicServices, string>
    {
        private readonly IAssemblyDynamicServices iService;

        public AssemblyDynamicController(IAssemblyDynamicServices _iService) : base(_iService)
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
        /// 动态条码校验
        /// 在Barcode 中传递当前条码，并将IsCurrentItem设置为true,其他则设置为false
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DynamicSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> DynamicSnVerifyAsync(
            [FromBody] AssemblyDynamicInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.DynamicSnVerifyAsync(input);
                commonResult = await FormatNGResultAsync(commonResult, listDyn, input.DataList.Where(x => x.IsCurrentItem).FirstOrDefault()?.Barcode);
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
        /// 释放治具
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ReleaseMachineSNAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> ReleaseMachineSNAsync([FromBody] AssemblyDynamicInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.ReleaseMachineSNAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.DataList.Where(x => x.IsCurrentItem).FirstOrDefault()?.Barcode);
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
