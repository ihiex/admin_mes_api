using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// CreateSNOutputDto
    /// </summary>
    [Serializable]
    public class CreateSNOutputDto
    {
        public TabVal MSG { get; set; }

        //public string Result { get; set; }
        public string LabelPath { get; set; }
        public List<string> ListSN { get; set; }

        public int UsageQTY { get; set; }
        public int SplitQTY { get; set; }

        public string WriteCOMValue { get; set; }
}
}
