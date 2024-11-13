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
    /// mesShift
    /// </summary>
    [Table("mesShift")]
    public class SC_mesShift : BaseCustomEntity<int>
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
        public string ShiftType { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public string ShiftCode { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ShiftDesc { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? State { get; set; }
        /// <summary>
        /// 班次详细内容
        /// </summary>
        [NotMapped]
        public IEnumerable<SC_mesShiftDetail> Details { get; set; }

    }

}
