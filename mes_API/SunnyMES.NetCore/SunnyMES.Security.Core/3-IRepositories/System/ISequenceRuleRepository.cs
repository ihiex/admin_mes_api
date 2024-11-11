using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// 定义序号编码规则表仓储接口
    /// </summary>
    public interface ISequenceRuleRepository:IRepository<SequenceRule, string>
    {
        Task<List<API_SequenceRule>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}