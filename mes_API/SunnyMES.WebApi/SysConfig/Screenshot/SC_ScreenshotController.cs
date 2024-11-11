using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Json;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Security.Repositories;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Services;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Data;
using SunnyMES.Commons.Pages;
using NPOI.POIFS.Properties;
using Microsoft.AspNetCore.Http;
using SunnyMES.Commons.Extensions;
using SunnyMES.Security.Application;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NPOI.HPSF;
using static System.Net.WebRequestMethods;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons;
using NPOI.OpenXmlFormats.Vml;
using System.Web;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  SC_Screenshot 可用接口
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    [NoPermissionRequired]
    [NoSignRequired]
    public class SC_ScreenshotController : AreaApiControllerReport<ISC_ScreenshotService, string>
    {

        private string _filePath;
        private string _dbFilePath;   //数据库中的文件路径
        private string _dbThumbnail;   //数据库中的缩略图路径
        private string _belongApp;//所属应用
        private string _belongAppId;//所属应用ID 
        private string _fileName;//文件名称
        private readonly IWebHostEnvironment _hostingEnvironment;


        FtpWeb FTP;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="hostingEnvironment"></param>
        public SC_ScreenshotController(ISC_ScreenshotService _iService, IWebHostEnvironment hostingEnvironment) : base(_iService)
        {
            iService = _iService;
            _hostingEnvironment = hostingEnvironment;
        }

        private CommonResult Com_Result(CommonResult v_CommonResult, List<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();
            List<dynamic> List_Result = new List<dynamic>();

            v_CommonResult.Sounds = S_Path_OK;

            if (List_Dyn.Count > 0)
            {
                try
                {
                    if (List_Dyn[0][0] is TabVal)
                    {
                        for (int i = 1; i < List_Dyn.Count; i++)
                        {
                            List_Result.Add(List_Dyn[i]);
                        }

                        TabVal v_TabVal = List_Dyn[0][0] as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = List_Result;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                            v_CommonResult.Sounds = v_TabVal.ValStr4;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                    else
                    {
                        List_Result = List_Dyn;
                    }
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Result;
                    v_CommonResult.Sounds = S_Path_NG;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Result;

            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }

        private CommonResult Com_Result(CommonResult v_CommonResult, IEnumerable<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();
            IEnumerable<dynamic> List_Result = List_Dyn;
            v_CommonResult.Sounds = S_Path_OK;

            foreach (var item in List_Dyn)
            {
                try
                {
                    if (item is TabVal)
                    {
                        TabVal v_TabVal = item as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = List_Result;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                            v_CommonResult.Sounds = v_TabVal.ValStr4;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                    //else
                    //{
                    //    List_Result = List_Dyn;
                    //}
                    continue;
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Result;
                    v_CommonResult.Sounds = S_Path_NG;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Result;

            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("GetPageInitialize")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPageInitialize(string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                v_ComResult.Success = true;
                v_ComResult.ResultCode = ErrCode.successCode;
                v_ComResult.ResultMsg = ErrCode.err0;
                v_ComResult.ResData = List_ConfInfo[0].ValStr1;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPageInitialize 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }



        /// <summary>
        ///  截图单文件上传接口
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>服务器存储的文件信息</returns>
        [HttpPost("FileUpload")]
        [NoSignRequired]
        public async Task<IActionResult> FileUpload([FromForm] IFormCollection formCollection)
        {
            CommonResult result = new CommonResult();

            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            string belongApp = formCollection["belongApp"].ToString();
            string belongAppId = formCollection["belongAppId"].ToString();
            _fileName = filelist[0].FileName;
            try
            {
                string S_Rresult=await Add(filelist[0], belongApp, belongAppId);
                if (S_Rresult == "OK")
                {
                    result.ResData = "OK";
                    result.ResultCode = ErrCode.successCode;
                    result.Success = true;
                }
                else 
                {
                    result.ResData = "NG";
                    result.ResultCode = ErrCode.err1;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
                Log4NetHelper.Error("", ex);
                throw ex;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetFtpIP 查看图片ftp地址 
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetFtpIP")]
        [NoSignRequired]
        public IActionResult GetFtpIP() 
        {
            CommonResult result = new CommonResult();
            try
            {
                string S_Rresult = Configs.GetConfigurationValue("AppSetting", "FTPIP");
                if (S_Rresult != "")
                {
                    result.ResData = "ftp://"+ S_Rresult;
                    result.ResultCode = ErrCode.successCode;
                    result.Success = true;
                }
                else
                {
                    result.ResData = "NG";
                    result.ResultCode = ErrCode.err1;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
                Log4NetHelper.Error("", ex);
                throw ex;
            }
            return ToJsonContent(result);

        }

        /// <summary>
        /// GetScreenshotIP
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetScreenshotIP")]
        [NoSignRequired]
        public IActionResult GetScreenshotIP() 
        {
            CommonResult result = new CommonResult();
            try
            {
                string S_ScreenshotFolder = Configs.GetConfigurationValue("AppSetting", "ScreenshotFolder");
                if (S_ScreenshotFolder != "")
                {                    
                    string currentUrl = HttpContext.Request.Host.ToString();
                    string S_URL = currentUrl + "/" + S_ScreenshotFolder;

                    if (S_ScreenshotFolder.Substring(0, 4) == "http") 
                    {
                        S_ScreenshotFolder= S_ScreenshotFolder.Substring(7, S_ScreenshotFolder.Length - 7);
                        S_URL = S_ScreenshotFolder;
                    }

                    result.ResData = S_URL;
                    result.ResultCode = ErrCode.successCode;
                    result.Success = true;
                }
                else
                {
                    result.ResData = "NG";
                    result.ResultCode = ErrCode.err1;
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
                Log4NetHelper.Error("", ex);
                throw ex;
            }
            return ToJsonContent(result);

        }




        /// <summary>
        /// 单个上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="belongApp"></param>
        /// <param name="belongAppId"></param>
        /// <returns></returns>
        private async Task<string> Add(IFormFile file, string belongApp, string belongAppId)
        {
            string S_Result = "OK";

            _belongApp = belongApp;
            _belongAppId = belongAppId;
            if (file != null && file.Length > 0 && file.Length < 10485760)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    try
                    {
                        var fileName = string.Empty;
                        fileName = _fileName;

                        var data = binaryReader.ReadBytes((int)file.Length);
                        //string S_UploadFile = await UploadFile(fileName, data);
                        string S_UploadFile = await UploadFile_FTP(fileName, data);


                        if (S_UploadFile != "OK")
                        {
                            S_Result = S_UploadFile;
                        }
                    }
                    catch (Exception ex) 
                    {
                        S_Result = ex.ToString();
                    }
                }
            }
            else
            {
                S_Result = "File is too large";
                Log4NetHelper.Info("File is too large");
                throw new Exception("File is too large");
            }
            return S_Result;
        }


        /// <summary>
        /// 实现文件上传到服务器保存，并生成缩略图
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileBuffers">文件字节流</param>
        private async Task<string> UploadFile(string fileName, byte[] fileBuffers)
        {
            string S_Rersult = "OK";
            try
            {
                //判断文件是否为空
                if (string.IsNullOrEmpty(fileName))
                {
                    Log4NetHelper.Info("File name cannot be empty");
                    throw new Exception("File name cannot be empty");
                }

                //判断文件是否为空
                if (fileBuffers.Length < 1)
                {
                    Log4NetHelper.Info("File  cannot be empty");
                    throw new Exception("File  cannot be empty");
                }

                YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                SysSetting sysSetting = yuebonCacheHelper.Get("SysSetting").ToJson().ToObject<SysSetting>();

                if (sysSetting == null)
                {
                    sysSetting = XmlConverter.Deserialize<SysSetting>("xmlconfig/sys.config");
                }

                string folder = DateTime.Now.ToString("yyyyMMdd");
                _filePath = _hostingEnvironment.WebRootPath;
                var _tempfilepath = sysSetting.Filepath;

                if (!string.IsNullOrEmpty(_belongApp))
                {
                    _tempfilepath += "/" + _belongApp;
                }
                if (!string.IsNullOrEmpty(_belongAppId))
                {
                    _tempfilepath += "/" + _belongAppId;
                }
                if (sysSetting.Filesave == "1")
                {
                    _tempfilepath = _tempfilepath + "/" + folder + "/";
                }
                if (sysSetting.Filesave == "2")
                {
                    DateTime date = DateTime.Now;
                    _tempfilepath = _tempfilepath + "/" + date.Year + "/" + date.Month + "/" + date.Day + "/";
                }

                var uploadPath = _filePath + "/" + _tempfilepath;
                if (sysSetting.Fileserver == "localhost")
                {
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                }
                string ext = Path.GetExtension(fileName).ToLower();
                string newName = GuidUtils.CreateNo();
                string newfileName = newName + ext;

                using (var fs = new FileStream(uploadPath + newfileName, FileMode.Create))
                {
                    fs.Write(fileBuffers, 0, fileBuffers.Length);
                    fs.Close();

                    SC_ScreenshotDto v_SC_ScreenshotDto = new SC_ScreenshotDto();
                    v_SC_ScreenshotDto.LineID = P_LineID.ToString();
                    v_SC_ScreenshotDto.StationID = P_StationID.ToString();
                    v_SC_ScreenshotDto.PartID = "";
                    v_SC_ScreenshotDto.ProductionOrderID = "";
                    v_SC_ScreenshotDto.IP = P_CurrentLoginIP.ToString();
                    v_SC_ScreenshotDto.PCName = "";

                    //string S1=@"/";
                   // string S2 = @"\";

                    v_SC_ScreenshotDto.IMGURL = (_tempfilepath + newfileName);
                    v_SC_ScreenshotDto.MSG = "";
                    v_SC_ScreenshotDto.Feedback = "";
                    v_SC_ScreenshotDto.IsFeedback = "0";

                    IDbTransaction trans = null;
                    string list = await iService.Insert(v_SC_ScreenshotDto, trans);


                    //生成缩略图
                    if (ext.Contains(".jpg") || ext.Contains(".jpeg") || ext.Contains(".png") || ext.Contains(".bmp") || ext.Contains(".gif"))
                    {
                        string thumbnailName = newName + "_" + sysSetting.Thumbnailwidth + "x" + sysSetting.Thumbnailheight + ext;
                        ImgHelper.MakeThumbnail(uploadPath + newfileName, uploadPath + thumbnailName, sysSetting.Thumbnailwidth.ToInt(), sysSetting.Thumbnailheight.ToInt());
                        _dbThumbnail = _tempfilepath + thumbnailName;
                    }
                    _dbFilePath = _tempfilepath + newfileName;
                }
            }
            catch (Exception ex) 
            {
                S_Rersult=ex.ToString();
            }
            return S_Rersult;
        }


        private async Task<string> UploadFile_FTP(string fileName, byte[] fileBuffers)
        {
            string S_Rersult = "OK";
            try
            {
                //判断文件是否为空
                if (string.IsNullOrEmpty(fileName))
                {
                    Log4NetHelper.Info("File name cannot be empty");
                    throw new Exception("File name cannot be empty");
                }

                //判断文件是否为空
                if (fileBuffers.Length < 1)
                {
                    Log4NetHelper.Info("File  cannot be empty");
                    throw new Exception("File  cannot be empty");
                }

                string S_FTPProject = "";
                if (FTP == null)
                {
                    try
                    {
                        string S_FTPIP = Configs.GetConfigurationValue("AppSetting", "FTPIP");
                        S_FTPProject = Configs.GetConfigurationValue("AppSetting", "FTPProject"); 
                        string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                        string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");


                        FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                    }
                    catch (Exception ex)
                    {
                        Log4NetHelper.Info(ex.Message);
                        throw new Exception(ex.Message);
                    }
                }

                string ext = Path.GetExtension(fileName).ToLower();
                string newName = GuidUtils.CreateNo();
                string newfileName = newName + ext;

                string S_SysPath = Directory.GetCurrentDirectory()+"\\";
                try
                {
                    using (var fs = new FileStream(S_SysPath + "Screenshot.png", FileMode.Create))
                    {
                        fs.Write(fileBuffers, 0, fileBuffers.Length);
                        fs.Close();

                        string S_DateTime = DateTime.Now.ToString("yyyy_MM_dd");
                        FTP.GotoDirectory("", true);
                        if (FTP.DirectoryExist(S_FTPProject) == false)
                        {
                            FTP.MakeDir(S_FTPProject);
                        }

                        if (FTP.DirectoryExist(S_FTPProject+"\\"+S_DateTime) == false)
                        {
                            FTP.MakeDir(S_FTPProject + "\\" + S_DateTime);
                        }
                        FTP.GotoDirectory(S_FTPProject + "\\" + S_DateTime, true);

                        string S_FTP_Path = S_SysPath + "Screenshot.png";
                        string S_FTPURL = FTP.Upload(S_FTP_Path, newfileName);

                        SC_ScreenshotDto v_SC_ScreenshotDto = new SC_ScreenshotDto();
                        v_SC_ScreenshotDto.LineID = P_LineID.ToString();
                        v_SC_ScreenshotDto.StationID = P_StationID.ToString();
                        v_SC_ScreenshotDto.PartID = "";
                        v_SC_ScreenshotDto.ProductionOrderID = "";
                        v_SC_ScreenshotDto.IP = P_CurrentLoginIP.ToString();
                        v_SC_ScreenshotDto.PCName = "";

                        string S_IMGURL = "/"+S_FTPProject + "/" + S_DateTime + "/" + newfileName;

                        v_SC_ScreenshotDto.IMGURL = S_IMGURL;
                        v_SC_ScreenshotDto.MSG = "";
                        v_SC_ScreenshotDto.Feedback = "";
                        v_SC_ScreenshotDto.IsFeedback = "0";

                        IDbTransaction trans = null;
                        string list = await iService.Insert(v_SC_ScreenshotDto, trans);
                    }
                }
                catch (Exception ex)
                {
                    S_Rersult = ex.ToString();
                }

            }
            catch (Exception ex)
            {
                S_Rersult = ex.ToString();
            }
            return S_Rersult;
        }



        ///// <summary>
        ///// Insert
        ///// </summary>
        ///// <param name="v_SC_ScreenshotDto"></param>
        ///// <returns></returns>
        //[HttpPost("Insert")]
        //[YuebonAuthorize("")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Insert(SC_ScreenshotDto v_SC_ScreenshotDto)
        //{
        //    CommonResult v_ComResult = new CommonResult();
        //    try
        //    {
        //        if (P_EmployeeID == 0) { P_EmployeeID = 116; }
        //        if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
        //        IDbTransaction trans = null;
        //        string list = await iService.Insert(v_SC_ScreenshotDto, trans);

        //        if (list == "OK")
        //        {
        //            v_ComResult.Success = true;
        //            v_ComResult.ResultCode = ErrCode.successCode;
        //            v_ComResult.ResultMsg = ErrCode.err0;
        //            v_ComResult.ResData = list;
        //            v_ComResult.Sounds = S_Path_OK;
        //        }
        //        else
        //        {
        //            v_ComResult.Success = false;
        //            v_ComResult.ResultCode = "NG";
        //            v_ComResult.ResultMsg = list;
        //            v_ComResult.ResData = null;
        //            v_ComResult.Sounds = S_Path_NG;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 Insert 异常", ex);
        //        v_ComResult.ResultMsg = ErrCode.err40110;
        //        v_ComResult.ResultCode = "40110";
        //    }
        //    return ToJsonContent(v_ComResult);
        //}



        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string Id)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                if (P_EmployeeID == 0) { P_EmployeeID = 116; }
                if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Delete(Id, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 Delete 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        ///  IsFeedback 0:插入  1:反馈  2:用户查看了反馈
        /// </summary>
        /// <param name="SC_ScreenshotFeedDto"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(SC_ScreenshotFeedDto SC_ScreenshotFeedDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                if (P_EmployeeID == 0) { P_EmployeeID = 116; }
                if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Update(SC_ScreenshotFeedDto, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 Update 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// 异步分页查询,LikeQuery有值时，其他查询参数无效。以模糊查询优先
        /// </summary>
        /// <param name="CurrentPageIndex">当前页</param>
        /// <param name="PageSize">页行数</param>
        /// <param name="LikeQuery">模糊查询字段 文本类型 </param>   
        /// <param name="ID"></param>
        /// <param name="LineID"></param>
        /// <param name="Line"></param>
        /// <param name="StationID"></param>
        /// <param name="Station"></param>
        /// <param name="PartID"></param>
        /// <param name="Part"></param>
        /// <param name="ProductionOrderID"></param>
        /// <param name="ProductionOrder"></param>
        /// <param name="IP"></param>
        /// <param name="PCName"></param>
        /// <param name="IsFeedback"></param>
        /// <param name="CreateTimeStart"></param>
        /// <param name="CreateTimeEnd"></param>
        /// <returns></returns>
        [HttpGet("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> FindWithPagerSearchAsync
            (
                int CurrentPageIndex,
                int PageSize,
                string LikeQuery,

                string ID,
                string LineID,
                string Line,
                string StationID,
                string Station,
                string PartID,
                string Part,
                string ProductionOrderID,
                string ProductionOrder,
                string IP,
                string PCName,
                string IsFeedback,
                DateTime CreateTimeStart,
                DateTime CreateTimeEnd
            )
        {

            CommonResult<PageResult<SC_ScreenshotDto>> v_ComResult = new CommonResult<PageResult<SC_ScreenshotDto>>();
            try
            {
                SC_mesScreenshotSearch search = new SC_mesScreenshotSearch();

                search.CurrentPageIndex = CurrentPageIndex;
                search.PageSize = PageSize;
                search.LikeQuery = LikeQuery;

                search.ID = ID;
                search.LineID = LineID;
                search.Line = Line;
                search.StationID = StationID;
                search.Station = Station;
                search.PartID = PartID;
                search.Part = Part;

                search.ProductionOrderID = ProductionOrderID;
                search.ProductionOrder = ProductionOrder;
                search.IP = IP;
                search.PCName = PCName;
                search.IsFeedback = IsFeedback;
                search.CreateTimeStart = CreateTimeStart;
                search.CreateTimeEnd = CreateTimeEnd;

                PageResult<SC_ScreenshotDto> list = await iService.FindWithPagerMyAsync(search);

                v_ComResult.Success = true;
                v_ComResult.ResultCode = ErrCode.successCode;
                v_ComResult.ResultMsg = ErrCode.err0;
                v_ComResult.ResData = list;
                v_ComResult.Sounds = S_Path_OK;
            }
            catch (Exception ex)
            {
                v_ComResult.Success = false;
                v_ComResult.ResultCode = "NG";
                v_ComResult.ResultMsg = ex.Message;
                v_ComResult.ResData = null;
                v_ComResult.Sounds = S_Path_NG;

                Log4NetHelper.Error("获取 Screenshot FindWithPagerSearchAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);

        }




    }
}

