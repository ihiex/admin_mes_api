using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.App;
using SunnyMES.Commons.Core.PublicFun.Model;

namespace SunnyMES.Commons.Log
{
    /// <summary>
    /// SysConfigLogHelper
    /// </summary>
    public class SysConfigLogHelper
    {
        //public static readonly string _IsMESLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");

        /// <summary>
        /// WriteLog
        /// </summary>
        /// <param name="MesLogContext"></param>
        /// <returns></returns>
        public static async Task WriteLog(SysConfigLogDIO MesLogContext)
        {
            //string IsMESLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");
            //if (string.IsNullOrEmpty(IsMESLog) || IsMESLog != "1")
            //    return;
            try
            {                
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string LogPath = $"{currentDir}MesLog\\{MesLogContext.CurrentIP}" +
                    $"\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{MesLogContext.UserName}\\{MesLogContext.DataType}" +
                    $"\\{MesLogContext.TableName}";

                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                await File.AppendAllTextAsync($"{LogPath}\\{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss")}.Log",
                    $"{MesLogContext.MSG}\r\n");
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(" write Log exception", ex);
            }
        }

    }
}

