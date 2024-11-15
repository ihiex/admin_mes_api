using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Quartz.Dtos;
using SunnyMES.Quartz.IServices;
using SunnyMES.Quartz.Models;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 定时任务执行日志接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class TaskJobsLogController : AreaApiController<TaskJobsLog, TaskJobsLogOutputDto,TaskJobsLogInputDto,ITaskJobsLogService,string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public TaskJobsLogController(ITaskJobsLogService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(TaskJobsLog info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(TaskJobsLog info)
        {
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(TaskJobsLog info)
        {
        }


        /// <summary>
        /// 根据任务Id查询最新的30条日志
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("FindWithByTaskIdAsync")]
        [YuebonAuthorize("List")]
        public  async Task<IActionResult> FindWithByTaskIdAsync([FromQuery] SearchModel search)
        {
            CommonResult result = new CommonResult();
            string where = "";
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" TaskId ='{0}' ", search.Keywords);
            }
            where += " order by CreatorTime desc";
            IEnumerable<TaskJobsLog> list = await iService.GetListTopWhereAsync(40,where);
            List<TaskJobsLogVueTimelineOutputDto> resultList = list.MapTo<TaskJobsLogVueTimelineOutputDto>();
            result.ResData = resultList;
            result.ResultCode = ErrCode.successCode;
            return ToJsonContent(result);
        }
    }
}