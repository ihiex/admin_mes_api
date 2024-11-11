using System;

namespace SunnyMES.Security._1_Models.MES;



/// <summary>
/// mesRouteDetail
/// </summary>
[Serializable]
public class mesRouteDetail
{
    /// <summary>
    /// 无
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int RouteID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int StationTypeID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? UnitStateID { get; set; }

}
