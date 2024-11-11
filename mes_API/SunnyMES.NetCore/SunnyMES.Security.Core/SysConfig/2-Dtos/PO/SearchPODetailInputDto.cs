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
    /// 工单分页查询输入参数
    /// </summary>
    public class SearchPODetailInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 工单名称
        /// </summary>
        public string POName { get; set; }
        /// <summary>
        /// 工单描述
        /// </summary>
        public string PODesc { get; set; }
        /// <summary>
        /// 详细属性名称
        /// </summary>
        public string DetailName { get; set; }
        /// <summary>
        /// 详细属性值
        /// </summary>
        public string DetailValue { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 属性名ID列表
        /// </summary>
        public List<int> DetailNameIds { get; set; }


    }
}
