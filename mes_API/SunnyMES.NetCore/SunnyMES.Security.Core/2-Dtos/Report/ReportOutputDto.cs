using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型 ReportOutput
    /// </summary>
    [Serializable]
    public class ReportOutputDto
    {
        /// <summary>
        /// List_ReportTotalALL
        /// </summary>

        public List<ReportTotalALLOutputDto> List_ReportTotalALL { get; set; }

        /// <summary>
        /// List_ReportTotalProject
        /// </summary>
        public List<ReportTotalProjectOutputDto> List_ReportTotalProject { get; set; }

        /// <summary>
        /// List_ReportColor
        /// </summary>
        public List<ReportColorOutputDto> List_ReportColor { get; set; }
        /// <summary>
        /// List_ReportColorDetail
        /// </summary>
        public List<ReportColorDetailOutputDto> List_ReportColorDetail { get; set; }
        /// <summary>
        /// List_ReportTotalPart
        /// </summary>
        public List<ReportTotalPartOutputDto> List_ReportTotalPart { get; set; }


    }
}
