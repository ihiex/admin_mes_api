using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos.MES.SAP;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IServices.MES.SAP
{
    public interface ITmpExcelShipmentNewServices : ICustomService<tmpExcelShipmentNew,tmpExcelShipmentNew,string>
    {
         Task<PageResult<tmpExcelShipmentNew>> GetListSapByHAWBAsync(SearchSAPDataModel search);

        Task<PageResult<tmpExcelShipmentNew>> GetListSapByShippingTimeAsync(SearchSAPDataModel search);

        Task<PageResult<tmpExcelShipmentNew>> FindWithPagerLikeAsync(SearchSAPDataModel search);
    }
}
