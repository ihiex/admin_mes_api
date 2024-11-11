using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._2_Dtos.SN
{
    /// <summary>
    /// 锁定条码参数
    /// </summary>
    [Serializable]
    public class SC_Lock_SN_Dto
    {
        /// <summary>
        /// 条码列表
        /// </summary>
        public string[] SNs { get; set; }

        /// <summary>
        /// 是否锁定  默认为true
        /// </summary>
        public bool isLock { get; set; } = true;

        /// <summary>
        /// 锁定信息
        /// </summary>
        public string lockMess { get; set; }

        public string user { get; set; }
    }
}
