using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IServices.Part
{
    public interface ISC_luPartFamilyTypeServices : ICustomService<SC_luPartFamilyType, SC_luPartFamilyType, string>
    {

        public Task<PageResult<SC_luPartFamilyType>> FindWithPagerSearchAsync(SearchPartFamilyTypeInputDto searchInputDto);
        Task<CommonResult> CloneDataAsync(SC_luPartFamilyType inputDto);
        Task<CommonResult> DeleteDataAsync(SC_luPartFamilyType inputDto);
    }
}
