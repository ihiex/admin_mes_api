using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Mapping;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Dtos;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(User))]
    [Serializable]
    public class UserFocusExtendOutPutDto : IOutputDto
    {

        #region Property Members

        /// <summary>
        /// 用户主键
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 关注的用户ID
        /// </summary>
        public virtual string FocusUserId { get; set; }

        /// <summary>
        /// 关注人
        /// </summary>
        public virtual string CreatorUserId { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        public virtual DateTime? CreatorTime { get; set; }

        /// <summary>
        /// 关注的用户昵称
        /// </summary>
        public virtual string FUserNickName { get; set; }

        /// <summary>
        /// 关注的用户头像
        /// </summary>
        public virtual string FUserHeadIcon { get; set; }

        /// <summary>
        /// 关注的用户手机
        /// </summary>
        public virtual string FUserMobilePhone { get; set; }

        /// <summary>
        /// 关注的用户资料开放程序
        /// </summary>
        public virtual string FUserOpenType { get; set; }

        /// <summary>
        /// 记录数
        /// </summary>
        public virtual int RecordCount { get; set; }


        /// <summary>
        /// 关注时间
        /// </summary>
        public virtual string ShowAddTime { get; set; }
        #endregion
    }
}
