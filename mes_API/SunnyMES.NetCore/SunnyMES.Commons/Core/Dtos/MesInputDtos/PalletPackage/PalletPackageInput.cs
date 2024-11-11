using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage
{
    public class PalletPackageInput : MesSnInputDto
    {
        /// <summary>
        /// 箱码
        /// </summary>
        public string S_BoxSN { get; set; }
    }
}
