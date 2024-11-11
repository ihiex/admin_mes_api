using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_nodeData
    {
        public string text { get; set; }
        public string key { get; set; }
        public string loc { get; set; }
        public string stationTypeID { get; set; }

        public string category { get; set; }
        public TBLR leftArray { get; set; }
        public TBLR rightArray { get; set; }
        public TBLR topArray { get; set; }
        public TBLR bottomArray { get; set; }
    }
}



