using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesSNRangeDto
    {
        public string ID { get; set; }
        public string SNSectionID { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string StatusID { get; set; }
        public string Order { get; set; }
        public string StatusValue { get; set; }
    }
}



