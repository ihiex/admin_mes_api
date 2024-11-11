using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;

namespace SunnyMES.Security.SysConfig.Dtos.Public
{
    public class SearchPropertiesInputDto : SearchSignalInputDto
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
    }
}
