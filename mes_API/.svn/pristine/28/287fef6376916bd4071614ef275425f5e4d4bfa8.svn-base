using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.PO
{

    /// <summary>
    /// luProductionOrderDetailDef
    /// </summary>
    [Table("luProductionOrderDetailDef")]
    public class SC_luProductionOrderDetailDef : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [ForceCheck]
        public string Description { get; set; }

        /// <summary>
        /// 检查当前项的父项ID所对应的详细值
        /// </summary>
        public int? ParentValueID { get; set; }

        /// <summary>
        /// 检查当前项的父项ID所对应的输入条码
        /// </summary>
        public int? ParentCheckID { get; set; }

        /// <summary>
        /// 值类型 默认为0，字符型，1为数值型
        /// </summary>
        public int? ValueType { get; set; }

        /// <summary>
        /// 检查类型
        /// 0 ： 未配置
        /// 1 ：值对比类型
        /// 2 ：存储过程
        /// 3 ：动态接口
        /// 4 : UPC SN 检查
        /// </summary>
        public int? CheckType { get; set; }

        /// <summary>
        /// 检查类型对应的参数
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// 是否是必须项
        /// </summary>
        public bool? Required { get; set; }

        public override bool KeyIsNull()
        {
            return true;
        }
        public override void GenerateDefaultKeyVal(int ID)
        {
            this.ID = ID;
        }
    }

}
