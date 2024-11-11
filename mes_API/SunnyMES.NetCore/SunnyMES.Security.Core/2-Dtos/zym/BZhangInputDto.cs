﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输入对象模型
    /// </summary>
    [AutoMap(typeof(BZhang))]
    [Serializable]
    public class BZhangInputDto : IInputDto<string>
    {


        public string Data1 { get; set; }

        public string Data2 { get; set; }
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }



        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Description { get; set; }

    }
}