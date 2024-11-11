using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IPublicRepository : IRepositoryReport<string>
    {
        Task<List<DictData>> GetDictData();
        Task<List<DictDataDetail>> GetDictDataDetail(string S_DictDataID,string S_EnCode);
        Task<List<dynamic>> GetData(string S_TabName, string S_Where);

        Task<List<dynamic>> MesGetProductionOrder(string ID, string PartID);
        Task<List<dynamic>> MesGetLine(string ID);
        Task<List<dynamic>> MesGetStation(string ID,string LineID, string StationTypeID);
        Task<List<dynamic>> MesGetStationType(string ID);
        Task<string> LuGetApplicationType(string StationID);
        Task<IEnumerable<dynamic>> MesGetPart(string ID, string PartFamilyID);
        Task<List<dynamic>> GetDefect(string S_PartFamilyTypeID);

        Task<string> GetRouteCheck(int Scan_StationTypeID, int Scan_StationID, string LineID, mesUnit DT_Unit, string S_Str);

        Task<List<mesStation>> MesGetStationAsync(string ID);

        List<DNShift> GetrptDashboardGetDNShift();

        void TestFn();
    }
}

