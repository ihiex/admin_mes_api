using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkBatch;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices;

namespace SunnyMES.Security.Services;

public class SNLinkBatchServices : BaseCommonService<string>, ISNLinkBatchServices
{
    private readonly ISNLinkBatchRepository iRepository;

    public SNLinkBatchServices(ISNLinkBatchRepository iRepository) : base(iRepository)
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

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        return await iRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus,
            S_URL);
    }

    public async Task<MesOutputDto> BatchSnVerifyAsync(SNLinkBatch_BSN_Input input)
    {
        return await iRepository.BatchSnVerifyAsync(input);
    }

    public async Task<MesOutputDto> LinkSnVerifyAsync(SNLinkBatch_SN_Input input)
    {
        return await iRepository.LinkSnVerifyAsync(input);
    }


}