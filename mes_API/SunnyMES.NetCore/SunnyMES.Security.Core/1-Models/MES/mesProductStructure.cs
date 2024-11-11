using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesProductStructure
    {
		//public int ID { get; set; }
		public int ParentPartID { get; set; }
		public string ParentPartNumber { get; set; }
		public string ParentPartDescription { get; set; }
		public int PartID { get; set; }
		public string PartNumber { get; set; }
		public string PartDescription { get; set; }
		public string PartPosition { get; set; }
		public bool IsCritical { get; set; }
		public object Status { get; set; }
		public int StationTypeID { get; set; }
	}
}
