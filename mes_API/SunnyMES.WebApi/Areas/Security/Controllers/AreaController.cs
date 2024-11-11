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
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;
using SunnyMES.Security.IServices;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 地区接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class AreaController : AreaApiController<Area, AreaOutputDto, AreaInputDto, IAreaService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public AreaController(IAreaService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Area info)
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
        protected override void OnBeforeUpdate(Area info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Area info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }
    }
}