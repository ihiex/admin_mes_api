using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using NPOI.SS.Formula.Functions;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using SunnyMES.Commons.Core.PublicFun;
using Dapper;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons;

namespace SunnyMES.Security.Repositories
{
    public class RoleRepository : BaseRepository<Role, string>, IRoleRepository
    {
        public RoleRepository()
        {
        }

        public RoleRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }


        private DataRow NewDT_Text(DataTable DT_Text, DataTable DT_Sql)
        {
            DataRow DR = DT_Text.NewRow();
            for (int k = 0; k < DT_Text.Columns.Count; k++)
            {
                DR[k] = DT_Sql.Rows[0][k].ToString();
            }
            //DT_Text.Rows.Add(DR);

            return DR;
        }

        private void SetData3(Role entity, string Type)
        {
            string S_SysPath = Directory.GetCurrentDirectory();
            DataTable dataTable3 = new DataTable();
            string S_Data3 = S_SysPath + "\\Data3.dll";

            string S_Sql3 = "";
            DataTable DT_Sql3 = null;

            try
            {
                if (File.Exists(S_Data3) == false)
                {
                    S_Sql3 = "select * from API_Role where ID='" + entity.Id + "'";
                    DT_Sql3 = Data_Table(S_Sql3);
                    string json1_Exists = PublicF.DataTableToJson(DT_Sql3);
                    string S_MI1_Exists = EncryptHelper2023.EncryptString(json1_Exists);
                    File.WriteAllText(S_Data3, S_MI1_Exists);

                }

                string S_MI1 = File.ReadAllText(S_Data3);
                string S_JsonJM1 = EncryptHelper2023.DecryptString(S_MI1);
                JArray jsonArray1 = JArray.Parse(S_JsonJM1);

                foreach (JProperty property in jsonArray1[0])
                {
                    dataTable3.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray1)
                {
                    DataRow row = dataTable3.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable3.Rows.Add(row);
                }

                S_Sql3 = "select * from API_Role where ID='" + entity.Id + "'";
                DT_Sql3 = Data_Table(S_Sql3);

                if (Type == "Insert")
                {
                    dataTable3.Rows.Add(NewDT_Text(dataTable3, DT_Sql3));
                }
                else if (Type == "Delete")
                {
                    for (int i = 0; i < dataTable3.Rows.Count; i++)
                    {
                        if (dataTable3.Rows[i]["Id"].ToString().Trim() == DT_Sql3.Rows[0]["Id"].ToString().Trim())
                        {
                            dataTable3.Rows[i].Delete();
                            break;
                        }
                    }
                }
                else if (Type == "Update")
                {
                    if (dataTable3.Select("Id='" + DT_Sql3.Rows[0]["Id"].ToString().Trim() + "'").Count() > 0)
                    {

                        for (int i = 0; i < dataTable3.Rows.Count; i++)
                        {
                            if (dataTable3.Rows[i]["Id"].ToString().Trim() == DT_Sql3.Rows[0]["Id"].ToString().Trim())
                            {
                                dataTable3.Rows[i].Delete();

                                DataRow DR = NewDT_Text(dataTable3, DT_Sql3);
                                dataTable3.Rows.Add(DR);

                                break;
                            }
                        }
                    }
                    else
                    {
                        DataRow DR = NewDT_Text(dataTable3, DT_Sql3);
                        dataTable3.Rows.Add(DR);
                    }
                }

                string json3_New = PublicF.DataTableToJson(dataTable3);
                string S_MI3_New = EncryptHelper2023.EncryptString(json3_New);
                File.WriteAllText(S_Data3, S_MI3_New);


                try
                {
                    string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                    string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                    foreach (var item in List_WinformWebDIR)
                    {
                        //string S_WinformWebDIR_Data3 = item + "\\Data3.dll";
                        //File.WriteAllText(S_WinformWebDIR_Data3, S_MI3_New);


                        try
                        {
                            string S_FTPIP = item;
                            string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                            string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                            FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                            FTP.GotoDirectory("", true);
                            FTP.Upload(S_Data3, "Data3.dll");

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

        /// <summary>
        /// InsertAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public override async Task<long> InsertAsync(Role entity, IDbTransaction trans = null)
        {
            if (entity.KeyIsNull())
            {
                entity.GenerateDefaultKeyVal();
            }
            long I_Result = _dbContext.Add(entity);

            if (I_Result > 0)
            {
                SetData3(entity, "Insert");
            }

            return I_Result;
        }

        public  async Task<string> Clone(Role entity, IDbTransaction trans = null)
        {
            string S_Result = "OK";
            try
            {                
                string S_Sql_RoleAuthorize = "SELECT * FROM API_RoleAuthorize WHERE ObjectId='" + entity.Id + "'";
                List<RoleAuthorize> roleAuthorizesList = new List<RoleAuthorize>();
                var v_roleAuthorizesList = await DapperConn.QueryAsync<RoleAuthorize>(S_Sql_RoleAuthorize);
                roleAuthorizesList = v_roleAuthorizesList.ToList();

                string S_Sql_roleDataList = "select * from  API_RoleData where RoleId='" + entity.Id + "'";
                List<RoleData> roleDataList = new List<RoleData>();
                var v_roleDataList = await DapperConn.QueryAsync<RoleData>(S_Sql_roleDataList);
                roleDataList= v_roleDataList.ToList();

                Role v_Role = new Role();
                string Id = v_Role.Id;
                v_Role = entity;
                v_Role.Id = Id;
                v_Role.CreatorTime = DateTime.Now;

                long I_Result = _dbContext.Add(v_Role);

                for (int i = 0; i < roleAuthorizesList.Count(); i++)
                {
                    roleAuthorizesList[i].Id = GuidUtils.CreateNo();
                    roleAuthorizesList[i].ObjectId= Id;
                }

                for (int i = 0; i < roleDataList.Count(); i++)
                {
                    roleDataList[i].Id = GuidUtils.CreateNo();
                    roleDataList[i].RoleId = Id;
                }

                if (I_Result > 0)
                {
                    SetData3(entity, "Insert");

                    RoleAuthorizeRepository v_RoleAuthorizeRepository = new RoleAuthorizeRepository();
                    await v_RoleAuthorizeRepository.SaveRoleAuthorize(v_Role.Id, roleAuthorizesList, roleDataList, trans);
                }
            }
            catch (Exception ex) 
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }


        /// <summary>
        /// UpdateAsync
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="primaryKey"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(Role entity, string primaryKey, IDbTransaction trans = null)
        {
            int I_Result = DbContext.Edit<Role>(entity);

            if (I_Result > 0)
            {
                SetData3(entity, "Update");
            }

            return I_Result > 0;
        }

        public async Task<List<API_Role>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_Role> list = new List<API_Role>();

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

            list = await SuDB.Queryable<API_Role>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }

    }
}