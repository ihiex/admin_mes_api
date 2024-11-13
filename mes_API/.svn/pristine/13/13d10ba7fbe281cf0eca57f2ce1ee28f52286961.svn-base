using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices;

namespace SunnyMES.Security.Services;

public class ToolingLinkToolingServices : BaseCommonService<string>,IToolingLinkToolingServices
{
    private readonly IToolingLinkToolingRepository _iRepository;

    public ToolingLinkToolingServices(IToolingLinkToolingRepository iRepository) : base(iRepository)
    {
        this._iRepository = iRepository;
    }


    public void GetConfInfo(CommonHeader commonHeader)
    {
        _iRepository.GetConfInfo(commonHeader);
    }

    public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
    {
        return await _iRepository.GetPageInitializeAsync(S_URL);
    }

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        return await _iRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);
    }

    public async Task<MesOutputDto> NewToolingSnVerifyAsync(ToolingLinkTooling_NewTooling_Input newToolingInput)
    {
        return await _iRepository.NewToolingSnVerifyAsync(newToolingInput);
    }

    public async Task<MesOutputDto> OldToolingSnVerifyAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput)
    {
        return await _iRepository.OldToolingSnVerifyAsync(oldToolingInput);
    }

    public async Task<MesOutputDto> OldToolingSnReleaseAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput)
    {
        return await _iRepository.OldToolingSnReleaseAsync(oldToolingInput);
    }
}