using System.ComponentModel.DataAnnotations;

namespace SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;

public class ToolingLinkTooling_OldTooling_Input:MesInputDto
{
    /// <summary>
    /// 新治具条码
    /// </summary>
    [Required]
    public string S_NewToolingSN { get; set; }
    /// <summary>
    /// 旧治具条码
    /// </summary>
    [Required]
    public string S_OldToolingSN { get; set; }
}