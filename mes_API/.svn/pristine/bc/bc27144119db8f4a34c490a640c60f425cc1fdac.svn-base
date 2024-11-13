using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;

namespace SunnyMES.Security.SysConfig.Dtos.Machine
{
    /// <summary>
    /// 机器设备分页查询输入参数
    /// </summary>
    public class SearchMachineInputDto : SearchSignalInputDto
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
        /// 设备条码
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        public string MachineFamilyName { get; set; }
        /// <summary>
        /// 警告状态ID列表
        /// </summary>
        public List<int> WarningStateIds { get; set; }

        /// <summary>
        /// 状态ID列表
        /// </summary>
        public List<int> StateIds { get; set; }
    }
}
