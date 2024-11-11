using System;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Quartz.IRepositories;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Quartz.Repositories
{
    /// <summary>
    /// 定时任务执行日志仓储接口的实现
    /// </summary>
    public class TaskJobsLogRepository : BaseRepository<TaskJobsLog, string>, ITaskJobsLogRepository
    {
		public TaskJobsLogRepository()
        {
        }

        public TaskJobsLogRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }
    }
}