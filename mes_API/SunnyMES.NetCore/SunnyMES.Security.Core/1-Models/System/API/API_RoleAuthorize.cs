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
    [Table("API_RoleAuthorize")]
    [Serializable]
    public partial class API_RoleAuthorize
    {
        public API_RoleAuthorize()
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
        private int? _ItemType ;
        /// <summary>
        /// 
        /// </summary>
        public int? ItemType
        {
            set { _ItemType = value; }
            get { return _ItemType; }
        }
        private string _ItemId ;
        /// <summary>
        /// 
        /// </summary>
        public string ItemId
        {
            set { _ItemId = value; }
            get { return _ItemId; }
        }
        private int? _ObjectType ;
        /// <summary>
        /// 
        /// </summary>
        public int? ObjectType
        {
            set { _ObjectType = value; }
            get { return _ObjectType; }
        }
        private string _ObjectId ;
        /// <summary>
        /// 
        /// </summary>
        public string ObjectId
        {
            set { _ObjectId = value; }
            get { return _ObjectId; }
        }
        private int? _SortCode ;
        /// <summary>
        /// 
        /// </summary>
        public int? SortCode
        {
            set { _SortCode = value; }
            get { return _SortCode; }
        }
        private DateTime? _CreatorTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreatorTime
        {
            set { _CreatorTime = value; }
            get { return _CreatorTime; }
        }
        private string _CreatorUserId ;
        /// <summary>
        /// 
        /// </summary>
        public string CreatorUserId
        {
            set { _CreatorUserId = value; }
            get { return _CreatorUserId; }
        }
    }
}
