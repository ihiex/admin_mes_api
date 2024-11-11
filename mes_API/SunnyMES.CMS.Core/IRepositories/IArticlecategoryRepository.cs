using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.CMS.Models;

namespace SunnyMES.CMS.IRepositories
{
    /// <summary>
    /// 定义文章分类仓储接口
    /// </summary>
    public interface IArticlecategoryRepository:IRepository<Articlecategory, string>
    {
    }
}