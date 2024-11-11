using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;

namespace SunnyMES.Security.IServices.MES
{
    public interface IPalletPackageServices : ICommonService<string>
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
