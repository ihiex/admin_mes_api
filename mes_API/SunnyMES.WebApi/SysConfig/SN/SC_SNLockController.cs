using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig._1_Models.SN;
using SunnyMES.Security.SysConfig._5_IServices.SN;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Security.IServices;
using SunnyMES.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using SunnyMES.Commons.Linq;
using SunnyMES.Security.SysConfig.Dtos.Part;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.SysConfig._2_Dtos.SN;

namespace SunnyMES.WebApi.SysConfig.SN
{

    /// <summary>
    /// 条码锁定/解锁
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_SNLockController : AreaApiControllerReport<ISC_SNLockServices, string>
    {
        private readonly ISC_SNLockServices sC_SnServices;

        public SC_SNLockController(ISC_SNLockServices sC_snServices) : base(sC_snServices)
        {
            sC_SnServices = sC_snServices;
        }

        ///// <summary>
        /////  通过CSV文件进行 锁定/解锁条码
        ///// </summary>
        ///// <param name="formCollection"></param>
        ///// <returns>服务器存储的文件信息</returns>
        //[HttpPost("LockFileSNAsync")]
        //[YuebonAuthorize("")]
        //public async Task<IActionResult> LockFileSNAsync([FromForm] IFormCollection formCollection, bool isLock = true)
        //{
        //    CommonResult result = new CommonResult();

        //    FormFileCollection filelist = (FormFileCollection)formCollection.Files;
        //    try
        //    {
        //        var file = filelist[0];
        //        string _fileName = filelist[0].FileName;
        //        var fileType = _fileName.Substring(_fileName.LastIndexOf('.') + 1);

        //        if (fileType != "csv")
        //        {
        //            result.Success = false;
        //            result.ResultCode = "000000";
        //            result.ResultMsg = "File format must be (.csv)";
        //            return ToJsonContent(result);
        //        }
        //        string tmpRes = string.Empty;
        //        using (var ms = new MemoryStream())
        //        {
        //            await filelist[0].CopyToAsync(ms);
        //            ms.Seek(0, SeekOrigin.Begin);

        //            #region
        //            //适合单步作战，不适合群体作战, 可直接转成model
        //            if (fileType == "csv")
        //            {
        //                var tmpCSVM = CSVHelper.CSVToDataTableByStreamReader<SC_SN>(ms);
        //                //return ToJsonContent(tmpCSVM);
        //                tmpRes = await sC_SnServices.UploadSNLock(tmpCSVM, isLock);
        //            }
        //            else
        //            {
        //                //返回的为动态类型
        //                var enitys = NPOIHelper.StreamToModel<SC_SN>(ms, fileType);
        //                //var tmpEn = NPOIHelper.StreamToModel(ms, fileType, "mesUnit");

        //            }
        //            #endregion
        //        }
        //        if (tmpRes == "OK")
        //        {
        //            result.ResData = "update success.";
        //            result.ResultCode = ErrCode.successCode;
        //            result.Success = true;
        //        }
        //        else
        //        {
        //            result.ResData = "update failed.   " + tmpRes;
        //            result.ResultCode = ErrCode.err1;
        //            result.Success = false;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        result.ResultCode = "500";
        //        result.ResultMsg = ex.Message;
        //        Log4NetHelper.Error("", ex);
        //        throw ex;
        //    }
        //    return ToJsonContent(result);
        //}

        /// <summary>
        /// 通过传达SN数组进行解锁锁定
        /// </summary>
        /// <param name="scLockSnDto"></param>
        /// <returns></returns>
        [HttpPost("LockSNAsync")]
        [YuebonAuthorize("")]
        //[AllowAnonymous]
        //[NoSignRequired]
        public async Task<IActionResult> LockSNAsync(SC_Lock_SN_Dto scLockSnDto)
        {
            CommonResult result = new CommonResult();

            try
            {
                scLockSnDto.user = CurrentUser?.UserFullName;
                var tmpRes = await sC_SnServices.UploadSNLock(scLockSnDto);
                if (tmpRes.result)
                {
                    result.ResData = "update success.";
                    result.ResultCode = ErrCode.successCode;
                    result.Success = true;
                }
                else
                {
                    result.ResData = string.IsNullOrEmpty(tmpRes.error) ? tmpRes.SNs.Select(x => x.SN) : tmpRes.error; 
                    result.ResultCode = ErrCode.err1;
                    result.ResultMsg = string.IsNullOrEmpty(tmpRes.error) ? "some barcode no match." : tmpRes.error;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
                Log4NetHelper.Error("", ex);
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        //[AllowAnonymous]
        //[NoSignRequired]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchSignalInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            try
            {
                var tmpData = await iService.FindWithPagerSearchAsync(search);
                commonResult.ResData = tmpData;
                commonResult.Success = true;
                commonResult.ResultCode = ErrCode.successCode;
                commonResult.ResultMsg = ErrCode.err0;
                commonResult.Sounds = S_Path_OK; 
            }
            catch (Exception ex)
            {
                commonResult.Success = false;
                commonResult.ResultCode = ErrCode.err1;
                commonResult.ResultMsg = ex.Message;
                commonResult.Sounds = S_Path_NG;
                Log4NetHelper.Error("",ex);
            }

            return ToJsonContent(commonResult);
        }
    }
}
