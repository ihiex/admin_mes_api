using System;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 定义单据编码服务接口
    /// </summary>
    public interface ISequenceService:IService<Sequence,SequenceOutputDto, string>
    {
        /// <summary>
        /// 获取最新业务单据编码
        /// </summary>
        /// <param name="sequenceName">业务单据编码名称</param>
        /// <returns></returns>
        Task<CommonResult> GetSequenceNextTask(string sequenceName);
        /// <summary>
        /// 获取最新业务单据编码
        /// </summary>
        /// <param name="sequenceName">业务单据编码名称</param>
        /// <returns></returns>
       CommonResult GetSequenceNext(string sequenceName);
    }
}
