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
    [AutoMap(typeof(RoleAuthorize))]
    [Serializable]
    public class RoleAuthorizeInputDto : IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? ItemType { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? ObjectType { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? SortCode { get; set; }


    }
}
