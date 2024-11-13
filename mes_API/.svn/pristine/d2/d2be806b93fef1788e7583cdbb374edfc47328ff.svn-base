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
    /// luMachineFamilyType
    /// </summary>
    [Table("luMachineFamilyType")]
    public class SC_luMachineFamilyType : BaseCustomEntity<string>
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

    }


}
