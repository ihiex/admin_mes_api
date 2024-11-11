using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig.Models.Shift
{

    /// <summary>
    /// mesShiftDetail
    /// </summary>
    [Table("mesShiftDetail")]
    public class SC_mesShiftDetail : BaseCustomEntity<int>
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
        public int ShiftCodeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? State { get; set; }

    }

}
