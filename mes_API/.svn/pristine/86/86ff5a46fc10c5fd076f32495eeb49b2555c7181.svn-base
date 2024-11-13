using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.CarrierLinkMaterialBatch;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services;

public class CarrierLinkMaterialBatchServices : BaseCommonService<string>, ICarrierLinkMaterialBatchServices
{
    private readonly ICarrierLinkMaterialBatchRepository _iRepository;

    public CarrierLinkMaterialBatchServices(ICarrierLinkMaterialBatchRepository iRepository) : base(iRepository)
    {
        _iRepository = iRepository;
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
        return await _iRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus,
            S_URL);
    }

    public async Task<MesOutputDto> CarrierSnVerifyAsync(CarrierLinkMaterialBatch_CarrierSN_Input carrierSNInput)
    {
        return await _iRepository.CarrierSnVerifyAsync(carrierSNInput);
    }

    public async Task<MesOutputDto> BatchNumberVerifyAsync(CarrierLinkMaterialBatch_BN_Input batchNumberInput)
    {
        return await _iRepository.BatchNumberVerifyAsync(batchNumberInput);
    }
}