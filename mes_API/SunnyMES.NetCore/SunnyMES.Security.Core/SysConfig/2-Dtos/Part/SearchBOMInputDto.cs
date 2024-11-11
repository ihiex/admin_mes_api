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
    /// BOM分页查询输入参数
    /// </summary>
    public class SearchBOMInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 父料号ID
        /// </summary>
        public List<int> ParentPartIDs { get; set; }
        /// <summary>
        /// 子料号ID
        /// </summary>
        public List<int> PartIDs { get; set; }
        /// <summary>
        /// 站点类型ID
        /// </summary>
        public List<int> StationTypeIDs { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// SEQ
        /// </summary>
        public int SEQ { get; set; }
    }
}
