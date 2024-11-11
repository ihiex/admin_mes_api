using Dapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    /// <summary>
    /// 应用仓储实现
    /// </summary>
    public class APPRepository : BaseRepository<APP,string>, IAPPRepository
    {
        public APPRepository()
        {
        }
        public APPRepository(IDbContextCore context) : base(context)
        {
        }
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        public APP GetAPP(string appid, string secret)
        {
            string sql = @"SELECT * FROM API_APP t WHERE t.AppId = @AppId and AppSecret=@AppSecret and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<APP>(sql, new { AppId = appid, AppSecret = secret });
        }

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        public APP GetAPP(string appid)
        {
            string sql = @"SELECT * FROM API_APP t WHERE t.AppId = @AppId and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<APP>(sql, new { AppId = appid });

        }
        public IList<AppOutputDto> SelectApp()
        {
            const string query = @"select a.*,u.id as Id,u.NickName,u.Account,u.HeadIcon FROM API_APP a,API_User u where a.CreatorUserId=u.Id ";
            return DapperConnRead.Query<AppOutputDto, User, AppOutputDto>(query, (app, user) => { app.UserInfo = user; return app; }, null, splitOn: "Id").ToList<AppOutputDto>();
        }


        public async Task<List<API_APP>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_APP> list = new List<API_APP>();

            if (HasInjectionData(condition))
            {
                Log4NetHelper.Info(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            if (string.IsNullOrEmpty(condition))
            {
                condition = "1=1";
            }
            if (desc)
            {
                fieldToSort += " desc";
            }
            RefAsync<int> totalCount = 0;

            list = await SuDB.Queryable<API_APP>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }
    }
}