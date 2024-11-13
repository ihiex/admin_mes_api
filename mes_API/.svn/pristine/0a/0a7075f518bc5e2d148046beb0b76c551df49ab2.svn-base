using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IRoleDataRepository:IRepository<RoleData, string>
    {
        /// <summary>
        /// 根据角色返回授权访问部门数据
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
       List<string> GetListDeptByRole(string roleIds);


        Task<List<API_RoleData>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}