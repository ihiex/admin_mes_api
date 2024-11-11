using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Models;
using SunnyMES.Messages.Models;

namespace SunnyMES.Messages.Dtos
{
    /// <summary>
    /// 输入对象模型
    /// </summary>
    [AutoMap(typeof(MemberSubscribeMsg))]
    [Serializable]
    public class MemberSubscribeMsgInputDto: IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取订阅用户
        /// </summary>
        public string SubscribeUserId { get; set; }

        /// <summary>
        /// 设置或获取订阅类型：SMS短信，WxApplet 微信小程序，InnerMessage站内消息 ，Email邮件通知
        /// </summary>
        public string SubscribeType { get; set; }

        /// <summary>
        /// 设置或获取消息模板Id主键
        /// </summary>
        public string MessageTemplateId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string SubscribeTemplateId { get; set; }

        /// <summary>
        /// 设置或获取订阅状态
        /// </summary>
        public string SubscribeStatus { get; set; }


    }
}
