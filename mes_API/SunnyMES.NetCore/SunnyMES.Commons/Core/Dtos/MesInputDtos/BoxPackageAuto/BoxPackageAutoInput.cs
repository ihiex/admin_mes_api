using System.Collections.Generic;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;

namespace SunnyMES.Security._2_Dtos.MES.BoxPackageAuto;

public class BoxPackageAutoInput: SNLinkUPCInput
{
    /// <summary>
    /// 箱码
    /// </summary>
    public override string S_SN { get; set; }

    /// <summary>
    /// 单个动态项
    /// </summary>
    public override SortedList<int, DynamicItemsDto> DataList { get; set; }

}