using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.Dtos.Models
{
    public class PalletConfirmed
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int SEQNO { get; set; }

        /// <summary>
        /// 已确认箱码
        /// </summary>
        public string KITSN { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime TIME { get; set; }
    }
}
