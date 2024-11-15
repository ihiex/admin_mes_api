﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 定时任务改变状态
    /// </summary>
    [Serializable]
    public class ChangeJobStatusReq
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 改变任务状态
        /// 0：停止；1：启动（任务变成正在运行）
        /// </summary>
        public int Status { get; set; }
    }
}
