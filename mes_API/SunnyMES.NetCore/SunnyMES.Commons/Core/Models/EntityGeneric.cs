﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SunnyMES.Commons.Models
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class EntityGeneric : IEntity
    {
        /// <summary>
        /// 判断主键是否为空
        /// </summary>
        /// <returns></returns>
        public abstract bool KeyIsNull();

        /// <summary>
        /// 创建默认的主键值
        /// </summary>
        public abstract void GenerateDefaultKeyVal();

        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityGeneric()
        {
            if (KeyIsNull())
            {
                GenerateDefaultKeyVal();
            }
        }
    }
}
