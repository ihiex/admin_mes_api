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
    public class CO_WH_ShipmentEntryNewController : AreaApiControllerCustom<CO_WH_ShipmentEntryNew_T, CO_WH_ShipmentEntryNew_T, CO_WH_ShipmentEntryNew_T, ICO_WH_ShipmentEntryNewServices, string>
    {
        private readonly ICO_WH_ShipmentEntryNewServices iService;

        public CO_WH_ShipmentEntryNewController(ICO_WH_ShipmentEntryNewServices _iService) : base(_iService)
        {
            iService = _iService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public override async Task<IActionResult> UpdateAsync(CO_WH_ShipmentEntryNew_T inInfo)
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
