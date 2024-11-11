
namespace SunnyMES.Commons.Core.Dtos;

public class PartAttributes
{
    public virtual string SNFormat { get; set; }
    /// <summary>
    /// 数据库中的名称为CCCC Code
    /// 实际处理中，需要进行特别处理
    /// </summary>
    public virtual string CCCC_Code { get; set; }
    public virtual string Batch_Pattern { get; set; }
    public virtual string ScanType { get; set; }
    public virtual string SN_Pattern { get; set; }
    public virtual string MaterialBatchQTY { get; set; }
    public virtual string MaterialCodeRules { get; set; }
    public virtual string MaterialAuto { get; set; }
    public virtual string MaterialLable { get; set; }
    public virtual string SplitBatchQTY { get; set; }
    public virtual string IsForceSplit { get; set; }
    public virtual string InnerSN_Pattern { get; set; }
    public virtual string Color { get; set; }
    public virtual string IsInsightModule { get; set; }
    public virtual string DOE_Parameter1 { get; set; }
    public virtual string GS1_PalletLabelName { get; set; }
    public virtual string GS2_PalletLabelName { get; set; }
    public virtual string ISModuleDescription { get; set; }
    public virtual string ISAPN { get; set; }
    public virtual string ISVendor { get; set; }
    public virtual string BoxWeightBase { get; set; }
    public virtual string BoxWeightUpperLimit { get; set; }
    public virtual string BoxWeightLowerLimit { get; set; }
    public virtual string BoxWeightBaseOffset { get; set; }
    public virtual string BoxWeightUnit { get; set; }
    public virtual string ColorValue { get; set; }
    public virtual string IsTimeCheck { get; set; }
    public virtual string TimeCheckStartStationType { get; set; }
    public virtual string TimeCheckEndStationType { get; set; }
    public virtual string TimeCheckMin { get; set; }
    public virtual string TimeCheckMax { get; set; }
    public virtual string IsInsightModuleSN { get; set; }
    public virtual string IsTTWIPModule { get; set; }
    public virtual string TTWIPModuleDescription { get; set; }
    public virtual string FullNumber { get; set; }
    /// <summary>
    /// 中箱称重参数配置
    /// Base=3.57;Unit=kg;UL=+0.05;LL=-0.04
    /// </summary>
    public string PackBoxWeightLimit { get; set; }
}