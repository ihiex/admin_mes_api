using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.PublicFun
{
    /// <summary>
    /// Public_SNPar
    /// </summary>
    [Serializable]
    public class Public_SNPar
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// mask
        /// </summary>
        public string mask { get; set; }

        /// <summary>
        /// pos
        /// </summary>
        public int pos { get; set; }

        /// <summary>
        /// len
        /// </summary>
        public int len { get; set; }

        /// <summary>
        /// type
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// luVal
        /// </summary>
        public string luVal { get; set; }


    }
}
