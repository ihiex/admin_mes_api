using static Dapper.SqlMapper;
using System.Collections.Generic;
using System.IO;
using System;
using SunnyMES.Commons;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security.Repositories;

namespace SunnyMES.AspNetCore.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class PsysLog
    {
        private static void CreateDIR(string S_CurrentLoginIP)
        {
            try
            {
                string S_Path = Directory.GetCurrentDirectory();
                string S_DayLog = S_Path + "\\ControllerLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";

                if (Directory.Exists(S_Path + "\\ControllerLog") == false)
                {
                    Directory.CreateDirectory(S_Path + "\\ControllerLog");
                }

                S_DayLog = S_Path + "\\ControllerLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\";
                if (Directory.Exists(S_DayLog) == false)
                {
                    Directory.CreateDirectory(S_DayLog);
                }

                S_DayLog += S_CurrentLoginIP + "\\";
                if (Directory.Exists(S_DayLog) == false)
                {
                    Directory.CreateDirectory(S_DayLog);
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="S_MSG"></param>
        /// <param name="S_CurrentLoginIP"></param>
        /// <param name="S_UserID"></param>
        public static void SetControllerLog(string S_MSG, string S_CurrentLoginIP, string S_UserID)
        {
            try
            {
                string S_IsControllerLog = Configs.GetConfigurationValue("AppSetting", "IsControllerLog");
                if (S_IsControllerLog == "1")
                {
                    CreateDIR(S_CurrentLoginIP);
                    PublicRepository publicRepository = new PublicRepository();
                    GridReader List_mesEmployee = publicRepository.GetList("mesEmployee", " ID='" + S_UserID + "'");
                    List<mesEmployee> v_mesEmployee = List_mesEmployee.Read<mesEmployee>().AsList();

                    string S_UserName = v_mesEmployee[0].Lastname;
                    S_MSG = "User:" + S_UserName + "  " + S_MSG;

                    string S_Path = Directory.GetCurrentDirectory();
                    string S_DayLog = S_Path + "\\ControllerLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" +
                        S_CurrentLoginIP + "\\";

                    File.AppendAllText(S_DayLog + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH") + "ControllerLog.Log",
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "  " + S_MSG + "\r\n");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

