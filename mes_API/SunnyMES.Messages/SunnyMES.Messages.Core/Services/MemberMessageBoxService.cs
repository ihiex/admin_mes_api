using System;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IServices;
using SunnyMES.Messages.IRepositories;
using SunnyMES.Messages.IServices;
using SunnyMES.Messages.Dtos;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.Services
{
    /// <summary>
    /// 服务接口实现
    /// </summary>
    public class MemberMessageBoxService: BaseService<MemberMessageBox,MemberMessageBoxOutputDto, string>, IMemberMessageBoxService
    {
		private readonly IMemberMessageBoxRepository _repository;
        private readonly ILogService _logService;
        public MemberMessageBoxService(IMemberMessageBoxRepository repository,ILogService logService) : base(repository)
        {
			_repository=repository;
			_logService=logService;
        }

        public int GetTotalCounts(int isread, string userid)
        {
            return _repository.GetTotalCounts(isread,userid);
        }

        public bool UpdateIsReadStatus(string id, int isread, string userid)
        {
            return _repository.UpdateIsReadStatus(id, isread, userid);
        }
    }
}