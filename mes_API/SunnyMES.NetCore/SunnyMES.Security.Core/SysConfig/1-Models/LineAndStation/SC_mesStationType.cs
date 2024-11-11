using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesStationType 
    { 
        public virtual string ID { get; set; }
        public virtual string Description { get; set; }
        public virtual string ApplicationTypeID { get; set; }

    }
}
