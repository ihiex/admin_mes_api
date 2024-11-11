using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent
{
    public class ShipMentReplaceInput : MesInputDto
    {
        /// <summary>
        /// 原始的BillNO
        /// </summary>
        public string OriginalBillNO { get; set; }

        /// <summary>
        /// 新的BillNO
        /// </summary>
        public string TargetBillNO { get; set; }
    }
}
