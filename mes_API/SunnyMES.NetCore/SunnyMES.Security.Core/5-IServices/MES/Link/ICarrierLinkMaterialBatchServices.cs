using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES.CarrierLinkMaterialBatch;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices;

public interface ICarrierLinkMaterialBatchServices: ICommonService<string>
{
    void GetConfInfo(CommonHeader commonHeader);
    Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

    Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<MesOutputDto> CarrierSnVerifyAsync(CarrierLinkMaterialBatch_CarrierSN_Input carrierSNInput);

    Task<MesOutputDto> BatchNumberVerifyAsync(CarrierLinkMaterialBatch_BN_Input batchNumberInput);
}