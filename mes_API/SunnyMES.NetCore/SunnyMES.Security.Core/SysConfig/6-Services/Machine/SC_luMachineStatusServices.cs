using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.IRepositories.Machine;
using SunnyMES.Security.SysConfig.IServices.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;

namespace SunnyMES.Security.SysConfig.Services.Machine
{
    public class SC_luMachineStatusServices : BaseCustomService<SC_luMachineStatus, SC_luMachineStatus, string>, ISC_luMachineStatusServices
    {
        public SC_luMachineStatusServices(ISC_luMachineStatusRepositories repositories) : base(repositories)
        {
            
        }
    }
}
