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
using Microsoft.AspNetCore.Authorization;

namespace SunnyMES.WebApi.Areas.CMS.Controllers
{
    /// <summary>
    /// 文章，通知公告接口
    /// </summary>
    [ApiController]
    [Route("api/CMS/[controller]")]
    public class ArticlenewsController : AreaApiController<Articlenews, ArticlenewsOutputDto,ArticlenewsInputDto,IArticlenewsService,string>
    {
        private IArticlecategoryService articlecategoryService;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="_articlecategoryService"></param>
        public ArticlenewsController(IArticlenewsService _iService,IArticlecategoryService _articlecategoryService) : base(_iService)
        {
            iService = _iService;
            articlecategoryService = _articlecategoryService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Articlenews info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CategoryName = articlecategoryService.Get(info.CategoryId).Title;
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.CompanyId = CurrentUser.OrganizeId;
            info.DeptId = CurrentUser.DeptId;
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
        protected override void OnBeforeUpdate(Articlenews info)
        {
            info.CategoryName = articlecategoryService.Get(info.CategoryId).Title;
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Articlenews info)
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
        public override async Task<IActionResult> UpdateAsync(ArticlenewsInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Articlenews info = iService.Get(tinfo.Id);
            info.CategoryId = tinfo.CategoryId;
            info.Title = tinfo.Title;
            info.EnabledMark = tinfo.EnabledMark;
            info.SortCode = tinfo.SortCode;
            info.Description = tinfo.Description;
            info.SubTitle = tinfo.SubTitle;
            info.LinkUrl = tinfo.LinkUrl;
            info.IsHot = tinfo.IsHot;
            info.IsNew = tinfo.IsNew;
            info.IsRed = tinfo.IsRed;
            info.IsTop = tinfo.IsTop;

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
        /// 获取分类及该分类文章标题
        /// </summary>
        [HttpGet("GetCategoryArticle")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryArticle()
        {
            CommonResult result = new CommonResult();
            result.ResData = await iService.GetCategoryArticleList();
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            return ToJsonContent(result);
        }
    }
}