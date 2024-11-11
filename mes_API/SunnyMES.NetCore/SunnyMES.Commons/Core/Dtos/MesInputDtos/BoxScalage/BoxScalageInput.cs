using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage
{
    [Serializable]
    public class BoxScalageInput : MesSnInputDto
    {
        /// <summary>
        /// 整箱重量
        /// </summary>
        public double BoxWeight { get; set; }
    }
}
