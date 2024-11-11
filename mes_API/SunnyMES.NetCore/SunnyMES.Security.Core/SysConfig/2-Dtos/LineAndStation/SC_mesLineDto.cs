
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesLineDto
    {
        public  string ID { get; set; }
        public  string Description { get; set; }
        public  string Location { get; set; }
        public  int? ServerID { get; set; }
        public  int StatusID { get; set; }

        public string StatusValue { get; set; }
    }
}