using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SC_IdSNDesc
    {
        public int ID { get; set; }
        public string SN { get; set; }
        public string Description { get; set; }
        
    }
}

