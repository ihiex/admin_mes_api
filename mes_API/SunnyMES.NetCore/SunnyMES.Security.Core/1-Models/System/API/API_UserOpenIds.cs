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
    [Table("API_UserOpenIds")]
    [Serializable]
    public partial class API_UserOpenIds
    {
        public API_UserOpenIds()
        { }
        private string _UserId ;
        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            set { _UserId = value; }
            get { return _UserId; }
        }
        private string _OpenIdType ;
        /// <summary>
        /// 
        /// </summary>
        public string OpenIdType
        {
            set { _OpenIdType = value; }
            get { return _OpenIdType; }
        }
        private string _OpenId ;
        /// <summary>
        /// 
        /// </summary>
        public string OpenId
        {
            set { _OpenId = value; }
            get { return _OpenId; }
        }
    }
}
