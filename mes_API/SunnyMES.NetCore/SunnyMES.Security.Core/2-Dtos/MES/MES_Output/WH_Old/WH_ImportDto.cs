using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// WH_ImportDto
    /// </summary>
    [Serializable]
    public class WH_ImportDto
    {
        public string MSG { get; set; }
        public List<ExcelDT> WH_ExcelDT { get; set; }
    }
}
