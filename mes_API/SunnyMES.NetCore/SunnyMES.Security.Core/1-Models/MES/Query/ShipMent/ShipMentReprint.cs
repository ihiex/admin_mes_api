using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._1_Models.MES.Query;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class ShipMentReprint : BaseQueryModel
    {
        public string SerialNumber { get; set; }
        /// <summary>
        /// 为空值时，为打印类型2，为1或者2时为打印类型为1
        /// </summary>
        public string LabelSCType { get; set; }
        public string PalletSN { get; set; }
        public int PalletID { get; set; }
    }
}
