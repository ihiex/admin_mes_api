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

        #region 用于uniapp下拉选项
        /// <summary>
        /// 获取所有可用的地区，用于uniapp下拉选项
        /// </summary>
        /// <returns></returns>
        List<AreaPickerOutputDto> GetAllByEnable();
        /// <summary>
        /// 获取省、市、县/区三级可用的地区，用于uniapp下拉选项
        /// </summary>
        /// <returns></returns>
        List<AreaPickerOutputDto> GetProvinceToAreaByEnable();
        #endregion
    }
}
