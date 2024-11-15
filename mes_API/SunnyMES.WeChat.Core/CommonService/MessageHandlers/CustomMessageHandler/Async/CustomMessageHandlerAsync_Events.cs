﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Senparc.NeuChar.Context;
using Senparc.Weixin.Exceptions;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using SunnyMES.WeChat.CommonService.Download;
using SunnyMES.WeChat.CommonService.Utilities;
using Senparc.NeuChar.Entities;
using Microsoft.AspNetCore.Http;


namespace SunnyMES.WeChat.CommonService.CustomMessageHandler
{
    /// <summary>
    /// 自定义MessageHandler
    /// </summary>
    public partial class CustomMessageHandler
    {
        public override Task<IResponseMessageBase> OnEvent_ClickRequestAsync(RequestMessageEvent_Click requestMessage)
        {
            return Task.Factory.StartNew<IResponseMessageBase>(() =>
            {
                var syncResponseMessage = OnEvent_ClickRequest(requestMessage);//这里为了保持Demo的连贯性，结果先从同步方法获取，实际使用过程中可以全部直接定义异步方法
                //常识获取Click事件的同步方法
                if (syncResponseMessage is ResponseMessageText)
                {
                    var textResponseMessage = syncResponseMessage as ResponseMessageText;
                    textResponseMessage.Content += "\r\n\r\n  -- 来自【异步MessageHandler】的回复";
                }

                return syncResponseMessage;
            });
        }
    }
}