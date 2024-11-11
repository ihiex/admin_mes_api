using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{

    /// <summary>
    /// luDefect
    /// </summary>
    [Table("luDefect")]
    public class SC_luDefect : BaseCustomEntity<string>
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
        public int? DefectTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public string DefectCode { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? LocaltionID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 不良类型
        /// </summary>
        [NotMapped]
        public string DefectType { get; set; }
    }



}
