﻿using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IServices.Part
{
    public interface ISC_luPartFamilyServices : ICustomService<SC_luPartFamily,SC_luPartFamily, string>
    {
        Task<PageResult<SC_luPartFamily>> FindWithPagerSearchAsync(SearchPartFamilyInputDto search);
        Task<CommonResult> CloneDataAsync(SC_luPartFamily inputDto);
        Task<CommonResult> DeleteDataAsync(SC_luPartFamily inputDto);
    }
}