using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Dtos.Models;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{

    /// <summary>
    /// mesPartFamilyTypeDetail
    /// </summary>
    [Table("mesPartDetail")]
    public class SC_mesPartDetail : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int PartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int PartDetailDefID { get; set; }

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
