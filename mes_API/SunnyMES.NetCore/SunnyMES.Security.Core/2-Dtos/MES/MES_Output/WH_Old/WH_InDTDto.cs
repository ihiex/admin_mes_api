using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// WH_InDTDto
    /// </summary>
    [Serializable]
    public class WH_InDTDto
    {
        public string ScanResult { get; set; }
        public string MSG { get; set; }
        public List<WH_InDT> WH_InGrid { get; set; }
        public List<WH_LCount> WH_InCount { get; set; }
        public int LCountS { get; set; }

    }
}
