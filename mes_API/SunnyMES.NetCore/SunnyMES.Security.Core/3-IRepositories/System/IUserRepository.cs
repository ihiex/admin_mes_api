using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRepository:IRepository<User, string>
    {
        /// <summary>
        /// GetRole
        /// </summary>
        /// <param name="S_RoleCode"></param>
        /// <returns></returns>
        Task<List<Role>> GetRole(string S_RoleCode);
        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        Task<List<User>> GetUser(string S_Sql);

        Task<List<Models.mesEmployee>> GetmesEmployee(string S_Sql);

        Task<string> GetOperatorUserPermission(string StationID, string EmployeeID);


        Task<string> SetSyncUserData(string S_EmployeeID);

        Task<string> ValidateSecond(string UserID,string PWD);


        /// <summary>
        /// �����û��˺Ų�ѯ�û���Ϣ
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User> GetByUserName(string userName);
        /// <summary>
        /// �����û��ֻ������ѯ�û���Ϣ
        /// </summary>
        /// <param name="mobilePhone">�ֻ�����</param>
        /// <returns></returns>
        Task<User> GetUserByMobilePhone(string mobilePhone);
        /// <summary>
        /// ����Email��Account���ֻ��Ų�ѯ�û���Ϣ
        /// </summary>
        /// <param name="account">��¼�˺�</param>
        /// <returns></returns>
        Task<User> GetUserByLogin(string account);
        /// <summary>
        /// ����Email��ѯ�û���Ϣ
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
       Task<User> GetUserByEmail(string email);
        /// <summary>
        /// ע���û�
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        bool Insert(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null);
        /// <summary>
        /// ע���û�
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        Task<bool> InsertAsync(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null);

        /// <summary>
        /// ע���û�,������ƽ̨
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        bool Insert(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null);
        /// <summary>
        /// ���ݵ�����OpenId��ѯ�û���Ϣ
        /// </summary>
        /// <param name="openIdType">����������</param>
        /// <param name="openId">OpenIdֵ</param>
        /// <returns></returns>
        User GetUserByOpenId(string openIdType, string openId);

        /// <summary>
        /// ����΢��UnionId��ѯ�û���Ϣ
        /// </summary>
        /// <param name="unionId">UnionIdֵ</param>
        /// <returns></returns>
        User GetUserByUnionId(string unionId);
        /// <summary>
        /// ����userId��ѯ�û���Ϣ
        /// </summary>
        /// <param name="openIdType">����������</param>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        UserOpenIds GetUserOpenIdByuserId(string openIdType, string userId);
        /// <summary>
        /// �����û���Ϣ,������ƽ̨
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        bool UpdateUserByOpenId(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null);

        /// <summary>
        /// �����û�ID�õ���Ƭ��Ϣ
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        //UserNameCardOutPutDto GetUserNameCardInfo(string userid);

        /// <summary>
        /// ������Ƭ
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="headicon"></param>
        /// <param name="nickName"></param>
        /// <param name="name"></param>
        /// <param name="company"></param>
        /// <param name="position"></param>
        /// <param name="weburl"></param>
        /// <param name="mobile"></param>
        /// <param name="email"></param>
        /// <param name="wx"></param>
        /// <param name="wximg"></param>
        /// <param name="industry"></param>
        /// <param name="area"></param>
        /// <param name="address"></param>
        /// <param name="openflag"></param>
        /// <returns></returns>
        ////bool SaveNameCard(string userid,string headicon, string nickName, string name, string company, string position,
        //    string weburl, string mobile, string email, string wx, string wximg,
        //    string industry, string area, string address, int openflag);


        /// <summary>
        /// �����û���Ϣ���ڹ�ע
        /// </summary>
        /// <param name="currentpage"></param>
        /// <param name="pagesize"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        IEnumerable<UserAllListFocusOutPutDto> GetUserAllListFocusByPage(string currentpage,
            string pagesize, string userid);


        Task<List<Models.mesEmployee>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc);

        string GetServerData();

    }
}