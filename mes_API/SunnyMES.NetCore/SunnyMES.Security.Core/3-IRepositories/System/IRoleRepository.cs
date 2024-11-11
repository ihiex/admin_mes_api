using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IRoleRepository:IRepository<Role, string>
    {
        Task<List<API_Role>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);

        Task<string> Clone(Role entity, IDbTransaction trans = null);
    }
}