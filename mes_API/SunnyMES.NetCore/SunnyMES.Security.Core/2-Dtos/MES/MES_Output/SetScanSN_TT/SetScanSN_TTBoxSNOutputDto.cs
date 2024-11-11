using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// SetScanSN_TTBoxSNOutputDto
    /// </summary>
    [Serializable]
    public class SetScanSN_TTBoxSNOutputDto
    {
        public TabVal MSG { get; set; }

        public string BoxSN { get; set; }        
        public int BindQTY { get; set; }
        public List<dynamic> ChildList { get; set; }

        public string PrintData { get; set; }
    }
}
