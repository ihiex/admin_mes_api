using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services;

public class PackageOverStationServices : BaseCommonService<string>,IPackageOverStationServices
{
    private readonly IPackageOverStationRepository _packageOverStationRepository;
    public PackageOverStationServices(IPackageOverStationRepository iRepository) : base(iRepository)
    {
        _packageOverStationRepository = iRepository;
    }

    public void GetConfInfo(CommonHeader commonHeader)
    {
        _packageOverStationRepository.GetConfInfo(commonHeader);
    }

    public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
    {
        return await _packageOverStationRepository.GetPageInitializeAsync(S_URL);
    }

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        return await _packageOverStationRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID,
            S_UnitStatus, S_URL);
    }

    public async Task<PalletSnVerifyOutput> PalletSnVerifyAsync(PackageOverStation_PalletSn_Input palletSnInput)
    {
        return await _packageOverStationRepository.PalletSnVerifyAsync(palletSnInput);
    }

    public async Task<BoxSnVerifyOutput> BoxSnVerifyAsync(PackageOverStation_BoxSn_Input boxSnInput)
    {
        return await _packageOverStationRepository.BoxSnVerifyAsync(boxSnInput);
    }
}