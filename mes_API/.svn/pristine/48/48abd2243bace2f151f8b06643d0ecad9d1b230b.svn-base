using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Application;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;

namespace SunnyMES.WebApi.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [NoPermissionRequired]
    [NoSignRequired]
    public class FilesController : ApiController
    {

        private string _filePath;
        private string _dbFilePath;   //数据库中的文件路径
        private string _dbThumbnail;   //数据库中的缩略图路径
        private string _belongApp;//所属应用
        private string _belongAppId;//所属应用ID 
        private string _fileName;//文件名称
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FilesController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        ///  Model 上传
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>服务器存储的文件信息</returns>
        [HttpPost("UploadModel")]
        [NoPermissionRequired]
        [NoSignRequired]
        [AllowAnonymous]
        public async Task<IActionResult> UploadModel([FromForm] IFormCollection formCollection)
        {
            CommonResult result = new CommonResult();

            //FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            //string tableName = "";
            //List<string> requireCheck = new List<string>();

            //_fileName = filelist[0].FileName;
            //var fileType = _fileName.Substring(_fileName.LastIndexOf('.') + 1);
            //DataTable dataTable;

            //using (var ms = new MemoryStream())
            //{
            //    await filelist[0].CopyToAsync(ms);
            //    ms.Seek(0, SeekOrigin.Begin);

            //    if (false)
            //    {
            //        #region
            //        //适合单步作战，不适合群体作战, 可直接转成model
            //        if (fileType == "csv")
            //        {
            //            var tmpCSVM = CSVHelper.CSVToDataTableByStreamReader<mesUnit>(ms);
            //            return ToJsonContent(tmpCSVM);
            //        }
            //        else
            //        {
            //            //返回的为动态类型
            //            var enitys = NPOIHelper.StreamToModel<mesUnit>(ms, fileType);
            //            var tmpEn = NPOIHelper.StreamToModel(ms, fileType, "mesUnit");
            //            return ToJsonContent(tmpEn);
            //        }
            //        #endregion
            //    }
            //    else
            //    {
            //        //datatable
            //        #region using datatable
            //        if (fileType == "csv")
            //        {
            //            dataTable = CSVHelper.CSVToDataTableByStreamReader(ms);
            //        }
            //        else
            //        {
            //            dataTable = NPOIHelper.StreamToTable(ms, fileType);
            //        }
            //        var jsonStr = JsonConvert.SerializeObject(dataTable);
            //        var unitss = JsonConvert.DeserializeObject<List<mesUnit>>(jsonStr);
            //        //var units = DataTableHelper.DataTableToList<mesUnit>(dataTable);






            //        #endregion
            //    }
            //}


            //try
            //{

            //    result.ResData = Add(filelist[0], belongApp, belongAppId);
            //    result.ResultCode = ErrCode.successCode;
            //    result.Success = true;
            //}
            //catch (Exception ex)
            //{
            //    result.ResultCode = "500";
            //    result.ResultMsg = ex.Message;
            //    Log4NetHelper.Error("", ex);
            //    throw ex;
            //}
            return ToJsonContent(result);
        }

        /// <summary>
        ///  单文件上传接口
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>服务器存储的文件信息</returns>
        [HttpPost("Upload")]
        [NoSignRequired]
        public IActionResult Upload([FromForm] IFormCollection formCollection)
        {
            CommonResult result = new CommonResult();

            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            string belongApp = formCollection["belongApp"].ToString();
            string belongAppId = formCollection["belongAppId"].ToString();
            _fileName = filelist[0].FileName;
            try
            {
                result.ResData = Add(filelist[0], belongApp, belongAppId);
                result.ResultCode = ErrCode.successCode;
                result.Success = true;
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
        ///  批量上传文件接口
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>服务器存储的文件信息</returns>
        [HttpPost("Uploads")]
        public IActionResult  Uploads([FromForm] IFormCollection formCollection)
        {
            CommonResult result = new CommonResult();
            FormFileCollection filelist = (FormFileCollection)formCollection.Files;
            string belongApp = formCollection["belongApp"].ToString();
            string belongAppId = formCollection["belongAppId"].ToString();
            try
            {
               result.ResData = Adds(filelist, belongApp, belongAppId);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("", ex);
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
            }

            return ToJsonContent(result);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteFile")]
        public IActionResult DeleteFile(string id)
        {
            CommonResult result = new CommonResult();
            try
            {
                UploadFile uploadFile = new UploadFileApp().Get(id);

                YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                SysSetting sysSetting = yuebonCacheHelper.Get("SysSetting").ToJson().ToObject<SysSetting>();
                string localpath = _hostingEnvironment.WebRootPath;
                if (uploadFile != null)
                {
                    string filepath = (localpath + "/" + uploadFile.FilePath).ToFilePath();
                    if (System.IO.File.Exists(filepath))
                        System.IO.File.Delete(filepath);
                    string filepathThu = (localpath + "/" + uploadFile.Thumbnail).ToFilePath();
                    if (System.IO.File.Exists(filepathThu))
                        System.IO.File.Delete(filepathThu);

                    result.ResultCode = ErrCode.successCode;
                    result.Success = true;
                }
                else
                {
                    result.ResultCode = ErrCode.failCode;
                    result.Success = false;
                }

            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("", ex);
                result.ResultCode = "500";
                result.ResultMsg = ex.Message;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="belongApp">所属应用，如文章article</param>
        /// <param name="belongAppId">所属应用ID，如文章id</param>
        /// <returns></returns>
        private List<UploadFileResultOuputDto> Adds(IFormFileCollection files, string belongApp, string belongAppId)
        {
            List<UploadFileResultOuputDto> result = new List<UploadFileResultOuputDto>();
            foreach (var file in files)
            {
                if (file != null)
                {
                    result.Add(Add(file, belongApp, belongAppId));
                }
            }
            return result;
        }
        /// <summary>
        /// 单个上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="belongApp"></param>
        /// <param name="belongAppId"></param>
        /// <returns></returns>
        private  UploadFileResultOuputDto Add(IFormFile file, string belongApp, string belongAppId)
        {
            _belongApp = belongApp;
            _belongAppId = belongAppId;
            if (file != null && file.Length > 0 && file.Length < 10485760)
            {
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    var fileName = string.Empty;
                        fileName = _fileName;
                    
                    var data = binaryReader.ReadBytes((int)file.Length);
                    UploadFile(fileName, data);

                    UploadFile filedb = new UploadFile
                    {
                        Id = GuidUtils.CreateNo(),
                        FilePath = _dbFilePath,
                        Thumbnail = _dbThumbnail,
                        FileName = fileName,
                        FileSize = file.Length.ToInt(),
                        FileType = Path.GetExtension(fileName),
                        Extension = Path.GetExtension(fileName),
                        BelongApp = _belongApp,
                        BelongAppId = _belongAppId
                    };
                    new UploadFileApp().InsertAsync(filedb);
                    //var v = SqlSugarHelper.Db.Insertable<UploadFile>(filedb);

                    UploadFileResultOuputDto uploadFileResultOuputDto = filedb.MapTo<UploadFileResultOuputDto>();
                    uploadFileResultOuputDto.PhysicsFilePath = (_hostingEnvironment.WebRootPath + "/"+ _dbThumbnail).ToFilePath(); ;
                    return uploadFileResultOuputDto;
                }
            }
            else
            {
                Log4NetHelper.Info("文件过大");
                throw new Exception("文件过大");
            }
        }
        /// <summary>
        /// 实现文件上传到服务器保存，并生成缩略图
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="fileBuffers">文件字节流</param>
        private void UploadFile(string fileName, byte[] fileBuffers)
        {

            //判断文件是否为空
            if (string.IsNullOrEmpty(fileName))
            {
                Log4NetHelper.Info("文件名不能为空");
                throw new Exception("文件名不能为空");
            }

            //判断文件是否为空
            if (fileBuffers.Length < 1)
            {
                Log4NetHelper.Info("文件不能为空");
                throw new Exception("文件不能为空");
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
                _tempfilepath += "/"+_belongApp;
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

            var uploadPath = _filePath +"/"+ _tempfilepath;
            if (sysSetting.Fileserver == "localhost")
            {
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
            }
            string ext = Path.GetExtension(fileName).ToLower();
            string newName = GuidUtils.CreateNo();
            string newfileName= newName + ext;

            using (var fs = new FileStream(uploadPath + newfileName, FileMode.Create))
            {
                fs.Write(fileBuffers, 0, fileBuffers.Length);
                fs.Close();
                //生成缩略图
                if (ext.Contains(".jpg") || ext.Contains(".jpeg") || ext.Contains(".png") || ext.Contains(".bmp") || ext.Contains(".gif"))
                {
                    string thumbnailName = newName + "_" + sysSetting.Thumbnailwidth + "x" + sysSetting.Thumbnailheight + ext;
                    ImgHelper.MakeThumbnail(uploadPath + newfileName, uploadPath + thumbnailName, sysSetting.Thumbnailwidth.ToInt(), sysSetting.Thumbnailheight.ToInt());
                    _dbThumbnail = _tempfilepath +  thumbnailName;
                }
                _dbFilePath = _tempfilepath + newfileName;
            }
        }



    }
}
