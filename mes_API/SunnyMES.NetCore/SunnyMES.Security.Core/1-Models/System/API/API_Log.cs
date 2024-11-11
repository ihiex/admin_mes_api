
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
    [Table("API_Log")]
    [Serializable]
    public class API_Log
    { 
        public API_Log() { }

        public virtual string Id { get; set; }

        public virtual DateTime? Date { get; set; }

        public virtual string Account { get; set; }

        public virtual string NickName { get; set; }

        public virtual string OrganizeId { get; set; }

        public virtual string Type { get; set; }

        public virtual string IPAddress { get; set; }

        public virtual string IPAddressName { get; set; }

        public virtual string ModuleId { get; set; }

        public virtual string ModuleName { get; set; }

        public virtual bool? Result { get; set; }

        public virtual string Description { get; set; }

        public virtual bool? DeleteMark { get; set; }

        public virtual bool? EnabledMark { get; set; }

        public virtual DateTime? CreatorTime { get; set; }

        public virtual string CreatorUserId { get; set; }

        public virtual DateTime? LastModifyTime { get; set; }

        public virtual string LastModifyUserId { get; set; }

        public virtual DateTime? DeleteTime { get; set; }

        public virtual string DeleteUserId { get; set; }


        public virtual int LineID { get; set; }

        public virtual int StationID { get; set; }

        public virtual int LanguageID { get; set; }


    }
}