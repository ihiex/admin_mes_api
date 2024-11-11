using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.IRepositories.PO;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Services.PO
{
    public class SC_mesLineOrderServices : BaseCustomService<SC_mesLineOrder, SC_mesLineOrder, string>, ISC_mesLineOrderServices
    {
        public SC_mesLineOrderServices(ISC_mesLineOrderRepositories repositories):base(repositories)
        {
            
        }
    }
}
