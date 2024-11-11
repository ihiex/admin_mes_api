using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// SetScanSN_TTChildSNOutputDto
    /// </summary>
    [Serializable]
    public class SetScanSN_TTChildSNOutputDto
    {
        public TabVal MSG { get; set; }

        public string BoxSN { get; set; }
        public int BindQTY { get; set; }
        public List<dynamic> ChildList { get; set; }
        public TabVal MSG2 { get; set; }

        public string PrintData { get; set;}
    }
}
