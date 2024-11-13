using System;
using System.Collections.Generic;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class RoleDataService: BaseService<RoleData, RoleDataOutputDto, string>, IRoleDataService
    {
		private readonly IRoleDataRepository _repository;
        private readonly ILogService _logService;
        public RoleDataService(IRoleDataRepository repository,ILogService logService) : base(repository)
        {
			_repository=repository;
			_logService=logService;
        }
        /// <summary>
        /// 根据角色返回授权访问部门数据
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public List<string> GetListDeptByRole(string roleIds)
        {
            return _repository.GetListDeptByRole(roleIds);
        }
    }
}