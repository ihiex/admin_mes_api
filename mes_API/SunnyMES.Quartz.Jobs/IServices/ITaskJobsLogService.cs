using System;
using SunnyMES.Commons.IServices;
using SunnyMES.Quartz.Dtos;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Quartz.IServices
{
    /// <summary>
    /// 定义定时任务执行日志服务接口
    /// </summary>
    public interface ITaskJobsLogService:IService<TaskJobsLog,TaskJobsLogOutputDto, string>
    {
    }
}
