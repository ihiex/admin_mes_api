using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Options;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Commons;

namespace SunnyMES.Security.Repositories
{
    public class RoleAuthorizeRepository : BaseRepository<RoleAuthorize, string>, IRoleAuthorizeRepository
    {
        public RoleAuthorizeRepository()
        {
        }

        public RoleAuthorizeRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        private DataRow NewDT_Text(DataTable DT_Text, DataRow DR_Sql)
        {
            DataRow DR = DT_Text.NewRow();
            for (int k = 0; k < DT_Text.Columns.Count; k++)
            {
                DR[k] = DR_Sql[k].ToString();
            }
            return DR;
        }


        /// <summary>
        /// 保存角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="roleAuthorizesList">角色功能模块</param>
        /// <param name="roleDataList">角色可访问数据</param>
        /// <param name="trans"></param>
        /// <returns>执行成功返回<c>true</c>，否则为<c>false</c>。</returns>
        public async Task<bool> SaveRoleAuthorize(string roleId,List<RoleAuthorize> roleAuthorizesList, List<RoleData> roleDataList,
           IDbTransaction trans = null)
        {
            Boolean B_Result = false;
            string S_SqlMain = "";

            var param = new List<Tuple<string, object>>();
            Tuple<string, object> tupel;

            S_SqlMain =
            @"delete FROM API_RoleAuthorize where ObjectId='"+ roleId + @"'
              delete FROM API_RoleData where RoleId='"+ roleId + @"'
            ";

            //tupel = new Tuple<string, object>(@"delete FROM API_RoleAuthorize where ObjectId=@RoleId;", new { RoleId = roleId } );
            //param.Add(tupel);
            //tupel = new Tuple<string, object>(@"delete FROM API_RoleData where RoleId=@RoleId;", new { RoleId = roleId });
            //param.Add(tupel);

            string S_SysPath = Directory.GetCurrentDirectory();
            string S_Data4 = S_SysPath + "\\Data4.dll";  //API_RoleAuthorize
            string S_Data5 = S_SysPath + "\\Data5.dll";  //API_RoleData 

            string S_Sql4 = "";
            DataTable DT_Sql4 = null;
            DataTable dataTable4 = new DataTable();

            string S_Sql5 = "";
            DataTable DT_Sql5 = null;
            DataTable dataTable5 = new DataTable();

            if (File.Exists(S_Data4) == false)
            {
                S_Sql4 = "select * FROM API_RoleAuthorize where ObjectId='"+ roleId + "'";
                DT_Sql4 = Data_Table(S_Sql4);
                string json4_Exists = PublicF.DataTableToJson(DT_Sql4);
                string S_MI4_Exists = EncryptHelper2023.EncryptString(json4_Exists);
                File.WriteAllText(S_Data4, S_MI4_Exists);
            }

            if (File.Exists(S_Data5) == false)
            {
                S_Sql5 = "select * FROM API_RoleData where RoleId='" + roleId + "'";
                DT_Sql5 = Data_Table(S_Sql5);
                string json5_Exists = PublicF.DataTableToJson(DT_Sql5);
                string S_MI5_Exists = EncryptHelper2023.EncryptString(json5_Exists);
                File.WriteAllText(S_Data5, S_MI5_Exists);
            }

            try
            {
                string S_MI4 = File.ReadAllText(S_Data4);
                string S_JsonJM4 = EncryptHelper2023.DecryptString(S_MI4);
                JArray jsonArray4 = JArray.Parse(S_JsonJM4);

                foreach (JProperty property in jsonArray4[0])
                {
                    dataTable4.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray4)
                {
                    DataRow row = dataTable4.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable4.Rows.Add(row);
                }
            }
            catch
            {

            }

            try
            {
                string S_MI5 = File.ReadAllText(S_Data5);
                string S_JsonJM5 = EncryptHelper2023.DecryptString(S_MI5);
                JArray jsonArray5 = JArray.Parse(S_JsonJM5);

                foreach (JProperty property in jsonArray5[0])
                {
                    dataTable5.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray5)
                {
                    DataRow row = dataTable5.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable5.Rows.Add(row);
                }
            }
            catch
            {

            }
            
            foreach (RoleAuthorize item in roleAuthorizesList)
            {
                S_SqlMain +=
                @"INSERT INTO API_RoleAuthorize
               (Id
               ,ItemType
               ,ItemId
               ,ObjectType
               ,ObjectId
               ,SortCode
               ,CreatorTime
               ,CreatorUserId
                ) Values
                (
                '"+ item.Id + @"' 
                ,'"+ item.ItemType + @"' 
                ,'"+ item.ItemId + @"'
                ,'"+ item.ObjectType + @"'
                ,'"+ item.ObjectId + @"'
                ,'"+ item.SortCode + @"'
                ,'"+ item.CreatorTime + @"'
                ,'"+ item.CreatorUserId + @"'
                )
                ";
            }

            foreach (RoleData roleData in roleDataList)
            {
                S_SqlMain +=
                @"
                INSERT INTO API_RoleData
               (Id
               ,RoleId
               ,AuthorizeData
               ,DType) Values
                (
                '"+ roleData.Id + @"' 
                ,'"+ roleData.RoleId + @"' 
                ,'"+ roleData.AuthorizeData + @"' 
                ,'"+ roleData.DType + @"' 
                )
                ";
            }


            //       foreach (RoleAuthorize item in roleAuthorizesList)
            //       {
            //           tupel = new Tuple<string, object>(@" INSERT INTO API_RoleAuthorize
            //      (Id
            //      ,ItemType
            //      ,ItemId
            //      ,ObjectType
            //      ,ObjectId
            //      ,SortCode
            //      ,CreatorTime
            //      ,CreatorUserId)
            //VALUES(@Id
            //      ,@ItemType
            //      ,@ItemId
            //      ,@ObjectType
            //      ,@ObjectId
            //      ,@SortCode
            //      ,@CreatorTime
            //      ,@CreatorUserId) ", new
            //           {
            //               Id = item.Id,
            //               ItemType = item.ItemType,
            //               ItemId = item.ItemId,
            //               ObjectType = item.ObjectType,
            //               ObjectId = item.ObjectId,
            //               SortCode = item.SortCode,
            //               CreatorTime = item.CreatorTime,
            //               CreatorUserId = item.CreatorUserId
            //           });
            //           param.Add(tupel);
            //       }
            //       foreach (RoleData roleData in roleDataList)
            //       {
            //           tupel = new Tuple<string, object>(@" INSERT INTO API_RoleData
            //      (Id
            //      ,RoleId
            //      ,AuthorizeData
            //      ,DType)
            //VALUES
            //      (@Id
            //      ,@RoleId
            //      ,@AuthorizeData
            //      ,@DType) ", new
            //           {
            //               Id = roleData.Id,
            //               RoleId = roleData.RoleId,
            //               AuthorizeData = roleData.AuthorizeData,
            //               DType = roleData.DType
            //           });
            //           param.Add(tupel);
            //       }
            //      var result = await ExecuteTransactionAsync(param);


            //if (result.Item1) 

            tupel = new Tuple<string, object>(S_SqlMain, null);
            param.Add(tupel);
            var result = await ExecuteTransactionAsync(param);
            B_Result = result.Item1;


            if (B_Result==true)
            {
                try
                {

                    DataRow[] List_DR_Del = dataTable4.Select("ObjectId='" + roleId + "'");
                    for (int i=0;i< dataTable4.Rows.Count; i++)
                    {
                        foreach (var item in List_DR_Del) 
                        {
                            if (dataTable4.Rows[i] == item) 
                            {
                                dataTable4.Rows[i].Delete();
                                break;
                            }
                        }
                    }

                    S_Sql4 = "select * FROM API_RoleAuthorize where ObjectId='" + roleId + "'";
                    DT_Sql4 = Data_Table(S_Sql4);

                    for (int i = 0; i < DT_Sql4.Rows.Count; i++)
                    {
                        DataRow DR = NewDT_Text(dataTable4, DT_Sql4.Rows[i]);
                        dataTable4.Rows.Add(DR);
                    }

                    string json4_Exists = PublicF.DataTableToJson(dataTable4);
                    string S_MI4_Exists = EncryptHelper2023.EncryptString(json4_Exists);
                    File.WriteAllText(S_Data4, S_MI4_Exists);

                    try
                    {
                        string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                        string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                        foreach (var item in List_WinformWebDIR)
                        {
                            //string S_WinformWebDIR_Data4 = item + "\\Data4.dll";
                            //File.WriteAllText(S_WinformWebDIR_Data4, S_MI4_Exists);


                            try
                            {
                                string S_FTPIP = item;
                                string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                                string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                                FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                                FTP.GotoDirectory("", true);

                                FTP.Upload(S_Data4, "Data4.dll");
                            }
                            catch (Exception ex)
                            {
                                Log4NetHelper.Info(ex.Message);
                                throw new Exception(ex.Message);
                            }
                        }

                    }
                    catch { }

                }
                catch
                {
                }


                try
                {
                    DataRow[] List_DR_Del = dataTable5.Select("RoleId='" + roleId + "'");
                    for (int i = 0; i < dataTable5.Rows.Count; i++)
                    {
                        foreach (var item in List_DR_Del)
                        {
                            if (dataTable5.Rows[i] == item)
                            {
                                dataTable5.Rows[i].Delete();
                                break;
                            }
                        }
                    }

                    S_Sql5 = "select * FROM API_RoleData where RoleId='" + roleId + "'";
                    DT_Sql5 = Data_Table(S_Sql5);

                    for (int i = 0; i < DT_Sql5.Rows.Count; i++)
                    {
                        DataRow DR = NewDT_Text(dataTable5, DT_Sql5.Rows[i]);
                        dataTable5.Rows.Add(DR);
                    }

                    string json5_Exists = PublicF.DataTableToJson(dataTable5);
                    string S_MI5_Exists = EncryptHelper2023.EncryptString(json5_Exists);
                    File.WriteAllText(S_Data5, S_MI5_Exists);

                    try
                    {
                        string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                        string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                        foreach (var item in List_WinformWebDIR)
                        {
                            //string S_WinformWebDIR_Data5 = item + "\\Data5.dll";
                            //File.WriteAllText(S_WinformWebDIR_Data5, S_MI5_Exists);


                            try
                            {
                                string S_FTPIP = item;
                                string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                                string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                                FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                                FTP.GotoDirectory("", true);
                                FTP.Upload(S_Data5, "Data5.dll");
                            }
                            catch (Exception ex)
                            {
                                Log4NetHelper.Info(ex.Message);
                                throw new Exception(ex.Message);
                            }
                        }

                    }
                    catch { }
                }
                catch
                {
                }

            }

            return B_Result; //result.Item1;
        }


        public async Task<List<API_RoleAuthorize>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_RoleAuthorize> list = new List<API_RoleAuthorize>();

            if (HasInjectionData(condition))
            {
                Log4NetHelper.Info(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                throw new Exception("检测出SQL注入的恶意数据");
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

            list = await SuDB.Queryable<API_RoleAuthorize>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }

    }
}