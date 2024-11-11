
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
    /// <summary>
    /// mesLine   Search
    /// </summary>
    public class SC_mesLineSearch: PagerInfo  //: SearchInputDto<SC_mesLineDto>
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ServerID { get; set; }
        public string StatusID { get; set; }
        public string StatusValue { get; set; }

        public string LineTypeDefID { get; set; }
        public string LineTypeDefValue { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 模糊查询字段 文本类型 Description，StatusValue，LineTypeDefValue，Content
        /// </summary>
        public string LikeQuery { get; set; }
    }
}