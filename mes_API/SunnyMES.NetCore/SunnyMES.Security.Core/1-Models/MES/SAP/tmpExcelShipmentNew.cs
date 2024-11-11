using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.Models.MES.SAP
{

    /// <summary>
    /// tmpExcelShipmentNew
    /// </summary>
    [Table("tmpExcelShipmentNew")]
    [Serializable]
    public class tmpExcelShipmentNew : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>
        [Column("ShipDate")]
        public DateTime ShipDate { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        //[Column("HAWB#")]
        [Column("HAWB#")]
        public string HAWB { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string HubCode { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("PO#")]
        public string PO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartNumberDesc { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string QTY { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string CartonQTY { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PalletQTY { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string LineItem { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string TruckNo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("Reference#")]
        public string Reference { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ShipID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ReturnAddress { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryToName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string AdditionalDeliveryToName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryStreetAddress { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryCityName { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryPostalCode { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryRegion { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string DeliveryCountry { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string TelNo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string MAWB_OceanContainerNumber { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string TransportMethod { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string TotalVolume { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string VolumeUnit { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Origion { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string POE_COC { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string POE { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string memo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string CTRY { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SHA { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Sales { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("Web#")]
        public string Web { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string UUI { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("DN#")]
        public string DN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("Delivery#")]
        public string Delivery { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Special { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SCAC { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string OEMSpecificPO1 { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string OEMSpecificPO2 { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? import { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        [Column("NO.")]
        [Key]
        public int NO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public override void GenerateDefaultKeyVal()
        {
            //throw new NotImplementedException();
        }

        public override bool KeyIsNull()
        {
            //不做检查
            return false;
        }
    }

}
