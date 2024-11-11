using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型 ReportTotalALL
    /// </summary>
    [Serializable]
    public class ReportTotalALLOutputDto
    {
        /// <summary>
        /// 设置或获取  Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// IO
        /// </summary>
        public string IO { get; set; }

        /// <summary>
        /// QTY
        /// </summary>
        public string QTY { get; set; }


    }
}
