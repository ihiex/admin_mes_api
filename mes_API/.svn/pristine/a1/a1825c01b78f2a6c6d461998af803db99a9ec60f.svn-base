
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.Shift;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig.IRepositories.Shift
{
    public interface ISC_mesShiftRepositories : ICustomRepository<SC_mesShift, int>
    {
        Task<bool> CloneDataAsync(SC_mesShift mainDto, IEnumerable<SC_mesShiftDetail> childDtos);
        Task<bool> CheckExistAsync(SC_mesShift mainDto);
        Task<bool> DeleteDataAsync(SC_mesShift inputDto, IEnumerable<SC_mesShiftDetail> childs);
    }
}
