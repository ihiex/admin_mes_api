using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// MaterailBomData
    /// </summary>
    [Serializable]
    public class MaterailBomData
    {
        public string PartID { get; set; }
        public string Description { get; set; }
    }
}





