using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dapper;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.DependencyInjection;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Log;

namespace SunnyMES.Commons.Repositories
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>    
    /// <typeparam name="TKey">实体主键类型</typeparam>
    public abstract class BaseCommonRepository<TKey> : ICommonRepository<TKey>, ITransientDependency
    {
        /// <summary>
        /// 数据库超时(10分)  600000 秒
        /// </summary>
        public int I_DBTimeout = 600000;
        /// <summary>
        /// 时间日期格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string S_DateTimeFormat = "yyyy-MM-dd HH:mm:fff";
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
        protected string S_IsLegalPage { get; set; }
        /// <summary>
        /// TT 扫描类型 //扫码类型 1:条码SN 2:BOX 3:MachineBox (未配置默认1) 
        /// 4:混合（前3种类型都可以扫描）当输入为1类型时，且条码被绑定在2或3中，这时对SN做解除动作。
        /// </summary>
        public string S_TTScanType = "1";
        /// <summary>
        /// TT 满箱数量
        /// </summary>
        public int I_FullBoxQTY = 0;


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
        /// 缓存读取实例
        /// </summary>
        protected YuebonCacheHelper cacheHelper = new YuebonCacheHelper();
        /// <summary>
        /// 站点详细属性集合
        /// </summary>
        protected StationAttributes oStationAttributes = new StationAttributes();
        /// <summary>
        /// 工单详细属性集合
        /// </summary>
        protected PoAttributes oPoAttributes = new PoAttributes();
        /// <summary>
        /// 请求头集合
        /// </summary>
        protected CommonHeader baseCommonHeader = new CommonHeader();

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
        public BaseCommonRepository()
        {
        }

        /// <summary>
        /// 构造方法，注入上下文
        /// </summary>
        /// <param name="dbContext">上下文</param>
        public BaseCommonRepository(IDbContextCore dbContext)
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
        public BaseCommonRepository(IDbContextFactory dbContextFactory)
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
            return DateTime.Now.ToString(S_DateTimeFormat) + " SN:" + S_SN + "   ";
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
                        return new Tuple<bool, string>(false, ex.Message);
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
        public async Task<string> ExecuteTransactionSqlAsync(string sqls)
        {
            string sResult = "0";
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
                        return sResult = "1";
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        return sResult = "ERROR:" + ex.Message;
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
        public string ExecuteTransactionSql(string sqls)
        {
            string sResult = "0";
            if (string.IsNullOrEmpty(sqls))
                return sResult = "sql command are empty.";
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                //开启事务
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(sqls, null, transaction, I_DBTimeout);
                        //提交事务
                        transaction.Commit();
                        return sResult = "1";
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("ExecuteTransaction", ex);
                        transaction.Rollback();
                        return sResult = "ERROR:" + ex.Message;
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
        public async Task<string> ExecuteTransactionSqlAsync(List<string> sqls)
        {
            string sResult = "0";
            if (sqls.Count <= 0)
                return sResult = "sql command are empty.";
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqls.Count; i++)
                        {
                            await connection.ExecuteAsync(sqls[i], null, transaction, I_DBTimeout);
                        }
                        //提交事务
                        transaction.Commit();
                        return sResult = "1";
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        return sResult = "ERROR: " + ex.Message;
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
        public string ExecuteTransactionSql(List<string> sqls)
        {
            string sResult = "0";
            if (sqls.Count <= 0)
                return sResult = "sql command are empty.";
            using (IDbConnection connection = DapperConn)
            {
                bool isClosed = connection.State == ConnectionState.Closed;
                if (isClosed) connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqls.Count; i++)
                        {
                            connection.Execute(sqls[i], null, transaction, I_DBTimeout);
                        }
                        //提交事务
                        transaction.Commit();
                        return sResult = "1";
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        return sResult = "ERROR: " + ex.Message;
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
        /// 返回多个实体
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteTransactionSqlAdapterAsync(string sqls)
        {
            if (string.IsNullOrEmpty(sqls))
                return null;
            using (SqlConnection connection = new SqlConnection(DapperConn.ConnectionString))
            {
                if (connection.State !=  ConnectionState.Open)
                    connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DataSet dataSet = new DataSet();
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                        sqlDataAdapter.SelectCommand = new SqlCommand(sqls,connection,transaction);
                        sqlDataAdapter.Fill(dataSet);
                        //提交事务
                        transaction.Commit();
                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        //回滚事务
                        Log4NetHelper.Error("", ex);
                        transaction.Rollback();
                        return null;
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
