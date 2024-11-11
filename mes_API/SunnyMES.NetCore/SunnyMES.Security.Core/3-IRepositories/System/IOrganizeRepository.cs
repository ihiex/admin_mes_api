using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// ��֯�ִ��ӿ�
    /// �����õ���Organizeҵ��������������
    /// </summary>
    public interface IOrganizeRepository:IRepository<Organize, string>
    {
        /// <summary>
        /// ��ȡ���ڵ���֯
        /// </summary>
        /// <param name="id">��֯Id</param>
        /// <returns></returns>
        Organize GetRootOrganize(string id);

        Task<List<API_Organize>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}