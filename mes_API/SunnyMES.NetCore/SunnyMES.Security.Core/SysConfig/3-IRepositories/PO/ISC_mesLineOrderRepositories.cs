using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IRepositories.PO
{
    public interface ISC_mesLineOrderRepositories : ICustomRepository<SC_mesLineOrder, string>
    {
    }
}
