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
    public class MessageMailBoxService: BaseService<MessageMailBox,MessageMailBoxOutputDto, string>, IMessageMailBoxService
    {
		private readonly IMessageMailBoxRepository _repository;
        private readonly ILogService _logService;
        public MessageMailBoxService(IMessageMailBoxRepository repository,ILogService logService) : base(repository)
        {
			_repository=repository;
			_logService=logService;
        }
    }
}