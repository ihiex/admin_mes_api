using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class AZhangController : AreaApiController<AZhang, AZhangOutputDto, AZhangInputDto, IAZhangService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public AZhangController(IAZhangService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(AZhang info)
        {
            //info.Id = GuidUtils.CreateNo();
            info.Id = "999";
            info.AppSecret = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            if (info.IsOpenAEKey)
            {
                info.EncodingAESKey = MD5Util.GetMD5_32(GuidUtils.NewGuidFormatN()).ToUpper();
            }
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            //info.CompanyId = CurrentUser.OrganizeId;
            //info.DeptId = CurrentUser.DeptId;
            info.DeleteMark = false;
        }

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(AZhang info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(AZhang info)
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
        public override async Task<IActionResult> UpdateAsync(AZhangInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            AZhang info = iService.Get(tinfo.Id);
            info.AppId = tinfo.AppId;
            info.RequestUrl = tinfo.RequestUrl;
            info.Token = tinfo.Token;
            info.EnabledMark = tinfo.EnabledMark;
            info.Description = tinfo.Description;
            info.Data1 = tinfo.Data1;
            info.Data2 = tinfo.Data2;

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info, tinfo.Id).ConfigureAwait(true);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
            }
            else
            {
                result.ResultMsg = ErrCode.err43002;
                result.ResultCode = "43002";
            }
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
            AZhang aPP = iService.Get(id);
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
            AZhang aPP = iService.Get(id);
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
            return ToJsonContent(result);
        }


        /// <summary>
        /// 异步批量物理删除
        /// </summary>
        /// <param name="info"></param>
        [HttpDelete("DeleteBatchAsync")]
        [YuebonAuthorize("Delete")]
        public override async Task<IActionResult> DeleteBatchAsync(DeletesInputDto info)
        {
            CommonResult result = new CommonResult();

            if (info.Ids.Length > 0)
            {
                result = await iService.DeleteBatchWhereAsync(info).ConfigureAwait(false);
                if (result.Success)
                {
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                }
                else
                {
                    result.ResultCode = "43003";
                }
            }
            return ToJsonContent(result);
        }


    }
}