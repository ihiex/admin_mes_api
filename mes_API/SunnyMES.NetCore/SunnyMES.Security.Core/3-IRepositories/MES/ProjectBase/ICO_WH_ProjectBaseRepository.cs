using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.IRepositories.MES.ProjectBase
{
    public interface ICO_WH_ProjectBaseRepository : ICustomRepository<CO_WH_ProjectBase,string>
    {
        public Task<CO_WH_ProjectBase> GetProjectBaseEntityByProjectNO(string projectNo);
    }
}
