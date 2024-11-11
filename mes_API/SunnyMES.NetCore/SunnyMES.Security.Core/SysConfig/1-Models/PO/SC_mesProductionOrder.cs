using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.SysConfig.Models.PO
{

    /// <summary>
    /// mesProductionOrder
    /// </summary>
    [Table("mesProductionOrder")]
    public class SC_mesProductionOrder : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ProductionOrderNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? PartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? OrderQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte? StatusID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? RequestedStartTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? ActualStartTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? RequestedFinishTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? ActualFinishTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? ShippedTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string UOM { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? PriorityByERP { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? IsLotAuditCompleted { get; set; }

        /// <summary>
        /// 料号组ID
        /// </summary>
        [NotMapped]
        public int PartFamilyID { get; set; }
        /// <summary>
        /// 料号组类别ID
        /// </summary>
        [NotMapped]
        public int PartFamilyTypeID { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        [NotMapped]
        public string PartNumber { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        public string StatusDesc { get; set; }

        /// <summary>
        /// 工单详细内容
        /// </summary>
        [NotMapped]
        public ICollection<SC_mesProductionOrderDetail> productionOrderDetails { get; set; } = new List<SC_mesProductionOrderDetail>();

        /// <summary>
        /// 工单分线
        /// </summary>
        [NotMapped]
        public ICollection<SC_mesLineOrder> productionOrderLines { get; set; } = new List<SC_mesLineOrder>();
        /// <summary>
        /// 无
        /// </summary>
        [NotMapped]
        public string EmployeeName { get; set; }
    }

}
