using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig._1_Models.SN;
using SunnyMES.Security.SysConfig._2_Dtos.SN;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Models.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.SN
{
    public interface ISC_SNLockServices : IServiceReport<string>
    {
        Task<PageResult<Sc_Lock_Sn_Search_Dto>> FindWithPagerSearchAsync(SearchSignalInputDto search);
        Task<SC_OperateState> UploadSNLock(SC_Lock_SN_Dto scLockSnDto);
    }
}
