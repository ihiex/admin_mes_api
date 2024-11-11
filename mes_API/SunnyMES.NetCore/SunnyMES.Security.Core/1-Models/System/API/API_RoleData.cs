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
    [Table("API_RoleData")]
    [Serializable]
    public partial class API_RoleData
    {
        public API_RoleData()
        { }
        private string _Id ;
        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _RoleId ;
        /// <summary>
        /// 
        /// </summary>
        public string RoleId
        {
            set { _RoleId = value; }
            get { return _RoleId; }
        }
        private string _AuthorizeData ;
        /// <summary>
        /// 
        /// </summary>
        public string AuthorizeData
        {
            set { _AuthorizeData = value; }
            get { return _AuthorizeData; }
        }
        private string _DType ;
        /// <summary>
        /// 
        /// </summary>
        public string DType
        {
            set { _DType = value; }
            get { return _DType; }
        }
    }
}
