using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using StackExchange.Redis;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UploadFileService : BaseService<UploadFile, UploadFileOutputDto, string>, IUploadFileService
    {
        private readonly IUploadFileRepository _uploadFileRepository;
        private readonly ILogService _logService;
        public UploadFileService(IUploadFileRepository repository, ILogService logService) : base(repository)
        {
            _uploadFileRepository = repository;
            _logService = logService;
        }

        /// <summary>
        /// 根据应用Id和应用标识批量更新数据
        /// </summary>
        /// <param name="beLongAppId">应用Id</param>
        /// <param name="oldBeLongAppId">更新前旧的应用Id</param>
        /// <param name="belongApp">应用标识</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool UpdateByBeLongAppId(string beLongAppId, string oldBeLongAppId,string belongApp = null, IDbTransaction trans = null)
        {
           return _uploadFileRepository.UpdateByBeLongAppId(beLongAppId, oldBeLongAppId, belongApp,trans);
        }


    }
}
