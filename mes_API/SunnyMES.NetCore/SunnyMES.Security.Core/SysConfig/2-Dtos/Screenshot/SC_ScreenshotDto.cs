using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_ScreenshotDto
    {
        public string ID { get; set; }
        public string LineID { get; set; }
        public string Line { get; set; }
        public string StationID { get; set; }
        public string Station { get; set; }
        public string PartID { get; set; }
        public string Part { get; set; }
        public string ProductionOrderID { get; set; }
        public string ProductionOrder { get; set; }
        public  string IP { get; set; }
        public  string PCName { get; set; }
        public  string IMGURL { get; set; }
        public  string MSG { get; set; }
        public  string Feedback { get; set; }
        public string IsFeedback { get; set; }
        //public string FeedbackStatus { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
