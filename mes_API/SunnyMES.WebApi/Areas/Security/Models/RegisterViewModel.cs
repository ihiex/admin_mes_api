﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnyMES.WebApi.Areas.Security.Models
{
    /// <summary>
    /// 注册信息
    /// </summary>
    [Serializable]
    public class RegisterViewModel
    {
        /// <summary>
        /// 设置账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 设置邮箱
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// 设置密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 设置验证码
        /// </summary>
        public string VerificationCode{ get; set; }
        /// <summary>
        /// 设置验证码Key
        /// </summary>
        public string VerifyCodeKey { get; set; }
        /// <summary>
        /// 用户类别
        /// </summary>
        public string UserType { get; set; }
    }
}
