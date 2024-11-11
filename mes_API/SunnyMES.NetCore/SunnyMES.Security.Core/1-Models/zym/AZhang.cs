using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SunnyMES.Security.Models
{
    /// <summary>
    /// ，数据实体对象
    /// </summary>
    [Table("API_AZhang")]
    [Serializable]
    // public class AZhang:BaseEntity<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    public class AZhang : BaseEntity<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    {

        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
	    public AZhang()
        {

        }

        /// <summary>
        /// 应用Id
        /// </summary>
        public virtual string AppId { get; set; }

        /// <summary>
        /// 应用密钥
        /// </summary>
        public virtual string AppSecret { get; set; }

        /// <summary>
        /// 消息加解密密钥
        /// </summary>
        public virtual string EncodingAESKey { get; set; }

        /// <summary>
        /// 授权请求地址url
        /// </summary>
        public virtual string RequestUrl { get; set; }



        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Data1 { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Data2 { get; set; }



        /// <summary>
        /// Token令牌
        /// </summary>

        public virtual string Token { get; set; }
        /// <summary>
        /// 是否开启消息加解密
        /// </summary>

        public virtual bool IsOpenAEKey { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]

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
