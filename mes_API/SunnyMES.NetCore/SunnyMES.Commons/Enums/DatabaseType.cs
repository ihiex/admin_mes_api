﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SunnyMES.Commons.Enums
{

    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SqlServer数据库
        /// </summary>
        SqlServer = 0,
        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle,
        /// <summary>
        /// Access数据库
        /// </summary>
        Access,
        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,
        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,
        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// Npgsql数据库
        /// </summary>
        Npgsql
    }
}
