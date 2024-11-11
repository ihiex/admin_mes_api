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
    public class SC_mesLabelSearch : PagerInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LabelFamilyID { get; set; }
        public string LabelFamily { get; set; }
        public string LabelType { get; set; }
        public string LabelTypeName { get; set; }
        public string TargetPath { get; set; }
        public string OutputType { get; set; }
        public string OutputTypeName { get; set; }
        public string PrintCMD { get; set; }
        public string SourcePath { get; set; }
        public string PageCapacity { get; set; }

        public string LabelFieldDefName { get; set; }

        public string LikeQuery { get; set; }

    }
}



