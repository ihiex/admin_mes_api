using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IServices.MES.SAP
{
    public interface ICO_ShipManifestData_SNServices : ICustomService<CO_ShipManifestData_SN,CO_ShipManifestData_SN,string>
    {

        Task<PageResult<CO_ShipManifestData_SN>> FindWithPagerLikeAsync(SearchSAPManifestDataModel search);
        Task<IEnumerable<TabVal>>  FindExportCSVAsync(SearchSAPManifestDataModel search);
    }
}
