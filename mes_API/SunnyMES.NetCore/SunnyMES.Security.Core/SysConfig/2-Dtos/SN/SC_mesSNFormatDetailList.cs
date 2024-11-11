
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{

    [Serializable]
    public class SC_mesSNFormatDetailList
    {
        public string MSG { get; set; }
        public List<SC_mesSNSectionDto> List_mesSNFormatDetail { get; set; }
    }
}
