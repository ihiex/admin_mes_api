using Dapper;
using System.Collections.Generic;
using System.Linq;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class MaxdataRepository : BaseRepository<Maxdata, string>, IMaxdataRepository
    {
        public MaxdataRepository()
        {
        }

        public MaxdataRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        public Maxdata GetMaxdata(string Maxdataid, string secret)
        {
            string sql = @"SELECT * FROM Maxdata t WHERE t.Id = @Id and Data1=@Secret and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<Maxdata>(sql, new { Id = Maxdataid, AAA = secret });
        }

        /// <summary>
        /// 获取A_Zhang对象
        /// </summary>
        /// <param name="Maxdataid">应用ID</param>
        /// <returns></returns>
        public Maxdata GetMaxdata(string Maxdataid)
        {
            string sql = @"SELECT * FROM Maxdata t WHERE t.Id = @Id and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<Maxdata>(sql, new { Id = Maxdataid });

        }
        public IList<MaxdataOutputDto> SelectMaxdata()
        {
            const string query = @"select * from Maxdata ";
            return DapperConnRead.Query<MaxdataOutputDto, Maxdata, MaxdataOutputDto>(query, (Maxdata, info) =>
            { return Maxdata; }, null, splitOn: "Id").ToList<MaxdataOutputDto>();
        }

    }
}