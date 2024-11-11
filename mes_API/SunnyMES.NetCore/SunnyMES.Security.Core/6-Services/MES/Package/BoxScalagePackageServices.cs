using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxScalage;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.Security.Services.MES
{
    public class BoxScalagePackageServices : BaseCommonService<string>, IBoxScalagePackageServices
    {
        private readonly IBoxScalagePackageRepository iRepository;

        public BoxScalagePackageServices(IBoxScalagePackageRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<BoxScalageOutput> FinalWeightSubmitAsync(BoxScalageInput input)
        {
            return await iRepository.FinalWeightSubmitAsync(input);
        }

        public void GetConfInfo(CommonHeader commonHeader)
        {
            iRepository.GetConfInfo(commonHeader);
        }

        public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
        {
            return await iRepository.GetPageInitializeAsync(S_URL);
        }

        public async Task<BoxScalageOutput> MainSnVerifyAsync(MesSnInputDto input)
        {
            return await iRepository.MainSnVerifyAsync(input);
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
        {
            return await iRepository.SetConfirmPOAsync(input);
        }
    }
}
