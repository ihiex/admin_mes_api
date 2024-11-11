using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IMenuRepository:IRepository<Menu, string>
    {

        /// <summary>
        /// ���ݽ�ɫID�ַ��������ŷֿ�)��ϵͳ����ID����ȡ��Ӧ�Ĳ��������б�
        /// </summary>
        /// <param name="roleIds">��ɫID</param>
        /// <param name="typeID">ϵͳ����ID</param>
        /// <param name="UserID">UserID</param>
        /// <param name="isMenu">�Ƿ��ǲ˵�</param>
        /// <returns></returns>
        IEnumerable<Menu> GetFunctions(string roleIds, string typeID, string UserID, bool isMenu = false);

        /// <summary>
        /// ����ϵͳ����ID����ȡ��Ӧ�Ĳ��������б�
        /// </summary>
        /// <param name="typeID">ϵͳ����ID</param>
        /// <returns></returns>
        IEnumerable<Menu> GetFunctions(string typeID);

        Task<List<API_Menu>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}