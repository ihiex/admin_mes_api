using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// 定义单据编码仓储接口
    /// </summary>
    public interface ISequenceRepository:IRepository<Sequence, string>
    {
        Task<List<API_Sequence>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}