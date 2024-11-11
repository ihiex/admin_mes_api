using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesLineGroupDto
    {
        public string ID { get; set; }
        public string LineGroupName { get; set; }
        public string LineID { get; set; }
        public string LineNumber { get; set; }
        public string LineType { get; set; }
        public string PartFamilyTypeID { get; set; }

        public  string Line { get; set; }
        public string PartFamilyType { get; set;}

    }
}
