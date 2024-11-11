using System.Reflection.Metadata.Ecma335;

namespace SunnyMES.Security._1_Models.MES.Query;

public class PoDetailDefs
{
    public int ID { get; set; }
    /// <summary>
    /// 工单ID
    /// </summary>
    public int ProductionOrderID { get; set; }
    /// <summary>
    /// 工单号
    /// </summary>
    public string ProductionOrderNumber { get; set; }
    /// <summary>
    /// PO详细描述Id
    /// </summary>
    public int ProductionOrderDetailDefID { get; set; }
    /// <summary>
    /// PO详细描述
    /// </summary>
    public string PODetailDef { get; set; }
    /// <summary>
    /// PO详细内容
    /// </summary>
    public string Content { get; set; }
}