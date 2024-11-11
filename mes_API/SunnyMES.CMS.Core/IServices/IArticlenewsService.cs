using System;
using SunnyMES.Commons.IServices;
using SunnyMES.CMS.Dtos;
using SunnyMES.CMS.Models;
using System.Threading.Tasks;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Dtos;
using System.Collections.Generic;

namespace SunnyMES.CMS.IServices
{
    /// <summary>
    /// 定义文章服务接口
    /// </summary>
    public interface IArticlenewsService:IService<Articlenews,ArticlenewsOutputDto, string>
    {
        /// <summary>
        /// 根据用户角色获取分类及该分类的文章
        /// </summary>
        /// <returns></returns>
        Task<List<CategoryArticleOutputDto>> GetCategoryArticleList();
    }
}
