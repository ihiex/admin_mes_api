using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesMachine
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StationTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RuningStationTypeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RuningQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? MaxUseQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RuningCapacityQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? CapacityQuantity { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? StartRuningTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? LastRuningTime { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? MachineFamilyID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? PartID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? WarningStatus { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? CheckStatus { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ValidFrom { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ValidDistribution { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string ValidTo { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartNO { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartDesc { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartGroup { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string PartGroupDesc { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? LastMaintenance { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? NextMaintenance { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Store { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StatusID { get; set; }

    }

}
