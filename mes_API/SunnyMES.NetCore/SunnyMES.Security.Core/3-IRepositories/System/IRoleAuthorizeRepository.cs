using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IRoleAuthorizeRepository:IRepository<RoleAuthorize,string>
    {
        /// <summary>
        /// �����ɫ��Ȩ
        /// </summary>
        /// <param name="roleId">��ɫId</param>
        /// <param name="roleAuthorizesList">��ɫ����ģ��</param>
        /// <param name="roleDataList">��ɫ�ɷ�������</param>
        /// <param name="trans"></param>
        /// <returns>ִ�гɹ�����<c>true</c>������Ϊ<c>false</c>��</returns>
        Task<bool> SaveRoleAuthorize(string roleId,List<RoleAuthorize> roleAuthorizesList, List<RoleData> roleDataList,
           IDbTransaction trans = null);


        Task<List<API_RoleAuthorize>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}