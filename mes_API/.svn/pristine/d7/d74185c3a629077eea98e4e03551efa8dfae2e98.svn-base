using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Part
{


    /// <summary>
    /// luPartFamilyType
    /// </summary>
    [Table("luPartFamilyType")]
    [Serializable]
    public class SC_luPartFamilyType : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte? Status { get; set; }


        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        public string StatusDesc { get; set; }
        /// <summary>
        /// 子类详细信息
        /// </summary>
        [NotMapped()]
        public List<SC_mesPartFamilyTypeDetail>? PartFamilyTypeDetails { get; set; }

    }


}
