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
        /// ��֤IP��ַ�Ƿ񱻾ܾ�
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
       bool ValidateIP(string ip);
    }
}
