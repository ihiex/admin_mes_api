using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.PO
{

    /// <summary>
    /// mesProductionOrderConsumeInfo
    /// </summary>
    [Table("mesProductionOrderConsumeInfo")]
    public class SC_mesProductionOrderConsumeInfo : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? ProductionOrderID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? LineID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StationTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StationID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? ConsumeQTY { get; set; }

    }

}
