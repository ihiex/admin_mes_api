using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class DbBackupRepository : BaseRepository<DbBackup, string>, IDbBackupRepository
    {
        public DbBackupRepository()
        {
        }

        public DbBackupRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }
    }
}