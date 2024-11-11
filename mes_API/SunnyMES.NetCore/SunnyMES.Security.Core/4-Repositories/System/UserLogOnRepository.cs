using Dapper;
using System;
using System.Data;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Options;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class UserLogOnRepository : BaseRepository<UserLogOn, string>, IUserLogOnRepository
    {
        public UserLogOnRepository()
        {
        }

        public UserLogOnRepository(IDbContextCore dbContext) : base(dbContext)
        {
         
        }



        /// <summary>
        /// ���ݻ�ԱID��ȡ�û���¼��Ϣʵ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserLogOn GetByUserId(string userId)
        {
            string sql = @"SELECT * FROM API_UserLogOn t WHERE t.UserId = @UserId";
            return DapperConn.QueryFirst<UserLogOn>(sql, new { UserId = userId });
        }
    }
}