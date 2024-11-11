using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{
    /// <summary>
    /// mesPart
    /// </summary>
    [Table("mesPart")]
    public class SC_mesPart : BaseCustomEntity<string>
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
        public string PartNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? PartFamilyID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string UOM { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? IsUnit { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 料号组名称
        /// </summary>
        [NotMapped]
        public string PartFamilyName { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        public string StatusDesc { get; set; }
        /// <summary>
        /// 子类详细信息
        /// </summary>
        [NotMapped()]
        public List<SC_mesPartDetail> PartDetails { get; set; }
    }

}
