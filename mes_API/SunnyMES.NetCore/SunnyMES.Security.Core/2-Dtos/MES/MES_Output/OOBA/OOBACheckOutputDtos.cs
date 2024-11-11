using System.Collections.Generic;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output;

namespace SunnyMES.Security.Dtos.MES.MES_Output.OOBA
{
    public class OOBACheckOutputDtos : MesOutputDto
    {
        public OOBACheckInfo BoxInformation { get; set; }
        public List<string> PrintSnList { get; set; }
        public PrinterParams PrinterParams { get; set; }
    }
}
