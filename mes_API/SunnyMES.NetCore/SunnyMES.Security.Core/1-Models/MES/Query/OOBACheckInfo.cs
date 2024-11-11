using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extend;

namespace SunnyMES.Security._1_Models.MES.Query
{
    [Serializable]
    public class OOBACheckInfo
    {
        /// <summary>
        /// BOX ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 关联的栈板条码
        /// </summary>
        public string PalletSN { get; set; }

        /// <summary>
        /// 包装表中的数量
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>
        /// 工单中配置的箱子数量
        /// </summary>
        public string BoxQty { get; set; }
        /// <summary>
        /// 工单中配置的MPN
        /// </summary>
        public string MPN { get; set; }
        /// <summary>
        /// 工单中配置的UPC
        /// </summary>
        public string UPC { get; set; }
        /// <summary>
        /// 工单中配置的SCC
        /// </summary>
        public string SCC { get; set; }      
        
        /// <summary>
        /// Part ID
        /// </summary>
        public int CurrPartID { get; set; }
        /// <summary>
        /// PO ID
        /// </summary>
        public int CurrProductionOrderID { get; set; }
    }
}
