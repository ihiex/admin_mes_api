using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;

namespace SunnyMES.Security.SysConfig.IServices.Machine
{
    public interface ISC_luMachineFamilyServices : ICustomService<SC_luMachineFamily, SC_luMachineFamily, string>
    {
        Task<PageResult<SC_luMachineFamily>> FindWithPagerSearchAsync(SearchMachineFamilyInputDto search);
    }
}
