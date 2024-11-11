using SunnyMES.Commons.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._2_Dtos.Shift
{
    public class MesShiftInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 班次类别ID列表
        /// </summary>
        public List<string> ShiftTypeNames { get; set; } = new List<string>();
        /// <summary>
        /// 班次状态列表
        /// </summary>
        public List<string> ShiftStates { get; set; } = new List<string>();
        /// <summary>
        /// 班次代码
        /// </summary>
        public string ShiftCode { get; set; } = string.Empty;
        /// <summary>
        /// 班次描述
        /// </summary>
        public string ShiftDesc { get; set; } = string.Empty;
    }
}
