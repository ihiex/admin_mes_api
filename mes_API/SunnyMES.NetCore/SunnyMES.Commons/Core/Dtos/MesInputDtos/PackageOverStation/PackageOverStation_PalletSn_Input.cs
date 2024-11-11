using System.ComponentModel.DataAnnotations;

namespace SunnyMES.Security._2_Dtos.MES.PackageOverStation;

/// <summary>
/// 栈板扫描输入类型
/// </summary>
public class PackageOverStation_PalletSn_Input :MesInputDto
{

    /// <summary>
    /// 栈板条码
    /// </summary>
    [Required]
    public string S_PalletSN { get; set; }
    /// <summary>
    /// 是否是反入库
    /// true: 正常入库
    /// false: 反入库
    /// </summary>
    [Required]
    public bool IsEntry { get; set; }
}