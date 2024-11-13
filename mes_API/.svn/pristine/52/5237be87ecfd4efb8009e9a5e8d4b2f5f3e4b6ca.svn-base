using System;
using System.Collections.Generic;
using System.Data;
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
    public class RoleService: BaseService<Role, RoleOutputDto, string>, IRoleService
    {
        private  IRoleRepository _repository;
        private  ILogService _logService;
        private IOrganizeService _organizeService;
        public RoleService(IRoleRepository repository, ILogService logService,IOrganizeService organizeService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
            _organizeService = organizeService;
        }

        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public Role GetRole(string enCode)
        {
            string where = string.Format("EnCode='{0}'",enCode);
            return _repository.GetWhere(where);
        }


        /// <summary>
        /// 根据用户角色ID获取角色编码
        /// </summary>
        /// <param name="ids">角色ID字符串，用“,”分格</param>
        /// <returns></returns>
        public string GetRoleEnCode(string ids)
        {
            string roleIDsStr = string.Format("'{0}'", ids.Replace(",", "','"));
            string sqlWhere = string.Format("Id in({0})", roleIDsStr);
            IEnumerable<Role> listRoles = _repository.GetListWhere(sqlWhere);
            string strEnCode = string.Empty;
            foreach (Role item in listRoles)
            {
                strEnCode += item.EnCode + ",";
            }
            return strEnCode;

        }


        /// <summary>
        /// 根据用户角色ID获取角色编码
        /// </summary>
        /// <param name="ids">角色ID字符串，用“,”分格</param>
        /// <returns></returns>
        public string GetRoleNameStr(string ids)
        {
            string roleIDsStr = string.Format("'{0}'", ids.Replace(",", "','"));
            string sqlWhere = string.Format("Id in({0})", roleIDsStr);
            IEnumerable<Role> listRoles = _repository.GetListWhere(sqlWhere);
            string strEnCode = string.Empty;
            foreach (Role item in listRoles)
            {
                strEnCode += item.FullName + ",";
            }
            return strEnCode;

        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>

        public override async Task<PageResult<RoleOutputDto>> FindWithPagerAsync(SearchInputDto<Role> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (FullName like '%{0}%' or EnCode like '%{0}%')", search.Keywords);
            };
            where += " and Category=1";
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<API_Role> list = await _repository.FindWithPagerMyAsync(where, pagerInfo, search.Sort, order);


            List<RoleOutputDto> resultList = new List<RoleOutputDto>() ;
            //List<RoleOutputDto> listResult = new List<RoleOutputDto>();


            foreach (var item in list)
            {
                RoleOutputDto v_Value = new RoleOutputDto();

                if (!string.IsNullOrEmpty(item.OrganizeId))
                {
                    v_Value.OrganizeName = _organizeService.Get(item.OrganizeId).FullName;
                }

                v_Value.Id = item.Id;
                v_Value.OrganizeId = item.OrganizeId;
                v_Value.Category= item.Category;
                v_Value.EnCode = item.EnCode;
                v_Value.FullName = item.FullName;
                v_Value.Type = item.Type;
                v_Value.AllowEdit = item.AllowEdit;
                v_Value.AllowDelete = item.AllowDelete;

                v_Value.SortCode = item.SortCode;
                v_Value.DeleteMark = item.DeleteMark;
                v_Value.EnabledMark = item.EnabledMark;
                v_Value.Description = item.Description;
                v_Value.CreatorTime = item.CreatorTime;
                v_Value.CreatorUserId = item.CreatorUserId;
                v_Value.LastModifyTime = item.LastModifyTime;
                v_Value.LastModifyUserId= item.LastModifyUserId;

                v_Value.DeleteTime= item.DeleteTime;
                v_Value.DeleteUserId= item.DeleteUserId;

                resultList.Add(v_Value);
            }

            PageResult<RoleOutputDto> pageResult = new PageResult<RoleOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = resultList,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }

        public async Task<string> Clone(Role entity, IDbTransaction trans = null) 
        {
            return await _repository.Clone(entity, trans);
        }


        //public override async Task<PageResult<RoleOutputDto>> FindWithPagerAsync(SearchInputDto<Role> search)
        //{
        //    bool order = search.Order == "asc" ? false : true;
        //    string where = GetDataPrivilege(false);
        //    if (!string.IsNullOrEmpty(search.Keywords))
        //    {
        //        where += string.Format(" and (FullName like '%{0}%' or EnCode like '%{0}%')", search.Keywords);
        //    };
        //    where += " and Category=1";
        //    PagerInfo pagerInfo = new PagerInfo
        //    {
        //        CurrentPageIndex = search.CurrentPageIndex,
        //        PageSize = search.PageSize
        //    };
        //    List<Role> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
        //    List<RoleOutputDto> resultList = list.MapTo<RoleOutputDto>();
        //    List<RoleOutputDto> listResult = new List<RoleOutputDto>();
        //    foreach (RoleOutputDto item in resultList)
        //    {
        //        if (!string.IsNullOrEmpty(item.OrganizeId))
        //        {
        //            item.OrganizeName = _organizeService.Get(item.OrganizeId).FullName;
        //        }
        //        listResult.Add(item);
        //    }
        //    PageResult<RoleOutputDto> pageResult = new PageResult<RoleOutputDto>
        //    {
        //        CurrentPage = pagerInfo.CurrentPageIndex,
        //        Items = listResult.MapTo<RoleOutputDto>(),
        //        ItemsPerPage = pagerInfo.PageSize,
        //        TotalItems = pagerInfo.RecordCount
        //    };
        //    return pageResult;
        //}
    }
}