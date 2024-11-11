namespace SunnyMES.Commons.Enums;

/// <summary>
///     系统存在数据_SN1 = 1 
///     非系统注册批次_Batch校验数量_不校验唯一性2 = 2,
///     设备数据_绑定过批次_类似托盘3 = 3,
///     设备数据_无批次_类似夹具4 = 4,
///     非系统注册SN_校验唯一性5 = 5,
///     设备数据_包含3_4类型_ValidFrom工位可以重复绑定6 = 6,
///     系统注册批次_Batch校验数量7 = 7
/// </summary>
public enum ScanType
{
    /// <summary>
    /// 系统存储数据
    /// </summary>
    系统存在数据_SN1 = 1,
    /// <summary>
    /// 非系统注册批次_Batch校验数量_不校验唯一性2
    /// </summary>
    非系统注册批次_Batch校验数量_不校验唯一性2 = 2,
    /// <summary>
    /// 设备数据_绑定过批次_类似托盘3
    /// </summary>
    设备数据_绑定过批次_类似托盘3 = 3,
    /// <summary>
    /// 设备数据_无批次_类似夹具4
    /// </summary>
    设备数据_无批次_类似夹具4 = 4,
    /// <summary>
    /// 非系统注册SN_校验唯一性5
    /// </summary>
    非系统注册SN_校验唯一性5 = 5,
    /// <summary>
    /// 设备数据_包含3_4类型_ValidFrom工位可以重复绑定6
    /// </summary>
    设备数据_包含3_4类型_ValidFrom工位可以重复绑定6 = 6,
    /// <summary>
    /// 系统注册批次_Batch校验数量7
    /// </summary>
    系统注册批次_Batch校验数量7 = 7,
    /// <summary>
    /// 注册公用批次_在子批次料号下校验
    /// 需要在BOM中建立公共批次与子批次的父子关系
    /// </summary>
    注册公用批次_扫描子批次校验8 = 8
}