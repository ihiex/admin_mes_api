using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.Dtos.MES.SAP
{
    public class SearchSAPDataModel : SearchSignalInputDto
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 运输单号
        /// </summary>
        public string HAWB { get; set; }

        /// <summary>
        /// 项目查询
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 货运号
        /// </summary>
        public string HubCode { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        public string PO { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// 料号描述
        /// </summary>
        public string PartNumberDesc { get; set; }
        /// <summary>
        /// 状态ID ,目前仅有4中状态   -- 0,1,2,3
        /// </summary>

        public int import { get; set; }
    }
}
