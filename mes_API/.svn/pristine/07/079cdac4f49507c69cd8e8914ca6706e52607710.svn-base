using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{

    /// <summary>
    /// mesProductStructure
    /// </summary>
    [Table("mesProductStructure")]
    public class SC_mesProductStructure : BaseCustomEntity<string>
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
        public int ParentPartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int PartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartPosition { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? IsCritical { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? StationTypeID { get; set; }

        public int? SEQ { get; set; }

        /// <summary>
        /// 父料号名
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }
        /// <summary>
        /// 子料号名
        /// </summary>
        [NotMapped]
        public string ChildName { get; set; }

        /// <summary>
        /// 站点类型名
        /// </summary>
        [NotMapped]
        public string StationTypeName { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        public string StatusDesc { get; set; }

    }
}
