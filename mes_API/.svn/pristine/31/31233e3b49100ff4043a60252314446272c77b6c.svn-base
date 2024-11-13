using System;
using System.Collections.Generic;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;


namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 定义服务接口
    /// </summary>
    public interface IMaxdataService : IService<Maxdata, MaxdataOutputDto, string>
    {
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        Maxdata GetData(string appid, string secret);

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        Maxdata GetData(string appid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<MaxdataOutputDto> SelectMaxdata();
        /// <summary>
        /// 更新可用的应用到缓存
        /// </summary>
        void UpdateCacheAllowApp();
    }
}
