using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class OverStationServices : BaseServiceReport<string>, IOverStationServices
    {
        private readonly IOverStationRepository _repository;
        private readonly ILogService _logService;

        public OverStationServices(IOverStationRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP) 
        {
            return await _repository.GetConfInfo(I_Language, I_LineID, I_StationID,I_EmployeeID, S_CurrentLoginIP);
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            return await _repository.GetPageInitialize(S_URL);
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO( string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            return await _repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus,S_URL);
        }
        public async Task<ConfirmPOOutputDto_TTBindBox> SetConfirmPO_TTBindBox(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            return await _repository.SetConfirmPO_TTBindBox(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);
        }

        public async Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL)
        {
            return await _repository.SetScanSN(S_SN, S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_DefectID,S_URL);
        }

        public async Task<string> GetScanTTCardID(string S_CardID, string S_IsCheckCardID, string S_CardIDPattern) 
        {
            return await _repository.GetScanTTCardID(S_CardID, S_IsCheckCardID, S_CardIDPattern);
        }

        public async Task<SetScanSNOutputDto> SetScanSNTT(string S_CardID, string S_IsCheckCardID,
            string S_CardIDPattern, string S_SN, string S_URL)
        {
            return await _repository.SetScanSNTT(S_CardID, S_IsCheckCardID, S_CardIDPattern,S_SN, S_URL);
        }

        public async Task<SetScanSN_TTRegisterOutputDto> SetScanSN_TTRegister(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL)
        {
            return await _repository.SetScanSN_TTRegister(S_SN, S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_UnitStatus, S_DefectID, S_URL);
        }


        public async Task<SetScanSN_TTBoxSNOutputDto> SetScanSN_TTBoxSN(string S_BoxSN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL, TTBindBox v_TTBindBox)
        {
            return await _repository.SetScanSN_TTBoxSN(S_BoxSN, S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, S_DefectID, S_URL,v_TTBindBox);
        }

        public async Task<SetScanSN_TTChildSNOutputDto> SetScanSN_TTChildSN(string S_BoxSN, string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL,
                    TTBindBox v_TTBindBox, Boolean B_IsEndBox)
        {
            return await _repository.SetScanSN_TTChildSN(S_BoxSN, S_SN, S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, S_DefectID, S_URL,v_TTBindBox,B_IsEndBox);
        }


    }
}
