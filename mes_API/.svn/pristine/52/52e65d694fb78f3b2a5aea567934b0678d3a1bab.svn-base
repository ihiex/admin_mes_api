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
    public interface IAZhangRepository:IRepository<AZhang, string>
    {
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        AZhang GetAZhang(string appid, string secret);

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        AZhang GetAZhang(string appid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<AZhangOutputDto> SelectAZhang();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v_AZhang"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        int InsertAZhang(AZhang v_AZhang, IDbTransaction trans = null);


    }
}