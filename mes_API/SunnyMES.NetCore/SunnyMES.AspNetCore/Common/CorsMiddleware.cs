﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SunnyMES.AspNetCore.Common
{
    /// <summary>
    /// 跨域中间件
    /// 解决net core 3.1 跨域 Cors 找不到 “Access-Control-Allow-Origin”
    /// </summary>
    public class CorsMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                if (!httpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
                await _next(httpContext);
            }
            catch
            { 
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
