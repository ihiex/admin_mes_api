using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialInitialServices : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<ConfirmPOOutputMateialDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_URL);

        Task<CreateSNOutputDto> SetRegister
            (
            string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string URL,

            string Batch_Pattern, string MaterialBatchQTY, string MaterialLable, string M_UnitConversion_PCS,
            string MaterialAuto, string MaterialCodeRules, Boolean B_LotCode, Boolean B_TranceCode,

            string VendorID, string VendorCode,
            string LotCode, string RollCode, string Unit, string Quantity, string MaterialDate,
            string MPN, string DateCode, string ExpiringTime, string TranceCode, string Type, string Assigned
            );

        Task<CreateSNOutputDto> RePrint(string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string LotCode, string URL);

        TranceCodeDto TranceCode_KeyDown(string TranceCode, string MaterialCodeRules, int Expires_Time);
    }
}
