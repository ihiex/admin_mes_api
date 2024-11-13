using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.IServices.MES.SAP;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Log;
using SunnyMES.Security.Models.MES;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;

namespace SunnyMES.WebApi.Areas.MES.Controllers.SAP
{
    /// <summary>
    /// 接收到SAP的出货数据
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class TmpExcelShipmentNewController : AreaApiControllerCustom<tmpExcelShipmentNew, tmpExcelShipmentNew, tmpExcelShipmentNew, ITmpExcelShipmentNewServices, string>
    {
        private readonly ITmpExcelShipmentNewServices iService;

        public TmpExcelShipmentNewController(ITmpExcelShipmentNewServices _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 根据HAWB查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("GetListSapByHAWBAsync")]
        [YuebonAuthorize("")]
        [Obsolete]
        public async Task<IActionResult> GetListSapByHAWBAsync(SearchSAPDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.GetListSapByHAWBAsync(search);
            commonResult =  base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 根据时间查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("GetListSapByShippingTimeAsync")]
        [YuebonAuthorize("")]
        [Obsolete]
        public async Task<IActionResult> GetListSapByShippingTimeAsync(SearchSAPDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.GetListSapByShippingTimeAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        [Obsolete]
        public override async Task<IActionResult> UpdateAsync(tmpExcelShipmentNew inInfo)
        {

            CommonResult commonResult = new CommonResult();
            try
            {
                base.OnBeforeUpdate(inInfo);
                var listDyn = await iService?.UpdateAsync(inInfo, inInfo.NO.ToString());
                commonResult = FormatOKResult(commonResult, null);
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
        /// 根据条码模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerLikeAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerLikeAsync(SearchSAPDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerLikeAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 获取SAP数据状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSAPDataState")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPDataState()
        {
            CommonResult commonResult = new CommonResult();
            var vList = ExtEnum.EnumToList(typeof(SAPDataState), e => P_Language == 0 ? e.ToString() : e.GetDescription());
            commonResult = base.FormatOKResult(commonResult, vList);


            //var dics = ExtEnum.EnumToDictionary(typeof(SAPDataState),e => P_Language == 0? e.ToString() : e.GetDescription());
            //commonResult = base.FormatOKResult(commonResult, dics);
            return ToJsonContent(commonResult);
        }
    }
}
