using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesLabelField
    {
        public string ID { get; set; }
        public string LabelID { get; set; }
        public string LabelFormatPos { get; set; }
        //public string LabelFieldID { get; set; }
        public string Description { get; set; }
        public string FieldDefinitionID { get; set; }
        public string Order { get; set; }
    }
}




