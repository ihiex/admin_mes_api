using Dapper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    /// <summary>
    /// 系统类型，也是子系统
    /// </summary>
    public class SystemTypeRepository : BaseRepository<SystemType, string>, ISystemTypeRepository
    {
        public SystemTypeRepository()
        {
        }

        public SystemTypeRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 根据系统编码查询系统对象
        /// </summary>
        /// <param name="appkey">系统编码</param>
        /// <returns></returns>
        public SystemType GetByCode(string appkey)
        {
            string sql = @"SELECT * FROM " + this.tableName + " t WHERE t.EnCode = @EnCode";
            return DapperConn.QueryFirstOrDefault<SystemType>(sql, new { EnCode = appkey });
        }


        public async Task<List<API_SystemType>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_SystemType> list = new List<API_SystemType>();

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

            list = await SuDB.Queryable<API_SystemType>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }
    }
}
