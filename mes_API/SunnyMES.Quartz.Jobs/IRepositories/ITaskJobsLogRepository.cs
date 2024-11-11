using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.Models;

namespace SunnyMES.Quartz.IRepositories
{
    /// <summary>
    /// 定义定时任务执行日志仓储接口
    /// </summary>
    public interface ITaskJobsLogRepository:IRepository<TaskJobsLog, string>
    {
    }
}