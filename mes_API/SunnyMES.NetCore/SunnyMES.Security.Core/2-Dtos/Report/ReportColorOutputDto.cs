using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型 ReportColor
    /// </summary>
    [Serializable]
    public class ReportColorOutputDto
    {
        /// <summary>
        /// 设置或获取  Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// PartFamilyType
        /// </summary>
        public string PartFamilyType { get; set; }

        /// <summary>
        /// PartFamily
        /// </summary>
        public string PartFamily { get; set; }

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
