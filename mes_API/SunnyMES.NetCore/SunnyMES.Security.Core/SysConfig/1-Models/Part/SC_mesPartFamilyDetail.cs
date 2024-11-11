using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{

    /// <summary>
    /// mesPartFamilyTypeDetail
    /// </summary>
    [Table("mesPartFamilyDetail")]
    public class SC_mesPartFamilyDetail : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int PartFamilyID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]

        public int PartFamilyDetailDefID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 描述ID
        /// </summary>
        [NotMapped]
        public int DefID { get; set; }

        /// <summary>
        /// 描述内容
        /// </summary>
        [NotMapped]
        public string DefDescription { get; set; }

    }


}
