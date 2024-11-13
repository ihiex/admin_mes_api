using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Models.PO
{

    /// <summary>
    /// mesProductionOrderDetail
    /// </summary>
    [Table("mesProductionOrderDetail")]
    public class SC_mesProductionOrderDetail : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]

        public int ProductionOrderID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public int ProductionOrderDetailDefID { get; set; }

        /// <summary>
        /// 详细信息值
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否动态检查项
        /// </summary>
        public bool? IsCheckList { get; set; }

        /// <summary>
        /// 当是动态检查项时，检查项的顺序，第一项为主条码
        /// </summary>
        public int? Sequence { get; set; }


        
        ///// <summary>
        ///// 工单属性定义
        ///// </summary>
        //[ForeignKey("ProductionOrderDetailDefID")]
        //public SC_luProductionOrderDetailDef productionOrderDetailDef { get; set; }
        ///// <summary>
        ///// 工单定义
        ///// </summary>
        //[ForeignKey("ProductionOrderID")]
        //public SC_mesProductionOrder productionOrder { get; set; }


        /// <summary>
        /// 描述ID
        /// </summary>
        [NotMapped]
        public int DefID { get; set; }

        /// <summary>
        /// 描述内容
        /// </summary>
        [NotMapped]
        public string DefDescription { get; set; }
    }

}
