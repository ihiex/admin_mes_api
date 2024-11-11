using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IRepositories.Part
{
    public interface ISC_luPartFamilyRepositories : ICustomRepository<SC_luPartFamily,string>
    {
        Task<bool> CloneDataAsync(SC_luPartFamily mainDto, IEnumerable<SC_mesPartFamilyDetail> childDtos);
        Task<bool> CheckExistAsync(SC_luPartFamily mainDto);
        Task<bool> DeleteDataAsync(SC_luPartFamily inputDto, IEnumerable<SC_mesPartFamilyDetail> childs);
    }
}
