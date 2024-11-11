
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
    /// mesLine
    /// </summary>
    [Table("mesLine")]
    [Serializable]
    public class SC_mesLine : BaseEntity<string>
    {
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
        public SC_mesLine()
        {

        }

        //public virtual int ID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Location { get; set; }
        public virtual int ServerID { get; set; }
        public virtual int StatusID { get; set; }
    }
}