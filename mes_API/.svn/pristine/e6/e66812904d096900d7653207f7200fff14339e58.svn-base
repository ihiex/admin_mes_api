using Npgsql.Replication.PgOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.SysConfig.Dtos.PO
{
    /// <summary>
    /// 工单分线输出参数
    /// </summary>
    public class SearchPOLineOutputDto 
    {
        public int ID { get; set; }
        /// <summary>
        /// 工单ID
        /// </summary>
        public int ProductionOrderID { get; set; }
        /// <summary>
        /// 工单名称
        /// </summary>
        public string POName { get; set; }
        /// <summary>
        /// 工单描述
        /// </summary>
        public string PODesc { get; set; }
        /// <summary>
        /// 分线描述
        /// </summary>
        public string POLineDesc { get; set; }
        public int LineID { get; set; }
        /// <summary>
        /// 线别名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 分配数量
        /// </summary>
        public int LineQuantity { get; set; }
        public int StartedQuantity { get; set; }
        public int ReadyQuantity { get; set; }
        public int AllowOverBuild { get; set; }
        public int Priority { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
