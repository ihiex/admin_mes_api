using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.IRepositories
{
    /// <summary>
    /// 定义用户登录信息仓储接口
    /// </summary>
    public interface ITenantLogonRepository:IRepository<TenantLogon, string>
    {
        /// <summary>
        /// 根据租户ID获取租户登录信息实体
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        TenantLogon GetByTenantId(string tenantId);
    }
}