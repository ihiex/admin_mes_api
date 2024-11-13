using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices;

public interface IAssemblyTwoInputServices : ICommonService<string>
{
    Task<List<TabVal>> GetConfInfoAsync(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
    Task<IEnumerable<dynamic>> GetPageInitializeAsync(string S_URL);

    Task<List<dynamic>> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);

    Task<List<dynamic>> GetColorCodeAsync();

    Task<List<dynamic>> MainSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN);
    Task<List<dynamic>> ChildSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, string ChildSN, string sColorCode, string sBuildCode, string sSpcaCode, string sPpCode);
}