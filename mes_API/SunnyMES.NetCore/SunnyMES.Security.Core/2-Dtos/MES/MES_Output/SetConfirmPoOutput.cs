using System;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES.MES_Output;

namespace SunnyMES.Security._2_Dtos.MES.PackageOverStation;

/// <summary>
/// 通用Set PO output
/// </summary>
[Serializable]
public class SetConfirmPoOutput : MesOutputDto
{
    /// <summary>
    /// 通用确认PO输出
    /// </summary>
    public UniversalConfirmPoOutput UniversalConfirmPoOutput { get; set; }
    /// <summary>
    /// 在确认完PO后，向串口发送的指令
    /// </summary>
    public dynamic DynamicCommand { get; set; }

    /// <summary>
    /// 打印相关参数
    /// </summary>
    public PrinterParams PrinterParams { get; set; }

    /// <summary>
    /// 称重参数
    /// </summary>
    public ScalageConfig ScalageConfig { get; set; }
}