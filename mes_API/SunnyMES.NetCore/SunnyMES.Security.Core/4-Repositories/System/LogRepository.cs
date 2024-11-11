using Dapper;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    /// <summary>
    /// ��־�ִ�ʵ��
    /// </summary>
    public class LogRepository : BaseRepository<Models.Log, long>, ILogRepository
    {
        /// <summary>
        /// 
        /// </summary>
        public LogRepository()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public LogRepository(IDbContextCore dbContext) : base(dbContext)
        {

        }
        ///// <summary>
        ///// �������ܣ�����ɾ��
        ///// </summary>
        ///// <param name="len"></param>
        ///// <returns></returns>
        //public long InsertTset(int len)
        //{
           
        //    int n = 0;

        //    var sb = new StringBuilder(" EF ��Dapper���÷������ܷ��� �� \n");
        //    Log logEntity1 = new Log()
        //    {
        //        Id = GuidUtils.IdGenerator(),
        //        Date = DateTime.Now,
        //        Account = "admin",
        //        NickName = "��������Ա",
        //        OrganizeId = "2020101619392209546893",
        //        Type = "SQL",
        //        IPAddress = "171.110.40.191",
        //        IPAddressName = "�й�����׳��������������",
        //        ModuleName = "Log",
        //        Result = true,
        //        Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //        DeleteMark = false,
        //        EnabledMark = true,
        //        CreatorTime = DateTime.Now,
        //        CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //    };
        //    using (var content=new MySqlDbContext())
        //    {
        //        content.Add<Log>(logEntity1); ;
        //    }
        //        Log logEntity2 = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //    Stopwatch stopwatch = new Stopwatch();
        //    Stopwatch stopwatch1 = new Stopwatch();
        //    Stopwatch stopwatch2 = new Stopwatch();
        //    Stopwatch stopwatch3 = new Stopwatch();
        //    Stopwatch stopwatch4 = new Stopwatch();
        //    Stopwatch stopwatch5 = new Stopwatch();
        //    Stopwatch stopwatch6 = new Stopwatch();
        //    Stopwatch stopwatch7 = new Stopwatch();
        //    Stopwatch stopwatch8 = new Stopwatch();
        //    Stopwatch stopwatch9 = new Stopwatch();
        //    Stopwatch stopwatch10 = new Stopwatch();
        //    Stopwatch stopwatch11 = new Stopwatch();
        //    Stopwatch stopwatch12 = new Stopwatch();
        //    Stopwatch stopwatch13 = new Stopwatch();
        //    Stopwatch stopwatch14 = new Stopwatch();

        //    stopwatch.Start();

        //    //DapperConn.Insert<Log>(logEntity1);
        //    stopwatch.Stop();
        //    sb.Append("Dapper �������ݲ���Insert��ʱ:" + (stopwatch.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch1.Start();
        //   Add(logEntity2);
        //    stopwatch1.Stop();
        //    sb.Append("EF  �������ݲ���Add��ʱ:" + (stopwatch1.ElapsedMilliseconds + "  ����\n"));

        //    logEntity1.DeleteMark = false;
        //    logEntity1.LastModifyTime = DateTime.Now;

        //    logEntity2.DeleteMark = false;
        //    logEntity2.LastModifyTime = DateTime.Now;
        //    stopwatch2.Start();
        //    Update(logEntity1);
        //    stopwatch2.Stop();
        //    sb.Append("Dapper  �������ݸ���Update��ʱ:" + (stopwatch2.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch3.Start();
        //    Edit(logEntity2);
        //    stopwatch3.Stop();
        //    sb.Append("EF �������ݸ���Edit��ʱ:" + (stopwatch3.ElapsedMilliseconds + "  ����\n"));

        //    List<Log> logList = new List<Log>();

        //    List<Log> logList2 = new List<Log>();
        //    List<Log> logList3 = new List<Log>();
        //    List<Log> logList4 = new List<Log>();
        //    List<Log> logList5 = new List<Log>();
        //    while (n < len.ToInt())
        //    {
        //        Log logEntity = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //        logList.Add(logEntity);
        //        Log logEntity3 = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //        logList2.Add(logEntity3);
        //        Log logEntity4 = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //        logList3.Add(logEntity4);
        //        Log logEntity5 = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //        logList4.Add(logEntity5);
        //        Log logEntity6 = new Log()
        //        {
        //            Id = GuidUtils.IdGenerator(),
        //            Date = DateTime.Now,
        //            Account = "admin",
        //            NickName = "��������Ա",
        //            OrganizeId = "2020101619392209546893",
        //            Type = "SQL",
        //            IPAddress = "171.110.40.191",
        //            IPAddressName = "�й�����׳��������������",
        //            ModuleName = "Log",
        //            Result = true,
        //            Description = "SQL���:update API_Role set EnabledMark=1 ,LastModifyUserId='2020100517554098226223',LastModifyTime=@LastModifyTime where id in ('2019091721053342871332')",
        //            DeleteMark = false,
        //            EnabledMark = true,
        //            CreatorTime = DateTime.Now,
        //            CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba"
        //        };
        //        logList5.Add(logEntity6);
        //        n++;
        //    }

        //    string sql = "insert into Log ([Date], [Account], [NickName], [OrganizeId], [Type], [IPAddress], [IPAddressName], [ModuleId], [ModuleName], [Result], [Description], [DeleteMark], [EnabledMark], [CreatorTime], [CreatorUserId], [LastModifyTime], [LastModifyUserId], [DeleteTime], [DeleteUserId], [Id]) values (@Date, @Account, @NickName, @OrganizeId, @Type, @IPAddress, @IPAddressName, @ModuleId, @ModuleName, @Result, @Description, @DeleteMark, @EnabledMark, @CreatorTime, @CreatorUserId, @LastModifyTime, @LastModifyUserId, @DeleteTime, @DeleteUserId, @Id)";

        //    stopwatch4.Start();
        //    DbContext.BulkInsert<Log>(logList);
        //    stopwatch4.Stop();
        //    sb.Append("ʹ��BulkInsert��������"+n+"�����ݺ�ʱ:" + (stopwatch4.ElapsedMilliseconds + "  ����\n"));

        //    stopwatch5.Start();
        //    DbContext.AddRange<Log>(logList3);
        //    stopwatch5.Stop();
        //    sb.Append("EF ��������" + n + "������AddRange��ʱ:" + (stopwatch5.ElapsedMilliseconds + "  ����\n"));
           
        //        //stopwatch6.Start();
        //        //conn.Insert(logList4);
        //        //stopwatch6.Stop();
        //        //sb.Append("Dapper ��������" + n + "������Insert������ʱ:" + (stopwatch6.ElapsedMilliseconds + "  ����\n"));

        //        stopwatch7.Start();
        //        DapperConn.Execute(sql, logList5);
        //        stopwatch7.Stop();
        //        sb.Append("Dapper ��������" + n + "������ExecuteAsync��ʱ:" + (stopwatch7.ElapsedMilliseconds + "  ����\n"));
           
        //    List<Log> newlogList = new List<Log>();
        //    foreach(Log item in logList)
        //    {
        //        Log info = new Log();
        //        info = item;
        //        info.LastModifyTime = DateTime.Now;
        //        info.Description += item.Description + item.Description +"��������";
        //        newlogList.Add(info);
        //    }
        //    Stopwatch stopwatch15 = new Stopwatch();
        //    stopwatch15.Start();
        //   // Update(newlogList);
        //    stopwatch15.Stop();
        //    sb.Append("Dapper��������" + n + "������Update��ʱ:" + (stopwatch15.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch8.Start();
        //    DbContext.EditRange<Log>(newlogList);
        //    stopwatch8.Stop();
        //    sb.Append("EF ��������" + n + "������EditRange��ʱ:" + (stopwatch8.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch9.Start();
        //    Get(logEntity1.Id);
        //    stopwatch9.Stop();
        //    sb.Append("Dapper ��ѯ����ʵ��Get��ʱ:" + (stopwatch9.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch10.Start();
        //    DbContext.GetDbSet<Log>().Find(logEntity1.Id);
        //    stopwatch10.Stop();
        //    sb.Append("Ef��ѯ����ʵ��Find��ʱ:" + (stopwatch10.ElapsedMilliseconds + "  ����\n"));
        //    sql = $"select * FROM Log where Id='" + logEntity1.Id + "'";
        //    stopwatch11.Start();
        //    DbContext.GetDbSet<Log>().FromSqlRaw<Log>(sql).FirstOrDefaultAsync<Log>();
        //    stopwatch11.Stop();
        //    sb.Append("Ef��ѯ����ʵ��FromSqlRaw FirstOrDefaultAsync��ʱ:" + (stopwatch11.ElapsedMilliseconds + "  ����\n"));
        //    PagerInfo pagerInfo = new PagerInfo
        //    {
        //        CurrentPageIndex = 1,
        //        PageSize = 50
        //    };
        //    stopwatch12.Start();
        //    List<Log> list = FindWithPager("", pagerInfo, "CreatorTime", true);
        //    PageResult<LogOutputDto> pageResult = new PageResult<LogOutputDto>
        //    {
        //        CurrentPage = pagerInfo.CurrentPageIndex,
        //        Items = list.MapTo<LogOutputDto>(),
        //        ItemsPerPage = pagerInfo.PageSize,
        //        TotalItems = pagerInfo.RecordCount
        //    };
        //    stopwatch12.Stop();
        //    sb.Append("Dapper ��ҳ��ѯ��ʱ:" + (stopwatch12.ElapsedMilliseconds + "  ����\n"));
        //    stopwatch13.Start();

        //    List<Log> list2 = GetByPagination(m => true, pagerInfo, true).ToList<Log>();
        //    PageResult<LogOutputDto> pageResult2 = new PageResult<LogOutputDto>
        //    {
        //        CurrentPage = pagerInfo.CurrentPageIndex,
        //        Items = list2.MapTo<LogOutputDto>(),
        //        ItemsPerPage = pagerInfo.PageSize,
        //        TotalItems = pagerInfo.RecordCount
        //    };
        //    stopwatch13.Stop();
        //    sb.Append("EF ��ҳ��ѯ��ʱ:" + (stopwatch13.ElapsedMilliseconds + "  ����\n")); stopwatch.Start();

        //    sql = $"select * FROM Log";
        //    string[] orderBys = { "CreatorTime desc" };
        //    stopwatch14.Start();
        //    PageResult<Log> list3 =DbContext.SqlQueryByPagination<Log>(sql, orderBys, 1,50);
        //    //PageResult<LogOutputDto> pageResult2 = new PageResult<LogOutputDto>
        //    //{
        //    //    CurrentPage = pagerInfo.CurrentPageIndex,
        //    //    Items = list2.MapTo<LogOutputDto>(),
        //    //    ItemsPerPage = pagerInfo.PageSize,
        //    //    TotalItems = pagerInfo.RecordCount
        //    //};
        //    stopwatch14.Stop();
        //    sb.Append("EF ��ҳ��ѯ��ʱ:" + (stopwatch14.ElapsedMilliseconds + "  ����\n"));
        //    Log4NetHelper.Info(sb.ToString());
        //    return 1;
        //}

        /// <summary>
        /// ����ɾ����Ϣ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Delete(Int64 id, IDbTransaction trans = null)
        {
            int row = DapperConn.Execute($"delete from {tableName} where Id=@id", new { @id = id }, trans);
            return row > 0 ? true : false;
        }


        public async Task<List<API_Log>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_Log> list = new List<API_Log>();

            if (HasInjectionData(condition))
            {
                Log4NetHelper.Info(string.Format("����SQLע��Ķ�������, {0}", condition));
                throw new Exception("����SQLע��Ķ�������");
            }
            if (string.IsNullOrEmpty(condition))
            {
                condition = "1=1";
            }
            if (desc)
            {
                fieldToSort += " desc";
            }
            RefAsync<int> totalCount = 0;

            list = await SuDB.Queryable<API_Log>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }


    }
}