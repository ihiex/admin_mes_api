using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// TranceCodeDto
    /// </summary>
    [Serializable]
    public class TranceCodeDto
    {
        public TabVal MSG { get; set; }

        public string VendorCode { get; set; }
        public string LotCode { get; set; }
        public string MaterialDate { get; set; }
        public string DateCode { get; set; }
        public string ExpiringTime { get; set; }
        public string Quantity { get; set; }    //BatchQTY
    }
}





