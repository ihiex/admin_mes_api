using System;

namespace SunnyMES.Security._1_Models.MES;


/// <summary>
/// mesUnitOutputState
/// </summary>
[Serializable]
public class mesUnitOutputState
{
    /// <summary>
    /// 无
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? StationTypeID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? RouteID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? CurrStateID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? OutputStateID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? OutputStateDefID { get; set; }

}
