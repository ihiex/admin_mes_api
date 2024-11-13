using SunnyMES.Commons.Device;

namespace SunnyMES.Commons.Core.Dtos;


/// <summary>
/// 动态项定义
/// </summary>
public class DynamicItemsDto
{
    /// <summary>
    /// Po detail def ID
    /// </summary>
    public int PoDefID { get; set; }
    /// <summary>
    /// 动态项名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 动态项描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 动态项值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; } = false;

    /// <summary>
    /// 是否有默认标准值，当有值时，需要检查当前项的值是否存在你当前默认值中
    /// </summary>
    public string[] DefaultSpec { get; set; }

    /// <summary>
    /// 当前项是提交请求项
    /// </summary>
    public bool IsCurrentItem { get; set; } = false;

    /// <summary>
    /// 当前项已扫描完成
    /// </summary>
    public bool IsScanFinished { get; set; } = false;

    /// <summary>
    /// 检查类型
    /// </summary>
    public int CheckType { get; set; }

    /// <summary>
    /// 动态参数
    /// </summary>
    public string Parameters { get; set; }

    /// <summary>
    /// 父类检查项ID
    /// </summary>
    public int ParentCheckID { get; set; }
    /// <summary>
    /// 是否是必须项
    /// </summary>
    public bool Required { get; set; }
}