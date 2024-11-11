using System.Collections.Generic;
using System.Linq;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.App;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.AspNetCore.Common
{
    /// <summary>
    /// 默认初始化内容
    /// </summary>
    public  class DefaultInitial : YuebonInitialization
    {
        IAPPService _aPPService = App.GetService<IAPPService>();

        /// <summary>
        /// 内存中缓存多应用
        /// </summary>
        public  void CacheAppList()
        {
            _aPPService.UpdateCacheAllowApp();
        }
    }
}
