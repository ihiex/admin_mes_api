using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRoleAuthorizeService:IService<RoleAuthorize, RoleAuthorizeOutputDto, string>
    {
        /// <summary>
        /// ���ݽ�ɫ����Ŀ���Ͳ�ѯȨ��
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        IEnumerable<RoleAuthorize> GetListRoleAuthorizeByRoleId(string roleIds, string itemType);


        /// <summary>
        /// ��ȡ���ܲ˵�������Vue Tree����
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleFunctionOutputDto>> GetAllFunctionTree();

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
    }
}