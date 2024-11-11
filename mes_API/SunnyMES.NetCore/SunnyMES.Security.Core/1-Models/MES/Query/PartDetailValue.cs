using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security._1_Models.MES.Query
{
    /// <summary>
    /// 料号及料号组指定项的值
    /// </summary>
    [Serializable]
    public class PartDetailValue
    {
        /// <summary>
        /// true : 在part detail 中配置的属性;  false : 在part family detail中配置的属性
        /// </summary>
        public bool IsPartSetup { get; set; }
        /// <summary>
        /// 详细项名称
        /// </summary>
        public string DetailName { get; set; }
        /// <summary>
        /// 详细值
        /// </summary>
        public string DetailValue { get; set; }
    }
}
