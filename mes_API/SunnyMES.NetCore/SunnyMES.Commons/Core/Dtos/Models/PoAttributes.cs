using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Module;


namespace SunnyMES.Commons.Core.Dtos;
public class PoAttributes
{
    public virtual string MPN { get; set; }
    public virtual string BoxLabelTemplatePath { get; set; }
    public virtual string PalletLabelTemplatePath { get; set; }
    public virtual string UPC { get; set; }
    public virtual string BoxQty { get; set; }
    public virtual string UPC_LabelTemplatePath { get; set; }
    public virtual string PalletQty { get; set; }
    public virtual string Jan { get; set; }
    public virtual string Region { get; set; }
    public virtual string GSI { get; set; }
    public virtual string BoxSN_Pattern { get; set; }
    public virtual string BoxSNFormatName { get; set; }
    public virtual string IsGenerateBoxSN { get; set; }
    public virtual string IsGeneratePalletSN { get; set; }
    public virtual string PalletSN_Pattern { get; set; }
    public virtual string PalletSNFormatName { get; set; }
    public virtual string IsMixedPO { get; set; }
    public virtual string IsMixedPN { get; set; }
    public virtual string SCC { get; set; }
    public virtual string SN_Pattern { get; set; }
    public virtual string UPC_BartenderPath { get; set; }
    public virtual string PalletBartenderPath { get; set; }
    public virtual string BoxBartenderPath { get; set; }
    /// <summary>
    /// 是否扫描UPC Code
    /// </summary>
    [FuncDescription("UPC Code", "")]
    public virtual string IsScanUPC { get; set; }
    /// <summary>
    /// 是否扫描JAN Code
    /// </summary>
    [FuncDescription("JAN Code", "")]
    public virtual string IsScanJAN { get; set; }
    public virtual string DOE_Parameter1 { get; set; }
    public virtual string MultipackScanOnlyFGSN { get; set; }
    public virtual string IsCreateUPCSN { get; set; }
    public virtual string DOE_ProjectPhase { get; set; }
    public virtual string DOE_ConfigNumber { get; set; }
    public virtual string ShipmentRegion { get; set; }
    public virtual string IsScanFGSN { get; set; }
    public virtual string IsScanUPCSN { get; set; }



    public virtual string _7ECode {get;set;}
    public string GTIN { get; set; }
    public string COO { get; set; }
    public string PRODUCT { get; set; }
    public string JANTite { get; set; }
    public string _7Q { get; set; }

    /// <summary>
    /// 针对新增的参数放在此处
    /// </summary>
    public Dictionary<string,string> OtherProperty { get; set; } = new Dictionary<string, string>();
    #region 给前端提供定制值
    public string[] ColorCode { get; set; }
    #endregion

}
