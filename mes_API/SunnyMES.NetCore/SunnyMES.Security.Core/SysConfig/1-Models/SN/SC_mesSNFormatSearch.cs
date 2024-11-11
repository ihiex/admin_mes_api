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
    public class SC_mesSNFormatSearch: PagerInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SNFamilyID { get; set; }
        public string SNFamily { get; set; }

        public string SNFormat { get; set; }

        public string SectionTypeID { get; set; }
        public string SectionType { get; set; }

        public string SectionParam { get; set; }
        public string Increment { get; set; }
        public string InvalidChar { get; set; }
        public string LastUsed { get; set; }
        //public string Order { get; set; }
        public Boolean? AllowReset { get; set; }

        public string LikeQuery { get; set; }

    }
}


