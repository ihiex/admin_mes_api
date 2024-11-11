using System.ComponentModel.DataAnnotations;

namespace SunnyMES.Security._2_Dtos.MES.SNLinkBatch;

/// <summary>
/// 批次号校验输入参数
/// </summary>
public class SNLinkBatch_BSN_Input : MesInputDto
{
    /// <summary>
    /// 批次号
    /// </summary>
    [Required]
    public string S_BatchNumber { get; set; }
}