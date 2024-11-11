using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
	[Serializable]
	public class mesPart
	{
        public int ID { get; set; }
        public string PartNumber { get; set; }
        public string Revision { get; set; }
        public string Description { get; set; }
        public int PartFamilyID { get; set; }
        public string UOM { get; set; }
        public bool IsUnit { get; set; }
        public int Status { get; set; }

        public string Color { get; set; }
        public string ColorValue { get; set; }

    }
}
