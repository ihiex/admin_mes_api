using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.Dtos
{
    public class TenantsProfile : Profile
    {
        public TenantsProfile()
        {
           CreateMap<Tenant, TenantOutputDto>();
           CreateMap<TenantInputDto, Tenant>();
            CreateMap<TenantLogon, TenantLogonOutputDto>();
            CreateMap<TenantLogonInputDto, TenantLogon>();

        }
    }
}
