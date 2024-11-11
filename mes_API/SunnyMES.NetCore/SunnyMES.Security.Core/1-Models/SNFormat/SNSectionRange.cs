using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyMES.Commons.Models;


namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SNSectionRange
    {
        /// <summary>
        /// [SID]
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        /// [STypeID]
        /// </summary>
        public string STypeID { get; set; }

        /// <summary>
        /// [SParam]
        /// </summary>
        public string SParam { get; set; }

        /// <summary>
        /// [Increment]
        /// </summary>
        public string Increment { get; set; }

        /// <summary>
        /// [InvalidChar]
        /// </summary>
        public string InvalidChar { get; set; }

        /// <summary>
        /// [LastUsed]
        /// </summary>
        public string LastUsed { get; set; }

        /// <summary>
        /// [RID]
        /// </summary>
        public string RID { get; set; }

        /// <summary>
        /// [Start]
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// [End]
        /// </summary>
        public string End { get; set; }
        /// <summary>
        /// CurROrder
        /// </summary>
        public string CurROrder { get; set; }

    }
}
