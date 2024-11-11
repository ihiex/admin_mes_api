using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;



namespace SunnyMES.Security.Repositories
{
    /// <summary>
    /// 仓储接口的实现
    /// </summary>
    public class BZhangRepository : BaseRepositoryGeneric<BZhang, string>, IBZhangRepository
    {
        public BZhangRepository()
        {
        }
        public BZhangRepository(IDbContextCoreGeneric context) : base(context)
        {
        }
        /// <summary>
        /// 获取A_Zhang对象
        /// </summary>
        /// <param name="BZhangid">应用ID</param>
        /// <param name="secret">应用密钥A_ZhangSecret</param>
        /// <returns></returns>
        public BZhang GetBZhang(string BZhangid, string secret)
        {
            string sql = @"SELECT * FROM API_ZYM t WHERE t.Id = @Id and Data1=@Secret and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<BZhang>(sql, new { Id = BZhangid, AAA = secret });
        }

        /// <summary>
        /// 获取A_Zhang对象
        /// </summary>
        /// <param name="BZhangid">应用ID</param>
        /// <returns></returns>
        public BZhang GetBZhang(string BZhangid)
        {
            string sql = @"SELECT * FROM API_ZYM t WHERE t.Id = @A_ZhangId and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<BZhang>(sql, new { Id = BZhangid });

        }
        public IList<BZhangOutputDto> SelectBZhang()
        {
            const string query = @"select * from API_ZYM ";
            return DapperConnRead.Query<BZhangOutputDto, BZhang, BZhangOutputDto>(query, (A_Zhang, info) =>
            { return A_Zhang; }, null, splitOn: "Id").ToList<BZhangOutputDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_BZhang"></param>
        /// <returns></returns>
        public int InsertBZhang(BZhang v_BZhang, IDbTransaction trans = null)
        {
            string S_Sql = "insert into API_ZYM(Data1,Data2,Description,CreatorTime,CreatorUserId) Values('" +
                 v_BZhang.Data1 + "','" +
                 v_BZhang.Data2 + "','" +
                 v_BZhang.Description+"','"+
                 v_BZhang.CreatorTime+"','"+ 
                 v_BZhang.CreatorUserId+"'"+
                ")";
            return DapperConn.Execute(S_Sql, null, trans);

        }


        public int UpdateBZhang2(BZhang v_BZhang,string S_Id, IDbTransaction trans = null)
        {
            string S_Sql = "Update API_ZYM set Data1='"+ v_BZhang.Data1 + "', Data2='"+ v_BZhang.Data2 + "',"+"\r\n"+
                           "  Description='"+ v_BZhang.Description + "' where Id='"+ S_Id + "'";
            return DapperConn.Execute(S_Sql, null, trans);

        }


    }
}