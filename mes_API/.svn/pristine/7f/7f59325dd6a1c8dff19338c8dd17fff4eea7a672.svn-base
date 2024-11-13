using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.Models.Machine;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IServices.Machine
{
    public interface ISC_mesMachineServices : ICustomService<SC_mesMachine, SC_mesMachine, string>
    {
        Task<PageResult<SC_mesMachine>> FindWithPagerSearchAsync(SearchMachineInputDto search);
    }
}
