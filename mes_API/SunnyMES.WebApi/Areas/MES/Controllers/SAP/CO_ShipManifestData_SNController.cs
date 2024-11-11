using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.IServices;
using SunnyMES.Security.IServices.MES.SAP;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Security.Models;
using System.Collections.Generic;

namespace SunnyMES.WebApi.Areas.MES.Controllers.SAP
{
    /// <summary>
    /// 出货数据
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class CO_ShipManifestData_SNController : AreaApiControllerCustom<CO_ShipManifestData_SN, CO_ShipManifestData_SN, CO_ShipManifestData_SN, ICO_ShipManifestData_SNServices, string>
    {
        private readonly ICO_ShipManifestData_SNServices iService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        public CO_ShipManifestData_SNController(ICO_ShipManifestData_SNServices _iService) : base(_iService)
        {
            iService = _iService;
        }

        /// <summary>
        /// 根据条码模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerLikeAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerLikeAsync(SearchSAPManifestDataModel search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerLikeAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);

            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 根据条件导出csv
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindExportCSVAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindExportCSVAsync(SearchSAPManifestDataModel search)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                IEnumerable<dynamic> List_Dyn = await iService.FindExportCSVAsync(search);

                List<dynamic> List_Main = new List<dynamic>();
                List_Main.Add(List_Dyn);

                if (List_Main.Count > 0)
                {
                    try
                    {
                        if (List_Main[0][0] is TabVal)
                        {
                            TabVal v_TabVal = List_Main[0][0] as TabVal;
                            if (v_TabVal.ValStr1 == "ERROR")
                            {
                                v_CommonResult.Success = false;
                                v_CommonResult.ResultCode = v_TabVal.ValStr1;
                                v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                                v_CommonResult.ResData = null;
                            }
                            else
                            {
                                v_CommonResult.Success = true;
                                v_CommonResult.ResultCode = ErrCode.successCode;
                                v_CommonResult.ResultMsg = ErrCode.err0;
                                v_CommonResult.ResData = v_TabVal.ValStr2;
                            }
                        }
                    }
                    catch
                    {
                        v_CommonResult.Success = false;
                        v_CommonResult.ResultCode = "ERROR";
                        v_CommonResult.ResultMsg = ErrCode.err1;
                        v_CommonResult.ResData = null;
                    }
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = "ERROR";
                    v_CommonResult.ResultMsg = ErrCode.err1;
                    v_CommonResult.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 FindExportCSVAsync 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        [CommonAuthorize]
        public override async Task<IActionResult> UpdateAsync(CO_ShipManifestData_SN inInfo)
        {

            CommonResult commonResult = new CommonResult();
            try
            {
                base.OnBeforeUpdate(inInfo);
                var listDyn = await iService?.UpdateAsync(inInfo, inInfo.ID.ToString());
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
