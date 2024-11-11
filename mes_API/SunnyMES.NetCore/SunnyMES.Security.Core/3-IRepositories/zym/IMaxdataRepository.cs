using System;
using System.Collections.Generic;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IMaxdataRepository : IRepository<Maxdata, string>
    {
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        Maxdata GetMaxdata(string appid, string secret);

        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        Maxdata GetMaxdata(string appid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<MaxdataOutputDto> SelectMaxdata();
    }
}

