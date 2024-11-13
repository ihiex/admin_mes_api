using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using SunnyMES.Commons.Repositories;
using SunnyMES.Messages.IRepositories;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.Repositories
{
    /// <summary>
    /// 仓储接口的实现
    /// </summary>
    public class MemberMessageBoxRepository : BaseRepository<MemberMessageBox, string>, IMemberMessageBoxRepository
    {
		public MemberMessageBoxRepository()
        {
            this.tableName = "API_MemberMessageBox";
            this.primaryKey = "Id";
        }

        /// <summary>
        /// 更新已读状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateIsReadStatus(string id, int isread, string userid)
        {
            string strwhere = " Accepter='" + userid + "' ";
            if (!string.IsNullOrEmpty(id))
            {
                strwhere += " and Id='" + id + "' ";
            }

            string sql = @"update API_MemberMessageBox set IsRead=" + isread +
                ", ReadDate='" + DateTime.Now.ToShortDateString() + "' ";
            if (strwhere != "")
            {
                sql += sql + " where " + strwhere;
            }

            return DapperConn.Execute(sql) > 0 ? true : false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isread">2:全部，0：未读，1：已读</param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetTotalCounts(int isread, string userid)
        {
            string strwhere = " Accepter='" + userid + "' ";
            if (isread != 2)
            {
                strwhere += " and IsRead=" + isread;
            }

            string sql = @"select * from API_MemberMessageBox ";
            if (strwhere != "")
            {
                sql = sql + " where " + strwhere;
            }

            IEnumerable<MemberMessageBox> list = DapperConn.Query<MemberMessageBox>(sql);

            if (list != null)
            {
                return list.AsList().Count;
            }
            else
            {
                return 0;
            }
        }
    }
}