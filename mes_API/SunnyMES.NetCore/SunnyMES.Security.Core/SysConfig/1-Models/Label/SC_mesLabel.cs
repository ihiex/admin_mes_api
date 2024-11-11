using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesLabel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LabelFamilyID { get; set; }
        public string LabelType { get; set; }
        public string TargetPath { get; set; }
        public string OutputType { get; set; }
        public string PrintCMD { get; set; }
        public string SourcePath { get; set; }
        public string PageCapacity { get; set; }
    }
}


