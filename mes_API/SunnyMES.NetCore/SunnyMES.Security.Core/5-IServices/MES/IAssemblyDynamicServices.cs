using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.AssemblyDynamic;
using SunnyMES.Security.Dtos.MES;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.IServices.MES
{
    public interface IAssemblyDynamicServices : ICommonService<string>
    {
        void GetConfInfo(CommonHeader commonHeader);

        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<AssemblySetPoOutputDto> SetConfirmPOAsync(MesInputDto inputDto);

        Task<AssemblyDynamicOutputDto> DynamicSnVerifyAsync(AssemblyDynamicInput input);
        Task<AssemblyDynamicOutputDto> ReleaseMachineSNAsync(AssemblyDynamicInput input);
    }
}
