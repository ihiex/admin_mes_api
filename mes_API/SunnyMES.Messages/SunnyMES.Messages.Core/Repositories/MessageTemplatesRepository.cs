using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Repositories;
using SunnyMES.Messages.Dtos;
using SunnyMES.Messages.IRepositories;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.Repositories
{
    /// <summary>
    /// 仓储接口的实现
    /// </summary>
    public class MessageTemplatesRepository : BaseRepository<MessageTemplates, string>, IMessageTemplatesRepository
    {
		public MessageTemplatesRepository()
        {
            this.tableName = "API_MessageTemplates";
            this.primaryKey = "Id";
        }

        /// <summary>
        /// 根据用户查询微信小程序订阅消息模板列表，关联用户订阅表
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public List<MemberMessageTemplatesOuputDto> ListByUseInWxApplet(string userId)
        {
            string sqlStr = @"select a.*,b.Id as MemberSubscribeMsgId,b.SubscribeStatus as SubscribeStatus  from API_MessageTemplates as a 
LEFT join API_MemberSubscribeMsg as b on a.Id = b.MessageTemplateId and a.UseInWxApplet =1 and b.SubscribeUserId='" + userId + "'  where  a.WxAppletSubscribeTemplateId is not null";

            return DapperConn.Query<MemberMessageTemplatesOuputDto>(sqlStr).AsToList();
        }
    }
}