using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IServices.Part
{
    public interface ISC_mesPartServices : ICustomService<SC_mesPart, SC_mesPart, string>
    {
        Task<PageResult<SC_mesPart>> FindWithPagerSearchAsync(SearchPartInputDto search);
        Task<CommonResult> CloneDataAsync(SC_mesPart inputDto);
        Task<CommonResult> DeleteDataAsync(SC_mesPart inputDto);
    }
}
