using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;

namespace SunnyMES.Commons.Core.Models
{
    [Serializable]
    public abstract class BaseCustomEntity<TKey> : Entity, IBaseCustomEntity<TKey>
    {
        public BaseCustomEntity()
        {
            
        }
        /// 判断主键是否为空
        /// </summary>
        /// <returns></returns>
        public override bool KeyIsNull()
        {
            return false;
        }

        /// <summary>
        /// 创建默认的主键值
        /// </summary>
        public override void GenerateDefaultKeyVal()
        {
            //if (Id == null)
            //{
            //    Id = GuidUtils.CreateNo().CastTo<TKey>();
            //}
        }
        /// <summary>
        /// 根据传递主键赋值
        /// </summary>
        /// <param name="ID"></param>
        public override void GenerateDefaultKeyVal(int ID)
        {
            
            //if (Id == null)
            //{
            //    Id = GuidUtils.CreateNo().CastTo<TKey>();
            //}
        }
    }
}
