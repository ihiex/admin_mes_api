using System;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFilterIPService:IService<FilterIP, FilterIPOutputDto, string>
    {
        /// <summary>
        /// 验证IP地址是否被拒绝
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
       bool ValidateIP(string ip);
    }
}
