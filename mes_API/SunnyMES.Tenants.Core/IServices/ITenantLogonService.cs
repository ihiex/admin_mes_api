using System;
using SunnyMES.Commons.IServices;
using SunnyMES.Tenants.Dtos;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.IServices
{
    /// <summary>
    /// 定义用户登录信息服务接口
    /// </summary>
    public interface ITenantLogonService:IService<TenantLogon,TenantLogonOutputDto, string>
    {

    }
}
