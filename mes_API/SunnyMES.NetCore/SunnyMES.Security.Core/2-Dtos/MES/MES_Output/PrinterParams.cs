using System;

namespace SunnyMES.Security._2_Dtos.MES.MES_Output;

/// <summary>
/// 打印相关参数
/// </summary>
[Serializable]
public class PrinterParams
{
    /// <summary>
    /// 是否需要打印
    /// </summary>
    public bool IsPrint { get; set; } = false;
    /// <summary>
    /// 打印IP及端口
    /// </summary>
    public string PrintIPPort { get; set; }

    /// <summary>
    /// 打印模板路径,仅供前端显示
    /// </summary>
    public string LabelPath { get; set; }
    /// <summary>
    /// 打印指令
    /// </summary>
    public string LabelCommand { get; set; }
    /// <summary>
    /// 打印格式
    /// </summary>
    public string SNFormatName { get; set; }
    /// <summary>
    /// 出货栈板打印指令
    /// </summary>
    public string GSCommand { get; set; }
    /// <summary>
    /// 一个出货单号包含一个MPN为true, 包含多个MPN则为false
    /// </summary>
    public bool IsNormalSKU { get; set; } = true;

    public string GS1Name { get; set; }
    public string GS2Name { get; set; }

}