using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Machine
{
    /// <summary>
    /// mesRouteMachineMap
    /// </summary>
    [Table("mesRouteMachineMap")]
    public class SC_mesRouteMachineMap : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int RouteID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int StationTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? MachineID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int PartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? MachinePartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? MachineFamilyID { get; set; }


        /// <summary>
        /// 工站类型名称
        /// </summary>
        [NotMapped]
        public string StationTypeName { get; set; }

        /// <summary>
        /// 料号
        /// </summary>
        [NotMapped]
        public string PartNumber { get; set; }

        /// <summary>
        /// 工艺路线
        /// </summary>
        [NotMapped]
        public string RouteName { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        [NotMapped]
        public string MachineFamilyName { get; set; }

        /// <summary>
        /// 设备料号
        /// </summary>
        [NotMapped]
        public string MachinePartNumber { get; set; }

        /// <summary>
        /// 设备SN
        /// </summary>
        [NotMapped]
        public string SN { get; set; }
    }


}
