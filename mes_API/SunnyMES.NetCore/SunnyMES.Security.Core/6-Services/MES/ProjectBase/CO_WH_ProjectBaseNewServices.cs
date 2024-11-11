using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories.MES.ProjectBase;
using SunnyMES.Security.IServices.MES.ProjectBase;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.Services.MES.ProjectBase
{
    public class CO_WH_ProjectBaseNewServices : BaseCustomService<CO_WH_ProjectBaseNew, CO_WH_ProjectBaseNew, string>, ICO_WH_ProjectBaseNewServices
    {
        private readonly ICO_WH_ProjectBaseNewRepository iRepository;

        public CO_WH_ProjectBaseNewServices(ICO_WH_ProjectBaseNewRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<CO_WH_ProjectBaseNew> GetProjectBaseEntityByProjectNO(string projectNo)
        {
            return await iRepository.GetProjectBaseEntityByProjectNO(projectNo);
        }

        public override Task<bool> UpdateAsync(CO_WH_ProjectBaseNew entity, string id, IDbTransaction trans = null)
        {
            return iRepository.UpdateAsync(entity, id, trans);
        }

    }
}
