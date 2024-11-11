using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class ItemsRepository : BaseRepository<Items, string>, IItemsRepository
    {
        public ItemsRepository()
        {
        }

        public ItemsRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// ���ݱ����ѯ�ֵ����
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public async Task<Items> GetByEnCodAsynce(string enCode)
        {
            return await DbContext.GetSingleOrDefaultAsync<Items>(u => u.EnCode == enCode);
        }


        /// <summary>
        /// ����ʱ�жϷ�������Ƿ���ڣ��ų��Լ���
        /// </summary>
        /// <param name="enCode">�������</param
        /// <param name="id">����Id</param>
        /// <returns></returns>
        public async Task<Items> GetByEnCodAsynce(string enCode,string id)
        {
            return await DbContext.GetSingleOrDefaultAsync<Items>(u => u.EnCode == enCode&&u.Id!=id);
        }
    }
}