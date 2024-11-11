using System.Collections.Generic;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;

namespace SunnyMES.Security.Models;

public class InitPageInfo
{
    /// <summary>
    /// 站点属性
    /// </summary>
    public StationAttributes stationAttribute { get; set; } = new StationAttributes();

    /// <summary>
    /// po属性
    /// </summary>
    public PoAttributes poAttributes { get; set; } = new PoAttributes();
    /// <summary>
    /// 是否合法
    /// </summary>
    public string IsLegalPage { get; set; } = "0";

    /// <summary>
    /// 动态数据
    /// </summary>
    public dynamic DataList { get; set; } = null;
}