using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Commons.Helpers;

namespace SunnyMES.Security.SysConfig.Models.Part
{


    /// <summary>
    /// luVendor
    /// </summary>
    [Table("luVendor")]
    public class SC_luVendo : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public string Code { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? PartID { get; set; }

        /// <summary>
        /// 料号名称
        /// </summary>
        [NotMapped]
        public string PartName { get; set; }
        /// <summary>
        /// 料号组ID
        /// </summary>
        [NotMapped]
        public int? PartFamilyID { get;set; }
        /// <summary>
        /// 料号组名称
        /// </summary>
        [NotMapped]
        public string PartFamilyName { get; set; }
        /// <summary>
        /// 料号组类别ID
        /// </summary>
        [NotMapped]
        public int? PartFamilyTypeID { get; set; }
        /// <summary>
        /// 料号组类别名称
        /// </summary>
        [NotMapped]
        public string PartFamilyTypeName { get; set; }
        public override bool KeyIsNull()
        {
            return true;
        }
        public override void GenerateDefaultKeyVal(int id)
        {
            this.ID = id;
        }
    }


}
