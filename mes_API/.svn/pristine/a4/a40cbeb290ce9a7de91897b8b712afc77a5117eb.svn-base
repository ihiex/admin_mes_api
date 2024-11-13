using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._1_Models.MES.Query.ShipMent;
using SunnyMES.Security.Models;

namespace SunnyMES.Security._2_Dtos.MES.MES_Output.ShipMent
{
    public class ShipMentOutput : MesOutputDto
    {
        /// <summary>
        /// 打印类型    
        /// 1:扫描完毕打印  （一个栈板全部只有一个PO）
        /// 2:每扫描一次打印一张   （一个栈板存在多个PO）
        /// </summary>
        public string PrintType { get; set; }
        /// <summary>
        /// 出货栈板条码
        /// </summary>
        public string ShippingPallet { get; set; }
        /// <summary>
        /// 是否打包完成
        /// </summary>
        public bool IsPackingFinish { get; set; }

        public PrinterParams PrinterParams { get; set; }
        
        /// <summary>
        /// 出货信息
        /// </summary>
        public List<ShipMentData> ShipMentDatas { get; set; }
        /// <summary>
        /// 绑定数据
        /// </summary>
        public List<ShipMentDetailData> ShipMentDetailDatas { get; set; }

        /// <summary>
        /// 打印类型为1时，只打印一张一个条码，且打印的条码为箱码
        /// 打印类型为2时，为GSLabel打印数据
        /// </summary>
        public string ShipmentSiglePrintData { get; set; }
        /// <summary>
        /// 打印类型为2时，打印一张多条码数据
        /// </summary>
        public List<ShipmentMupltipack> ShipmentPrintData { get; set; }
    }
}
