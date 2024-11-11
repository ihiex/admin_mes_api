using System;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IServices;
using SunnyMES.Messages.IRepositories;
using SunnyMES.Messages.IServices;
using SunnyMES.Messages.Dtos;
using SunnyMES.Messages.Models;
using System.Collections.Generic;

namespace SunnyMES.Messages.Services
{
    /// <summary>
    /// 服务接口实现
    /// </summary>
    public class MessageTemplatesService: BaseService<MessageTemplates,MessageTemplatesOutputDto, string>, IMessageTemplatesService
    {
		private readonly IMessageTemplatesRepository _repository;
        private readonly ILogService _logService;
        public MessageTemplatesService(IMessageTemplatesRepository repository,ILogService logService) : base(repository)
        {
			_repository=repository;
			_logService=logService;
        }

        /// <summary>
        /// 根据消息类型查询消息模板
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <returns></returns>
        public MessageTemplates GetByMessageType(string messageType)
        {
            return _repository.GetWhere("messageType='" + messageType + "'");
        }

        /// <summary>
        /// 根据用户查询微信小程序订阅消息模板列表，关联用户订阅表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public List<MemberMessageTemplatesOuputDto> ListByUseInWxApplet(string userId)
        {
            return _repository.ListByUseInWxApplet(userId);
        }
    }
}