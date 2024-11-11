using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Attributes;
using SunnyMES.Commons.Core.Models;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.SysConfig.Models.Machine
{
    /// <summary>
    /// mesMachine
    /// </summary>
    [Table("mesMachine")]
    public class SC_mesMachine : BaseCustomEntity<string>
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
        [ForceCheck]
        public string SN { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? RuningStationTypeID { get; set; }

        /// <summary>
        /// 运行容量
        /// </summary>
        public int? RuningQuantity { get; set; }

        /// <summary>
        /// 最大使用数量
        /// </summary>
        public int? MaxUseQuantity { get; set; }

        /// <summary>
        /// 运行容量数量
        /// </summary>
        public int? RuningCapacityQuantity { get; set; }

        /// <summary>
        /// 容量数量
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

        /// <summary>
        /// 父设备ID
        /// </summary>
        public int? ParentID { get; set; }


        /// <summary>
        /// 父设备名称
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [NotMapped]
        public string StatusDesc { get; set; }
        /// <summary>
        /// 站点类型名称
        /// </summary>
        [NotMapped]
        public string StationTypeName { get; set; }
        /// <summary>
        /// 料号
        /// </summary>
        [NotMapped]
        public string PartNumber { get; set; }

        /// <summary>
        /// 设备组
        /// </summary>
        [NotMapped]
        public string MachineFamilyName { get; set; }


        /// <summary>
        /// 无
        /// </summary>
        [NotMapped]
        public List<SC_IdDesc> ValidFroms { get; set; } = new List<SC_IdDesc>{ };

        /// <summary>
        /// 无
        /// </summary>
        [NotMapped]
        public List<SC_IdDescCount> ValidDistributions { get; set; } = new List<SC_IdDescCount> { };

        /// <summary>
        /// 无
        /// </summary>
        [NotMapped]
        public List<SC_IdDesc> ValidTos { get; set; } = new List<SC_IdDesc> { };


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
