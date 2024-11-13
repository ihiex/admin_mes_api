using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IOverStationRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID,string S_PartFamilyID,
            string S_PartID,string S_POID,string S_UnitStatus, string S_URL);
        Task<ConfirmPOOutputDto_TTBindBox> SetConfirmPO_TTBindBox(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_URL);


        Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL);


        Task<string> GetScanTTCardID(string S_CardID, string S_IsCheckCardID, string S_CardIDPattern);

        Task<SetScanSNOutputDto> SetScanSNTT(string S_CardID, string S_IsCheckCardID,
            string S_CardIDPattern, string S_SN, string S_URL);
        Task<SetScanSN_TTRegisterOutputDto> SetScanSN_TTRegister(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL);


        Task<SetScanSN_TTBoxSNOutputDto> SetScanSN_TTBoxSN(string S_BoxSN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL, TTBindBox v_TTBindBox);
        Task<SetScanSN_TTChildSNOutputDto> SetScanSN_TTChildSN(string S_BoxSN, string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL, 
                    TTBindBox v_TTBindBox, Boolean B_IsEndBox);

    }
}


