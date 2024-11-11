﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// uniapp 地区选择
    /// </summary>
    [Serializable]
    public class AreaPickerOutputDto
    {
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 显示内容
        /// </summary>		
        public  string label { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public  List<AreaPickerOutputDto> children { get; set; }
    }
}
