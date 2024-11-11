using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SC_IdPOSNDesc
    {
        public int ID { get; set; }
        public string ProductionOrderNumber { get; set; }
        public string SN { get; set; }
        public string Description { get; set; }

       
    }
}
