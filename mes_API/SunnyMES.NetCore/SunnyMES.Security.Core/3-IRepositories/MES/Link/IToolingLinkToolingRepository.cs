﻿using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security._2_Dtos.MES.CarrierLinkMaterialBatch;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;

namespace SunnyMES.Security._3_IRepositories.MES;

public interface IToolingLinkToolingRepository : ICommonRepository<string>
{
    void GetConfInfo(CommonHeader commonHeader);
    Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

    Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<MesOutputDto> NewToolingSnVerifyAsync(ToolingLinkTooling_NewTooling_Input newToolingInput);

    Task<MesOutputDto> OldToolingSnVerifyAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput);
    Task<MesOutputDto> OldToolingSnReleaseAsync(ToolingLinkTooling_OldTooling_Input oldToolingInput);
}