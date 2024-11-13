using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IFilterIPRepository:IRepository<FilterIP, string>
    {
        /// <summary>
        /// 验证IP地址是否被拒绝
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool ValidateIP(string ip);
    }
}