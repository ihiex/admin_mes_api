using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    public class DictDataDetail
    {
        public string Id { get; set; }
        public string DictDataId { get; set; }
        public string ParentId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int SortCode { get; set; }
    }
}
