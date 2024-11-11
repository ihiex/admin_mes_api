using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.Dtos.MES.SAP
{
    public class SearchSAPManifestDataModel : SearchSignalInputDto
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
        /// BillNO
        /// </summary>
        public string BillNO { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string PalletSN { get; set; }
        public string Box_SN { get; set; }
        public string FG_SN { get; set; }
    }
}
