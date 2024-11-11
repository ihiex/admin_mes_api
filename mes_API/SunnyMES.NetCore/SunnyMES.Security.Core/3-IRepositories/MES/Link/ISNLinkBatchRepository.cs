﻿using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.ToolingLinkTooling;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.SNLinkBatch;

namespace SunnyMES.Security._3_IRepositories.MES;

public interface ISNLinkBatchRepository : ICommonRepository<string>
{
    void GetConfInfo(CommonHeader commonHeader);
    Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

    Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<MesOutputDto> BatchSnVerifyAsync(SNLinkBatch_BSN_Input input);

    Task<MesOutputDto> LinkSnVerifyAsync(SNLinkBatch_SN_Input input);
}