using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._2_Dtos.SN
{
    [Serializable]
    public class Sc_Lock_Sn_Search_Dto
    {
        /// <summary>
        /// 已锁定条码
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 锁定信息
        /// </summary>
        public string LockMessage { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime LockTime { get; set; }
    }
}
