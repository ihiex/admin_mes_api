using System;
using System.Collections.Generic;
using System.Data;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;


namespace SunnyMES.Security.IRepositories
{
    /// <summary>
    /// 定义仓储接口
    /// </summary>
    public interface IBZhangRepository : IRepositoryGeneric<BZhang, string>
    {
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        BZhang GetBZhang(string appid, string secret);

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        BZhang GetBZhang(string appid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<BZhangOutputDto> SelectBZhang();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_BZhang"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
         int InsertBZhang(BZhang v_BZhang, IDbTransaction trans = null);


         int UpdateBZhang2(BZhang v_BZhang, string S_Id, IDbTransaction trans = null);

    }
}