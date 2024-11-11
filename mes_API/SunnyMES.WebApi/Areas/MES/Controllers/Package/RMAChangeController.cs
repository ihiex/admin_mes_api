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
    /// RMA 返工
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class RMAChangeController : AreaApiControllerCommon<IRMAChangeServices, string>
    {
        private readonly IRMAChangeServices iService;

        public RMAChangeController(IRMAChangeServices _iService) : base(_iService)
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
        /// 条码检查并修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("SnCheckAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public async Task<IActionResult> SnCheckAsync(
            [FromBody] MesSnInputDto input)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                iService?.GetConfInfo(commonHeader);
                var listDyn = await iService.SnCheckAsync(input);
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
