using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// GetReworkDto
    /// </summary>
    [Serializable]
    public class GetReworkDto
    {
        public TabVal MSG { get; set; }
        public List<dynamic> ProductionOrderQTY { get; set; }
        //public string PartID { get; set; }


        public List<ReworkData> ReworkData{ get; set; }
        public List<ReworkPrintSN> PrintSN{ get; set; }
    }
}




