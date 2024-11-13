using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.DataManager;

namespace SunnyMES.Commons.Helpers
{
    /// <summary>
    /// SqlSugarHelper-单例
    /// ① 再外面验证是否是单例：SqlSugarHelper.Db.HasCode();  // 只要看这个hascode在服务启动后都一个那么说明成功了
    /// ② 使用示例：继承、调用静态方法
    /// </summary>
    public class SqlSugarHelper
    {
        /// <summary>
        /// 单例-SqlSugarScope（官方推荐使用这种单例）
        /// </summary>
        public static SqlSugarScope Db = new SqlSugarScope(new ConnectionConfig()  // 静态单例模式；如果是泛型类 Db要扔到外面 ,DbContext<T>.Db会导致产生多个实例。
        {
            ConnectionString = SFCEncode.DecryptPassword(Configs.GetConfigurationValue("DbConnections:DefaultDb:MasterDB", "ConnectionString"), "") == "-1:ciphertext is wrong"? Configs.GetConfigurationValue("DbConnections:DefaultDb:MasterDB", "ConnectionString"): SFCEncode.DecryptPassword(Configs.GetConfigurationValue("DbConnections:DefaultDb:MasterDB", "ConnectionString"), ""),
            DbType = DbType.SqlServer,  //数据库类型
            IsAutoCloseConnection = true //不设成true要手动close
        },
          db =>
          {
              db.Ado.CommandTimeOut = 12;  // 12分钟
              // UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
              // 每次Sql执行前事件，记录进行的操作
              db.Aop.OnLogExecuting = (sql, pars) =>
              {
                  StringBuilder sqlStr = new StringBuilder();

                  if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                  {
                      Console.ForegroundColor = ConsoleColor.Blue;
                      sqlStr.AppendLine($"==============将要执行新增/修改操作==============");
                  }
                  if (sql.StartsWith("DELETE"))
                  {
                      Console.ForegroundColor = ConsoleColor.Red;
                      sqlStr.AppendLine($"==============将要执行删除操作==============");
                  }
                  if (sql.StartsWith("SELECT"))
                  {
                      Console.ForegroundColor = ConsoleColor.Green;
                      sqlStr.AppendLine($"==============将要执行查询操作==============");
                  }
                  sqlStr.AppendLine("【SQL预执行语句】：");
                  sqlStr.AppendLine("    " + sql);
                  sqlStr.AppendLine("【参数】：");
                  string sqlPars = db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                  sqlStr.AppendLine("    " + sqlPars);

                  Console.WriteLine(sqlStr.ToString());  // 打印
                  Console.ForegroundColor = ConsoleColor.White;

                  // 记录执行的信息
                  //_logger.LogCritical("\r\n 【开始打印仓储日志】：\r\n 【执行时间】：{0} \r\n 【SQL预执行语句】：{1} \r\n 【参数】：{2}", DateTime.Now.ToString(), sql, sqlPars);
              };

              // 每次Sql执行后事件，记录SQL执行完的信息
              db.Aop.OnLogExecuted = (sql, pars) =>
              {
                  // 执行时间超过1秒
                  if (db.Ado.SqlExecutionTime.TotalSeconds > 1)
                  {
                      StringBuilder sqlPStr = new StringBuilder();
                      sqlPStr.AppendLine($"==============执行了下面的操作==============");
                      var fileName = db.Ado.SqlStackTrace.FirstFileName;           // 代码CS文件名
                      sqlPStr.AppendLine("【代码CS文件名】：" + fileName);
                      var fileLine = db.Ado.SqlStackTrace.FirstLine;               // 代码行数
                      sqlPStr.AppendLine("【代码行数】：" + fileLine);
                      var FirstMethodName = db.Ado.SqlStackTrace.FirstMethodName;  // 方法名
                      sqlPStr.AppendLine("【方法名】：" + FirstMethodName);
                      sqlPStr.AppendLine("【SQL执行语句】：");
                      sqlPStr.AppendLine("    " + sql);
                      sqlPStr.AppendLine("【参数】：");
                      string sqlPars = db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                      sqlPStr.AppendLine("    " + sqlPars);

                      // 打印
                      Console.ForegroundColor = ConsoleColor.Green;
                      Console.WriteLine(sqlPStr);
                      Console.ForegroundColor = ConsoleColor.White;

                      // 记录执行的信息
                      //_logger.LogCritical("\r\n 【结束打印仓储日志】：\r\n 【执行时间】：{0} \r\n 【SQL执行时间】：{1} \r\n 【代码CS文件名】：{2} \r\n 【代码行数】：{3} \r\n 【方法名】：{4} \r\n 【SQL执行语句】：{5} \r\n 【参数】：{6}",
                      //    DateTime.Now.ToString(), base.Context.Ado.SqlExecutionTime.ToString(), fileName, fileLine, FirstMethodName, sql, sqlPars);
                  }
              };

              // 记录SQL报错
              db.Aop.OnError = (exp) =>
              {
                  StringBuilder sqlStr = new StringBuilder();
                  sqlStr.AppendLine($"==============数据库执行报错==============");
                  sqlStr.AppendLine("【SQL执行语句】：");
                  sqlStr.AppendLine("    " + exp.Sql);
                  sqlStr.AppendLine("【参数】：");
                  string sqlPars = JsonConvert.SerializeObject(exp.Parametres);
                  sqlStr.AppendLine("    " + sqlPars);
                  sqlStr.AppendLine("【报错信息】：");
                  sqlStr.AppendLine("    " + exp.Message);
                  sqlStr.AppendLine("【报错位置】：");
                  string expStackTrace = exp.StackTrace.Substring(exp.StackTrace.LastIndexOf("\\") + 1, exp.StackTrace.Length - exp.StackTrace.LastIndexOf("\\") - 1);
                  sqlStr.AppendLine("    " + expStackTrace);
                  Console.ForegroundColor = ConsoleColor.Red;
                  Console.WriteLine(sqlStr);  // 打印
                  Console.ForegroundColor = ConsoleColor.White;

                  // 记录执行的信息
                  //_logger.LogCritical("\r\n 【打印仓储sql报错日志】：\r\n 【执行时间】：{0} \r\n 【SQL执行语句】：{1} \r\n 【参数】：{2} \r\n 【报错信息】：{3} \r\n 【报错位置】：{4}", DateTime.Now.ToString(), exp.Sql, sqlPars, exp.Message, expStackTrace);
              };
          });

