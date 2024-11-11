using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.Public;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Models.Public;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security._4_Repositories.Public
{
    public class PublicPropertiesRepository : BaseCommonRepository<string>, IPublicPropertiesRepository
    {
        public PublicPropertiesRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        public async Task<bool> CheckIsExists(string sql)
        {            
            var exist = await DapperConn.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
            return exist is not null;
        }

        public async Task<long> CloneAsync(SC_IdDescTab idDescTab)
        {
            var existId = await CheckIsIdentityAsync(idDescTab.TableName);
            var dc = new Dictionary<string, object>();
            if (!existId)
            {
                dc.Add("ID", GetMaxId(idDescTab.TableName) + 1);
            }
            dc.Add("Description", idDescTab.Description);

            return await SqlSugarHelper.Db.Insertable(dc).AS(idDescTab.TableName).ExecuteCommandAsync();
        }

        public async Task<long> DeleteAsync(SC_IdDescTab idDescTab)
        {
            return await SqlSugarHelper.Db.Deleteable<object>().AS(idDescTab.TableName).Where("ID=@ID", new {ID=idDescTab.ID}).ExecuteCommandAsync();
        }

        public async Task<List<SC_IdDesc>> GetCommonTabList()
        {
            string sql = $@"
            SELECT b.name
            INTO #tmpTables
            FROM sys.columns a
            JOIN sys.sysobjects b ON a.object_id = b.id AND b.type = 'U'
            GROUP BY b.name
            HAVING COUNT(*) = 2

            SELECT a.name,c.object_id, c.name columnName,ROW_NUMBER() OVER(PARTITION BY c.object_id ORDER BY c.name) rowIndex
            INTO #tmpTableColumn
            FROM #tmpTables a
            JOIN sys.sysobjects b ON b.name = a.name
            JOIN sys.columns c ON c.object_id = b.id AND c.name IN ( 'Description', 'ID')

            SELECT object_id ID, name Description
            FROM #tmpTableColumn 
            WHERE rowIndex = 2	
            GROUP BY object_id,name
            ";
            return await SqlSugarHelper.Db.Ado.SqlQueryAsync<SC_IdDesc>(sql);
        }

        public async Task<long> InsertAsync(SC_IdDescTab idDescTab)
        {
            var existId = await CheckIsIdentityAsync(idDescTab.TableName);
            var dc = new Dictionary<string, object>();
            if (!existId)
            {
                int maxId = GetMaxId(idDescTab.TableName);
                dc.Add("ID", maxId + 1);
            }
            dc.Add("Description", idDescTab.Description);

            return await SqlSugarHelper.Db.Insertable(dc).AS(idDescTab.TableName).ExecuteCommandAsync();
        }

        public async Task<long> UpdateAsync(SC_IdDescTab idDescTab)
        {
            var dc = new Dictionary<string, object>();
            dc.Add("ID", idDescTab.ID);
            dc.Add("Description", idDescTab.Description);

            return await SqlSugarHelper.Db.Updateable(dc).AS(idDescTab.TableName).WhereColumns("ID").ExecuteCommandAsync();
        }
        private async Task<bool> CheckIsIdentityAsync(string tablename)
        {
            string sql = $@"
                    SELECT 
                        COLUMN_NAME 
                    FROM 
                        INFORMATION_SCHEMA.COLUMNS 
                    WHERE 
                        TABLE_NAME = '{tablename}' AND 
                        COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1;";

            var v = await SqlSugarHelper.Db.Ado.GetScalarAsync(sql);
            return v is not null && v.ToString() == "ID";
        }

        private  int GetMaxId(string tableName)
        {
            string sql = $@"SELECT MAX(ID) FROM {tableName}";
            var v =  SqlSugarHelper.Db.Ado.GetScalar(sql);
            return v.ToString().ToInt();
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="condition">查询的条件</param>
        /// <param name="info">分页实体</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <param name="desc">排序方式 true为desc，false为asc</param>
        /// <param name="tableName"></param>
        /// <returns>指定对象的集合</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<SC_IdDesc>> FindWithPagerAsync(string condition, PagerInfo info, string fieldToSort, bool desc, string tableName)
        {

            List<SC_IdDesc> list = new List<SC_IdDesc>();

            if (HasInjectionData(condition))
            {
                Log4NetHelper.Info(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
            }
            if (string.IsNullOrEmpty(condition))
            {
                condition = "1=1";
            }

            PagerHelper pagerHelper = new PagerHelper(tableName, this.selectedFields, string.IsNullOrEmpty(fieldToSort) ? "ID" : fieldToSort, info.PageSize, info.CurrentPageIndex <= 0 ? 1 : info.CurrentPageIndex, desc, condition);

            string pageSql = pagerHelper.GetPagingSql(true, dbConnectionOptions.DatabaseType);
            pageSql += ";" + pagerHelper.GetPagingSql(false, dbConnectionOptions.DatabaseType);
            var reader = await DapperConn.QueryMultipleAsync(pageSql);
            info.RecordCount = reader.ReadFirst<int>();
            list = reader.Read<SC_IdDesc>().ToList();
            return list;
        }

    }
}
