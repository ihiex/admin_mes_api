using System;
using System.Threading.Tasks;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class UserLogOnService: BaseService<UserLogOn, UserLogOnOutputDto, string>, IUserLogOnService
    {
        private readonly IUserLogOnRepository _userLogOnRepository;
        private readonly ILogService _logService;
        public UserLogOnService(IUserLogOnRepository repository, ILogService logService) : base(repository)
        {
            _userLogOnRepository = repository;
            _logService = logService;
        }

        /// <summary>
        /// 根据会员ID获取用户登录信息实体
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
       public UserLogOn GetByUserId(string userId)
        {
           return _userLogOnRepository.GetByUserId(userId);
        }

        /// <summary>
        /// 根据会员ID获取用户登录信息实体
        /// </summary>
        /// <param name="info">主题配置信息</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<bool> SaveUserTheme(UserThemeInputDto info,string userId)
        {
            string themeJsonStr = info.ToJson();
            string where = $"UserId='{userId}'";
            return await _userLogOnRepository.UpdateTableFieldAsync("Theme",themeJsonStr, where);
        }
    }
}