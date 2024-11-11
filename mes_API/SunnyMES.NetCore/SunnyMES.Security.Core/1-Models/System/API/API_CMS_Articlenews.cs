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
    [Table("API_CMS_Articlenews")]
    [Serializable]
    public partial class API_CMS_Articlenews
    {
        public API_CMS_Articlenews()
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
        private string _Title ;
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set { _Title = value; }
            get { return _Title; }
        }
        private string _CategoryId ;
        /// <summary>
        /// 
        /// </summary>
        public string CategoryId
        {
            set { _CategoryId = value; }
            get { return _CategoryId; }
        }
        private string _CategoryName ;
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName
        {
            set { _CategoryName = value; }
            get { return _CategoryName; }
        }
        private string _SubTitle ;
        /// <summary>
        /// 
        /// </summary>
        public string SubTitle
        {
            set { _SubTitle = value; }
            get { return _SubTitle; }
        }
        private string _LinkUrl ;
        /// <summary>
        /// 
        /// </summary>
        public string LinkUrl
        {
            set { _LinkUrl = value; }
            get { return _LinkUrl; }
        }
        private string _ImgUrl ;
        /// <summary>
        /// 
        /// </summary>
        public string ImgUrl
        {
            set { _ImgUrl = value; }
            get { return _ImgUrl; }
        }
        private string _SeoTitle ;
        /// <summary>
        /// 
        /// </summary>
        public string SeoTitle
        {
            set { _SeoTitle = value; }
            get { return _SeoTitle; }
        }
        private string _SeoKeywords ;
        /// <summary>
        /// 
        /// </summary>
        public string SeoKeywords
        {
            set { _SeoKeywords = value; }
            get { return _SeoKeywords; }
        }
        private string _SeoDescription ;
        /// <summary>
        /// 
        /// </summary>
        public string SeoDescription
        {
            set { _SeoDescription = value; }
            get { return _SeoDescription; }
        }
        private string _Tags ;
        /// <summary>
        /// 
        /// </summary>
        public string Tags
        {
            set { _Tags = value; }
            get { return _Tags; }
        }
        private string _Zhaiyao ;
        /// <summary>
        /// 
        /// </summary>
        public string Zhaiyao
        {
            set { _Zhaiyao = value; }
            get { return _Zhaiyao; }
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
        private bool _IsMsg ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMsg
        {
            set { _IsMsg = value; }
            get { return _IsMsg; }
        }
        private bool _IsTop ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsTop
        {
            set { _IsTop = value; }
            get { return _IsTop; }
        }
        private bool _IsRed ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRed
        {
            set { _IsRed = value; }
            get { return _IsRed; }
        }
        private bool _IsHot ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsHot
        {
            set { _IsHot = value; }
            get { return _IsHot; }
        }
        private bool _IsSys ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSys
        {
            set { _IsSys = value; }
            get { return _IsSys; }
        }
        private bool _IsNew ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsNew
        {
            set { _IsNew = value; }
            get { return _IsNew; }
        }
        private bool _IsSlide ;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSlide
        {
            set { _IsSlide = value; }
            get { return _IsSlide; }
        }
        private int? _Click ;
        /// <summary>
        /// 
        /// </summary>
        public int? Click
        {
            set { _Click = value; }
            get { return _Click; }
        }
        private int? _LikeCount ;
        /// <summary>
        /// 
        /// </summary>
        public int? LikeCount
        {
            set { _LikeCount = value; }
            get { return _LikeCount; }
        }
        private int? _TotalBrowse ;
        /// <summary>
        /// 
        /// </summary>
        public int? TotalBrowse
        {
            set { _TotalBrowse = value; }
            get { return _TotalBrowse; }
        }
        private string _Source ;
        /// <summary>
        /// 
        /// </summary>
        public string Source
        {
            set { _Source = value; }
            get { return _Source; }
        }
        private string _Author ;
        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            set { _Author = value; }
            get { return _Author; }
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
