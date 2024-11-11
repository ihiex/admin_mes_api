using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class TTBindBox
    {
        public virtual string IsTTRegistSN { get; set; }
        public virtual string SNFormatName { get; set; }
        public virtual string LabelPath { get; set; }
        //public virtual string PrintIPPort { get; set; }
        public virtual int FullBoxQTY { get; set; }
        /// <summary>
        /// 1:只注册 2:注册且系统生成SN 3:系统Machine表存在
        /// </summary>
        public virtual string BoxType { get; set; }
        //public virtual string UnitStatusID { get; set; }
        //public virtual string BoxUnitID{ get; set; }        
    }
}

