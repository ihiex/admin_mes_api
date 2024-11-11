using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.Dtos.Models;

namespace SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage
{
    public class PalletPackageOutput:MesOutputDto
    {
        /// <summary>
        /// 卡通箱中已经装入的产品
        /// </summary>
        public List<PalletConfirmed> PalletConfirmeds { get; set; }
        /// <summary>
        /// 是否打包完成
        /// </summary>
        public bool IsPackingFinish { get; set; }
        /// <summary>
        /// 栈板条码
        /// </summary>
        public string PalletSn { get; set; }
    }
}
