using System;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface IDbBackupRepository:IRepository<DbBackup, string>
    {
    }
}