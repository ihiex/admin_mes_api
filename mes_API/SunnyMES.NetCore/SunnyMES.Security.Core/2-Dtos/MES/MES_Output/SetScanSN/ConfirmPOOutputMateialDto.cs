using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// ConfirmPOOutputMateialDto
    /// </summary>
    [Serializable]
    public class ConfirmPOOutputMateialDto
    {
        public TabVal MSG { get; set; }

        public List<mesProductStructure> mesProductStructures { get; set; }
        public List<mesRoute> Route { get; set; }
        public List<dynamic> RouteDataDiagram1 { get; set; }
        public List<dynamic> RouteDataDiagram2 { get; set; }
        public List<dynamic> RouteDetail { get; set; }
        public List<dynamic> ProductionOrderQTY { get; set; }

        public List<luVendor> Vendor { get; set; }
        public List<MaterailBomData> GetMaterailBomData { get; set; }
        public List<string> GetTranceCode { get; set; }


        public string LabelPath { get; set; }
        public string SNFormatName { get; set; }

        public string SN_Pattern { get; set; }
        public string Batch_Pattern { get; set; }
        public string MaterialBatchQTY { get; set; }
        public string MaterialLable { get; set; }
        public string M_UnitConversion_PCS { get; set; }
        public string MaterialAuto { get; set; }
        public string MaterialCodeRules { get; set; }
        public string Expires_Time { get; set; }

        public Boolean IsForceSplit { get; set; }
        public string SplitBatchQTY { get; set; }

        public string Unit{ get; set; }
        public string BatchQTY { get; set; }

        public Boolean LotCode { get; set; }
        public Boolean TranceCode { get; set; }

        public List<MaterailBatchData> List_MaterailBatchData { get; set; }


        public Boolean DOE_BuildNameEnabled { get; set; }
        public List<string> List_DOE_BuildName{ get; set; }

        public Boolean DOE_CCCCEnabled { get; set; }
        public int DOE_CCCC_Length { get; set; }

    }
}
