using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// IP地址禁用/可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class FilterIPController : AreaApiController<FilterIP, FilterIPOutputDto, FilterIPInputDto, IFilterIPService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public FilterIPController(IFilterIPService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(FilterIP info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(FilterIP info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(FilterIP info)
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
        public override async Task<IActionResult> UpdateAsync(FilterIPInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            FilterIP info = iService.Get(tinfo.Id);
            info.FilterType = tinfo.FilterType;
            info.EndIP = tinfo.EndIP;
            info.StartIP = tinfo.StartIP;
            info.SortCode = tinfo.SortCode;
            info.EnabledMark = tinfo.EnabledMark;
            info.Description = tinfo.Description;

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

    }
}