using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.SysConfig.Models.Shift
{

    /// <summary>
    /// mesShiftUPH
    /// </summary>
    [Table("mesShiftUPH")]
    public class SC_mesShiftUPH : BaseCustomEntity<int>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }



        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int ShiftID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int LineID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int UPH { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public float? YieldTarget { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        ///  班次日期
        /// </summary>
        [ForceCheck]
        public DateTime ShiftDate { get; set; }

        /// <summary>
        /// 班次类型
        /// </summary>
        [NotMapped]
        public string ShiftType { get; set; }
        /// <summary>
        /// 班次代码
        /// </summary>
        [NotMapped]
        public string ShiftCode { get; set; }

        /// <summary>
        /// 班次描述
        /// </summary>
        [NotMapped]
        public string ShiftDesc { get; set; }
       /// <summary>
       /// 线别名称
       /// </summary>
        [NotMapped]
        public string LineName { get; set; }

    }

}
