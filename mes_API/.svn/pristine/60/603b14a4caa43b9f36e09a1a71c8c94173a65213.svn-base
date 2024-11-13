using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxScalage;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage;

namespace SunnyMES.Security._3_IRepositories.MES
{
    public interface IBoxScalagePackageRepository : ICommonRepository<string>
    {
        void GetConfInfo(CommonHeader commonHeader);
        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input);

        Task<BoxScalageOutput> MainSnVerifyAsync(MesSnInputDto input);

        Task<BoxScalageOutput> FinalWeightSubmitAsync(BoxScalageInput input);
    }
}
