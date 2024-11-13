using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface IPrintService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<List<mesLineGroup>> mesLineGroup(string LineType, int PartFamilyTypeID);

        Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_LineNumber, string S_UnitStatus, string Type,string S_URL);

        Task<CreateSNOutputDto> CreateSN(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, int I_QTY, string S_LineNumber, ConfirmPOOutputDto v_ConfirmPOOutputDto,
                     string ET, string SPAC,
                     string B, string PP, string PredictQTY,
                    string Type);
    }
}
