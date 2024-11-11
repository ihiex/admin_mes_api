using SunnyMES.Commons.Enums;
using SunnyMES.Commons.IServices;
using SunnyMES.Quartz.Dtos;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Quartz.IServices
{
    /// <summary>
    /// 定义定时任务服务接口
    /// </summary>
    public interface ITaskManagerService:IService<TaskManager,TaskManagerOutputDto, string>
    {
        /// <summary>
        /// 记录任务运行结果
        /// </summary>
        /// <param name="jobId">任务Id</param>
        /// <param name="jobAction">任务执行动作</param>
        /// <param name="blresultTag">任务执行结果表示，true成功，false失败，初始执行为true</param>
        /// <param name="msg">任务记录描述</param>
        void RecordRun(string jobId, JobAction jobAction, bool blresultTag = true, string msg = "");

    }
}
