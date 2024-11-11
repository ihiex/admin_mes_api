using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// ConfirmPOOutputDto_TTBindBox
    /// </summary>
    [Serializable]
    public class ConfirmPOOutputDto_TTBindBox
    {
        public TabVal MSG { get; set; }
       
        public List<mesProductStructure> mesProductStructures { get; set; }
        public List<mesRoute> Route { get; set; }
        public List<dynamic> RouteDataDiagram1 { get; set; }
        public List<dynamic> RouteDataDiagram2 { get; set; }
        public List<dynamic> RouteDetail { get; set; }
        public List<dynamic> ProductionOrderQTY { get; set; }
        //public TTBindBox TTBindBox { get; set; }

        public string IsTTRegistSN { get; set; }
        public string SNFormatName { get; set; }
        public string LabelPath { get; set; }
        public string FullBoxQTY { get; set; }
        public string BoxType { get; set; }        
    }
}
