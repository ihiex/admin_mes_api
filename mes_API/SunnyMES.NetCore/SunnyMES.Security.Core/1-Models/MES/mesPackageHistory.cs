using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security._1_Models.MES
{

    /// <summary>
    /// mesPackageHistory
    /// </summary>
    public class mesPackageHistory
    {
        /// <summary>
        /// 无
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int PackageID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public byte PackageStatusID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? StationID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public DateTime? Time { get; set; }

    }

}
