using API_MSG;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// �û�����ӿ�
    /// </summary>
    public interface IUserService:IService<User, UserOutputDto, string>
    {
        /// <summary>
        /// �û���½��֤��
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="password">���루��һ��md5���ܺ�</param>
        /// <returns>��֤�ɹ������û�ʵ�壬��֤ʧ�ܷ���null|��ʾ��Ϣ</returns>
        Task<Tuple<User, string>> Validate(string userName, string password);
        /// <summary>
        /// ��ȡ�û�
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<Tuple<User, string>> Get_My_User(string userName);

        Task<Tuple<User, string>> Validate(string S_userName, string S_password,
            int I_LineID, int I_StationID, MSG_Login v_MSG_Login);

        /// <summary>
        /// �û���½��֤��
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="password">���루��һ��md5���ܺ�</param>
        /// <param name="userType">�û�����</param>
        /// <returns>��֤�ɹ������û�ʵ�壬��֤ʧ�ܷ���null|��ʾ��Ϣ</returns>
        Task<Tuple<User, string>> Validate(string userName, string password, UserType userType);

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
        /// <param name="userOpenIds"></param>
        /// <param name="trans"></param>
        bool Insert(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds,IDbTransaction trans = null);
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
        /// ΢��ע����ͨ��Ա�û�
        /// </summary>
        /// <param name="userInPut">����������</param>
        /// <returns></returns>
       bool CreateUserByWxOpenId(UserInputDto userInPut);
        /// <summary>
        /// �����û�
        /// </summary>
        /// <param name="userInPut"></param>
        /// <returns></returns>
        bool UpdateUserByOpenId(UserInputDto userInPut);

        /// <summary>
        /// ����������ѯ���ݿ�,�����ض��󼯺�(���ڷ�ҳ������ʾ)
        /// </summary>
        /// <param name="search">��ѯ������</param>
        /// <returns>ָ������ļ���</returns>
        Task<PageResult<UserOutputDto>> FindWithPagerSearchAsync(SearchUserModel search, YuebonCurrentUser Current_User);

        Task<string> SetSyncUserData(string S_EmployeeID);

        string GetServerData();
    }
}