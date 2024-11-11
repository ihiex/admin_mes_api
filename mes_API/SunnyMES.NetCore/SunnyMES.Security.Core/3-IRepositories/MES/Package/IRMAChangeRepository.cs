﻿using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES;
using SunnyMES.Security.Dtos.MES.MES_Output.OOBA;
using SunnyMES.Security.Dtos.MES.MES_Output.RMAChange;

namespace SunnyMES.Security.IRepositories.MES.Package
{
    public interface IRMAChangeRepository: ICommonRepository<string>
    {
        void GetConfInfo(CommonHeader commonHeader);
        Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL);

        Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

        Task<RMAChangeOutputDto> SnCheckAsync(MesSnInputDto input);

    }
}