        /// <summary>
        /// 单例-SqlSugarClient
        /// </summary>
        public static SqlSugarClient Db2 = new SqlSugarClient(new ConnectionConfig()  // 静态单例模式；如果是泛型类 Db要扔到外面 ,DbContext<T>.Db会导致产生多个实例。
        {
            ConnectionString = "Server=.xxxxx",//连接符字串
            DbType = DbType.SqlServer,  //数据库类型
            IsAutoCloseConnection = true //不设成true要手动close
        },
          db =>
          {
              db.Ado.CommandTimeOut = 720;  // 12分钟
              // UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
              // 每次Sql执行前事件，记录进行的操作
              db.Aop.OnLogExecuting = (sql, pars) =>
              {
                  StringBuilder sqlStr = new StringBuilder();

                  if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                  {
                      Console.ForegroundColor = ConsoleColor.Blue;
                      sqlStr.AppendLine($"==============将要执行新增/修改操作==============");
                  }
                  if (sql.StartsWith("DELETE"))
                  {
                      Console.ForegroundColor = ConsoleColor.Red;
                      sqlStr.AppendLine($"==============将要执行删除操作==============");
                  }
                  if (sql.StartsWith("SELECT"))
                  {
                      Console.ForegroundColor = ConsoleColor.Green;
                      sqlStr.AppendLine($"==============将要执行查询操作==============");
                  }
                  sqlStr.AppendLine("【SQL预执行语句】：");
                  sqlStr.AppendLine("    " + sql);
                  sqlStr.AppendLine("【参数】：");
                  string sqlPars = db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                  sqlStr.AppendLine("    " + sqlPars);

                  Console.WriteLine(sqlStr.ToString());  // 打印
                  Console.ForegroundColor = ConsoleColor.White;

                  // 记录执行的信息
                  //_logger.LogCritical("\r\n 【开始打印仓储日志】：\r\n 【执行时间】：{0} \r\n 【SQL预执行语句】：{1} \r\n 【参数】：{2}", DateTime.Now.ToString(), sql, sqlPars);
              };

              // 每次Sql执行后事件，记录SQL执行完的信息
              db.Aop.OnLogExecuted = (sql, pars) =>
              {
                  // 执行时间超过1秒
                  if (db.Ado.SqlExecutionTime.TotalSeconds > 1)
                  {
                      StringBuilder sqlPStr = new StringBuilder();
                      sqlPStr.AppendLine($"==============执行了下面的操作==============");
                      var fileName = db.Ado.SqlStackTrace.FirstFileName;           // 代码CS文件名
                      sqlPStr.AppendLine("【代码CS文件名】：" + fileName);
                      var fileLine = db.Ado.SqlStackTrace.FirstLine;               // 代码行数
                      sqlPStr.AppendLine("【代码行数】：" + fileLine);
                      var FirstMethodName = db.Ado.SqlStackTrace.FirstMethodName;  // 方法名
                      sqlPStr.AppendLine("【方法名】：" + FirstMethodName);
                      sqlPStr.AppendLine("【SQL执行语句】：");
                      sqlPStr.AppendLine("    " + sql);
                      sqlPStr.AppendLine("【参数】：");
                      string sqlPars = db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value));
                      sqlPStr.AppendLine("    " + sqlPars);

                      // 打印
                      Console.ForegroundColor = ConsoleColor.Green;
                      Console.WriteLine(sqlPStr);
                      Console.ForegroundColor = ConsoleColor.White;

                      // 记录执行的信息
                      //_logger.LogCritical("\r\n 【结束打印仓储日志】：\r\n 【执行时间】：{0} \r\n 【SQL执行时间】：{1} \r\n 【代码CS文件名】：{2} \r\n 【代码行数】：{3} \r\n 【方法名】：{4} \r\n 【SQL执行语句】：{5} \r\n 【参数】：{6}",
                      //    DateTime.Now.ToString(), base.Context.Ado.SqlExecutionTime.ToString(), fileName, fileLine, FirstMethodName, sql, sqlPars);
                  }
              };

