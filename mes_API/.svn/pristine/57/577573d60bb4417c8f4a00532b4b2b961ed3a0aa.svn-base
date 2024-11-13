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
    [Table("mesPartFamilyTypeDetail")]
    public class SC_mesPartFamilyTypeDetail : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? PartFamilyTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int? PartFamilyTypeDetailDefID { get; set; }

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
