using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security.Dtos.MES;

namespace SunnyMES.Security.IRepositories.MES
{
    public interface IAssemblyDynamicRepository : IMesBaseRepository
    {
        Task<AssemblySetPoOutputDto> SetConfirmPOAsync(MesInputDto inputDto);
        Task<AssemblyDynamicOutputDto> DynamicSnVerifyAsync(AssemblyDynamicInput input);
        Task<AssemblyDynamicOutputDto> ReleaseMachineSNAsync(AssemblyDynamicInput input);
    }
}
