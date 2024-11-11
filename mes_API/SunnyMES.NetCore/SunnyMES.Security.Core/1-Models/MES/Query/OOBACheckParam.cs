using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security._1_Models.MES.Query
{

    [Serializable]
    public class OOBACheckParam : SqlOutputStr
    {
        public OOBACheckInfo BoxInformation { get; set; }
        public List<string> PrintSnList { get; set; }
    }
}
