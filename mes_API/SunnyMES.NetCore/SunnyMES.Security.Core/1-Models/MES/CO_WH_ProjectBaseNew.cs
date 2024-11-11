using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models.MES
{
    /// <summary>
    /// CO_WH_ProjectBaseNew  泰国出货参数设定
    /// </summary>
    [Table("CO_WH_ProjectBaseNew")]
    [Serializable]
    public class CO_WH_ProjectBaseNew : BaseCustomEntity<string>
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

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FWeightByPiece { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FWeightByEmptyPallet { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? TypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public bool? IsMixedPO { get; set; }

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
