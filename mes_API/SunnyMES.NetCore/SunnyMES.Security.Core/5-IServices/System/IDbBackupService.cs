using System;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface IDbBackupService:IService<DbBackup, DbBackupOutputDto, string>
    {
    }
}
