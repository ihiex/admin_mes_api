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
    [Table("API_TaskManager")]
    [Serializable]
    public partial class API_TaskManager
    {
        public API_TaskManager()
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
        private string _TaskName ;
        /// <summary>
        /// 
        /// </summary>
        public string TaskName
        {
            set { _TaskName = value; }
            get { return _TaskName; }
        }
        private string _GroupName ;
        /// <summary>
        /// 
        /// </summary>
        public string GroupName
        {
            set { _GroupName = value; }
            get { return _GroupName; }
        }
        private DateTime? _StartTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartTime
        {
            set { _StartTime = value; }
            get { return _StartTime; }
        }
        private DateTime? _EndTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime
        {
            set { _EndTime = value; }
            get { return _EndTime; }
        }
        private string _Cron ;
        /// <summary>
        /// 
        /// </summary>
        public string Cron
        {
            set { _Cron = value; }
            get { return _Cron; }
        }
        private bool _IsLocal ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsLocal
        {
            set { _IsLocal = value; }
            get { return _IsLocal; }
        }
        private string _JobCallAddress ;
        /// <summary>
        /// 
        /// </summary>
        public string JobCallAddress
        {
            set { _JobCallAddress = value; }
            get { return _JobCallAddress; }
        }
        private string _JobCallParams ;
        /// <summary>
        /// 
        /// </summary>
        public string JobCallParams
        {
            set { _JobCallParams = value; }
            get { return _JobCallParams; }
        }
        private DateTime? _LastRunTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastRunTime
        {
            set { _LastRunTime = value; }
            get { return _LastRunTime; }
        }
        private DateTime? _LastErrorTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastErrorTime
        {
            set { _LastErrorTime = value; }
            get { return _LastErrorTime; }
        }
        private DateTime? _NextRunTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? NextRunTime
        {
            set { _NextRunTime = value; }
            get { return _NextRunTime; }
        }
        private int? _RunCount ;
        /// <summary>
        /// 
        /// </summary>
        public int? RunCount
        {
            set { _RunCount = value; }
            get { return _RunCount; }
        }
        private int? _ErrorCount ;
        /// <summary>
        /// 
        /// </summary>
        public int? ErrorCount
        {
            set { _ErrorCount = value; }
            get { return _ErrorCount; }
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
        private int? _SendMail ;
        /// <summary>
        /// 
        /// </summary>
        public int? SendMail
        {
            set { _SendMail = value; }
            get { return _SendMail; }
        }
        private string _EmailAddress ;
        /// <summary>
        /// 
        /// </summary>
        public string EmailAddress
        {
            set { _EmailAddress = value; }
            get { return _EmailAddress; }
        }
        private int? _Status ;
        /// <summary>
        /// 
        /// </summary>
        public int? Status
        {
            set { _Status = value; }
            get { return _Status; }
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
        private bool _DeleteMark ;
        /// <summary>
        /// 
        /// </summary>
        public bool DeleteMark
        {
            set { _DeleteMark = value; }
            get { return _DeleteMark; }
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
        private string _CompanyId ;
        /// <summary>
        /// 
        /// </summary>
        public string CompanyId
        {
            set { _CompanyId = value; }
            get { return _CompanyId; }
        }
        private string _DeptId ;
        /// <summary>
        /// 
        /// </summary>
        public string DeptId
        {
            set { _DeptId = value; }
            get { return _DeptId; }
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
