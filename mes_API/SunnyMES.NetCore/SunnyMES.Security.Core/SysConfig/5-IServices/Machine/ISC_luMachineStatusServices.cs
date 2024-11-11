using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.SysConfig.Models.Machine;

namespace SunnyMES.Security.SysConfig.IServices.Machine
{
    public interface ISC_luMachineStatusServices : ICustomService<SC_luMachineStatus, SC_luMachineStatus, string>
    {
    }
}
