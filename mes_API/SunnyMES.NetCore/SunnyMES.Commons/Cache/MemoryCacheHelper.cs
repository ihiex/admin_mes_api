using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Cache
{
    /// <summary>
    /// 内存缓存操作类
    /// </summary>
    public static class MemoryCacheHelper
    {
        /// <summary>
        /// 用键和值将某个缓存项插入缓存中，并指定基于时间的过期详细信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="I_Hours"></param>
        public static void Set(string key, object obj, int I_Hours = 168)
        {
            var cache = MemoryCache.Default;

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now.AddHours(I_Hours)
                //AbsoluteExpiration = DateTime.Now.AddMinutes(2)  
            };

            cache.Set(key, obj, policy);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            var cache = MemoryCache.Default;

            try
            {
                return (T)cache[key];
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}
