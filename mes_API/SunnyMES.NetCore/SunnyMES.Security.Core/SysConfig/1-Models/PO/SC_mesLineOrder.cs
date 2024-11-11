using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.PO
{

    /// <summary>
    /// mesLineOrder
    /// </summary>
    [Table("mesLineOrder")]
    public class SC_mesLineOrder : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 分配数量
        /// </summary>
        public int? LineQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 生成中数量
        /// </summary>
        public int? StartedQuantity { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int? ReadyQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? AllowOverBuild { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? LineID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? ProductionOrderID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 线别名称
        /// </summary>
        [NotMapped]
        public string LineName { get; set; }
    }

}
