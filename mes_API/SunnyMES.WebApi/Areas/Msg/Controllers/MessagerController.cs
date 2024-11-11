﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.Containers;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Messages.Application;

namespace SunnyMES.WebApi.Areas.Msg
{
    /// <summary>
    /// 消息推送接口
    /// </summary>
    [Route("api/Msg/[controller]")]
    [ApiController]
    public class MessagerController : ApiController
    {
        /// <summary>
        /// 拨打电话推送消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tell">接受者电话号码</param>
        /// <param name="tell2">推送内容电话号码</param>
        /// <returns></returns>
        [HttpGet("MakePhoneCallNotice")]
        [YuebonAuthorize("")]
        [NoPermissionRequired]
        public IActionResult SendMakePhoneCallNotice(string userId, string tell, string tell2)
        {
            CommonResult result = new CommonResult();
            try
            {
                tell = tell.Replace(" ", "");
                string message = "{\"telphone\":\"" + tell2 + "\"}";
                result = Messenger.SendMakePhoneCallNotice(userId, tell, message, "");
            }
            catch (Exception ex)
            {
                result.ResultMsg = ex.Message;
                Log4NetHelper.Error("更新用户电话号码 UpdatePhone", ex);
            }
            return ToJsonContent(result);
        }
    }
}