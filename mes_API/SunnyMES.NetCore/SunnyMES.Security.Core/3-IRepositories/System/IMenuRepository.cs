using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IMenuRepository:IRepository<Menu, string>
    {

        /// <summary>
        /// 根据角色ID字符串（逗号分开)和系统类型ID，获取对应的操作功能列表
        /// </summary>
        /// <param name="roleIds">角色ID</param>
        /// <param name="typeID">系统类型ID</param>
        /// <param name="UserID">UserID</param>
        /// <param name="isMenu">是否是菜单</param>
        /// <returns></returns>
        IEnumerable<Menu> GetFunctions(string roleIds, string typeID, string UserID, bool isMenu = false);

        /// <summary>
        /// 根据系统类型ID，获取对应的操作功能列表
        /// </summary>
        /// <param name="typeID">系统类型ID</param>
        /// <returns></returns>
        IEnumerable<Menu> GetFunctions(string typeID);

        Task<List<API_Menu>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}