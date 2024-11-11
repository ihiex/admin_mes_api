using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models.Public
{
    [Serializable]
    public class ProcOutput
    {
        public int PackageID { get; set; }
        public int partID { get; set; }
        public int prodID { get; set; }
        public int stationID { get; set; }
        public int stationTypeID { get; set; }
        public int EmployeeId { get; set; }
        public int lineID { get; set; }
        public int stage { get; set; }
        public string strOutput { get; set; }
    }
}
