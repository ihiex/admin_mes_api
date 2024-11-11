namespace SunnyMES.Commons.Core.PublicFun.Model;

/// <summary>
/// 通用请求头
/// </summary>
public class CommonHeader
{
    /// <summary>
    ///当前使用的语言
    /// 0: CH
    /// 1: EN
    /// </summary>
    public int Language { get; set; }
    /// <summary>
    /// 当前登录的线别ID
    /// </summary>
    public int LineId { get; set; }

    /// <summary>
    /// 线别名称
    /// </summary>
    public string LineName { get; set; }

    /// <summary>
    /// 当前登录的站点ID
    /// </summary>
    public int StationId { get; set; } = 0;

    /// <summary>
    /// 站点名称
    /// </summary>
    public string StationName { get; set; }
    /// <summary>
    /// 当前登录的用户ID
    /// </summary>
    public int EmployeeId { get; set; }
    /// <summary>
    /// 当前连接客户端的IP
    /// </summary>
    public string CurrentLoginIp { get; set; }

    /// <summary>
    /// 后端配置链接 
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 前端传递的页面ID
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserFullName { get; set; }
}