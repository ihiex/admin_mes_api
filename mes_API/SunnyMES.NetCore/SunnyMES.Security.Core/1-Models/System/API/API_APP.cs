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
    /// API_APP
    /// </summary>
    [AppDBContext("DefaultDb")]
    [Table("API_APP")]
    [Serializable]
    public partial class API_APP
    {
        public API_APP()
        { }
        private string _Id;
        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _AppId;
        /// <summary>
        /// 
        /// </summary>
        public string AppId
        {
            set { _AppId = value; }
            get { return _AppId; }
        }
        private string _AppSecret;
        /// <summary>
        /// 
        /// </summary>
        public string AppSecret
        {
            set { _AppSecret = value; }
            get { return _AppSecret; }
        }
        private string _EncodingAESKey;
        /// <summary>
        /// 
        /// </summary>
        public string EncodingAESKey
        {
            set { _EncodingAESKey = value; }
            get { return _EncodingAESKey; }
        }
        private string _RequestUrl;
        /// <summary>
        /// 
        /// </summary>
        public string RequestUrl
        {
            set { _RequestUrl = value; }
            get { return _RequestUrl; }
        }
        private string _Token;
        /// <summary>
        /// 
        /// </summary>
        public string Token
        {
            set { _Token = value; }
            get { return _Token; }
        }
        private bool _IsOpenAEKey;
        /// <summary>
        /// 
        /// </summary>
        public bool IsOpenAEKey
        {
            set { _IsOpenAEKey = value; }
            get { return _IsOpenAEKey; }
        }
        private bool _DeleteMark;
        /// <summary>
        /// 
        /// </summary>
        public bool DeleteMark
        {
            set { _DeleteMark = value; }
            get { return _DeleteMark; }
        }
        private bool _EnabledMark;
        /// <summary>
        /// 
        /// </summary>
        public bool EnabledMark
        {
            set { _EnabledMark = value; }
            get { return _EnabledMark; }
        }
        private string _Description;
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set { _Description = value; }
            get { return _Description; }
        }
        private DateTime? _CreatorTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreatorTime
        {
            set { _CreatorTime = value; }
            get { return _CreatorTime; }
        }
        private string _CreatorUserId;
        /// <summary>
        /// 
        /// </summary>
        public string CreatorUserId
        {
            set { _CreatorUserId = value; }
            get { return _CreatorUserId; }
        }
        private string _CompanyId;
        /// <summary>
        /// 
        /// </summary>
        public string CompanyId
        {
            set { _CompanyId = value; }
            get { return _CompanyId; }
        }
        private string _DeptId;
        /// <summary>
        /// 
        /// </summary>
        public string DeptId
        {
            set { _DeptId = value; }
            get { return _DeptId; }
        }
        private DateTime? _LastModifyTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastModifyTime
        {
            set { _LastModifyTime = value; }
            get { return _LastModifyTime; }
        }
        private string _LastModifyUserId;
        /// <summary>
        /// 
        /// </summary>
        public string LastModifyUserId
        {
            set { _LastModifyUserId = value; }
            get { return _LastModifyUserId; }
        }
        private DateTime? _DeleteTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeleteTime
        {
            set { _DeleteTime = value; }
            get { return _DeleteTime; }
        }
        private string _DeleteUserId;
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
