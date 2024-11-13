using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.ShipMent;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES.Package;
using SunnyMES.Security.IServices.MES.Package;

namespace SunnyMES.Security.Services.MES.Package
{
    public class ShipMentServices : BaseCommonService<string>, IShipMentServices
    {
        private readonly IShipMentRepository iRepository;

        public ShipMentServices(IShipMentRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public void GetConfInfo(CommonHeader commonHeader)
        {
            iRepository.GetConfInfo(commonHeader);
        }

        public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
        {
            return await iRepository.GetPageInitializeAsync(S_URL);
        }

        public async Task<ShipMentOutput> MainSnVerifyAsync(MesSnInputDto input)
        {
            return await iRepository.MainSnVerifyAsync(input);
        }

        public async Task<ShipMentOutput> MultipackSnVerifyAsync(ShipMentInput input)
        {
            return await iRepository.MultipackSnVerifyAsync(input);
        }



        public async Task<ShipMentOutput> ReplaceBillNOAsync(ShipMentReplaceInput input)
        {
            return await iRepository.ReplaceBillNOAsync(input);
        }

        public async Task<ShipMentOutput> ReprintSnAsync(MesSnInputDto input)
        {
            return await iRepository.ReprintSnAsync(input);
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
        {
            return await iRepository.SetConfirmPOAsync(input);
        }
        public async Task<ShipMentOutput> RemoveMultipackSnAsync(ShipMentInput input) => await iRepository.RemoveMultipackSnAsync(input);
        public async Task<ShipMentOutput> UnpackShipmentPalletAsync(ShipMentInput input) => await iRepository.UnpackShipmentPalletAsync(input);
    }
}
