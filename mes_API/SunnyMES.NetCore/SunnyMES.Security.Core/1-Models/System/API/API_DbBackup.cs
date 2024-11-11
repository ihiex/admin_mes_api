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
    [Table("API_DbBackup")]
    [Serializable]
    public partial class API_DbBackup
    {
        public API_DbBackup()
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
        private string _BackupType ;
        /// <summary>
        /// 
        /// </summary>
        public string BackupType
        {
            set { _BackupType = value; }
            get { return _BackupType; }
        }
        private string _DbName ;
        /// <summary>
        /// 
        /// </summary>
        public string DbName
        {
            set { _DbName = value; }
            get { return _DbName; }
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
        private string _FileSize ;
        /// <summary>
        /// 
        /// </summary>
        public string FileSize
        {
            set { _FileSize = value; }
            get { return _FileSize; }
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
        private DateTime? _BackupTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? BackupTime
        {
            set { _BackupTime = value; }
            get { return _BackupTime; }
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
