using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Helpers;
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
    public class AZhangRepository : BaseRepository<AZhang, string>, IAZhangRepository
    {
        public AZhangRepository()
        {
        }
        public AZhangRepository(IDbContextCore context) : base(context)
        {
        }
        /// <summary>
        /// 获取A_Zhang对象
        /// </summary>
        /// <param name="AZhangid">应用ID</param>
        /// <param name="secret">应用密钥A_ZhangSecret</param>
        /// <returns></returns>
        public AZhang GetAZhang(string AZhangid, string secret)
        {
            string sql = @"SELECT * FROM API_AZhang t WHERE t.Id = @Id and Data1=@Secret and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<AZhang>(sql, new { Id = AZhangid, AAA = secret });
        }

        /// <summary>
        /// 获取A_Zhang对象
        /// </summary>
        /// <param name="AZhangid">应用ID</param>
        /// <returns></returns>
        public AZhang GetAZhang(string AZhangid)
        {
            string sql = @"SELECT * FROM API_AZhang t WHERE t.Id = @A_ZhangId and EnabledMark=1";
            return DapperConnRead.QueryFirstOrDefault<AZhang>(sql, new { Id = AZhangid });

        }
        public IList<AZhangOutputDto> SelectAZhang()
        {
            //const string query = @"select * from API_AZhang ";
            //return DapperConnRead.Query<AZhangOutputDto, AZhang, AZhangOutputDto>(query, (A_Zhang, info) =>
            //{ return A_Zhang; }, null, splitOn: "Id").ToList<AZhangOutputDto>();


            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_AZhang"></param>
        /// <returns></returns>
        public int InsertAZhang(AZhang v_AZhang, IDbTransaction trans = null)
        {
            //string S_Sql = "insert into API_AZhang(AppId,Data1,Data2,Token) Values('" +
            //    v_AZhang.AppId + "','" + v_AZhang.Data1 + "','" + v_AZhang.Data2 + "','" + v_AZhang.Token +
            //    "')";
            //return DapperConn.Execute(S_Sql, null, trans);



            string S_Sql = "SELECT MAX(Id)+1 AS LastID FROM API_AZhang";
            MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);

            v_AZhang.Id = v_MaxID.LastID.ToString();

            Zhang v_Zhang=new Zhang ();
            v_Zhang.Id=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            v_Zhang.AAA="AAA"+ DateTime.Now.ToString("FFFF");
            v_Zhang.BBB = "BBB" + DateTime.Now.ToString("FFFF");


            DbContext.GetDbSet<AZhang>().Add(v_AZhang);
            DbContext.GetDbSet<Zhang>().Add(v_Zhang);

            return DbContext.SaveChanges();
        }


    }
}