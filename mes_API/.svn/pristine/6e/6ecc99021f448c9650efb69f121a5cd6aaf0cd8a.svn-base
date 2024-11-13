using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Reflection.Emit;
using System.Threading.Tasks;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services;

public class AssemblyTwoInputServices : BaseCommonService<string>, IAssemblyTwoInputServices
{
    private readonly IAssemblyTwoInputRepository _iRepository;

    public AssemblyTwoInputServices(IAssemblyTwoInputRepository iRepository) : base(iRepository)
    {
        _iRepository = iRepository;
    }

    public Task<List<TabVal>> GetConfInfoAsync(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
    {
        return _iRepository.GetConfInfoAsync(I_Language, I_LineID, I_StationID, I_EmployeeID, S_CurrentLoginIP);
    }

    public Task<IEnumerable<dynamic>> GetPageInitializeAsync(string S_URL)
    {
        return _iRepository.GetPageInitializeAsync(S_URL);
    }

    public Task<List<dynamic>> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus,
        string S_URL)
    {
        return _iRepository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);
    }


    public Task<List<dynamic>> GetColorCodeAsync()
    {
        return _iRepository.GetColorCodeAsync();
    }

    public Task<List<dynamic>> MainSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus,
        string S_URL, string MainSN)
    {
        return _iRepository.MainSnVerifyAsync(S_PartFamilyTypeID, S_PartFamilyID,
            S_PartID, S_POID, S_UnitStatus, S_URL, MainSN);
    }

    public Task<List<dynamic>> ChildSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL, string MainSN, string ChildSN, string sColorCode, string sBuildCode, string sSpcaCode, string sPpCode)
    {
        return _iRepository.ChildSnVerifyAsync(S_PartFamilyTypeID, S_PartFamilyID,
            S_PartID, S_POID, S_UnitStatus, S_URL, MainSN, ChildSN, sColorCode, sBuildCode, sSpcaCode, sPpCode);
    }
}