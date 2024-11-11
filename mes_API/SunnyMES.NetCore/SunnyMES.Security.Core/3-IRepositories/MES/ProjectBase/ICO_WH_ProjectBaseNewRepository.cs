using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.IRepositories.MES.ProjectBase
{
    public interface ICO_WH_ProjectBaseNewRepository : ICustomRepository<CO_WH_ProjectBaseNew, string>
    {
        public Task<CO_WH_ProjectBaseNew> GetProjectBaseEntityByProjectNO(string projectNo);
    }
}
