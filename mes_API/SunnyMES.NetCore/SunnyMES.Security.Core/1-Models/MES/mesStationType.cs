﻿using System;

namespace SunnyMES.Security._1_Models.MES;


/// <summary>
/// mesStationType
/// </summary>
[Serializable]
public class mesStationType
{
    /// <summary>
    /// 无
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int ApplicationTypeID { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public int? SortCode { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public bool? DeleteMark { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public bool? EnabledMark { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public DateTime? CreatorTime { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public string CreatorUserId { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public DateTime? LastModifyTime { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public string LastModifyUserId { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 无
    /// </summary>
    public string DeleteUserId { get; set; }

}