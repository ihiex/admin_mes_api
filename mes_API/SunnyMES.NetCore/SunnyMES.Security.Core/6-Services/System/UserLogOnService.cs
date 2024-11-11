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
        /// ���ݻ�ԱID��ȡ�û���¼��Ϣʵ��
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
       public UserLogOn GetByUserId(string userId)
        {
           return _userLogOnRepository.GetByUserId(userId);
        }

        /// <summary>
        /// ���ݻ�ԱID��ȡ�û���¼��Ϣʵ��
        /// </summary>
        /// <param name="info">����������Ϣ</param>
        /// <param name="userId">�û�Id</param>
        /// <returns></returns>
        public async Task<bool> SaveUserTheme(UserThemeInputDto info,string userId)
        {
            string themeJsonStr = info.ToJson();
            string where = $"UserId='{userId}'";
            return await _userLogOnRepository.UpdateTableFieldAsync("Theme",themeJsonStr, where);
        }
    }
}