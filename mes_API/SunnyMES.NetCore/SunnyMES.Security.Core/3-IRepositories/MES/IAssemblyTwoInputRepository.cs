using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories;

public interface IAssemblyTwoInputRepository : ICommonRepository<string>
{
    Task<List<TabVal>> GetConfInfoAsync(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
    Task<IEnumerable<dynamic>> GetPageInitializeAsync(string S_URL);

    Task<List<dynamic>> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL);


    Task<List<dynamic>> GetColorCodeAsync();
    Task<List<dynamic>> MainSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSn);
    Task<List<dynamic>> ChildSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
        string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, string ChildSn, string sColorCode, string sBuildCode, string sSpcaCode, string sPpCode);

}