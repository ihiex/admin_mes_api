using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Tenants.IRepositories;
using SunnyMES.Tenants.IServices;
using SunnyMES.Tenants.Dtos;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.Services
{
    /// <summary>
    /// 用户登录信息服务接口实现
    /// </summary>
    public class TenantLogonService : BaseService<TenantLogon,TenantLogonOutputDto, string>, ITenantLogonService
    {
		private readonly ITenantLogonRepository _repository;
        public TenantLogonService(ITenantLogonRepository repository) : base(repository)
        {
			_repository=repository;
        }
    }
}