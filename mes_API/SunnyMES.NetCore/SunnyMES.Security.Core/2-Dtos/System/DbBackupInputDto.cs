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
    [AutoMap(typeof(DbBackup))]
    [Serializable]
    public class DbBackupInputDto: IInputDto<string>
    {
        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string BackupType { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 设置或获取 
        /// </summary>
        public DateTime? BackupTime { get; set; }

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
