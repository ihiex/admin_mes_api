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
    [Table("API_UserLogOn")]
    [Serializable]
    public partial class API_UserLogOn
    {
        public API_UserLogOn()
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
        private Double _UserId ;
        /// <summary>
        /// 
        /// </summary>
        public Double UserId
        {
            set { _UserId = value; }
            get { return _UserId; }
        }
        private string _UserPassword ;
        /// <summary>
        /// 
        /// </summary>
        public string UserPassword
        {
            set { _UserPassword = value; }
            get { return _UserPassword; }
        }
        private string _UserSecretkey ;
        /// <summary>
        /// 
        /// </summary>
        public string UserSecretkey
        {
            set { _UserSecretkey = value; }
            get { return _UserSecretkey; }
        }
        private DateTime? _AllowStartTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? AllowStartTime
        {
            set { _AllowStartTime = value; }
            get { return _AllowStartTime; }
        }
        private DateTime? _AllowEndTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? AllowEndTime
        {
            set { _AllowEndTime = value; }
            get { return _AllowEndTime; }
        }
        private DateTime? _LockStartDate ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LockStartDate
        {
            set { _LockStartDate = value; }
            get { return _LockStartDate; }
        }
        private DateTime? _LockEndDate ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LockEndDate
        {
            set { _LockEndDate = value; }
            get { return _LockEndDate; }
        }
        private DateTime? _FirstVisitTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FirstVisitTime
        {
            set { _FirstVisitTime = value; }
            get { return _FirstVisitTime; }
        }
        private DateTime? _PreviousVisitTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PreviousVisitTime
        {
            set { _PreviousVisitTime = value; }
            get { return _PreviousVisitTime; }
        }
        private DateTime? _LastVisitTime ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastVisitTime
        {
            set { _LastVisitTime = value; }
            get { return _LastVisitTime; }
        }
        private DateTime? _ChangePasswordDate ;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ChangePasswordDate
        {
            set { _ChangePasswordDate = value; }
            get { return _ChangePasswordDate; }
        }
        private bool _MultiUserLogin ;
        /// <summary>
        /// 
        /// </summary>
        public bool MultiUserLogin
        {
            set { _MultiUserLogin = value; }
            get { return _MultiUserLogin; }
        }
        private int? _LogOnCount ;
        /// <summary>
        /// 
        /// </summary>
        public int? LogOnCount
        {
            set { _LogOnCount = value; }
            get { return _LogOnCount; }
        }
        private bool _UserOnLine ;
        /// <summary>
        /// 
        /// </summary>
        public bool UserOnLine
        {
            set { _UserOnLine = value; }
            get { return _UserOnLine; }
        }
        private string _Question ;
        /// <summary>
        /// 
        /// </summary>
        public string Question
        {
            set { _Question = value; }
            get { return _Question; }
        }
        private string _AnswerQuestion ;
        /// <summary>
        /// 
        /// </summary>
        public string AnswerQuestion
        {
            set { _AnswerQuestion = value; }
            get { return _AnswerQuestion; }
        }
        private bool _CheckIPAddress ;
        /// <summary>
        /// 
        /// </summary>
        public bool CheckIPAddress
        {
            set { _CheckIPAddress = value; }
            get { return _CheckIPAddress; }
        }
        private string _Language ;
        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            set { _Language = value; }
            get { return _Language; }
        }
        private string _Theme ;
        /// <summary>
        /// 
        /// </summary>
        public string Theme
        {
            set { _Theme = value; }
            get { return _Theme; }
        }
    }
}
