using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories.MES.ProjectBase;
using SunnyMES.Security.IServices.MES.ProjectBase;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.Services.MES.ProjectBase
{
    public class CO_WH_ProjectBaseServices : BaseCustomService<CO_WH_ProjectBase, CO_WH_ProjectBase, string>, ICO_WH_ProjectBaseServices
    {
        private readonly ICO_WH_ProjectBaseRepository iRepository;

        public CO_WH_ProjectBaseServices(ICO_WH_ProjectBaseRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }
        public override Task<bool> UpdateAsync(CO_WH_ProjectBase entity, string id, IDbTransaction trans = null)
        {

            return iRepository.UpdateAsync(entity, id, trans);
        }
    }
}
