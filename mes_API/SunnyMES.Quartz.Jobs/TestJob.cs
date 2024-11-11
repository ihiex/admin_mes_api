﻿using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.App;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Email;
using SunnyMES.Quartz.IServices;
using SunnyMES.Quartz.Models;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Quartz.Jobs
{
    /// <summary>
    /// 测试demo
    /// 必须实现IJob接口中的Execute()方法
    /// </summary>
    public class TestJob : IJob
    {
        ITaskManagerService iService = App.GetService<ITaskManagerService>();
        ILogRepository iLogService = App.GetService<ILogRepository>();

        public Task Execute(IJobExecutionContext context)
        { 
            DateTime dateTime = DateTime.Now;
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            SysSetting sysSetting = yuebonCacheHelper.Get("SysSetting").ToJson().ToObject<SysSetting>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            string sqlWhere = string.Format("Id='{0}' and GroupName='{1}'", trigger.Name, trigger.Group);
            TaskManager taskManager = iService.GetWhere(sqlWhere);
            if (taskManager == null)
            {
                FileQuartz.WriteErrorLog($"任务不存在");
                return Task.Delay(1);
            }
            try
            {
                string msg = $"开始时间:{dateTime.ToString("yyyy-MM-dd HH:mm:ss ffff")}";
                //记录任务执行记录
                iService.RecordRun(taskManager.Id, JobAction.开始,true, msg);
                //初始化任务日志
                FileQuartz.InitTaskJobLogPath(taskManager.Id);
                var jobId = context.MergedJobDataMap.GetString("OpenJob");
                //todo:这里可以加入自己的自动任务逻辑
                Log4NetHelper.Info(DateTime.Now.ToString() + "执行任务");


                stopwatch.Stop();
                string content = $"结束时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} 共耗时{stopwatch.ElapsedMilliseconds} 毫秒\r\n";
                iService.RecordRun(taskManager.Id, JobAction.结束, true, content);
                if ((MsgType)taskManager.SendMail == MsgType.All)
                {
                    string emailAddress = sysSetting.Email;
                    if (!string.IsNullOrEmpty(taskManager.EmailAddress))
                    {
                        emailAddress = taskManager.EmailAddress;
                    }

                    List<string> recipients = new List<string>();
                    recipients = emailAddress.Split(",").ToList();
                    var mailBodyEntity = new MailBodyEntity()
                    {
                        Body = msg + content + ",请勿回复本邮件",
                        Recipients = recipients,
                        Subject = taskManager.TaskName
                    };
                    SendMailHelper.SendMail(mailBodyEntity);

                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                string content = $"结束时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} 共耗时{stopwatch.ElapsedMilliseconds} 毫秒\r\n";
                iService.RecordRun(taskManager.Id, JobAction.结束, false, content + ex.Message);
                FileQuartz.WriteErrorLog(ex.Message);
                if ((MsgType)taskManager.SendMail == MsgType.Error || (MsgType)taskManager.SendMail == MsgType.All)
                {
                    string emailAddress = sysSetting.Email;
                    if (!string.IsNullOrEmpty(taskManager.EmailAddress))
                    {
                        emailAddress = taskManager.EmailAddress;
                    }

                    List<string> recipients = new List<string>();
                    recipients = emailAddress.Split(",").ToList();
                    var mailBodyEntity = new MailBodyEntity()
                    {
                        Body = "处理失败," + ex.Message + ",请勿回复本邮件",
                        Recipients = recipients,
                        Subject = taskManager.TaskName
                    };
                    SendMailHelper.SendMail(mailBodyEntity);
                }
            }

            return Task.Delay(1);
        }
    }
}
