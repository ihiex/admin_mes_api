using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Json;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Security.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  SNFormat 可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class SNFormatController :AreaApiControllerGeneric<TabVal,TabVal,TabVal, ISNFormatService,string>  //AreaApiControllerReport<ISNFormatService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public SNFormatController(ISNFormatService _iService) : base(_iService)
        {
            iService = _iService;
        }

        /// <summary>
        /// GetSNRGetNext
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSNRGetNext")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]        
        public async Task<IActionResult> GetSNRGetNext(string S_SNFormat, string S_ReuseSNByStation,
            string S_ProdOrder, string S_Part, string S_Station, string S_ExtraData)
        {            
            CommonResult result = new CommonResult();
            try
            {
                string S_SN = await iService.GetSNRGetNext(S_SNFormat, S_ReuseSNByStation, S_ProdOrder, S_Part, S_Station, S_ExtraData);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = S_SN;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSNRGetNext 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }
    }
}
