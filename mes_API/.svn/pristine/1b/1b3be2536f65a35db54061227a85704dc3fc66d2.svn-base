using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Security.Models;

namespace SunnyMES.Commons.Core.Dtos
{
    [AutoMap(typeof(BomPartInfo))]
    [Serializable]
    public class BomLinkedInfo : BomPartInfo
    {
        /// <summary>
        /// 当前项已扫描完成
        /// </summary>
        public bool IsScanFinished { get; set; } = false;

        /// <summary>
        /// 当前项是否为输入项
        /// </summary>
        public bool IsCurrentItem { get; set; }

        /// <summary>
        /// 托盘关联的批次条码
        /// </summary>
        public string TrayBatchSN { get; set; }

        /// <summary>
        /// 针对批次需要校验数量的类型，判断是否需要强制解锁输入
        /// </summary>
        public bool IsClearInput { get; set; }
    }
}
