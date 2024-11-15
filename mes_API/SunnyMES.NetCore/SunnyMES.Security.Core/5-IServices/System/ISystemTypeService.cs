﻿using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 子系统服务接口
    /// </summary>
    public interface ISystemTypeService : IService<SystemType, SystemTypeOutputDto, string>
    {

        /// <summary>
        /// 根据系统编码查询系统对象
        /// </summary>
        /// <param name="appkey">系统编码</param>
        /// <returns></returns>
        SystemType GetByCode(string appkey);

        /// <summary>
        /// 根据角色获取可以访问子系统
        /// </summary>
        /// <param name="roleIds">角色Id，用','隔开</param>
        /// <returns></returns>
        List<SystemTypeOutputDto> GetSubSystemList(string roleIds);
    }
}
