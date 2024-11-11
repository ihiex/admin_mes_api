using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    public class LoginList
    {
        public int? StationTypeID { get; set; }
        public string StationType { get; set; }

        public int LineID { get; set; }
        public string Line { get; set; }
        public int StationID { get; set; }
        public string Station { get; set; }
        public int EmployeeID { get; set; }

        public string UserName { get; set; }
        public int LanguageID { get; set; }


        public Boolean IsCheckPO { get; set; }
        public Boolean IsCheckNG { get; set; }
        public string CurrentLoginIP { get; set; }

        public string TTBoxSN_Pattern { get; set; }
        public string IsTTBoxUnpack { get; set; }
        public string PrintIPPort { get; set; }
        public int PrintQTY { get; set; }  

    }
}
