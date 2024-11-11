using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPublicService : IServiceReport<string>
    {
        Task<IEnumerable<DictData>> GetDictData();
        Task<IEnumerable<DictDataDetail>> GetDictDataDetail(string S_DictDataID, string S_EnCode);
        Task<IEnumerable<dynamic>> GetData(string S_TabName, string S_Where);

        Task<IEnumerable<dynamic>> MesGetLine(string ID);
        Task<IEnumerable<dynamic>> MesGetStation(string ID, string LineID, string StationTypeID);
        Task<IEnumerable<dynamic>> MesGetStationType(string ID);
        Task<string> LuGetApplicationType(string StationID);

        Task<IEnumerable<dynamic>> MesGetPartFamilyType(string ID);
        Task<IEnumerable<dynamic>> MesGetPartFamily(string ID, string PartFamilyTypeID);
        Task<IEnumerable<dynamic>> MesGetPart(string ID, string PartFamilyID);
        Task<List<dynamic>> MesGetProductionOrder(string S_PartID, string S_LineID);
        Task<IEnumerable<dynamic>> LuGetUnitStatus();

        Task<List<dynamic>> GetDefect(string S_PartFamilyTypeID);
        Task<List<mesStation>> MesGetStationAsync(string ID);

        List<DNShift> GetrptDashboardGetDNShift();
        void TestFn();

    }
}