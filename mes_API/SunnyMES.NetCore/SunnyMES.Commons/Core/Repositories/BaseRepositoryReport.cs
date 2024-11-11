using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dapper;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.DataManager;
using SunnyMES.Commons.DependencyInjection;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using Dapper.Contrib.Extensions;
using SunnyMES.Commons.Cache;
using System.Web;
using System.Data.SqlClient;

namespace SunnyMES.Commons.Repositories
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>    
    /// <typeparam name="TKey">实体主键类型</typeparam>
    public abstract class BaseRepositoryReport<TKey> : IRepositoryReport<TKey>, ITransientDependency         
    {
        /// <summary>
        /// 数据库超时(1分)  60000 毫秒
        /// </summary>
        public int I_DBTimeout = 60000;
        /// <summary>
        /// 时间日期格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string S_DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 字符串密钥 123456
        /// </summary>
        public string S_PwdKey = "123456";

        /// <summary>
        /// 是否检查料号
        /// </summary>
        public string S_IsCheckPN = "1";
        /// <summary>
        /// 是否检查工单
        /// </summary>
        public string S_IsCheckPO = "1";
        /// <summary>
        /// 程序类别
        /// </summary>
        public string S_ApplicationType = "";
        /// <summary>
        /// 是否合法的页面
        /// </summary>
        public string S_IsLegalPage = "0";
        /// <summary>
        /// TT 扫描类型 //扫码类型 1:条码SN 2:BOX 3:MachineBox (未配置默认1) 
        /// 4:混合（前3种类型都可以扫描）当输入为1类型时，且条码被绑定在2或3中，这时对SN做解除动作。
        /// </summary>
        public string S_TTScanType = "1";
        /// <summary>
        /// IsTTRegistSN
        /// </summary>
        public string S_IsTTRegistSN= "0";
        /// <summary>
        /// TTBoxType
        /// </summary>
        public string S_TTBoxType = "1";
        /// <summary>
        /// IsChangePN
        /// </summary>
        public string S_IsChangePN="0";
        /// <summary>
        /// IsChangePO
        /// </summary>
        public string S_IsChangePO="0";
        /// <summary>
        /// ChangedUnitStateID
        /// </summary>
        public string S_ChangedUnitStateID="0";

        /// <summary>
        /// JumpFromUnitStateID
        /// </summary>
        public string S_JumpFromUnitStateID="";
        /// <summary>
        /// JumpToUnitStateID
        /// </summary>
        public string S_JumpToUnitStateID="";
        /// <summary>
        /// JumpStatusID
        /// </summary>
        public string S_JumpStatusID="";
        /// <summary>
        /// JumpUnitStateID
        /// </summary>
        public string S_JumpUnitStateID = "";
        /// <summary>
        /// OperationType
        /// </summary>
        public string S_OperationType = "";
        /// <summary>
        /// IsCheckCardID
        /// </summary>
        public string S_IsCheckCardID = "0";
        /// <summary>
        /// CardIDPattern
        /// </summary>
        public string S_CardIDPattern = "[\\s\\S]*";




        #region 构造函数及基本配置
        /// <summary>
        ///  EF DBContext
        /// </summary>
        public IDbContextCore _dbContext;
        private IDbContextFactory _dbContextFactory;

        
        /// <summary>
        /// 获取访问数据库配置
        /// </summary>
        protected DbConnectionOptions dbConnectionOptions = DBServerProvider.GeDbConnectionOptions2();

        /// <summary>
        /// 数据库参数化访问的占位符
        /// </summary>
        protected string parameterPrefix = "@";
        /// <summary>
        /// 防止和保留字、关键字同名的字段格式，如[value]
        /// </summary>
        protected string safeFieldFormat = "[{0}]";
        /// <summary>
        /// 数据库的主键字段名,若主键不是Id请重载BaseRepository设置
        /// </summary>
        protected string primaryKey = "Id";
        /// <summary>
        /// 排序字段
        /// </summary>
        protected string sortField;
        /// <summary>
        /// 是否为降序
        /// </summary>
        protected bool isDescending = true;
        /// <summary>
        /// 选择的字段，默认为所有(*) 
        /// </summary>
        protected string selectedFields = " * ";
        /// <summary>
        /// 是否开启多租户
        /// </summary>
        protected bool isMultiTenant = false;


        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField
        {
            get
            {
                return sortField;
            }
            set
            {
                sortField = value;
            }
        }

        /// <summary>
        /// 数据库访问对象的外键约束
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                return primaryKey;
            }
        }


        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseRepositoryReport()
        {
        }

        /// <summary>
        /// 构造方法，注入上下文
        /// </summary>
        /// <param name="dbContext">上下文</param>
        public BaseRepositoryReport(IDbContextCore dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
            //_dbContext.EnsureCreated();

            //YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            //S_EmployeeID = yuebonCacheHelper.Get("Cache_EmployeeID").ToString();


            //var v_List_Login = yuebonCacheHelper.Get("List_Login");
            //if (v_List_Login != null) 
            //{
            //    Obj_Login=v_List_Login;
            //}
        }

        /// <summary>
        /// 构造方法，注入上下文
        /// </summary>
        /// <param name="dbContextFactory">上下文</param>
        public BaseRepositoryReport(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        /// <summary>
        /// GetSNDateTime
        /// </summary>
        /// <param name="S_SN"></param>
        /// <returns></returns>
        public string GetSNDateTime(string S_SN) 
        {
            return DateTime.Now.ToString(S_DateTimeFormat)+ " SN:" + S_SN+"   "; 
        }

        /// <summary>
        /// Data_Table
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        public DataTable Data_Table(string S_Sql)
        {
            DataTable DT = new DataTable();
            try
            {
                SqlConnection Conn = new SqlConnection(dbConnectionOptions.ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                SqlDataAdapter Sql_DA = new SqlDataAdapter(cmd);
                Sql_DA.Fill(DT);
                Sql_DA.Dispose();
                Conn.Close();
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ERROR");
                DataRow dr = dt.NewRow();
                dr["ERROR"] = ex.ToString();
                dt.Rows.Add(dr);

                DT = dt;
            }
            return DT;
        }

        /// <summary>
        /// Data_Set
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        public DataSet Data_Set(string S_Sql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection Conn = new SqlConnection(dbConnectionOptions.ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                SqlDataAdapter Sql_DA = new SqlDataAdapter(cmd);
                Sql_DA.Fill(ds);
                Conn.Close();
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ERROR");
                DataRow dr = dt.NewRow();
                dr["ERROR"] = ex.ToString();
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
            }

            return ds;
        }

        /// <summary>
        /// ExecSql
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        public string ExecSql(string S_Sql)
        {
            string S_Result = "OK";
            try
            {
                SqlConnection Conn = new SqlConnection(dbConnectionOptions.ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                cmd.ExecuteNonQuery();
                Conn.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }
        public string ExecFirstSql(string S_Sql)
        {
            string S_Result = "OK";
            try
            {
                SqlConnection Conn = new SqlConnection(dbConnectionOptions.ConnectionString);
                Conn.Open();
                SqlCommand cmd = new SqlCommand(S_Sql, Conn);
                object o = cmd.ExecuteScalar();
                S_Result = o?.ToString();
                Conn.Close();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }

        /// <summary>
        /// 多表多数据操作批量插入、更新、删除--事务
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="commandTimeout">超时</param>
        /// <param name="I_MaxID">MaxID</param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> ExecuteTransactionAsync(List<Tuple<string, object>> trans, int? commandTimeout = null,int? I_MaxID=null)
        {
            if (!trans.Any()) return new Tuple<bool, string>(false, "执行事务SQL语句不能为空！");
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var tran in trans)
                        {
                            await connection.ExecuteAsync(tran.Item1, tran.Item2, transaction, commandTimeout);
                        }
                        //提交事务
                        transaction.Commit();
                        return new Tuple<bool, string>(true, I_MaxID.ToString());
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return new Tuple<bool, string>(false, ex.Message);
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// 多表多数据操作批量插入、更新、删除--事务
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="commandTimeout">超时</param>
        /// <returns></returns>
        public Tuple<bool, string> ExecuteTransaction(List<Tuple<string, object>> trans, int? commandTimeout = null)
        {
            if (!trans.Any()) return new Tuple<bool, string>(false, "执行事务SQL语句不能为空！");
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                //开启事务
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var tran in trans)
                        {
                            connection.Execute(tran.Item1, tran.Item2, transaction, commandTimeout);
                        }
                        //提交事务
                        transaction.Commit();
                        return new Tuple<bool, string>(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("ExecuteTransaction", ex);
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return new Tuple<bool, string>(false, ex.ToString());
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 多表多数据操作批量插入、更新、删除--事务
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="commandTimeout">超时</param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> ExecuteTransactionAsync(List<Tuple<string, object>> trans, int? commandTimeout = null)
        {
            if (!trans.Any()) return new Tuple<bool, string>(false, "执行事务SQL语句不能为空！");
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var tran in trans)
                        {
                            await connection.ExecuteAsync(tran.Item1, tran.Item2, transaction, commandTimeout);
                        }
                        //提交事务
                        transaction.Commit();
                        return new Tuple<bool, string>(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return new Tuple<bool, string>(false, ex.Message);
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 多表多数据操作批量插入、更新、删除--事务
        /// </summary>
        /// <param name="trans">事务</param>
        /// <param name="commandTimeout">超时</param>
        /// <returns></returns>
        public async Task<string> ExecuteTransactionSqlAsync(string sqls)
        {
            string sResult = "OK";
            if (string.IsNullOrEmpty(sqls))
                return sResult = "sql command are empty.";
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(sqls, null, transaction, I_DBTimeout);
                        //提交事务
                        transaction.Commit();
                        return sResult;
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        connection.Close();
                        connection.Dispose();
                        return sResult = "ERROR:" + ex.Message;
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        #endregion





        #region 辅助类方法


        /// <summary>
        /// 验证是否存在注入代码(条件语句）
        /// </summary>
        /// <param name="inputData"></param>
        public virtual bool HasInjectionData(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return false;

            //里面定义恶意字符集合
            //验证inputData是否包含恶意集合
            if (Regex.IsMatch(inputData.ToLower(), GetRegexString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取正则表达式
        /// </summary>
        /// <returns></returns>
        private static string GetRegexString()
        {
            //构造SQL的注入关键字符
            string[] strBadChar =
            {
                "select\\s",
                "from\\s",
                "insert\\s",
                "delete\\s",
                "update\\s",
                "drop\\s",
                "truncate\\s",
                "exec\\s",
                "count\\(",
                "declare\\s",
                "asc\\(",
                "mid\\(",
                //"char\\(",
                "net user",
                "xp_cmdshell",
                "/add\\s",
                "exec master.dbo.xp_cmdshell",
                "net localgroup administrators"
            };

            //构造正则表达式
            string str_Regex = ".*(";
            for (int i = 0; i < strBadChar.Length - 1; i++)
            {
                str_Regex += strBadChar[i] + "|";
            }
            str_Regex += strBadChar[^1] + ").*";

            return str_Regex;
        }


        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 
        /// </summary>
        public virtual IDbContextCore DbContext
        {
            get { return _dbContext; }
        }
        /// <summary>
        /// 
        /// </summary>
        public IDbConnection DapperConn
        {
            get { return new DapperDbContext().GetConnection2(); }
        }


        /// <summary>
        /// 用Dapper原生方法，仅用于只读数据库
        /// </summary>
        public IDbConnection DapperConnRead2
        {
            get { return new DapperDbContext().GetConnection2(false); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
            if (DbContext != null)
            {
                DbContext.Dispose();
            }
            if (DapperConn != null)
            {
                DapperConn?.Dispose();
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~BaseRepository() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);

            DbContext?.Dispose();
            DapperConn?.Dispose();
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
