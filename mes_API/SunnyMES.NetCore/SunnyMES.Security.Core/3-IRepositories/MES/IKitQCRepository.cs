using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.KitQC;

namespace SunnyMES.Security.IRepositories.MES
{
    public interface IKitQCRepository : IMesBaseRepository
    {

        Task<MesMainOutputDtos> MainSnVerifyAsync(MesSnInputDto input);
    }
}
