using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesRouteDetailDto
    {
        public string ID { get; set; }
        public string RouteID { get; set; }
        public string RouteName { get; set; }
        public string StationTypeID { get; set; }
        public string StationType { get; set; }
        public string UnitStateID { get; set; }
        public string UnitState { get; set; }
        public string SequenceMod { get; set; }
        public string Sequence { get; set; }
        public string Description { get; set; }

    }
}


