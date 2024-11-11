﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 数据字典选项主表，数据实体对象
    /// </summary>
    [Table("API_Items")]
    [Serializable]
    public class Items : BaseEntity<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    { 
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
	    public Items()
		{
           
 		}

        #region Property Members


        /// <summary>
        /// 父级
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public virtual string EnCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// 树型
        /// </summary>
        public virtual bool IsTree { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public virtual int? Layers { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public virtual int? SortCode { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }


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