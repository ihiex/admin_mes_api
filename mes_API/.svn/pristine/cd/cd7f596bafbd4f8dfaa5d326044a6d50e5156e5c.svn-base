using System;
using System.Collections.Generic;
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
    /// 定义服务接口
    /// </summary>
    public interface IAZhangService:IService<AZhang,AZhangOutputDto, string>
    {
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        AZhang GetA_Zhang(string appid, string secret);

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        AZhang GetA_Zhang(string appid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<AZhangOutputDto> SelectAZhang();
        /// <summary>
        /// 更新可用的应用到缓存
        /// </summary>
        void UpdateCacheAllowApp();


        /// <summary>
        /// 按条件批量删除
        /// </summary>
        /// <param name="ids">主键Id集合</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        CommonResult DeleteBatchWhere(DeletesInputDto ids, IDbTransaction trans = null);
        /// <summary>
        /// 异步按条件批量删除
        /// </summary>
        /// <param name="ids">主键Id集合</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        Task<CommonResult> DeleteBatchWhereAsync(DeletesInputDto ids, IDbTransaction trans = null);
    }
}
