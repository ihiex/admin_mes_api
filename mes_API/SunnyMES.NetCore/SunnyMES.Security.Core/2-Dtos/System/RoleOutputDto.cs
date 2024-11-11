using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型
    /// </summary>
    [Serializable]
    public class RoleOutputDto
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string OrganizeId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public virtual string OrganizeName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string EnCode { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string FullName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string Type { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? AllowEdit { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? AllowDelete { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? DeleteMark { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

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
        [MaxLength(500)]
        public string DeleteUserId { get; set; }


    }
}
