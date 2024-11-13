using System;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZhangService : IService<Zhang, ZhangOutputDto, string>
    {
        /// <summary>
        /// 异步按条件批量删除
        /// </summary>
        /// <param name="ids">主键Id集合</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        Task<CommonResult> DeleteBatchWhereAsync(DeletesInputDto ids, IDbTransaction trans = null);
    }
}
