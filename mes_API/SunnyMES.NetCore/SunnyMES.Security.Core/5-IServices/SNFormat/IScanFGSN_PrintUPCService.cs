using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface IScanFGSN_PrintUPCService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

        Task<CreateSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, ConfirmPOOutputDto v_ConfirmPOOutputDto);
    }
}
