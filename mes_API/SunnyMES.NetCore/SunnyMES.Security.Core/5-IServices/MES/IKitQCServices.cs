using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.KitQC;

namespace SunnyMES.Security.IServices.MES
{
    public interface IKitQCServices : ICommonService<string>
    {
        void GetConfInfo(CommonHeader commonHeader);

        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto inputDto);
        Task<MesMainOutputDtos> MainSnVerifyAsync(MesSnInputDto input);
    }
}
