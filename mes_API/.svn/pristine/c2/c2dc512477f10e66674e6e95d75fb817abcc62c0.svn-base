using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES.SNLinkBatch;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Services;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;
using Senparc.NeuChar.Entities;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Link
{
    /// <summary>
    /// 序列号关联批次号
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class SNLinkBatchController : AreaApiControllerCommon<ISNLinkBatchServices, string>
    {
        private readonly ISNLinkBatchServices iService;

        public SNLinkBatchController(ISNLinkBatchServices _iService) : base(_iService)
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
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService?.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID,
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
        /// 批次号输入校验
        /// </summary>
        /// <param name="newToolingInput"></param>
        /// <returns></returns>
        [HttpPost("BatchSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> BatchSnVerifyAsync(
            [FromBody] SNLinkBatch_BSN_Input input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.BatchSnVerifyAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, input.S_BatchNumber);
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
        /// 关联序列号输入校验
        /// </summary>
        /// <param name="oldToolingInput"></param>
        /// <returns></returns>
        [HttpPost("LinkSnVerifyAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> LinkSnVerifyAsync(
            [FromBody] SNLinkBatch_SN_Input input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.LinkSnVerifyAsync(input);
                commonResult = await FormatResultAsync(commonResult, listDyn, $"{input.S_BatchNumber},{input.S_SN}");
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
