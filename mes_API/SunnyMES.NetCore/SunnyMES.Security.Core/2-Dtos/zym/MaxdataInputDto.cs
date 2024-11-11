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
    /// 大数据测试输入对象模型
    /// </summary>
    [AutoMap(typeof(Maxdata))]
    [Serializable]
    public class MaxdataInputDto : IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int UnitStateID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public byte StatusID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? PanelID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? LineID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? ProductionOrderID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? RMAID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? PartID { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? LooperCount { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Description { get; set; }

    }
}
