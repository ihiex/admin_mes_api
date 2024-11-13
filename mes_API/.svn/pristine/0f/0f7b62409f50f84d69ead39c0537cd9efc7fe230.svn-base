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
    public interface ISC_mesRouteMachineMapServices : ICustomService<SC_mesRouteMachineMap, SC_mesRouteMachineMap, string>
    {
        Task<PageResult<SC_mesRouteMachineMap>> FindWithPagerSearchAsync(SearchMachineMapInputDto search);

    }
}
