using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.Models;

namespace SunnyMES.Security._2_Dtos.MES
{
    /// <summary>
    /// 通用输出结果
    /// </summary>
    [Serializable]
    public class MesOutputDto:IOutputDto
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public dynamic ErrorMsg { get; set; } = "";

        /// <summary>
        /// 工单数量
        /// </summary>
        public dynamic ProductionOrderQTY { get; set; }
        /// <summary>
        /// 向controller传递其他数据
        /// </summary>
        public SettingInfo CurrentSettingInfo { get;set; } = new SettingInfo();

        public InitPageInfo CurrentInitPageInfo { get; set; } = new InitPageInfo();
    }
}
