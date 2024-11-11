using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IRepositories.MES.SAP
{
    public interface ITmpExcelShipmentNewRepository : ICustomRepository<tmpExcelShipmentNew,string>
    {
        Task<IEnumerable<tmpExcelShipmentNew>> GetListSapByHAWBAsync(string HAWB);


    }
}
