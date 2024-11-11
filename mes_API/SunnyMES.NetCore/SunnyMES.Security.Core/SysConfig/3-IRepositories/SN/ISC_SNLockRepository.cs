using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig._1_Models.SN;
using SunnyMES.Security.SysConfig._2_Dtos.SN;
using SunnyMES.Security.SysConfig.Models.Part;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._3_IRepositories.SN
{
    public interface ISC_SNLockRepository : IRepositoryReport<string>
    {
        Task<PageResult<SC_SN>> FindWithPagerSearchAsync(SearchSignalInputDto search);
        Task<SC_OperateState> UploadSNLock(SC_Lock_SN_Dto scLockSnDto);
        Task<List<T>> FindWithPagerCustomSqlAsync<T>(string sql, PagerInfo info, string fieldToSort, bool desc, IDbTransaction trans = null);
    }
}
