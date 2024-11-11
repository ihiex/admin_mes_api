using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;


namespace SunnyMES.Security.Models
{
    /// <summary>
    /// 系统日志，数据实体对象
    /// </summary>
    [AppDBContext("DefaultDb")]
    [Table("API_OpenIdSettings")]
    [Serializable]
    public partial class API_OpenIdSettings
    {
        public API_OpenIdSettings()
        { }
        private string _OpenIdType ;
        /// <summary>
        /// 
        /// </summary>
        public string OpenIdType
        {
            set { _OpenIdType = value; }
            get { return _OpenIdType; }
        }
        private string _Name ;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }
        private string _Description ;
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set { _Description = value; }
            get { return _Description; }
        }
        private string _Settings ;
        /// <summary>
        /// 
        /// </summary>
        public string Settings
        {
            set { _Settings = value; }
            get { return _Settings; }
        }
    }
}
