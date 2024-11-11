using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Models.Public;

namespace SunnyMES.Security.IRepositories.Public
{
    public interface IPublicPropertiesRepository : ICommonRepository<string>
    {
        Task<List<SC_IdDesc>> GetCommonTabList();
        /// <summary>
        /// ture  存在，反之亦然
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<bool> CheckIsExists(string sql);

        Task<long> InsertAsync(SC_IdDescTab idDescTab);
        Task<long> UpdateAsync(SC_IdDescTab idDescTab);
        Task<long> DeleteAsync(SC_IdDescTab idDescTab);
        Task<long> CloneAsync(SC_IdDescTab idDescTab);

        Task<List<SC_IdDesc>> FindWithPagerAsync(string condition, PagerInfo info, string fieldToSort, bool desc, string tableName);
    }
}
