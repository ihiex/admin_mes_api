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
    public class ReworkServices : BaseServiceReport<string>, IReworkServices
    {
        private readonly IReworkRepository _repository;
        private readonly ILogService _logService;

        public ReworkServices(IReworkRepository repository, ILogService logService) : base(repository)
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

        //public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
        //    string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        //{
        //    return await _repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);
        //}

        public async Task<GetReworkDto> SetScanSN_GetRework(string S_SN, string S_URL, string S_Type)

        {
            return await _repository.SetScanSN_GetRework(S_SN, S_URL,S_Type);
        }

        public async Task<string> SetRework(string S_SN, string S_Type)
        {
            return await _repository.SetRework(S_SN, S_Type);
        }

        public async Task<ReworkPrintDto> ReworkPrint(string S_SN, string S_ListSN, string S_Type, string S_URL)
        {
            return await _repository.ReworkPrint(S_SN,S_ListSN, S_Type, S_URL);
        }


    }
}