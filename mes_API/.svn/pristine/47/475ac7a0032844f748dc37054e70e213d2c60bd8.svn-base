using Dapper;
using SqlSugar;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class OrganizeRepository : BaseRepository<Organize, string>, IOrganizeRepository
    {
        public OrganizeRepository()
        {
        }

        public OrganizeRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 获取根节点组织
        /// </summary>
        /// <param name="id">组织Id</param>
        /// <returns></returns>
        public Organize GetRootOrganize(string id)
        {
            var sb = new StringBuilder(";WITH ");
            if(dbConnectionOptions.DatabaseType == DatabaseType.MySql)
            {
                sb.Append(" Recursive ");
            }
            sb.Append(" T AS (");
            sb.Append(" SELECT Id, ParentId, FullName, Layers FROM API_Organize");
            sb.AppendFormat(" WHERE Id = '{0}'",id);
            sb.Append(" UNION ALL ");
            sb.Append(" SELECT A.Id, A.ParentId, A.FullName, A.Layers FROM API_Organize AS A JOIN T AS B ON A.Id = B.ParentId ) SELECT* FROM T ORDER BY Layers");
            return  DapperConn.QueryFirstOrDefault<Organize>(sb.ToString());
        }

        public async Task<List<API_Organize>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_Organize> list = new List<API_Organize>();

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

            list = await SuDB.Queryable<API_Organize>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }

    }
}