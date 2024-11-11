using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Models;

namespace SunnyMES.Security.SysConfig.Models.Machine
{

    /// <summary>
    /// luMachineStatus
    /// </summary>
    [Table("luMachineStatus")]
    public class SC_luMachineStatus : BaseCustomEntity<string>
    {
        /// <summary>
        /// 无
        /// </summary>        
        public int ID { get; set; }

        /// <summary>
        /// 无
        /// </summary>
        public string Description { get; set; }
        public override bool KeyIsNull()
        {
            return true;
        }
        public override void GenerateDefaultKeyVal(int ID)
        {
            this.ID = ID;
        }
    }


}
