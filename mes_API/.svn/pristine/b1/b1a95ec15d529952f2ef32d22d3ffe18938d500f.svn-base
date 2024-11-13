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
    public interface IMaterialLineCropping_SNServices : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<ConfirmPOOutputMateialDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_LineNumber,string Type, string S_URL);

        Task<CreateSNOutputDto> SetSplit
            (
                string PartFamilyTypeID, string PartFamilyID,
                string PartID, string POID, string LineNumber, string URL,

                string Type,  string SN_Pattern, string Batch_Pattern, string BatchNo, string RollNO, int SpecQTY,

                int ParentID, int BatchQTY, int UsageQTY,
                Boolean DOE_BuildNameEnabled, string DOE_BuildName, Boolean DOE_CCCCEnabled, int DOE_CCCC_Length, string CCCC
            );


        CroppingSNDto ParentBatchNo_ValueChanged(string S_PartID, string ParentID, string S_URL);
    }
}

