using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Security.Models
{
    public class SC_mesSNFormatMapSearch : PagerInfo
    {
        public string ID { get; set; }
        public string SNFormatID { get; set; }
        public string SNFormat { get; set; }
        public string PartID { get; set; }
        public string Part { get; set; }
        public string PartFamilyID { get; set; }
        public string PartFamily { get; set; }
        public string LineID { get; set; }
        public string Line { get; set; }
        public string ProductionOrderID { get; set; }
        public string ProductionOrder { get; set; }
        public string StationTypeID { get; set; }
        public string StationType { get; set; }

        public string LikeQuery { get; set; }

    }
}





