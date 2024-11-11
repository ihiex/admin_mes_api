using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IFilterIPRepository:IRepository<FilterIP, string>
    {
        /// <summary>
        /// ��֤IP��ַ�Ƿ񱻾ܾ�
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool ValidateIP(string ip);
    }
}