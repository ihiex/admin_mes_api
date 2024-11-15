using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// 组织仓储接口
    /// 这里用到的Organize业务对象，是领域对象
    /// </summary>
    public interface IOrganizeRepository:IRepository<Organize, string>
    {
        /// <summary>
        /// 获取根节点组织
        /// </summary>
        /// <param name="id">组织Id</param>
        /// <returns></returns>
        Organize GetRootOrganize(string id);

        Task<List<API_Organize>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}