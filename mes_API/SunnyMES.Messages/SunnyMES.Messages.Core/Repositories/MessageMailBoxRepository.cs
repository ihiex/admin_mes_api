using System;

using SunnyMES.Commons.Repositories;
using SunnyMES.Messages.IRepositories;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.Repositories
{
    /// <summary>
    /// 仓储接口的实现
    /// </summary>
    public class MessageMailBoxRepository : BaseRepository<MessageMailBox, string>, IMessageMailBoxRepository
    {
		public MessageMailBoxRepository()
        {
            this.tableName = "API_MessageMailBox";
            this.primaryKey = "Id";
        }
    }
}