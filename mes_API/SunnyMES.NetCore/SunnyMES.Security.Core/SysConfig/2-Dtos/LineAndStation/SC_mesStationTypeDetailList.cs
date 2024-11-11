using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    public class SC_mesStationTypeDetailList
    {
        public string MSG { get; set; }
        public List<SC_mesStationTypeDetailDto> List_mesStationTypeDetail { get; set; }
    }
}

