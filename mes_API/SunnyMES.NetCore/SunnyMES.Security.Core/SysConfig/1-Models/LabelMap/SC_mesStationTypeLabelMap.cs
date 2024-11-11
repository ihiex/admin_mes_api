using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesStationTypeLabelMap
    {
        public string ID { get; set; }
        public string StationTypeID { get; set; }
        public string LabelID { get; set; }
        public string PartID { get; set; }
        public string PartFamilyID { get; set; }
        public string ProductionOrderID { get; set; }
        public string LineID { get; set; }
    }
}



