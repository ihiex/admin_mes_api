using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;


namespace SunnyMES.Security.Models
{
    /// <summary>
    /// ，数据实体对象
    /// </summary>
    [Table("API_ZYM")]
    [Serializable]
    public class BZhang : BaseEntityGeneric<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    {

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Data1 { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Data2 { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [Description("描述")]
        [MaxLength(200)]
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

    }
}
