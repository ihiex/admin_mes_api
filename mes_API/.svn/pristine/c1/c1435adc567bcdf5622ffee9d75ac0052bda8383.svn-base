using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.Security.Services.MES
{
    public class PalletPackageServices : BaseCommonService<string>, IPalletPackageServices
    {
        private readonly IPalletPackageRepository iRepository;

        public PalletPackageServices(IPalletPackageRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<PalletPackageOutput> DynamicSnVerifyAsync(PalletPackageInput input)
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

        public async Task<PalletPackageOutput> LastPalletSubmitAsync(MesSnInputDto input)
        {
            return await iRepository.LastPalletSubmitAsync(input);
        }

        public async Task<PalletPackageOutput> MainSnVerifyAsync(PalletPackageInput input)
        {
            return await iRepository.MainSnVerifyAsync(input);
        }

        public async Task<PalletPackageOutput> RemoveSingleAsync(PalletPackageInput input)
        {
            return await iRepository.RemoveSingleAsync(input);
        }

        public async Task<PalletPackageOutput> ReprintBoxSnAsync(MesSnInputDto input)
        {
            return await iRepository.ReprintBoxSnAsync(input);
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
        {
            return await iRepository.SetConfirmPOAsync(input);
        }
    }
}
