using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IServices.Part
{
    public interface ISC_mesProductStructureServices : ICustomService<SC_mesProductStructure, SC_mesProductStructure, string>
    {
        Task<PageResult<SC_mesProductStructure>> FindWithPagerSearchAsync(SearchBOMInputDto search);
    }
}
