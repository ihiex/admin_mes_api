using AutoMapper;
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
    [AutoMap(typeof(ItemsDetail))]
    [Serializable]
    public class ItemsDetailInputDto: IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string SimpleSpelling { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? Layers { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? SortCode { get; set; }

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
