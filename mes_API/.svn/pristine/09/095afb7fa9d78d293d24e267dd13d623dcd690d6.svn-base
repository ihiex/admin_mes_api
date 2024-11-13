using System;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class DbBackupService: BaseService<DbBackup, DbBackupOutputDto, string>, IDbBackupService
    {
        private readonly IDbBackupRepository _repository;
        private readonly ILogService _logService;
        public DbBackupService(IDbBackupRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }
    }
}