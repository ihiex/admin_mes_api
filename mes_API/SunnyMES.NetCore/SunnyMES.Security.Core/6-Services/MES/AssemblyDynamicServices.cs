using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.AssemblyDynamic;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.IRepositories.MES;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.Security.Services.MES
{
    public class AssemblyDynamicServices : BaseCommonService<string>, IAssemblyDynamicServices
    {
        private readonly IAssemblyDynamicRepository iRepository;

        public AssemblyDynamicServices(IAssemblyDynamicRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<AssemblyDynamicOutputDto> DynamicSnVerifyAsync(AssemblyDynamicInput input)
        {
            return await iRepository.DynamicSnVerifyAsync(input);
        }

        public void GetConfInfo(CommonHeader commonHeader)
        {
            iRepository.GetConfInfo(commonHeader);
        }

        public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
        {
            return await iRepository.GetPageInitializeAsync(S_URL);
        }

        public async Task<AssemblyDynamicOutputDto> ReleaseMachineSNAsync(AssemblyDynamicInput input)
        {
            return await iRepository.ReleaseMachineSNAsync(input);
        }

        public async Task<AssemblySetPoOutputDto> SetConfirmPOAsync(MesInputDto inputDto)
        {
            return await iRepository.SetConfirmPOAsync(inputDto);
        }
    }
}
