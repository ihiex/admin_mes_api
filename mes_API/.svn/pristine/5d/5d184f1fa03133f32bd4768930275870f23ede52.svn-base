using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxScalage;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage;

namespace SunnyMES.Security.IServices.MES
{
    public interface IBoxScalagePackageServices : ICommonService<string>
    {
        void GetConfInfo(CommonHeader commonHeader);
        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input);

        Task<BoxScalageOutput> MainSnVerifyAsync(MesSnInputDto input);

        Task<BoxScalageOutput> FinalWeightSubmitAsync(BoxScalageInput input);
    }
}
