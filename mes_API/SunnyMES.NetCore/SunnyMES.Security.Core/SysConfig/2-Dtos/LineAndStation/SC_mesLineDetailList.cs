using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// SC_mesLineDetailList
    /// </summary>
    [Serializable]
    public class SC_mesLineDetailList
    {
        public string MSG { get; set; }
        public List<SC_mesLineDetailDto> List_mesLineDetail { get; set; }
    }
}
