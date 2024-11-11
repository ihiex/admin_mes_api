
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesLineDetailDto
    {
        public int ID { get; set; }
        public int LineID { get; set; }
        public int LineTypeDefID { get; set; }

        public string LineTypeDefValue { get; set; }
        public string Content { get; set; }
    }
}