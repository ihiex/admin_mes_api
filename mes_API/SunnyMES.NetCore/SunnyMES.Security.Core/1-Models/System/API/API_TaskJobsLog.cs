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
    [Table("API_TaskJobsLog")]
    [Serializable]
    public partial class API_TaskJobsLog
    {
        public API_TaskJobsLog()
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
        private string _TaskId ;
        /// <summary>
        /// 
        /// </summary>
        public string TaskId
        {
            set { _TaskId = value; }
            get { return _TaskId; }
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
        private string _JobAction ;
        /// <summary>
        /// 
        /// </summary>
        public string JobAction
        {
            set { _JobAction = value; }
            get { return _JobAction; }
        }
        private bool _Status ;
        /// <summary>
        /// 
        /// </summary>
        public bool Status
        {
            set { _Status = value; }
            get { return _Status; }
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
    }
}
