using Dapper;
using System;
using System.Data;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SqlSugar;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunnyMES.Security.Repositories
{
    public class UploadFileRepository : BaseRepository<UploadFile, string>, IUploadFileRepository
    {
        public UploadFileRepository()
        {
        }

        public UploadFileRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 根据应用Id和应用标识批量更新数据
        /// </summary>
        /// <param name="beLongAppId">更新后的应用Id</param>
        /// <param name="oldBeLongAppId">更新前旧的应用Id</param>
        /// <param name="belongApp">应用标识</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool UpdateByBeLongAppId(string beLongAppId, string oldBeLongAppId, string belongApp = null, IDbTransaction trans = null)
        {
            try
            {
                trans = DapperConn.BeginTransaction();
                string sqlStr = string.Format("update {0} set beLongAppId='{1}' where beLongAppId='{2}'", this.tableName, beLongAppId, oldBeLongAppId);
                if (!string.IsNullOrEmpty(belongApp))
                {
                    sqlStr = string.Format(" and BelongApp='{0}'", belongApp);
                }
                int num = DapperConn.Execute(sqlStr, null, trans);
                trans.Commit();
                return num >= 0;
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
        }

    }
}
