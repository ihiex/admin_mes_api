using System.Collections.Generic;
using SunnyMES.Commons.Core.Dtos;

namespace SunnyMES.Security._2_Dtos.MES.MES_Output.BoxPackageAuto;

/// <summary>
/// 产品装箱输出
/// </summary>
public class BoxPackageMesOutputDtos : MesOutputDto
{
    /// <summary>
    /// 卡通箱中已经装入的产品
    /// </summary>
    public List<CartonBoxConfirmed> CartonBoxConfirmeds { get; set; }
    /// <summary>
    /// 是否打包完成
    /// </summary>
    public bool IsPackingFinish { get; set; }

    /// <summary>
    /// 格式化后的箱码
    /// </summary>
    public string BoxSN { get; set; }
}