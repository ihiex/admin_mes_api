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
using SunnyMES.Security.IServices;
using SunnyMES.Security.IServices.MES.ProjectBase;
using SunnyMES.Security.Models.MES;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Extensions;
using API_MSG;

namespace SunnyMES.WebApi.Areas.MES.Controllers.ProjectBase
{
    /// <summary>
    /// 泰国仓库出货参数信息
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class CO_WH_ProjectBaseNewController : AreaApiControllerCustom<CO_WH_ProjectBaseNew, CO_WH_ProjectBaseNew, CO_WH_ProjectBaseNew, ICO_WH_ProjectBaseNewServices, string>
    {
        private readonly ICO_WH_ProjectBaseNewServices iService;
        private readonly ILogService logService;

        public CO_WH_ProjectBaseNewController(ICO_WH_ProjectBaseNewServices _iService, ILogService logService) : base(_iService, logService)
        {
            iService = _iService;
            this.logService = logService;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public override async Task<IActionResult> UpdateAsync(CO_WH_ProjectBaseNew inInfo)
        {

            CommonResult commonResult = new CommonResult();
            try
            {
                base.OnBeforeUpdate(inInfo);
                var listDyn = await iService?.UpdateAsync(inInfo, "FID");
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


        public override async Task<IActionResult> InsertAsync(CO_WH_ProjectBaseNew tinfo)
        {
            CommonResult result = new CommonResult();
            //CO_WH_ProjectBaseNew info = tinfo.MapTo<CO_WH_ProjectBaseNew>();
            OnBeforeInsert(tinfo);

            long ln = await base.iService.InsertAsync(tinfo).ConfigureAwait(false);

            if (ln > 0)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                MSG_Public mp = new MSG_Public(P_Language);
                switch (ln)
                {
                    case -1:
                        result.ResultMsg = mp.MSG_Public_6051;
                        break;
                    case -2:
                        result.ResultMsg = mp.MSG_Public_6052;
                        break;
                    case -3:
                        result.ResultMsg = mp.MSG_Public_6053;
                        break;
                    case -4:
                        result.ResultMsg = mp.MSG_Public_6054;
                        break;
                    case -5:
                        result.ResultMsg = mp.MSG_Public_6055;
                        break;
                    default:
                        result.ResultMsg = ErrCode.err43001;
                        break;
                }
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
            }
            return ToJsonContent(result);
        }

    }
}
