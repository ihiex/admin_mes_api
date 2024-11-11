using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 过滤IP，数据实体对象
    /// </summary>

    [Table("API_Maxdata")]
    [Serializable]
    public class Maxdata : BaseEntity<string>, ICreationAudited, IModificationAudited, IDeleteAudited
    {
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>

        public Maxdata()
        {

        }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int UnitStateID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public byte StatusID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? PanelID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? LineID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? ProductionOrderID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? RMAID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? PartID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? LooperCount { get; set; }




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




    }
}