using System;
using System.Collections.Generic;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAreaService:IService<Area, AreaOutputDto, string>
    {

        #region ����uniapp����ѡ��
        /// <summary>
        /// ��ȡ���п��õĵ���������uniapp����ѡ��
        /// </summary>
        /// <returns></returns>
        List<AreaPickerOutputDto> GetAllByEnable();
        /// <summary>
        /// ��ȡʡ���С���/���������õĵ���������uniapp����ѡ��
        /// </summary>
        /// <returns></returns>
        List<AreaPickerOutputDto> GetProvinceToAreaByEnable();
        #endregion
    }
}