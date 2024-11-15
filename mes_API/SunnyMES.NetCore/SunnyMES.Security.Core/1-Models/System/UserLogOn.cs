﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 用户登录信息表，数据实体对象
    /// </summary>
    [Table("API_UserLogOn")]
    [Serializable]
    public class UserLogOn: BaseEntity<string>
    { 
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
	    public UserLogOn()
		{
        }

        #region Property Members


        /// <summary>
        /// 用户主键
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public virtual string UserPassword { get; set; }

        /// <summary>
        /// 用户秘钥
        /// </summary>
        public virtual string UserSecretkey { get; set; }

        /// <summary>
        /// 允许登录时间开始
        /// </summary>
        public virtual DateTime? AllowStartTime { get; set; }

        /// <summary>
        /// 允许登录时间结束
        /// </summary>
        public virtual DateTime? AllowEndTime { get; set; }

        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
        public virtual DateTime? LockStartDate { get; set; }

        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
        public virtual DateTime? LockEndDate { get; set; }

        /// <summary>
        /// 第一次访问时间
        /// </summary>
        public virtual DateTime? FirstVisitTime { get; set; }

        /// <summary>
        /// 上一次访问时间
        /// </summary>
        public virtual DateTime? PreviousVisitTime { get; set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public virtual DateTime? LastVisitTime { get; set; }

        /// <summary>
        /// 最后修改密码日期
        /// </summary>
        public virtual DateTime? ChangePasswordDate { get; set; }

        /// <summary>
        /// 允许同时有多用户登录
        /// </summary>
        public virtual bool? MultiUserLogin { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public virtual int? LogOnCount { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public virtual bool? UserOnLine { get; set; }

        /// <summary>
        /// 密码提示问题
        /// </summary>
        public virtual string Question { get; set; }

        /// <summary>
        /// 密码提示答案
        /// </summary>
        public virtual string AnswerQuestion { get; set; }

        /// <summary>
        /// 是否访问限制
        /// </summary>
        public virtual bool? CheckIPAddress { get; set; }

        /// <summary>
        /// 系统语言
        /// </summary>
        public virtual string Language { get; set; }

        /// <summary>
        /// 系统样式
        /// </summary>
        public virtual string Theme { get; set; }

        #endregion

    }
}