using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Models;

namespace SunnyMES.Security.Models.MES.SAP
{

    /// <summary>
    /// CO_ShipManifestData_SN
    /// </summary>
    [Table("CO_ShipManifestData_SN")]
    [Serializable]
    public class CO_ShipManifestData_SN : Entity
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 无
        /// </summary>
        public string HAWB { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string BillNO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ItemNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PalletSN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Box_SN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string FG_SN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SAPGet { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public string? ShipmentWeight { get; set; }

        [NotMapped]
        public string MPN { get; set; }

        public override void GenerateDefaultKeyVal()
        {

        }

        public override bool KeyIsNull()
        {
            return false;
        }
        public override void GenerateDefaultKeyVal(int ID)
        {

        }
    }

}
