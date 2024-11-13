using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.IServices.MES.SAP;
using SunnyMES.Security.Models;

namespace SunnyMES.WebApi.Areas.MES.Controllers.SAP
{
    /// <summary>
    /// 接收到SAP的出货数据
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    [Obsolete]
    public class CO_WH_ShipmentNewController : AreaApiControllerCustom<CO_WH_ShipmentNew_T, CO_WH_ShipmentNew_T, CO_WH_ShipmentNew_T, ICO_WH_ShipmentNewServices, string>
    {
        private readonly ICO_WH_ShipmentNewServices iService;

        public CO_WH_ShipmentNewController(ICO_WH_ShipmentNewServices _iService) : base(_iService)
        {
            iService = _iService;
        }



        [HttpPost("GetListSapByHAWBAsync")]
        [YuebonAuthorize("")]
        [Obsolete]
        public async Task<IActionResult> GetListSapByHAWBAsync(SearchShipmentNewDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.GetListSapByHAWBAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        [HttpPost("GetListSapByShippingTimeAsync")]
        [YuebonAuthorize("")]
        [Obsolete]
        public async Task<IActionResult> GetListSapByShippingTimeAsync(SearchShipmentNewDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.GetListSapByShippingTimeAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public override async Task<IActionResult> UpdateAsync(CO_WH_ShipmentNew_T inInfo)
        {

            CommonResult commonResult = new CommonResult();
            try
            {
                base.OnBeforeUpdate(inInfo);
                var listDyn = await iService?.UpdateAsync(inInfo, inInfo.FInterID.ToString());
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
    }
}
