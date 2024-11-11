using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.IRepositories.Part
{
    public interface ISC_luPartFamilyTypeRepositories : ICustomRepository<SC_luPartFamilyType,string>
    {
        Task<bool> CloneDataAsync(SC_luPartFamilyType mainDto, IEnumerable<SC_mesPartFamilyTypeDetail> childDtos);
        Task<bool> CheckExistAsync(SC_luPartFamilyType mainDto);
        Task<bool> DeleteDataAsync(SC_luPartFamilyType inputDto, IEnumerable<SC_mesPartFamilyTypeDetail> childs);
    }
}
