using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES.CarrierLinkMaterialBatch;
using SunnyMES.Security.IServices;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;

namespace SunnyMES.WebApi.Areas.MES.Controllers.Link
{
    /// <summary>
    /// 托盘绑定批次
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class CarrierLinkMaterialBatchController : AreaApiControllerCommon<ICarrierLinkMaterialBatchServices, string>
    {
        private readonly ICarrierLinkMaterialBatchServices _iServices;

        public CarrierLinkMaterialBatchController(ICarrierLinkMaterialBatchServices iServices) : base(iServices)
        {
            _iServices = iServices;
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
                _iServices.GetConfInfo(commonHeader);
                var listDyn = await _iServices.GetPageInitializeAsync(S_URL);
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
                _iServices.GetConfInfo(commonHeader);
                var listDyn = await _iServices.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID,
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
        /// 托盘条码校验
        /// </summary>
        /// <param name="carrierSNInput"></param>
        /// <returns></returns>
        [HttpPost("CarrierSnVerifyAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        public async Task<IActionResult> CarrierSnVerifyAsync([FromBody] CarrierLinkMaterialBatch_CarrierSN_Input carrierSNInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iServices.GetConfInfo(commonHeader);
                var listDyn = await _iServices.CarrierSnVerifyAsync(carrierSNInput);
                commonResult = await FormatResultAsync(commonResult, listDyn, carrierSNInput.S_CarrierSN);
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
        /// 批次号校验
        /// </summary>
        /// <param name="batchNumberInput"></param>
        /// <returns></returns>
        [HttpPost("BatchNumberVerifyAsync")]
        [CommonAuthorize]
        [YuebonAuthorize("")]
        public async Task<IActionResult> BatchNumberVerifyAsync([FromBody] CarrierLinkMaterialBatch_BN_Input batchNumberInput)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                _iServices.GetConfInfo(commonHeader);
                var listDyn = await _iServices.BatchNumberVerifyAsync(batchNumberInput);

                commonResult = await FormatResultAsync(commonResult, listDyn, batchNumberInput.S_BatchNumber);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, throwMsg, ex);
                commonResult.ResultMsg = ErrCode.err40110;
                commonResult.ResultCode = "40110";
            }
            return ToJsonContent(commonResult);
        }
    }
}
