using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.IServices;

public interface IBoxPackageAutoServices:ICommonService<string>
{
    void GetConfInfo(CommonHeader commonHeader);
    Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

    Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<BoxPackageMesOutputDtos> MainSnVerifyAsync(BoxPackageAutoInput input);

    Task<BoxPackageMesOutputDtos> DynamicSnVerifyAsync(BoxPackageAutoInput input);
    Task<BoxPackageMesOutputDtos> RemoveSingleAsync(BoxPackageRemove input);
    Task<BoxPackageMesOutputDtos> ReprintBoxSnAsync(MesSnInputDto input);
    Task<BoxPackageMesOutputDtos> LastBoxSubmitAsync(MesSnInputDto input);
}