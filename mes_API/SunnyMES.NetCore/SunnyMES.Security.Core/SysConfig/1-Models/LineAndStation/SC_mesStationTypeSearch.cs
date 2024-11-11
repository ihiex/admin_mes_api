using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Security.Models
{
    public class SC_mesStationTypeSearch : PagerInfo
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string ApplicationTypeID { get; set; }
        public string ApplicationType { get; set; }

        //public string StationTypeID { get; set; }  关联主表  ID
        //public string StationType { get; set; }  关联主表  Description
        public string StationTypeDetailDefID { get; set; }
        public string StationTypeDetailDef { get; set; }
        public string Content { get; set; }
        public string DetailDescription { get; set; }


        /// <summary>
        /// 模糊查询字段 文本类型
        /// </summary>
        public string LikeQuery { get; set; }
    }
}