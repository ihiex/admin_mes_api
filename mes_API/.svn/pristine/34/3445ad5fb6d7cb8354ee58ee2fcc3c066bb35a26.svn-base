using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Reflection;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;
using SunnyMES.Security.IServices;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Link
{
    /// <summary>
    /// SN link UPC
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class SNLinkUPCController : AreaApiControllerCommon<ISNLinkUPCServices, string>
    {
        private readonly ISNLinkUPCServices iServices;

        public SNLinkUPCController(ISNLinkUPCServices iServices) : base(iServices)
        {
            this.iServices = iServices;
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
        public async Task<IActionResult> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService?.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID,
                    S_POID, S_UnitStatus, S_URL);
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
        /// 主条码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("MainSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> MainSnVerifyAsync(
            [FromBody] SNLinkUPCInput input)
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
        /// 动态条码校验
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DynamicSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> DynamicSnVerifyAsync(
            [FromBody] SNLinkUPCInput input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.DynamicSnVerifyAsync(input);

                string sns = string.Empty;
                foreach (var itemsDto in input.DataList)
                {
                    if (itemsDto.Value.IsEnable == false)
                        continue;
                    if (string.IsNullOrEmpty(itemsDto.Value.Value))
                        continue;
                    sns += $"name : {itemsDto.Value.Description}, value : {itemsDto.Value.Value}; ";
                }
                commonResult = await FormatResultAsync(commonResult, listDyn, sns);
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
