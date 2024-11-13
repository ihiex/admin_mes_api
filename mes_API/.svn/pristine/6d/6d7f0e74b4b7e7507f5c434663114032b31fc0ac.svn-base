using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.OOBA;
using SunnyMES.Security.IRepositories.MES.Package;
using SunnyMES.Security.IServices.MES.Package;

namespace SunnyMES.Security.Services.MES.Package
{
    public class OOBAServices : BaseCommonService<string>, IOOBAServices
    {
        private readonly IOOBARepository iRepository;

        public OOBAServices(IOOBARepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<OOBACheckOutputDtos> BoxSnCheckAsync(MesSnInputDto input)
        {
            return await this.iRepository.BoxSnCheckAsync(input);
        }

        public async Task<OOBAReworkOutputDtos> BoxSnReworkAsync(MesSnInputDto input)
        {
            return await this.iRepository.BoxSnReworkAsync(input);
        }

        public void GetConfInfo(CommonHeader commonHeader)
        {
             this.iRepository.GetConfInfo(commonHeader);
        }

        public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
        {
            return await this.iRepository.GetPageInitializeAsync(S_URL);
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            return await iRepository.SetConfirmPOAsync(S_PartFamilyTypeID,S_PartFamilyID,S_PartID,S_POID,S_UnitStatus,S_URL);
        }
    }
}
