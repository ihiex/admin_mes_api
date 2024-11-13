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
    public class BZhangController : AreaApiControllerGeneric<BZhang, BZhangOutputDto, BZhangInputDto, IBZhangService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public BZhangController(IBZhangService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(BZhang info)
        {
            //info.Id = GuidUtils.CreateNo();
            info.Id = "999";

            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            //info.CompanyId = CurrentUser.OrganizeId;
            //info.DeptId = CurrentUser.DeptId;
            info.DeleteMark = false;
        }


        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        [YuebonAuthorize("Add")]
        public override async Task<IActionResult> InsertAsync(BZhangInputDto tinfo)
        {
            CommonResult result = new CommonResult();
           
            BZhang v_bZhang = new BZhang();
            OnBeforeInsert(v_bZhang);

            v_bZhang.Data1 = tinfo.Data1;
            v_bZhang.Data2 = tinfo.Data2;

            long ln  = await iService.InsertAsync(v_bZhang).ConfigureAwait(false); 
            if (ln>0)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
            }
            else
            {
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(BZhang info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(BZhang info)
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
        public override async Task<IActionResult> UpdateAsync(BZhangInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            BZhang info = iService.Get(tinfo.Id);

            info.EnabledMark = tinfo.EnabledMark;
            info.Description = tinfo.Description;
            info.Data1 = tinfo.Data1;
            info.Data2 = tinfo.Data2;

            OnBeforeUpdate(info);
            long bl = await iService.UpdateAsync2(info, tinfo.Id).ConfigureAwait(false);
            if (bl>0)
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