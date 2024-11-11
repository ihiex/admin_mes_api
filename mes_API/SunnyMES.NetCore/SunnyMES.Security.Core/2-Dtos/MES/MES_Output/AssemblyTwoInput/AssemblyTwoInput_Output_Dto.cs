using System.Collections.Generic;
using SunnyMES.Security.Models;

namespace SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyTwoInput
{
    public class AssemblyTwoInput_Output_Dto :MesOutputDto
    {
        public InitPageInfo initPageInfos { get; set; }
        public List<BomPartInfo> BomPartInfo { get; set; }
        public int AssemblyQty { get; set; } = 0;
    }
}
