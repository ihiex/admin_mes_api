﻿using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extensions;
//通过HostingStartup指定要启动的类型
[assembly: HostingStartup(typeof(SunnyMES.Commons.Core.App.HostingStartup))]
namespace SunnyMES.Commons.Core.App
{
    /// <summary>
    /// 配置程序启动时自动注入
    /// </summary>
    public sealed class HostingStartup : IHostingStartup
    {
        /// <summary>
        /// 配置应用启动
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(IWebHostBuilder builder)
        {
            //可以添加配置
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                // 自动装载配置
                App.AddConfigureFiles(config, hostingContext.HostingEnvironment);
            });

            //可以添加ConfigureServices
            // 自动注入 AddApp() 服务
            builder.ConfigureServices(services =>
            {
                
            });
        }
    }
}
