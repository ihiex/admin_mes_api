using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.CMS.Dtos;
using SunnyMES.CMS.Models;
using SunnyMES.CMS.IServices;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.Dtos;
using System.Data;

namespace SunnyMES.WebApi.Areas.CMS.Controllers
{
    /// <summary>
    /// 文章分类接口
    /// </summary>
    [ApiController]
    [Route("api/CMS/[controller]")]
    public class ArticlecategoryController : AreaApiController<Articlecategory, ArticlecategoryOutputDto,ArticlecategoryInputDto,IArticlecategoryService,string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public ArticlecategoryController(IArticlecategoryService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Articlecategory info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.CompanyId = CurrentUser.OrganizeId;
            info.DeptId = CurrentUser.DeptId;
            info.DeleteMark = false;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
            if (string.IsNullOrEmpty(info.ParentId))
            {
                info.ClassLayer = 1;
                info.ParentId = "";
            }
            else
            {
                info.ClassLayer = iService.Get(info.ParentId).ClassLayer + 1;
            }
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Articlecategory info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
            if (string.IsNullOrEmpty(info.ParentId))
            {
                info.ClassLayer = 1;
                info.ParentId = "";
            }
            else
            {
                info.ClassLayer = iService.Get(info.ParentId).ClassLayer + 1;
            }
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Articlecategory info)
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
        public override async Task<IActionResult> UpdateAsync(ArticlecategoryInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Articlecategory info = iService.Get(tinfo.Id);
            info.ParentId = tinfo.ParentId;
            info.Title = tinfo.Title;
            info.EnabledMark = tinfo.EnabledMark;
            info.SortCode = tinfo.SortCode;
            info.Description = tinfo.Description;

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info, tinfo.Id).ConfigureAwait(false);
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
        /// 获取文章分类适用于Vue 树形列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCategoryTreeTable")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> GetAllCategoryTreeTable(string keyword)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<ArticlecategoryOutputDto> list = await iService.GetAllArticlecategoryTreeTable(keyword);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取分类异常", ex);
                result.ResultMsg = ErrCode.err40110;
                result.ResultCode = "40110";
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