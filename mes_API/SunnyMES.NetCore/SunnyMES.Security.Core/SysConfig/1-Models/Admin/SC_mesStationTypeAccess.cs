using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesStationTypeAccess
    {
        public string ID { get; set; }
        public string StationTypeID { get; set; }
        public string EmployeeID { get; set; }
        public string Status { get; set; }
    }
}


