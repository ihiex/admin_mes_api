using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IServices.Part
{
    public interface ISC_luVendoServices : ICustomService<SC_luVendo, SC_luVendo,string>
    {
        Task<PageResult<SC_luVendo>> FindWithPagerSearchAsync(SearchVendoInputDto search);
    }
}
