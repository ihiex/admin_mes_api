using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IRepositories.PO
{
    public interface ISC_mesProductionOrderDetailRepositories : ICustomRepository<SC_mesProductionOrderDetail, string>
    {
    }
}
