using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;

namespace SunnyMES.Security._3_IRepositories.MES
{
    public interface IPalletPackageRepository : ICommonRepository<string>
    {
        void GetConfInfo(CommonHeader commonHeader);
        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input);

        Task<PalletPackageOutput> MainSnVerifyAsync(PalletPackageInput input);

        Task<PalletPackageOutput> DynamicSnVerifyAsync(PalletPackageInput input);
        Task<PalletPackageOutput> RemoveSingleAsync(PalletPackageInput input);
        Task<PalletPackageOutput> LastPalletSubmitAsync(MesSnInputDto input);
        Task<PalletPackageOutput> ReprintBoxSnAsync(MesSnInputDto input);
    }
}
