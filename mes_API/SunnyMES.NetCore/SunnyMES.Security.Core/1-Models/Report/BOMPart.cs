using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;


namespace SunnyMES.Security.Models
{
    [Serializable]
    public class BOMPart    //: BaseEntityGeneric<string>
    {
        /// <summary>
        /// PartID
        /// </summary>
        public int PartID { get; set; }
        /// <summary>
        /// PartNumber
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// ParentPartID
        /// </summary>
        public int ParentPartID { get; set; }

        /// <summary>
        /// StationTypeID
        /// </summary>
        public int StationTypeID { get; set; }

        /// <summary>
        /// StationType
        /// </summary>
        public string StationType { get; set; }
        /// <summary>
        /// Level
        /// </summary>
        public int Level { get; set; }


    }
}
