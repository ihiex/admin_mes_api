using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 通过BillNO获取相关的分板信息
    /// </summary>
    [Serializable]
    public class ShipMentData
    {
        /// <summary>
        /// 
        /// </summary>
        public int FDetailID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? FLineItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? FKPONO { get; set; }
        /// <summary>
        /// 规格数量
        /// </summary>
        public int FCTN { get; set; }
        /// <summary>
        /// 包装数量
        /// </summary>
        public int FOutSN { get; set; }
        /// <summary>
        /// 航空号
        /// </summary>
        public string? FMPNNO { get; set; }
    }

}
