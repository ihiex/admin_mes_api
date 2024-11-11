using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// ��֯����
    /// </summary>
    public interface IOrganizeService:IService<Organize, OrganizeOutputDto, string>
    {

        /// <summary>
        /// ��ȡ��֯����������Vue �����б�
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizeOutputDto>> GetAllOrganizeTreeTable();

        /// <summary>
        /// ��ȡ���ڵ���֯
        /// </summary>
        /// <param name="id">��֯Id</param>
        /// <returns></returns>
        Organize GetRootOrganize(string id);


        /// <summary>
        /// ����������ɾ��
        /// </summary>
        /// <param name="ids">����Id����</param>
        /// <param name="trans">�������</param>
        /// <returns></returns>
        CommonResult DeleteBatchWhere(DeletesInputDto ids, IDbTransaction trans = null);
        /// <summary>
        /// �첽����������ɾ��
        /// </summary>
        /// <param name="ids">����Id����</param>
        /// <param name="trans">�������</param>
        /// <returns></returns>
        Task<CommonResult> DeleteBatchWhereAsync(DeletesInputDto ids, IDbTransaction trans = null);
    }
}