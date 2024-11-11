using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesStationTypeDetail
    {
        public virtual string ID { get; set; }
        public virtual string StationTypeID { get; set; }       
        public virtual string StationTypeDetailDefID { get; set; }
        public virtual string Content { get; set; }
        public virtual string Description { get; set; }
    }
}
