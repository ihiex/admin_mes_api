using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;

namespace SunnyMES.Commons.Models
{
    /// <summary>
    /// 定义领域对象框架实体类的基类，系统默认主键为Id
    /// </summary>
    /// <typeparam name="TKey">实体主键类型</typeparam>

    [Serializable]
    public abstract class BaseEntityGeneric<TKey> : EntityGeneric, IBaseEntityGeneric<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        protected BaseEntityGeneric()
        {
        }

        /// <summary>
        /// 获取或设置 编号
        /// </summary>
        [DisplayName("编号")]
        [Key]
        [Column("Id")]
        public virtual TKey Id { get; set; }


        /// <summary>
        /// 判断主键是否为空
        /// </summary>
        /// <returns></returns>
        public override bool KeyIsNull()
        {
            if (Id == null)
            {
                return true;
            }
            else
            {
                return string.IsNullOrEmpty(Id.ToString());
            }
        }

        /// <summary>
        /// 创建默认的主键值
        /// </summary>
        public override void GenerateDefaultKeyVal()
        {
            //if (Id == null)
            //{
            //    Id = GuidUtils.CreateNo().CastTo<TKey>();
            //}
        }
    }
}
