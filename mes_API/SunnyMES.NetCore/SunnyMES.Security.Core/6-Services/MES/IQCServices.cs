﻿using System;
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
    public class IQCServices : BaseServiceReport<string>, IIQCServices
    {
        private readonly IIQCRepository _repository;
        private readonly ILogService _logService;

        public IQCServices(IIQCRepository repository, ILogService logService) : base(repository)
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

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            return await _repository.SetConfirmPO( S_PartFamilyTypeID,  S_PartFamilyID,
             S_PartID,  S_POID,  S_UnitStatus,  S_URL);
        }


        public async Task<SetScanSNOutputDto> SetScanSN(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_InnerSN_Pattern, string S_URL)
        {
            return await _repository.SetScanSN(S_SN, S_PartFamilyTypeID, S_PartFamilyID,
             S_PartID, S_POID, S_UnitStatus, S_DefectID, S_InnerSN_Pattern, S_URL);
        }

        public async Task<SetScanSNOutputDto> SetScanChildSN(string S_SN, string S_ChildSN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_InnerSN_Pattern, string S_URL)
        {
            return await _repository.SetScanChildSN( S_SN,  S_ChildSN,  S_PartFamilyTypeID,  S_PartFamilyID,
             S_PartID,  S_POID,  S_UnitStatus,  S_DefectID,  S_InnerSN_Pattern,  S_URL);
        }


    }
}