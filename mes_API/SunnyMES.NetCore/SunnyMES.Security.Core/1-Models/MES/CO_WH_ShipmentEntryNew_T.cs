using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.Models
{

    /// <summary>
    /// [CO_WH_ShipmentEntryNew]表实体类
    /// </summary>
    [Table("CO_WH_ShipmentEntryNew")]
    [Serializable]
    public class CO_WH_ShipmentEntryNew_T : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        
        public int FInterID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int FEntryID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Key]
        public int FDetailID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FKPONO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FMPNNO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FCTN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FStatus { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FOutSN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FLineItem { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FPartNumberDesc { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? FQTY { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FCrossWeight { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public decimal? FNetWeight { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FCarrierNo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FCommercialInvoice { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FProjectNO { get; set; }

        public override void GenerateDefaultKeyVal()
        {

        }

        public override bool KeyIsNull()
        {
            return  false;
        }
    }

}
