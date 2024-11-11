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
    [Table("API_Area")]
    [Serializable]
    public partial class API_Area
    {
        public API_Area()
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
        private string _ParentId ;
        /// <summary>
        /// 
        /// </summary>
        public string ParentId
        {
            set { _ParentId = value; }
            get { return _ParentId; }
        }
        private int? _Layers ;
        /// <summary>
        /// 
        /// </summary>
        public int? Layers
        {
            set { _Layers = value; }
            get { return _Layers; }
        }
        private string _EnCode ;
        /// <summary>
        /// 
        /// </summary>
        public string EnCode
        {
            set { _EnCode = value; }
            get { return _EnCode; }
        }
        private string _FullName ;
        /// <summary>
        /// 
        /// </summary>
        public string FullName
        {
            set { _FullName = value; }
            get { return _FullName; }
        }
        private string _SimpleSpelling ;
        /// <summary>
        /// 
        /// </summary>
        public string SimpleSpelling
        {
            set { _SimpleSpelling = value; }
            get { return _SimpleSpelling; }
        }
        private string _FullIdPath ;
        /// <summary>
        /// 
        /// </summary>
        public string FullIdPath
        {
            set { _FullIdPath = value; }
            get { return _FullIdPath; }
        }
        private bool _IsLast ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsLast
        {
            set { _IsLast = value; }
            get { return _IsLast; }
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
        private bool _DeleteMark ;
        /// <summary>
        /// 
        /// </summary>
        public bool DeleteMark
        {
            set { _DeleteMark = value; }
            get { return _DeleteMark; }
        }
        private bool _EnabledMark ;
        /// <summary>
        /// 
        /// </summary>
        public bool EnabledMark
        {
            set { _EnabledMark = value; }
            get { return _EnabledMark; }
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
        private DateTime? _LastModifyTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastModifyTime
        {
            set { _LastModifyTime = value; }
            get { return _LastModifyTime; }
        }
        private string _LastModifyUserId ;
        /// <summary>
        /// 
        /// </summary>
        public string LastModifyUserId
        {
            set { _LastModifyUserId = value; }
            get { return _LastModifyUserId; }
        }
        private DateTime? _DeleteTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeleteTime
        {
            set { _DeleteTime = value; }
            get { return _DeleteTime; }
        }
        private string _DeleteUserId ;
        /// <summary>
        /// 
        /// </summary>
        public string DeleteUserId
        {
            set { _DeleteUserId = value; }
            get { return _DeleteUserId; }
        }
    }
}
