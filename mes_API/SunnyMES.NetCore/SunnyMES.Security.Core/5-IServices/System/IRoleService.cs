using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRoleService:IService<Role, RoleOutputDto, string>
    {
        /// <summary>
        /// ���ݽ�ɫ�����ȡ��ɫ
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        Role GetRole(string enCode);


        /// <summary>
        /// �����û���ɫID��ȡ��ɫ����
        /// </summary>
        /// <param name="ids">��ɫID�ַ������á�,���ָ�</param>
        /// <returns></returns>
        string GetRoleEnCode(string ids);


        /// <summary>
        /// �����û���ɫID��ȡ��ɫ����
        /// </summary>
        /// <param name="ids">��ɫID�ַ������á�,���ָ�</param>
        /// <returns></returns>
       string GetRoleNameStr(string ids);

        Task<string> Clone(Role entity, IDbTransaction trans = null);

        //Task<List<RoleOutputDto>> FindWithPagerAsync(string condition, PagerInfo info, string fieldToSort, bool desc);
    }
}
