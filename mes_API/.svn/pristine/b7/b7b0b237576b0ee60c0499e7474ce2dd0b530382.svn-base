using System.Collections.Generic;
using SunnyMES.Security.Models;

namespace SunnyMES.Security._2_Dtos.MES;

public class UniversalConfirmPoOutput
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public dynamic ErrorMsg { get; set; } = null;
    /// <summary>
    /// Bom信息
    /// </summary>
    public List<mesProductStructure> MesProductStructures { get; set; }

    /// <summary>
    /// 图标工艺路线主表
    /// </summary>
    public List<dynamic> RouteDataDiagram1 { get; set; }
    /// <summary>
    /// 图标工艺路线子表
    /// </summary>
    public List<dynamic> RouteDataDiagram2 { get; set; }
    /// <summary>
    /// 表格工艺路线
    /// </summary>
    public List<dynamic> RouteDetail { get; set; }
    /// <summary>
    /// 工单数量
    /// </summary>
    public List<dynamic> ProductionOrderQTY { get; set; }

    /// <summary>
    /// 批次号正则校验
    /// </summary>
    public string S_Batch_Pattern { get; set; }

    /// <summary>
    /// 序列化正则校验
    /// </summary>
    public string S_SN_Pattern { get; set; }
}