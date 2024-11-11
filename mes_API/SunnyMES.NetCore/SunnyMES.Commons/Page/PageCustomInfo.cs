using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Page
{
    [Serializable]
    [DataContract]
    public class PageCustomInfo
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sortfield { get; set; }
        /// <summary>
        /// 是否是升序
        /// </summary>
        public bool IsAsc { get; set; } = true;

        /// <summary>
        /// 默认为0，查询所有，为1时，查询启用的，为-1时查询不启用
        /// </summary>
        public int IsEnabled { get; set; } = 0;


    }
}
