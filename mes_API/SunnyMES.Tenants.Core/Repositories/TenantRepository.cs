using Dapper;
using System;
using System.Threading.Tasks;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Tenants.Dtos;
using SunnyMES.Tenants.IRepositories;
using SunnyMES.Tenants.Models;

namespace SunnyMES.Tenants.Repositories
{
    /// <summary>
    /// 租户仓储接口的实现
    /// </summary>
    public class TenantRepository : BaseRepository<Tenant, string>, ITenantRepository
    {
		public TenantRepository()
        {
        }
        /// <summary>
        /// 注入EF上下文
        /// </summary>
        /// <param name="context"></param>
        public TenantRepository(IDbContextCore context) : base(context)
        {
        }

        /// <summary>
        /// 根据租户账号查询租户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<Tenant> GetByUserName(string userName)
        {
            string sql = $"SELECT * FROM {this.tableName} t WHERE t.TenantName = @TenantName";
            return await DapperConn.QueryFirstOrDefaultAsync<Tenant>(sql, new { @TenantName = userName });
        }



        /// <summary>
        /// 注册租户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tenantLogOnEntity"></param>
        public async Task<bool> InsertAsync(Tenant entity, TenantLogon tenantLogOnEntity)
        {
            tenantLogOnEntity.Id = GuidUtils.CreateNo();
            tenantLogOnEntity.TenantId = entity.Id;
            tenantLogOnEntity.TenantSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
            tenantLogOnEntity.TenantPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(tenantLogOnEntity.TenantPassword).ToLower(), tenantLogOnEntity.TenantSecretkey).ToLower()).ToLower();
            DbContext.GetDbSet<Tenant>().Add(entity);
            DbContext.GetDbSet<TenantLogon>().Add(tenantLogOnEntity);
            return await DbContext.SaveChangesAsync() > 0;
        }

    }
}