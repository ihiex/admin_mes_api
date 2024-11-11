using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.Models;
namespace SunnyMES.Security.Dtos.MES.SAP
{
    public class SearchShipmentNewDataModel : SearchSignalInputDto
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
    }
}
