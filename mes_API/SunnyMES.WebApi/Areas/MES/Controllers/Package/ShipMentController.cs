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
using SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent;
using SunnyMES.Security.IServices.MES.Package;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Package
{
    /// <summary>
    /// 出货
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class ShipMentController : AreaApiControllerCommon<IShipMentServices, string>
    {
        private readonly IShipMentServices iService;
        public ShipMentController(IShipMentServices _iService) : base(_iService)
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
        /// 主条码(Bill NO)校验
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
        /// 箱码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MultipackSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> MultipackSnVerifyAsync(
            [FromBody] ShipMentInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.MultipackSnVerifyAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.MultipackSn);
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
        /// 出货单号替换
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ReplaceBillNOAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> ReplaceBillNOAsync(
            [FromBody] ShipMentReplaceInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.ReplaceBillNOAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.OriginalBillNO + "," + input.TargetBillNO);
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
        /// 通过箱码进行补打印
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ReprintSnAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> ReprintSnAsync(
            [FromBody] MesSnInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.ReprintSnAsync(input);
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
        /// 从出货栈板中解绑单个箱码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("RemoveMultipackSnAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> RemoveMultipackSnAsync([FromBody] ShipMentInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.RemoveMultipackSnAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.MultipackSn);
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
        /// 将整个出货栈板解除关联
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UnpackShipmentPalletAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> UnpackShipmentPalletAsync([FromBody] ShipMentInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.UnpackShipmentPalletAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.ShippingPallet);
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
