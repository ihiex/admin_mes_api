using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._2_Dtos.MES.MES_Output.ShipMent;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.IServices.MES.Package
{
    public interface IShipMentServices : ICommonService<string>
    {
        void GetConfInfo(CommonHeader commonHeader);
        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);
        Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input);
        Task<ShipMentOutput> MainSnVerifyAsync(MesSnInputDto input);
        Task<ShipMentOutput> MultipackSnVerifyAsync(ShipMentInput input);
        Task<ShipMentOutput> ReplaceBillNOAsync(ShipMentReplaceInput input);
        Task<ShipMentOutput> ReprintSnAsync(MesSnInputDto input);

        Task<ShipMentOutput> RemoveMultipackSnAsync(ShipMentInput input);
        Task<ShipMentOutput> UnpackShipmentPalletAsync(ShipMentInput input);
    }
}
