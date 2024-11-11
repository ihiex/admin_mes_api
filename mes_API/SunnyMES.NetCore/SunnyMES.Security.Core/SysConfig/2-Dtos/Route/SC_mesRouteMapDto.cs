using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesRouteMapDto
    {
        public string ID { get; set; }
        public string PartFamilyID { get; set; }
        public string PartFamily { get; set; }
        public string PartID { get; set; }
        public string PartNumber { get; set; }
        public string LineID { get; set; }
        public string Line { get; set; }
        public string RouteID { get; set; }
        public string Route { get; set; }
        public string ProductionOrderID { get; set; }
        public string ProductionOrderNumber { get; set;}

    }
}



