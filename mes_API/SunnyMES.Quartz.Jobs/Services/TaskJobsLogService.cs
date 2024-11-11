using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Quartz.Dtos;
using SunnyMES.Quartz.IRepositories;
using SunnyMES.Quartz.IServices;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.IServices;

namespace SunnyMES.Quartz.Services
{
    /// <summary>
    /// 定时任务执行日志服务接口实现
    /// </summary>
    public class TaskJobsLogService: BaseService<TaskJobsLog,TaskJobsLogOutputDto, string>, ITaskJobsLogService
    {
		private readonly ITaskJobsLogRepository _repository;
        private readonly ILogService _logService;
        public TaskJobsLogService(ITaskJobsLogRepository repository,ILogService logService) : base(repository)
        {
			_repository=repository;
			_logService=logService;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<TaskJobsLogOutputDto>> FindWithPagerAsync(SearchInputDto<TaskJobsLog> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (TaskId like '%{0}%' or  TaskName like '%{0}%')", search.Keywords);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<TaskJobsLog> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<TaskJobsLogOutputDto> pageResult = new PageResult<TaskJobsLogOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<TaskJobsLogOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}