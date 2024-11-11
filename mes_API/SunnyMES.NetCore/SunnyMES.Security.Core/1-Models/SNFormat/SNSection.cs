using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;


namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SNSection
    {
        /// <summary>
        /// SNFormatID
        /// </summary>
        public int SNFormatID { get; set; }

        /// <summary>
        /// SectionType
        /// </summary>
        public int SectionType { get; set; }

        /// <summary>
        /// SectionParam
        /// </summary>
        public string SectionParam { get; set; }

        /// <summary>
        /// Increment
        /// </summary>
        public int? Increment { get; set; }

        /// <summary>
        /// InvalidChar
        /// </summary>
        public string InvalidChar { get; set; }

        /// <summary>
        /// LastUsed
        /// </summary>
        public string LastUsed { get; set; }

        /// <summary>
        /// [Order]
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// AllowReset
        /// </summary>
        public Boolean AllowReset { get; set; }

        /// <summary>
        /// EmailSent
        /// </summary>
        public int? EmailSent { get; set; }
    }
}
