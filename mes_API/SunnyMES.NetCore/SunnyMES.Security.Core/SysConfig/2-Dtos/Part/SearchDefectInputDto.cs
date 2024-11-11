using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.SysConfig.Dtos.Part
{
    /// <summary>
    /// 不良代码分页查询输入参数
    /// </summary>
    public class SearchDefectInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 不良代码类别ID列表
        /// </summary>
        public List<int> DefectTypeIds { get; set; }

        /// <summary>
        /// 不良代码
        /// </summary>
        public string DefectCode { get; set; }

        /// <summary>
        /// 不良代码描述
        /// </summary>
        public string DefectDesc { get; set; }
    }
}
