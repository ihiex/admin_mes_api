using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRoleService:IService<Role, RoleOutputDto, string>
    {
        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        Role GetRole(string enCode);


        /// <summary>
        /// 根据用户角色ID获取角色编码
        /// </summary>
        /// <param name="ids">角色ID字符串，用“,”分格</param>
        /// <returns></returns>
        string GetRoleEnCode(string ids);


        /// <summary>
        /// 根据用户角色ID获取角色编码
        /// </summary>
        /// <param name="ids">角色ID字符串，用“,”分格</param>
        /// <returns></returns>
       string GetRoleNameStr(string ids);

        Task<string> Clone(Role entity, IDbTransaction trans = null);

        //Task<List<RoleOutputDto>> FindWithPagerAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}
