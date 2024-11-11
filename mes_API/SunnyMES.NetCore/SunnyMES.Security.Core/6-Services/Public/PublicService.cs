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
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class PublicService : BaseServiceReport<string>, IPublicService
    {
        private readonly IPublicRepository _repository;
        private readonly ILogService _logService;

        public PublicService(IPublicRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public  async Task<IEnumerable<DictData>> GetDictData()
        {
            return await _repository.GetDictData();
        }

        public async Task<IEnumerable<DictDataDetail>> GetDictDataDetail(string S_DictDataID, string S_EnCode)
        {
            return await _repository.GetDictDataDetail(S_DictDataID,S_EnCode);
        }

        public async Task<IEnumerable<dynamic>> GetData(string S_TabName, string S_Where)
        {
            return await _repository.GetData(S_TabName, S_Where);
        }


        public async Task<IEnumerable<dynamic>> MesGetLine(string ID)
        {
            return await _repository.MesGetLine(ID);
        }

        public async Task<IEnumerable<dynamic>> MesGetStation(string ID, string LineID, string StationTypeID)
        {
            return await _repository.MesGetStation(ID, LineID, StationTypeID);
        }

        public async Task<IEnumerable<dynamic>> MesGetStationType(string ID)
        {
            return await _repository.MesGetStationType(ID);
        }

        public async Task<string> LuGetApplicationType(string StationID)
        {
            return await  _repository.LuGetApplicationType(StationID);
        }



        public async Task<IEnumerable<dynamic>> MesGetPartFamilyType(string ID)
        {
            ID = ID ?? "";
            string S_Where = " Status=1";
            if (ID != "") 
            {
                S_Where = " and ID='"+ID+ "' ORDER BY Name";
            }

            return await _repository.GetData("luPartFamilyType", S_Where);
        }

        public async Task<IEnumerable<dynamic>> MesGetPartFamily(string ID, string PartFamilyTypeID)
        {
            ID = ID ?? "";
            PartFamilyTypeID = PartFamilyTypeID ?? "";
            string S_Where = " Status=1";

            if (ID != "")
            {
                S_Where += " and ID='" + ID + "'";
            }
            if (PartFamilyTypeID != "")
            {
                S_Where += " and PartFamilyTypeID='" + PartFamilyTypeID + "'";
            }

            S_Where += " ORDER BY Name";
            return await _repository.GetData("luPartFamily", S_Where);
        }

        public async Task<IEnumerable<dynamic>> MesGetPart(string ID, string PartFamilyID)
        {
            return await _repository.MesGetPart(ID, PartFamilyID);            
        }

        public async Task<List<dynamic>> MesGetProductionOrder(string S_PartID, string S_LineID)
        {
            return await _repository.MesGetProductionOrder(S_PartID, S_LineID);
        }

        public async Task<IEnumerable<dynamic>> LuGetUnitStatus() 
        {
            return await _repository.GetData("luUnitStatus", "");  
        }

        public async Task<List<dynamic>> GetDefect(string S_PartFamilyTypeID) 
        {
            return await _repository.GetDefect(S_PartFamilyTypeID);
        }

        public async Task<List<mesStation>> MesGetStationAsync(string ID)
        {
            return await _repository.MesGetStationAsync(ID);
        }

        public  List<DNShift> GetrptDashboardGetDNShift() 
        {
            return  _repository.GetrptDashboardGetDNShift();
        }

        public void TestFn()
        {
              _repository.TestFn();
        }
    }
}
