using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// mesLineGroup
    /// </summary>
    [Table("mesLineGroup")]
    [Serializable]
    public class SC_mesLineGroup : BaseEntity<string>
    {
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
        public SC_mesLineGroup()
        {

        }

        //public virtual int ID { get; set; }
        public virtual string LineGroupName { get; set; }
        public virtual int? LineID { get; set; }
        public virtual int? LineNumber { get; set; }
        public virtual string LineType { get; set; }
        public virtual int? PartFamilyTypeID { get; set; }
    }
}