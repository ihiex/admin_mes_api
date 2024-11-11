using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IReworkRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        //Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
        //    string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

        Task<GetReworkDto> SetScanSN_GetRework(string S_SN, string S_Type, string S_URL);

        Task<string> SetRework(string S_SN, string S_Type);

        Task<ReworkPrintDto> ReworkPrint(string S_SN, string S_ListSN, string S_Type, string S_URL);
    }
}