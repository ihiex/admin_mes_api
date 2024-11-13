using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.OOBA;
using SunnyMES.Security.Dtos.MES.MES_Output.RMAChange;
using SunnyMES.Security.IRepositories.MES.Package;
using SunnyMES.Security.IServices.MES.Package;

namespace SunnyMES.Security.Services.MES.Package
{
    public class RMAChangeServices : BaseCommonService<string>, IRMAChangeServices
    {
        private readonly IRMAChangeRepository iRepository;

        public RMAChangeServices(IRMAChangeRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
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

        public async Task<RMAChangeOutputDto> SnCheckAsync(MesSnInputDto input)
        {
            return await this.iRepository.SnCheckAsync(input);
        }
    }
}
