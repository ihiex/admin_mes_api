using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public interface ILogService:IService<Log, LogOutputDto, long>
    {
        /// <summary>
        /// 根据相关信息，写入用户的操作日志记录
        /// 主要用于写数据库日志
        /// </summary>
        /// <param name="tableName">操作表名称</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="note">操作详细表述</param>
        /// <returns></returns>
         bool OnOperationLog(string tableName, string operationType, string note);

        /// <summary>
        /// 根据相关信息，写入用户的操作日志记录
        /// 主要用于写操作模块日志
        /// </summary>
        /// <param name="module">操作模块名称</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="note">操作详细表述</param>
        /// <param name="currentUser">操作用户</param>
        /// <returns></returns>
        bool OnOperationLog(string module, string operationType,  string note, YuebonCurrentUser currentUser);
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        Task<PageResult<API_Log>> FindWithPagerSearchAsync(SearchLogModel search);

        
    }
}
