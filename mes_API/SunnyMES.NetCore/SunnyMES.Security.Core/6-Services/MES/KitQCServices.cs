using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.KitQC;
using SunnyMES.Security.IRepositories.MES;
using SunnyMES.Security.IServices.MES;

namespace SunnyMES.Security.Services.MES
{
    public class KitQCServices : BaseCommonService<string>, IKitQCServices
    {
        private readonly IKitQCRepository iRepository;

        public KitQCServices(IKitQCRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public void GetConfInfo(CommonHeader commonHeader)
        {
            iRepository.GetConfInfo(commonHeader);
        }

        public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
        {
            return await iRepository.GetPageInitializeAsync(S_URL);
        }

        public async Task<MesMainOutputDtos> MainSnVerifyAsync(MesSnInputDto input)
        {
            return await iRepository.MainSnVerifyAsync(input);
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto inputDto)
        {
            return await iRepository.SetConfirmPOAsync(inputDto.S_PartFamilyTypeID,inputDto.S_PartFamilyID,inputDto.S_PartID,inputDto.S_POID,inputDto.S_UnitStatus,inputDto.S_URL);
        }
    }
}
