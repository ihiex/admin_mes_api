using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IRepositories.MES.SAP
{
    public interface ICO_ShipManifestData_SNRepository : ICustomRepository<CO_ShipManifestData_SN,string>
    {
        public Task<List<CO_ShipManifestData_SN>> FindWithPagerLikeAsync(string condition, PagerInfo info, string fieldToSort, bool desc, IDbTransaction trans = null);
        Task<IEnumerable<TabVal>> FindExportCSVAsync(SearchSAPManifestDataModel search);

    }
}
