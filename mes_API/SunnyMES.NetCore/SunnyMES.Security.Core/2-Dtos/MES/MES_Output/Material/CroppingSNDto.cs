using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// CroppingSNDto
    /// </summary>
    [Serializable]
    public class CroppingSNDto
    {
        public TabVal MSG { get; set; }

        public int ParentID { get; set; }
        public int BatchQTY { get; set; }
        public int UsageQTY { get; set; }
        public string RoolNo { get; set; }
    }
}






