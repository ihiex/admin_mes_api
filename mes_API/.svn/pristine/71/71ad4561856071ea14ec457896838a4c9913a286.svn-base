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
    public class MaterialInitialServices : BaseServiceReport<string>, IMaterialInitialServices
    {
        private readonly IMaterialInitialRepository _repository;
        private readonly ILogService _logService;

        public MaterialInitialServices(IMaterialInitialRepository repository, ILogService logService) : base(repository)
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
            string S_PartID, string S_POID, string S_URL)
        {
            return await _repository.SetConfirmPO
                (
                     S_PartFamilyTypeID,  S_PartFamilyID,
                                 S_PartID,  S_POID,  S_URL
                );
        }

        public async Task<CreateSNOutputDto> SetRegister
            (
            string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string URL,

            string Batch_Pattern, string MaterialBatchQTY, string MaterialLable, string M_UnitConversion_PCS,
            string MaterialAuto, string MaterialCodeRules, Boolean B_LotCode, Boolean B_TranceCode,

            string VendorID, string VendorCode,
            string LotCode, string RollCode, string Unit, string Quantity, string MaterialDate,
            string MPN, string DateCode, string ExpiringTime, string TranceCode, string Type, string Assigned
            )
        {
            return await _repository.SetRegister
                (
                     PartFamilyTypeID,  PartFamilyID,
                     PartID,  POID,  URL,

                     Batch_Pattern,  MaterialBatchQTY,  MaterialLable,  M_UnitConversion_PCS,
                     MaterialAuto,  MaterialCodeRules,  B_LotCode,  B_TranceCode,

                     VendorID,  VendorCode,
                     LotCode,  RollCode,  Unit,  Quantity,  MaterialDate,
                     MPN,  DateCode,  ExpiringTime,  TranceCode,  Type,  Assigned
                );
        }


        public async Task<CreateSNOutputDto> RePrint(string PartFamilyTypeID, string PartFamilyID,
            string PartID, string POID, string LotCode, string URL)
        {
            return await _repository.RePrint
                (
                     PartFamilyTypeID,  PartFamilyID,
                                 PartID,  POID,  LotCode,  URL
                );
        }

        public TranceCodeDto TranceCode_KeyDown(string TranceCode, string MaterialCodeRules, int Expires_Time) 
        {
            return  _repository.TranceCode_KeyDown
                (
                    TranceCode,  MaterialCodeRules, Expires_Time
                );
        }

    }
}