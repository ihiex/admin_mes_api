using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class MaterialLineCropping_SNServices : BaseServiceReport<string>, IMaterialLineCropping_SNServices
    {
        private readonly IMaterialLineCropping_SNRepository _repository;
        private readonly ILogService _logService;

        public MaterialLineCropping_SNServices(IMaterialLineCropping_SNRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            return await _repository.GetConfInfo(I_Language, I_LineID, I_StationID, I_EmployeeID, S_CurrentLoginIP);
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            return await _repository.GetPageInitialize(S_URL);
        }

        public async Task<ConfirmPOOutputMateialDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_LineNumber,string Type, string S_URL)
        {
            return await _repository.SetConfirmPO
                (
                     S_PartFamilyTypeID, S_PartFamilyID,
                                 S_PartID, S_POID, S_LineNumber,Type, S_URL
                );
        }

        public async Task<CreateSNOutputDto> SetSplit
            (
                string PartFamilyTypeID, string PartFamilyID,
                string PartID, string POID, string LineNumber, string URL,

                string Type, string SN_Pattern, string Batch_Pattern, string BatchNo, string RollNO, int SpecQTY,

                int ParentID, int BatchQTY, int UsageQTY,
                Boolean DOE_BuildNameEnabled, string DOE_BuildName, Boolean DOE_CCCCEnabled, int DOE_CCCC_Length, string CCCC
            )
        {
            return await _repository.SetSplit
                (
                 PartFamilyTypeID,  PartFamilyID,
                 PartID,  POID,  LineNumber,  URL,

                 Type,SN_Pattern,  Batch_Pattern, BatchNo,  RollNO,  SpecQTY,

                 ParentID,  BatchQTY,  UsageQTY,
                 DOE_BuildNameEnabled, DOE_BuildName, DOE_CCCCEnabled, DOE_CCCC_Length, CCCC
                );
        }


        public CroppingSNDto ParentBatchNo_ValueChanged(string S_PartID, string ParentID, string S_URL) 
        {
            return _repository.ParentBatchNo_ValueChanged(S_PartID, ParentID, S_URL);
        }

    }
}