              // 记录SQL报错
              db.Aop.OnError = (exp) =>
              {
                  StringBuilder sqlStr = new StringBuilder();
                  sqlStr.AppendLine($"==============数据库执行报错==============");
                  sqlStr.AppendLine("【SQL执行语句】：");
                  sqlStr.AppendLine("    " + exp.Sql);
                  sqlStr.AppendLine("【参数】：");
                  string sqlPars = JsonConvert.SerializeObject(exp.Parametres);
                  sqlStr.AppendLine("    " + sqlPars);
                  sqlStr.AppendLine("【报错信息】：");
                  sqlStr.AppendLine("    " + exp.Message);
                  sqlStr.AppendLine("【报错位置】：");
                  string expStackTrace = exp.StackTrace.Substring(exp.StackTrace.LastIndexOf("\\") + 1, exp.StackTrace.Length - exp.StackTrace.LastIndexOf("\\") - 1);
                  sqlStr.AppendLine("    " + expStackTrace);
                  Console.ForegroundColor = ConsoleColor.Red;
                  Console.WriteLine(sqlStr);  // 打印
                  Console.ForegroundColor = ConsoleColor.White;

                  // 记录执行的信息
                  //_logger.LogCritical("\r\n 【打印仓储sql报错日志】：\r\n 【执行时间】：{0} \r\n 【SQL执行语句】：{1} \r\n 【参数】：{2} \r\n 【报错信息】：{3} \r\n 【报错位置】：{4}", DateTime.Now.ToString(), exp.Sql, sqlPars, exp.Message, expStackTrace);
              };
          });



    }
}
