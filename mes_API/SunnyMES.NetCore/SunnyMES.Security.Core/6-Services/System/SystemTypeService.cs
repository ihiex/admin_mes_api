using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemTypeService : BaseService<SystemType, SystemTypeOutputDto, string>, ISystemTypeService
    {
        private readonly ISystemTypeRepository _repository;
        private readonly IRoleAuthorizeService roleAuthorizeService;
        private readonly ILogService _logService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="logService"></param>
        public SystemTypeService(ISystemTypeRepository repository, ILogService logService, IRoleAuthorizeService _roleAuthorizeService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
            roleAuthorizeService = _roleAuthorizeService;
        }

        /// <summary>
        /// 根据系统编码查询系统对象
        /// </summary>
        /// <param name="appkey">系统编码</param>
        /// <returns></returns>
        public SystemType GetByCode(string appkey)
        {
            return _repository.GetByCode(appkey);

        }

        /// <summary>
        /// 根据角色获取可以访问子系统
        /// </summary>
        /// <param name="roleIds">角色Id，用','隔开</param>
        /// <returns></returns>
        public List<SystemTypeOutputDto> GetSubSystemList(string roleIds)
        {
            string roleIDsStr = string.Empty;
            if (roleIds.IndexOf(',')>0)
            {
                roleIDsStr=string.Format("'{0}'", roleIds.Replace(",", "','"));
            }
            else
            {
                roleIDsStr = string.Format("'{0}'", roleIds); 
            }
            
            IEnumerable<RoleAuthorize> roleAuthorizes = roleAuthorizeService.GetListRoleAuthorizeByRoleId(roleIDsStr, "0");
            string strWhere = string.Empty;
            if (roleAuthorizes.Count() > 0)
            {
                strWhere = " Id in (";
                foreach (RoleAuthorize item in roleAuthorizes)
                {
                    strWhere += "'" + item.ItemId + "',";
                }
                strWhere = strWhere.Substring(0, strWhere.Length - 1) + ")";
            }
            List<SystemTypeOutputDto> list = _repository.GetAllByIsNotDeleteAndEnabledMark(strWhere).OrderBy(t => t.SortCode).ToList().MapTo<SystemTypeOutputDto>();
            return list;        
        }



        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<SystemTypeOutputDto>> FindWithPagerAsync(SearchInputDto<SystemType> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (FullName like '%{0}%' or EnCode like '%{0}%')", search.Keywords);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<SystemType> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SystemTypeOutputDto> pageResult = new PageResult<SystemTypeOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SystemTypeOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}