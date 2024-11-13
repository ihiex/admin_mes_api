using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{

    /// <summary>
    /// luPartFamilyDetailDef
    /// </summary>
    [Table("luPartFamilyDetailDef")]
    public class SC_luPartFamilyDetailDef : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public string Description { get; set; }
        public override bool KeyIsNull()
        {
            return true;
        }
        public override void GenerateDefaultKeyVal(int ID)
        {
            this.ID = ID;
        }

    }


}
