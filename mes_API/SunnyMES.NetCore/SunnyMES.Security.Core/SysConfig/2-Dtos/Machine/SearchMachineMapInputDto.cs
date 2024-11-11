using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;

namespace SunnyMES.Security.SysConfig.Dtos.Machine
{
    /// <summary>
    /// 机器设备分配分页查询输入参数
    /// </summary>
    public class SearchMachineMapInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 工站类型名称
        /// </summary>
        public string StationTypeName { get; set; }

        /// <summary>
        /// 料号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 工艺路线
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        public string MachineFamilyName { get; set; }
        /// <summary>
        /// 设备料号
        /// </summary>
        public string MachinePartNumber { get; set; }
        /// <summary>
        /// 设备SN
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 工艺路线编号列表 
        /// </summary>
        public int[] RouteIds { get; set; }

        /// <summary>
        /// 工站类型编号列表 
        /// </summary>
        public int[] StationTypeIds { get; set; }
        /// <summary>
        /// 料号编号列表 
        /// </summary>
        public int[] PartIds { get; set; }
        /// <summary>
        /// 设备料号编号列表 
        /// </summary>
        public int[] MachinePartIds { get; set; }
        /// <summary>
        /// 设备组编号列表 
        /// </summary>
        public int[] MachineFamilyIds { get; set; }
    }
}
