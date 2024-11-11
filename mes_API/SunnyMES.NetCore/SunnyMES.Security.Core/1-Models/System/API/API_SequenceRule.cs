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
    [Table("API_SequenceRule")]
    [Serializable]
    public partial class API_SequenceRule
    {
        public API_SequenceRule()
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
        private string _SequenceName ;
        /// <summary>
        /// 
        /// </summary>
        public string SequenceName
        {
            set { _SequenceName = value; }
            get { return _SequenceName; }
        }
        private int? _RuleOrder ;
        /// <summary>
        /// 
        /// </summary>
        public int? RuleOrder
        {
            set { _RuleOrder = value; }
            get { return _RuleOrder; }
        }
        private string _RuleType ;
        /// <summary>
        /// 
        /// </summary>
        public string RuleType
        {
            set { _RuleType = value; }
            get { return _RuleType; }
        }
        private string _RuleValue ;
        /// <summary>
        /// 
        /// </summary>
        public string RuleValue
        {
            set { _RuleValue = value; }
            get { return _RuleValue; }
        }
        private string _PaddingSide ;
        /// <summary>
        /// 
        /// </summary>
        public string PaddingSide
        {
            set { _PaddingSide = value; }
            get { return _PaddingSide; }
        }
        private int? _PaddingWidth ;
        /// <summary>
        /// 
        /// </summary>
        public int? PaddingWidth
        {
            set { _PaddingWidth = value; }
            get { return _PaddingWidth; }
        }
        private string _PaddingChar ;
        /// <summary>
        /// 
        /// </summary>
        public string PaddingChar
        {
            set { _PaddingChar = value; }
            get { return _PaddingChar; }
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
