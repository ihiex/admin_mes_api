using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.Dtos.MES.MES_Output.KitQC
{
    public class MesMainOutputDtos : MesOutputDto
    {
        /// <summary>
        /// 是否显示且启用子码输入框
        /// </summary>
        public bool IsShowChildInput { get; set; } = false;
    }
}
