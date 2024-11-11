using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface ILogRepository:IRepository<Log, long>
    {
        //long InsertTset(int len);

        Task<List<API_Log>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}