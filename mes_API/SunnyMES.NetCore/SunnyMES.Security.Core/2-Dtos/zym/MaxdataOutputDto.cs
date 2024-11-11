using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 大数据测试输出对象模型
    /// </summary>
    [Serializable]
    public class MaxdataOutputDto
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        [MaxLength(50)]
        public string Id { get; set; }

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
