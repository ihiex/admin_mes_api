using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig._2_Dtos.Shift;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.Shift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.Shift
{
    public interface ISC_mesShiftServices : ICustomService<SC_mesShift, SC_mesShift,int>
    {
        public Task<CommonResult> CloneDataAsync(SC_mesShift shift);
        public Task<CommonResult> DeleteDataAsync(SC_mesShift inputDto);
        public Task<PageResult<SC_mesShift>> FindWithPagerFilterAsync(MesShiftInputDto search);
    }
}
