using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IRepositories.MES.SAP
{
    public interface ICO_WH_ShipmentEntryNewRepository : ICustomRepository<CO_WH_ShipmentEntryNew_T,string>
    {
        public Task<int> GetMaxFInterID();
        public Task<int> GetMaxFEntryID(string FInterID);
    }
}
