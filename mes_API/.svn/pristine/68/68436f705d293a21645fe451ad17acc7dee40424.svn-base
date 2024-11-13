using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Application;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.WebApi.Areas.Tenants.Controllers
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Route("api/Tenants/[controller]")]
    [ApiController]
    public class UploadFileController : AreaApiController<UploadFile, UploadFileOutputDto, UploadFileInputDto, IUploadFileService, string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        public UploadFileController(IUploadFileService _iService) : base(_iService)
        {
            iService = _iService;
        }


        /// <summary>
        /// 异步批量物理删除
        /// </summary>
        /// <param name="info">主键Id数组</param>
        [HttpDelete("DeleteBatchAsync")]
        [YuebonAuthorize("Delete")]
        public override async Task<IActionResult> DeleteBatchAsync(DeletesInputDto info)
        {
            CommonResult result = new CommonResult();
            string where = string.Empty;
            where = "id in ('" + info.Ids.Join(",").Trim(',').Replace(",", "','") + "')";

            if (!string.IsNullOrEmpty(where))
            {
                dynamic[] jobsId = info.Ids;
                foreach (var item in jobsId)
                {
                    if (string.IsNullOrEmpty(item.ToString())) continue;
                    UploadFile uploadFile = new UploadFileApp().Get(item.ToString());
                    YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                    SysSetting sysSetting = yuebonCacheHelper.Get("SysSetting").ToJson().ToObject<SysSetting>();
                    if (uploadFile != null)
                    {
                        if (System.IO.File.Exists(sysSetting.LocalPath + "/" + uploadFile.FilePath))
                            System.IO.File.Delete(sysSetting.LocalPath + "/" + uploadFile.FilePath);
                        if (!string.IsNullOrEmpty(uploadFile.Thumbnail))
                        {
                            if (System.IO.File.Exists(sysSetting.LocalPath + "/" + uploadFile.Thumbnail))
                                System.IO.File.Delete(sysSetting.LocalPath + "/" + uploadFile.Thumbnail);
                        }
                    }
                }
                bool bl = await iService.DeleteBatchWhereAsync(where).ConfigureAwait(false);
                if (bl)
                {
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                }
                else
                {
                    result.ResultMsg = ErrCode.err43003;
                    result.ResultCode = "43003";
                }
            }
            return ToJsonContent(result);
        }

    }
}