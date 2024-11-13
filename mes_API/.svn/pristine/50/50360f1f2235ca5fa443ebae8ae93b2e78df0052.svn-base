using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// ConfirmPOOutputDto
    /// </summary>
    [Serializable]
    public class ConfirmPOOutputDto
    {
        public TabVal MSG { get; set; }
       
        public List<mesProductStructure> mesProductStructures { get; set; }
        public List<mesRoute> Route { get; set; }
        public List<dynamic> RouteDataDiagram1 { get; set; }
        public List<dynamic> RouteDataDiagram2 { get; set; }
        public List<dynamic> RouteDetail { get; set; }
        public List<dynamic> ProductionOrderQTY { get; set; }

        public string LabelPath { get; set; }
        public string SNFormatName { get; set; }
        public string COF { get; set;}

        public string IsCreateUPCSN { get; set; }

        public string JumpFromUnitStateID { get; set; }
        public string JumpToUnitStateID { get; set; }
        public string JumpStatusID { get; set; }
        public string JumpUnitStateID { get; set; }

        public string InnerSN_Pattern { get; set; }


        public List<string> ET { get; set; }
        public string SPAC { get; set; }


        public string IsAuto { get; set; } = "0";

        public string Port { get; set; } = "COM1";
        public string PassCmd { get; set; } = "PASS";

        public string FailCmd { get; set; } = "FAIL";

        public string ReadCmd { get; set; } = "READ";

        public string BaudRate { get; set; } = "9600";
        public string DataBits { get; set; } = "8";

        public string Parity { get; set; } = "Odd";

        public string StopBits { get; set; } = "1";



        public string[] List_PrintUPCUnitStateID { get; set; }
        public string PrintUnitStateID { get; set; }
        public string PrintStationTypeID { get; set;}

    }
}
