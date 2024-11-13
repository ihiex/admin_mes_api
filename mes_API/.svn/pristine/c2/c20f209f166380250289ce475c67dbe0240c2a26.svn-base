using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Dtos.Public;
using SunnyMES.Security.SysConfig.Models.Public;

namespace SunnyMES.Security.IServices.Public
{
    public interface IPublicPropertiesService : ICommonService<string>
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
        Task<PageResult<SC_IdDesc>> FindWithPagerAsync(SearchPropertiesInputDto search);

    }
}
