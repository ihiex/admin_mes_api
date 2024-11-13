
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
    /// mesScreenshot
    /// </summary>
    [Table("mesScreenshot")]
    [Serializable]
    public class SC_mesScreenshot : BaseEntity<string>
    {
        /// <summary>
        /// 默认构造函数（需要初始化属性的在此处理）
        /// </summary>
        public SC_mesScreenshot()
        {

        }

        //public virtual int ID { get; set; }
        public virtual string LineID { get; set; }
        public virtual string StationID { get; set; }
        public virtual string PartID { get; set; }
        public virtual string ProductionOrderID { get; set; }
        public virtual  string IP { get; set; }
        public virtual string PCName { get; set; }
        public virtual string IMGURL { get; set; }
        public virtual string MSG { get; set; }
        public virtual string Feedback { get; set; }
        public virtual string IsFeedback { get; set; }
    }
}
