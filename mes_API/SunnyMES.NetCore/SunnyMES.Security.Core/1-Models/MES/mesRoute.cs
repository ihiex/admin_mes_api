using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
	[Serializable]
	public class mesRoute
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int RouteType { get; set; }
	}
}
