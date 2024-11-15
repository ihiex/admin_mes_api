﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Helpers;
using SunnyMES.Security.Models;

namespace SunnyMES.AspNetCore.Common
{
    /// <summary>
    /// 系统初始化内容，根据需要继承实现
    /// </summary>
    public abstract class YuebonInitialization
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual  void Initial()
        {
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            SysSetting sysSetting = XmlConverter.Deserialize<SysSetting>("xmlconfig/sys.config");
            if (sysSetting != null)
            {
                yuebonCacheHelper.Add("SysSetting", sysSetting);
            }

       }
    }
}
