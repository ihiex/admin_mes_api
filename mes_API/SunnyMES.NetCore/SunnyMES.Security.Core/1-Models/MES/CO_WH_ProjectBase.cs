using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models.MES
{

    /// <summary>
    /// CO_WH_ProjectBase   东莞出货参数配置
    /// </summary>
    [Table("CO_WH_ProjectBase")]
    [Serializable]
    public class CO_WH_ProjectBase : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int FID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FProjectNO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FCountByPallet { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FCountByCase { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FWeightByCase { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FWeightByPallet { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FTotalWeight { get; set; }

        public override void GenerateDefaultKeyVal()
        {
            //throw new NotImplementedException();
        }

        public override bool KeyIsNull()
        {
            return false; 
        }
    }

}
