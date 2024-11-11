using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices;

namespace SunnyMES.Security.Services;

public class BoxPackageAutoSimplifyServices : BaseCommonService<string>, IBoxPackageAutoSimplifyServices
{
    private readonly IHttpContextAccessor accessor;
    private readonly IBoxPackageAutoSimplifyRepository iRepository;

    public BoxPackageAutoSimplifyServices(IBoxPackageAutoSimplifyRepository iRepository) : base(iRepository)
    {
        this.iRepository = iRepository;
    }

    public BoxPackageAutoSimplifyServices(IBoxPackageAutoSimplifyRepository iRepository, IHttpContextAccessor accessor) : base(iRepository, accessor)
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

    public async Task<BoxPackageMesOutputDtos> MainSnVerifyAsync(BoxPackageAutoInput input)
    {
        return await iRepository.MainSnVerifyAsync(input);
    }

    public async Task<BoxPackageMesOutputDtos> DynamicSnVerifyAsync(BoxPackageAutoInput input)
    {
        return await iRepository.DynamicSnVerifyAsync(input);
    }

    public async Task<BoxPackageMesOutputDtos> RemoveSingleAsync(BoxPackageRemove input)
    {
        return await iRepository.RemoveSingleAsync(input);
    }

    public async Task<BoxPackageMesOutputDtos> ReprintBoxSnAsync(MesSnInputDto input)
    {
        return await iRepository.ReprintBoxSnAsync(input);
    }

    public async Task<BoxPackageMesOutputDtos> LastBoxSubmitAsync(MesSnInputDto input)
    {
        return await iRepository.LastBoxSubmitAsync(input);
    }
}