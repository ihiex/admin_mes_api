using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型 ReportUPHYieldOutputDto
    /// </summary>
    [Serializable]
    public class ReportUPHYieldOutputDto
    {
        /// <summary>
        /// List_1
        /// </summary>

        public List<ReportUPHYieldData1OutputDto> List_1 { get; set; }

        /// <summary>
        /// List_2
        /// </summary>
        public List<ReportUPHYieldData2OutputDto> List_2 { get; set; }




    }
}
