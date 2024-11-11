using System;
using SunnyMES.Commons.IServices;
using SunnyMES.Messages.Dtos;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.IServices
{
    /// <summary>
    /// 定义服务接口
    /// </summary>
    public interface IMessageMailBoxService:IService<MessageMailBox,MessageMailBoxOutputDto, string>
    {
    }
}
