using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SunnyMES.Security._2_Dtos.MES.PackageOverStation;

/// <summary>
/// 箱码扫描输入实体类型
/// </summary>
[Serializable]
public class PackageOverStation_BoxSn_Input : MesInputDto
{
    /// <summary>
    /// 中箱条码
    /// </summary>
    [Required]
    public string S_BoxSN { get; set; }
    /// <summary>
    /// 是否是正常入库
    /// true:正常入库
    /// false:退仓
    /// </summary>
    [Required]
    public bool IsEntry { get; set; }
}