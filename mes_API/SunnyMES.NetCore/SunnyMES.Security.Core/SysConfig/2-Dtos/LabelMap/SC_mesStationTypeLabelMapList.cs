using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{

    [Serializable]
    public class SC_mesStationTypeLabelMapList
    {
        public string MSG { get; set; }
        public List<SC_mesStationTypeLabelMapDto> List_mesStationTypeLabelMapDto { get; set; }
    }
}



