using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IUserLogOnRepository:IRepository<UserLogOn, string>
    {

        /// <summary>
        /// ���ݻ�ԱID��ȡ�û���¼��Ϣʵ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserLogOn GetByUserId(string userId);
    }
}