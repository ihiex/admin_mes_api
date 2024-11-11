using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 输出对象模型 ComData
    /// </summary>
    [Serializable]
    public class ComDataOutputDto
    {
        /// <summary>
        /// 设置或获取  Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// PartFamilyTypeID
        /// </summary>
        public string PartFamilyTypeID { get; set; }

        /// <summary>
        /// PartFamilyType
        /// </summary>
        public string PartFamilyType { get; set; }

        /// <summary>
        /// PartFamilyID
        /// </summary>
        public string PartFamilyID { get; set; }

        /// <summary>
        /// PartFamily
        /// </summary>
        public string PartFamily { get; set; }

        /// <summary>
        /// PartID
        /// </summary>
        public string PartID { get; set; }

        /// <summary>
        /// PartNumber
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// ProductionOrder
        /// </summary>
        public string ProductionOrder { get; set; }


        /// <summary>
        /// LineName
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// Shift
        /// </summary>
        public string Shift { get; set; }

        /// <summary>
        /// StationTypeID
        /// </summary>
        public string StationTypeID { get; set; }

        /// <summary>
        /// StationType
        /// </summary>
        public string StationType { get; set; }


        /// <summary>
        /// StationID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// Station
        /// </summary>
        public string Station { get; set; }

    }
}
