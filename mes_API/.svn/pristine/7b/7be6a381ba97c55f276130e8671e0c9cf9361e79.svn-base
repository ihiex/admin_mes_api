using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using System.Linq;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 应用管理接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class APPController : AreaApiController<APP, AppOutputDto, APPInputDto, IAPPService,string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public APPController(IAPPService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(APP info)
        {
            info.Id = GuidUtils.CreateNo();
            info.AppSecret = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            if (info.IsOpenAEKey)
            {
                info.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            }
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.CompanyId = CurrentUser.OrganizeId;
            info.DeptId = CurrentUser.DeptId;
            info.DeleteMark = false;
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(APP info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(APP info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }


        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(APPInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            APP info = iService.Get(tinfo.Id);
            info.AppId = tinfo.AppId;
            info.RequestUrl = tinfo.RequestUrl;
            info.Token = tinfo.Token;
            info.EnabledMark = tinfo.EnabledMark;
            info.Description = tinfo.Description;

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info, tinfo.Id).ConfigureAwait(true);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;

                PsysLog.SetControllerLog(" APP Update", P_CurrentLoginIP, P_EmployeeID.ToString());
            }
            else
            {
                result.ResultMsg = ErrCode.err43002;
                result.ResultCode = "43002";
            }
            MemoryCacheHelper.Set("cacheAppList", iService.GetAll().ToList());
            return ToJsonContent(result);
        }



        /// <summary>
        /// 重置AppSecret
        /// </summary>
        /// <returns></returns>
        [HttpGet("ResetAppSecret")]
        [YuebonAuthorize("ResetAppSecret")]
        public async Task<IActionResult> ResetAppSecret(string id)
        {
            CommonResult result = new CommonResult();
            APP aPP = iService.Get(id);
            aPP.AppSecret = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            bool bl = await iService.UpdateAsync(aPP, id);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = aPP.AppSecret;
                result.Success = true;
            }
            else
            {
                result.ResultMsg = ErrCode.err43002;
                result.ResultCode = "43002";
            }
            MemoryCacheHelper.Set("cacheAppList", iService.GetAll().ToList());
            return ToJsonContent(result);
        }

        /// <summary>
        /// 重置消息加密密钥EncodingAESKey
        /// </summary>
        /// <returns></returns>
        [HttpGet("ResetEncodingAESKey")]
        [YuebonAuthorize("ResetEncodingAESKey")]
        public async Task<IActionResult> ResetEncodingAESKey(string id)
        {
            CommonResult result = new CommonResult();
            APP aPP = iService.Get(id);
            aPP.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            bool bl = await iService.UpdateAsync(aPP, id);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = aPP.EncodingAESKey;
                result.Success = true;
            }
            else
            {
                result.ResultMsg = ErrCode.err43002;
                result.ResultCode = "43002";
            }
            MemoryCacheHelper.Set("cacheAppList", iService.GetAll().ToList());
            return ToJsonContent(result);
        }
    }
}