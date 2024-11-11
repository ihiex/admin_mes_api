using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IServices.MES.SAP
{
    public interface ICO_WH_ShipmentNewServices : ICustomService<CO_WH_ShipmentNew_T, CO_WH_ShipmentNew_T, string>
    {
        Task<PageResult<CO_WH_ShipmentNew_T>> GetListSapByHAWBAsync(SearchShipmentNewDataModel search);

        Task<PageResult<CO_WH_ShipmentNew_T>> GetListSapByShippingTimeAsync(SearchShipmentNewDataModel search);
    }
}
