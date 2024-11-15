﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 行政区域表，数据实体对象
    /// </summary>
    [Table("API_Area")]
    [Serializable]
    public class Area:BaseEntity<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    { 
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
	    public Area()
		{
            

 		}

        #region Property Members
        ///// <summary>
        ///// 主键
        ///// </summary>
        //[MaxLength(50)]
        //[ExplicitKey]
        //public virtual string Id { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        [MaxLength(50)]
        public virtual string ParentId { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public virtual int? Layers { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [MaxLength(50)]
        public virtual string EnCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        public virtual string FullName { get; set; }

        /// <summary>
        /// 简拼
        /// </summary>
        [MaxLength(200)]
        public virtual string SimpleSpelling { get; set; }

        /// <summary>
        /// 父级路径
        /// </summary>
        [MaxLength(600)]
        public virtual string FullIdPath { get; set; }

        /// <summary>
        /// 是否是最后一级
        /// </summary>
        public virtual bool IsLast { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public virtual int? SortCode { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        public virtual bool? DeleteMark { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        public virtual bool EnabledMark { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public virtual DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 创建用户主键
        /// </summary>
        [MaxLength(50)]
        public virtual string CreatorUserId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public virtual DateTime? LastModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户
        /// </summary>
        [MaxLength(50)]
        public virtual string LastModifyUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public virtual DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 删除用户
        /// </summary>
        [MaxLength(50)]
        public virtual string DeleteUserId { get; set; }
        #endregion

    }
}