using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// SetScanSNOutputDto
    /// </summary>
    [Serializable]
    public class SetScanSN_TTRegisterOutputDto
    {
        public TabVal MSG { get; set; }

        public List<dynamic> ProductionOrderQTY { get; set; }
    }
}
