using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Net;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LogService : BaseService<Log, LogOutputDto, long>, ILogService
    {
        private readonly ILogRepository _iLogRepository;
        private readonly IUserRepository _iuserRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userRepository"></param>
        public LogService(ILogRepository repository, IUserRepository userRepository) : base(repository)
        {
            _iLogRepository = repository;
            _iuserRepository = userRepository;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public async Task<PageResult<API_Log>> FindWithPagerSearchAsync(SearchLogModel search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.CreatorTime1))
            {
                where += " and CreatorTime >='"+ search.CreatorTime1.ToDateTime()+ "'";
            }
            if (!string.IsNullOrEmpty(search.CreatorTime2))
            {
                where += " and CreatorTime <='" + search.CreatorTime2.ToDateTime() + "'";
            }
            if (!string.IsNullOrEmpty(search.Filter.Type))
            {
                where += " and Type='"+ search.Filter.Type + "'";
            }
            if (!string.IsNullOrEmpty(search.Filter.IPAddress))
            {
                where += string.Format(" and IPAddress = '{0}'", search.Filter.IPAddress);
            };
            if (!string.IsNullOrEmpty(search.Filter.Account))
            {
                where += string.Format(" and Account = '{0}'", search.Filter.Account);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            //List<Log> list = await _iLogRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);

            List<API_Log> list = await _iLogRepository.FindWithPagerMyAsync(where, pagerInfo, search.Sort, order);

            PageResult<API_Log> pageResult = new PageResult<API_Log>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<API_Log>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }

        /// <summary>
        /// 根据相关信息，写入用户的操作日志记录
        /// </summary>
        /// <param name="tableName">操作表名称</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="note">操作详细表述</param>
        /// <returns></returns>
        public bool OnOperationLog(string tableName, string operationType, string note)
        {
            try
            {
                //虽然实现了这个事件，但是我们还需要判断该表是否在配置表里面，如果不在，则不记录操作日志。
                //var identities = _httpContextAccessor.HttpContext.User.Identities;
                if (HttpContextHelper.HttpContext == null)
                {
                    return false;
                }
                var identities =HttpContextHelper.HttpContext.User.Identities;
                var claimsIdentity = identities.First<ClaimsIdentity>();
                List<Claim> claimlist = claimsIdentity.Claims as List<Claim>;
                string userId = claimlist[0].Value;
                YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                YuebonCurrentUser CurrentUser = new YuebonCurrentUser();
                var user = yuebonCacheHelper.Get("login_user_" + userId).ToJson().ToObject<YuebonCurrentUser>();
                if (user != null)
                {
                    CurrentUser = user;
                    bool insert = operationType == DbLogType.Create.ToString(); ;//&& settingInfo.InsertLog;
                    bool update = operationType == DbLogType.Update.ToString();// && settingInfo.UpdateLog;
                    bool delete = operationType == DbLogType.Delete.ToString();// && settingInfo.DeleteLog;
                    bool deletesoft = operationType == DbLogType.DeleteSoft.ToString();// && settingInfo.DeleteLog;
                    bool exception = operationType == DbLogType.Exception.ToString();// && settingInfo.DeleteLog;
                    bool sql = operationType == DbLogType.SQL.ToString();// && settingInfo.DeleteLog;

                    if (insert || update || delete || deletesoft || exception || sql)
                    {
                        Log info = new Log();
                        info.ModuleName = tableName;
                        info.Type = operationType;
                        info.Description = note;
                        info.Date = info.CreatorTime = DateTime.Now;
                        info.CreatorUserId = CurrentUser.UserId;
                        info.Account = CurrentUser.Account;
                        info.NickName = CurrentUser.NickName;
                        info.OrganizeId = CurrentUser.OrganizeId;
                        info.IPAddress = CurrentUser.CurrentLoginIP;
                        info.IPAddressName = CurrentUser.IPAddressName;
                        info.Result = true;
                        long lg = _iLogRepository.Insert(info);
                        if (lg > 0)
                        {
                            return true;
                        }
                    }
                }
            }catch(Exception ex)
            {
                Log4NetHelper.Error("",ex);
                return false;
            }
            return false;
        }


        /// <summary>
        /// 根据相关信息，写入用户的操作日志记录
        /// 主要用于写操作模块日志
        /// </summary>
        /// <param name="module">操作模块名称</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="note">操作详细表述</param>
        /// <param name="currentUser">操作用户</param>
        /// <returns></returns>
        public bool OnOperationLog(string module, string operationType, string note, YuebonCurrentUser currentUser)
        {
            //虽然实现了这个事件，但是我们还需要判断该表是否在配置表里面，如果不在，则不记录操作日志。
            //OperationLogSettingInfo settingInfo = BLLFactory<OperationLogSetting>.Instance.FindByTableName(tableName, trans);
            
            if (currentUser != null)
            {
                bool login = operationType == DbLogType.Login.ToString();
                bool visit = operationType == DbLogType.Visit.ToString();
                bool exit = operationType == DbLogType.Exit.ToString();
                bool other = operationType == DbLogType.Other.ToString();
                bool insert = operationType == DbLogType.Create.ToString();
                bool update = operationType == DbLogType.Update.ToString();
                bool delete = operationType == DbLogType.Delete.ToString();
                bool deletesoft = operationType == DbLogType.DeleteSoft.ToString();
                bool exception = operationType == DbLogType.Exception.ToString();
                if (login|| visit|| exit|| other||insert || update || delete || deletesoft || exception)
                {
                    Log info = new Log();
                    info.ModuleName = module;
                    info.Type = operationType;
                    info.Description = note;
                    info.Date = info.CreatorTime = DateTime.Now;
                    info.CreatorUserId = currentUser.UserId;
                    info.Account = currentUser.Account;
                    info.NickName = currentUser.NickName;
                    info.OrganizeId = currentUser.OrganizeId;
                    info.IPAddress = currentUser.CurrentLoginIP;
                    info.IPAddressName = IpAddressUtil.GetCityByIp(currentUser.CurrentLoginIP);
                    info.Result = true;
                    long lg = _iLogRepository.Insert(info);
                    if (lg > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}