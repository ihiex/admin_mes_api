using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons.Core.Dtos;

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class PackageOverStationController : AreaApiControllerCommon<IPackageOverStationServices, string>
    {
        private readonly IPackageOverStationServices _iService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iService"></param>
        public PackageOverStationController(IPackageOverStationServices iService) : base(iService)
        {
            this._iService = iService;
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <param name="S_URL">配置链接</param>
        /// <returns></returns>
        [HttpGet("GetPageInitializeAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPageInitializeAsync(string S_URL)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iService.GetConfInfo(commonHeader);
                var listDyn = await _iService.GetPageInitializeAsync(S_URL);
                commonResult = await FormatResultAsync(commonResult, listDyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 确认PO
        /// </summary>
        /// <param name="S_PartFamilyTypeID">料号组类型ID</param>
        /// <param name="S_PartFamilyID">料号组ID</param>
        /// <param name="S_PartID">料号ID</param>
        /// <param name="S_POID">POID</param>
        /// <param name="S_UnitStatus">产品状态</param>
        /// <param name="S_URL">配置链接</param>
        /// <returns></returns>
        [HttpGet("SetConfirmPOAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        public async Task<IActionResult> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID,
            string S_POID, string S_UnitStatus, string S_URL)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iService.GetConfInfo(commonHeader);
                var listDyn = await _iService.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID,
                    S_UnitStatus, S_URL);

                commonResult = await FormatResultAsync(commonResult, listDyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 栈板条码校验
        /// </summary>
        /// <param name="palletSnInput">栈板条码及相关参数输入</param>
        /// <returns></returns>
        [HttpPost("PalletSnVerifyAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        public async Task<IActionResult> PalletSnVerifyAsync([FromBody]PackageOverStation_PalletSn_Input palletSnInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iService.GetConfInfo(commonHeader);
                await _iService.GetPageInitializeAsync(palletSnInput.S_URL);

                var listDyn = await _iService.PalletSnVerifyAsync(palletSnInput);
                commonResult = await FormatResultAsync(commonResult, listDyn,palletSnInput.S_PalletSN);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 箱码检查
        /// </summary>
        /// <param name="boxSnInput">中箱条码及相关参数输入</param>
        /// <returns></returns>
        [HttpPost("BoxSnVerifyAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        public async Task<IActionResult> BoxSnVerifyAsync([FromBody] PackageOverStation_BoxSn_Input boxSnInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iService.GetConfInfo(commonHeader);
                var listDyn = await _iService.BoxSnVerifyAsync(boxSnInput);

                commonResult = await FormatResultAsync(commonResult, listDyn,boxSnInput.S_BoxSN);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }
            return ToJsonContent(commonResult);
        }

        ///// <summary>
        ///// test query
        ///// </summary>
        ///// <param name="inputDto">abc</param>
        ///// <returns></returns>
        //[HttpPost("TestQuery")]
        //[CommonAuthorize]
        //[YuebonAuthorize("")]
        //public async Task<IActionResult> TestQuery([FromQuery] PackageOverstationInputDto inputDto)
        //{
        //    CommonResult commonResult = new CommonResult();
        //    try
        //    {
        //        return ToJsonContent(new CommonResult());
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
        //        commonResult.ResultMsg = ErrCode.err40110;
        //        commonResult.ResultCode = "40110";
        //    }
        //    return ToJsonContent(commonResult);
        //}

        //[HttpPost("TestBody")]
        //public async Task<IActionResult> TestBody([FromBody] PackageOverstationInputDto inputDto)
        //{
        //    CommonResult commonResult = new CommonResult();
        //    try
        //    {
        //        return ToJsonContent(new CommonResult());
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
        //        commonResult.ResultMsg = ErrCode.err40110;
        //        commonResult.ResultCode = "40110";
        //    }
        //    return ToJsonContent(commonResult);
        //}
    }
}
