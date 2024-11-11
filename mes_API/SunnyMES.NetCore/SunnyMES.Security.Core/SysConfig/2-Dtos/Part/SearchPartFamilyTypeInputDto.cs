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
    /// 料号组类别分页查询输入参数
    /// </summary>
    public class SearchPartFamilyTypeInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 料号组类别名称
        /// </summary>
        public string PartFamilyTypeName { get; set; }
        /// <summary>
        /// 料号组类别描述
        /// </summary>
        public string PartFamilyTypeDesc { get; set; }
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
