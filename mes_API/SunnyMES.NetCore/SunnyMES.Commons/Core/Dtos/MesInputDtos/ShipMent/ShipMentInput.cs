using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent
{
    [Serializable]
    public class ShipMentInput : MesSnInputDto
    {
        /// <summary>
        /// 箱码
        /// </summary>
        public string MultipackSn { get; set; }
        /// <summary>
        /// 后端生成的出货栈板码
        /// </summary>
        public string ShippingPallet { get; set; }
    }
}
