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
    public class PrintService : BaseServiceReport<string>, IPrintService
    {
        private readonly IPrintRepository _repository;
        private readonly ILogService _logService;

        public PrintService(IPrintRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP) 
        {
            return await _repository.GetConfInfo(I_Language,I_LineID,I_StationID,I_EmployeeID,S_CurrentLoginIP);
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            return await _repository.GetPageInitialize(S_URL);
        }

        public async Task<List<mesLineGroup>> mesLineGroup(string LineType, int PartFamilyTypeID) 
        {
            return await _repository.mesLineGroup(LineType, PartFamilyTypeID);
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, string S_LineNumber, string S_UnitStatus, string Type, string S_URL)
        {
            return await _repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_LineNumber, S_UnitStatus, Type, S_URL);
        }

        public async Task<CreateSNOutputDto> CreateSN(string S_PartFamilyTypeID, string S_PartFamilyID,
                    string S_PartID, string S_POID, int I_QTY, string S_LineNumber, ConfirmPOOutputDto v_ConfirmPOOutputDto,
                     string ET, string SPAC,
                     string B, string PP, string PredictQTY,
                    string Type)
        {
            return await _repository.CreateSN(S_PartFamilyTypeID,S_PartFamilyID,S_PartID,S_POID,I_QTY,S_LineNumber,v_ConfirmPOOutputDto,
                  ET,  SPAC,
                   B,  PP,  PredictQTY,
                Type);
        }
    }
}
