using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    public enum LangEnum
    {       
        ZH_CN,
        EN
    }

    public enum SSType
    {
        Null,
        FixedString,
        UdfProcedure,
        DateTime,
        Counter
    }


    public enum PType
    {
        Null,
        DateTime,
        Counter
    }



    public enum UnitStatus
    {
        /// <summary>
        /// 这个不使用，为了占位置0
        /// </summary>
        ZeroNotUse,
        /// <summary>
        /// 正常的 Pass
        /// </summary>
        PASS,
        /// <summary>
        /// 不良品
        /// </summary>
        FAIL,
        /// <summary>
        /// 报废
        /// </summary>
        SCRAP,
        /// <summary>
        /// 暂停生产
        /// </summary>
        ONHOLD
    }


}
