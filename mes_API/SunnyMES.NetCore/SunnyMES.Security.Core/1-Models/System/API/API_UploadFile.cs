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
    [Table("API_UploadFile")]
    [Serializable]
    public partial class API_UploadFile
    {
        public API_UploadFile()
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
        private string _FileName ;
        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            set { _FileName = value; }
            get { return _FileName; }
        }
        private string _FilePath ;
        /// <summary>
        /// 
        /// </summary>
        public string FilePath
        {
            set { _FilePath = value; }
            get { return _FilePath; }
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
        private string _FileType ;
        /// <summary>
        /// 
        /// </summary>
        public string FileType
        {
            set { _FileType = value; }
            get { return _FileType; }
        }
        private int? _FileSize ;
        /// <summary>
        /// 
        /// </summary>
        public int? FileSize
        {
            set { _FileSize = value; }
            get { return _FileSize; }
        }
        private string _Extension ;
        /// <summary>
        /// 
        /// </summary>
        public string Extension
        {
            set { _Extension = value; }
            get { return _Extension; }
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
        private string _CreatorUserId ;
        /// <summary>
        /// 
        /// </summary>
        public string CreatorUserId
        {
            set { _CreatorUserId = value; }
            get { return _CreatorUserId; }
        }
        private string _CreateUserName ;
        /// <summary>
        /// 
        /// </summary>
        public string CreateUserName
        {
            set { _CreateUserName = value; }
            get { return _CreateUserName; }
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
        private string _Thumbnail ;
        /// <summary>
        /// 
        /// </summary>
        public string Thumbnail
        {
            set { _Thumbnail = value; }
            get { return _Thumbnail; }
        }
        private string _BelongApp ;
        /// <summary>
        /// 
        /// </summary>
        public string BelongApp
        {
            set { _BelongApp = value; }
            get { return _BelongApp; }
        }
        private string _BelongAppId ;
        /// <summary>
        /// 
        /// </summary>
        public string BelongAppId
        {
            set { _BelongAppId = value; }
            get { return _BelongAppId; }
        }
    }
}
