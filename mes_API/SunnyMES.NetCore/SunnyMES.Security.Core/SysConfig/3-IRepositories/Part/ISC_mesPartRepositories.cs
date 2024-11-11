using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IRepositories.Part
{
    public interface ISC_mesPartRepositories : ICustomRepository<SC_mesPart,string>
    {
        Task<bool> CloneDataAsync(SC_mesPart mainDto, IEnumerable<SC_mesPartDetail> childDtos);
        Task<bool> CheckExistAsync(SC_mesPart mainDto);
        Task<bool> DeleteDataAsync(SC_mesPart inputDto, IEnumerable<SC_mesPartDetail> childs);
    }
}
