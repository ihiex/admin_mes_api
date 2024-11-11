using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// WH_OutDTDto
    /// </summary>
    [Serializable]
    public class WH_OutDTDto
    {
        public string ScanResult { get; set; }
        public string MSG { get; set; }
        public List<WH_OutDT> WH_OutGrid { get; set; }
        public List<WH_LCount> WH_OutCount { get; set; }
        public int LCountS { get; set; }

        public List<WH_BillNo> POMPN { get; set; }

    }
}
