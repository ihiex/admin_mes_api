using System.ComponentModel.DataAnnotations;

namespace SunnyMES.Security._2_Dtos.MES.SNLinkBatch;

public class SNLinkBatch_SN_Input : MesInputDto   
{
    /// <summary>
    /// 批次号
    /// </summary>
    [Required]
    public string S_BatchNumber { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    [Required]
    public string S_SN { get; set; }
}