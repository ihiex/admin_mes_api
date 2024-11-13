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
    public class MesLogHelper
    {
        public static readonly string _IsMESLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");
        public static async Task WriteLog(MesLogDIO MesLogContext)
        {
            string IsMESLog = Configs.GetConfigurationValue("AppSetting", "IsMesLog");
            if (string.IsNullOrEmpty(IsMESLog) || IsMESLog != "1")
                return;
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string LogPath = $"{currentDir}MesLog\\{MesLogContext.StationName}_{MesLogContext.StationId}\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{MesLogContext.CurrentIP}\\{MesLogContext.Result}";

            try
            {
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                await File.AppendAllTextAsync($"{LogPath}\\{DateTime.Now.ToString("yyyy-MM-dd HH")}.Log",
                    $"{MesLogContext.Msg}\r\n");
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(" write Log exception", ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        public static void LogEor(string msg, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (string.IsNullOrEmpty(_IsMESLog) || _IsMESLog != "1")
                return;

            log4net.ILog Log = log4net.LogManager.GetLogger("");
            Log.Error(string.Format("{3}\r\nCallerFilePath:\t[{0}]\r\nCallerMemberName:\t[{1}]\r\nCallerLineNumber:\t[{2}]", sourceFilePath, memberName, sourceLineNumber, msg));
        }
    }
}
