using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输入对象模型
    /// </summary>
    [AutoMap(typeof(Log))]
    [Serializable]
    public class LogInputDto: IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string OrganizeId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string IPAddressName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? Result { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string CreatorUserId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string LastModifyUserId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string DeleteUserId { get; set; }


        /// <summary>
        /// 老系统用户ID
        /// </summary>
        [MaxLength(50)]
        public  string UserID { get; set; }

        /// <summary>
        /// 老系统用户密码
        /// </summary>
        [MaxLength(200)]
        public  string Password { get; set; }

        /// <summary>
        /// 老系统Lastname
        /// </summary>
        [MaxLength(500)]
        public  string Lastname { get; set; }

        /// <summary>
        /// 老系统Firstname
        /// </summary>
        [MaxLength(500)]
        public  string Firstname { get; set; }

    }
}
