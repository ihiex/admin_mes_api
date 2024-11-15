using System;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Tenants.Dtos;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.IRepositories
{
    /// <summary>
    /// 定义租户仓储接口
    /// </summary>
    public interface ITenantRepository:IRepository<Tenant, string>
    {
        /// <summary>
        /// 根据租户账号查询租户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Tenant> GetByUserName(string userName);

        /// <summary>
        /// 注册租户户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantLogOnEntity"></param>
        Task<bool> InsertAsync(Tenant entity, TenantLogon tenantLogOnEntity);

    }
}