using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices;

namespace SunnyMES.Security.Services;

public class SNLinkUPCServices : BaseCommonService<string>, ISNLinkUPCServices
{
    private readonly IHttpContextAccessor accessor;
    private readonly ISNLinkUPCRepository iRepository;

    public SNLinkUPCServices(ISNLinkUPCRepository iRepository) : base(iRepository)
    {
        this.iRepository = iRepository;
    }

    public SNLinkUPCServices(ISNLinkUPCRepository iRepository, IHttpContextAccessor accessor) : base(iRepository, accessor)
    {
        this.iRepository = iRepository;
        this.accessor = accessor;
    }

    public void GetConfInfo(CommonHeader commonHeader)
    {
        iRepository.GetConfInfo(commonHeader);
    }

    public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
    {
        return  await iRepository.GetPageInitializeAsync(S_URL);
    }

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        return await iRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus,
            S_URL);
    }

    public async Task<MesOutputDto> MainSnVerifyAsync(SNLinkUPCInput input)
    {
        return await iRepository.MainSnVerifyAsync(input);
    }

    public async Task<MesOutputDto> DynamicSnVerifyAsync(SNLinkUPCInput input)
    {
        return await iRepository.DynamicSnVerifyAsync(input);
    }


}