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
    public class SC_mesStationTypeAccessSearch : PagerInfo
    {
        public string ID { get; set; }
        public string StationTypeID { get; set; }
        public string StationType { get; set; }
        public string EmployeeID { get; set; }
        public string Employee { get; set; }
        public string Status { get; set; }
        public string StatusValue { get; set; }

        public string LikeQuery { get; set; }
    }
}



