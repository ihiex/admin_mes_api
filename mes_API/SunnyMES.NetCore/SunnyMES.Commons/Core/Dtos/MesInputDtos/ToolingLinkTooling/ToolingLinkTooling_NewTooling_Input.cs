using System.ComponentModel.DataAnnotations;

namespace SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;

public class ToolingLinkTooling_NewTooling_Input:MesInputDto
{
    /// <summary>
    /// 新治具条码
    /// </summary>
    [Required]
    public string S_NewToolingSN { get; set; }
}