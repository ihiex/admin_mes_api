using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Dtos;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.Dtos
{
    /// <summary>
    /// 用户登录信息输入对象模型
    /// </summary>
    [AutoMap(typeof(TenantLogon))]
    [Serializable]
    public class TenantLogonInputDto: IInputDto<string>
    {
        /// <summary>
        /// 设置或获取
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 设置或获取密码
        /// </summary>
        public string TenantPassword { get; set; }

        /// <summary>
        /// 设置或获取加密密钥
        /// </summary>
        public string TenantSecretkey { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? AllowStartTime { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? AllowEndTime { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? LockStartDate { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? LockEndDate { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? FirstVisitTime { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? PreviousVisitTime { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? LastVisitTime { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public DateTime? ChangePasswordDate { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public bool? MultiUserLogin { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public int? LogOnCount { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public bool? TenantOnLine { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public string AnswerQuestion { get; set; }

        /// <summary>
        /// 设置或获取
        /// </summary>
        public bool? CheckIPAddress { get; set; }

        /// <summary>
        /// 设置或获取软件语言
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 设置或获取软件风格设置信息
        /// </summary>
        public string Theme { get; set; }


    }
}
