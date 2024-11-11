using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.IServices;
using Microsoft.AspNetCore.Authorization;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Models;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Log;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Link
{
    /// <summary>
    /// 新旧治具绑定
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class ToolingLinkToolingController : AreaApiControllerCommon<IToolingLinkToolingServices, string>
    {
        private readonly IToolingLinkToolingServices _iToolingLinkToolingServices;

        public ToolingLinkToolingController(IToolingLinkToolingServices iToolingLinkToolingServices) : base(iToolingLinkToolingServices)
        {
            _iToolingLinkToolingServices = iToolingLinkToolingServices;
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
                _iToolingLinkToolingServices?.GetConfInfo(commonHeader);
                var listDyn = await _iToolingLinkToolingServices?.GetPageInitializeAsync(S_URL);
                commonResult = await FormatResultAsync(commonResult, listDyn);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
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
        public async Task<IActionResult> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iToolingLinkToolingServices?.GetConfInfo(commonHeader);
                var listDyn = await _iToolingLinkToolingServices?.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID,
                    S_POID, S_UnitStatus, S_URL);
                commonResult = await FormatResultAsync(commonResult, listDyn);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 新治具条码输入校验
        /// </summary>
        /// <param name="newToolingInput"></param>
        /// <returns></returns>
        [HttpPost("NewToolingSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> NewToolingSnVerifyAsync(
            [FromBody] ToolingLinkTooling_NewTooling_Input newToolingInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iToolingLinkToolingServices?.GetConfInfo(commonHeader);
                var listDyn = await _iToolingLinkToolingServices.NewToolingSnVerifyAsync(newToolingInput);
                commonResult = await FormatResultAsync(commonResult, listDyn, newToolingInput.S_NewToolingSN);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }
        /// <summary>
        /// 旧治具条码校验
        /// </summary>
        /// <param name="oldToolingInput"></param>
        /// <returns></returns>
        [HttpPost("OldToolingSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> OldToolingSnVerifyAsync(
            [FromBody] ToolingLinkTooling_OldTooling_Input oldToolingInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iToolingLinkToolingServices?.GetConfInfo(commonHeader);
                var listDyn = await _iToolingLinkToolingServices.OldToolingSnVerifyAsync(oldToolingInput);
                commonResult = await FormatResultAsync(commonResult, listDyn, $"{oldToolingInput.S_NewToolingSN},{oldToolingInput.S_OldToolingSN}");
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 解除旧治具条码锁定状态
        /// </summary>
        /// <param name="oldToolingInput"></param>
        /// <returns></returns>
        [HttpPost("OldToolingSnReleaseAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> OldToolingSnReleaseAsync(
            [FromBody] ToolingLinkTooling_OldTooling_Input oldToolingInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iToolingLinkToolingServices?.GetConfInfo(commonHeader);
                var listDyn = await _iToolingLinkToolingServices.OldToolingSnReleaseAsync(oldToolingInput);
                commonResult = await FormatResultAsync(commonResult, listDyn, oldToolingInput.S_OldToolingSN);
            }
            catch (Exception e)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, e);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }

            return ToJsonContent(commonResult);
        }
    }
}
