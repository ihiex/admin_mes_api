﻿using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.IServices;

public interface ISNLinkUPCServices:ICommonService<string>
{
    void GetConfInfo(CommonHeader commonHeader);
    Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

    Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<MesOutputDto> MainSnVerifyAsync(SNLinkUPCInput input);

    Task<MesOutputDto> DynamicSnVerifyAsync(SNLinkUPCInput input);
}