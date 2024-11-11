using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesRoute
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string XMLRoute { get; set; }
        public string XMLRouteV2 { get; set; }
        public string RouteType { get; set; }


    }
}


