using Dapper;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Commons;

namespace SunnyMES.Security.Repositories
{
    public class MenuRepository : BaseRepository<Menu, string>, IMenuRepository
    {
        public MenuRepository()
        {
        }

        public MenuRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }


        /// <summary>
        /// 根据角色ID字符串（逗号分开)和系统类型ID，获取对应的操作功能列表
        /// </summary>
        /// <param name="roleIds">角色ID</param>
        /// <param name="typeID">系统类型ID</param>
        /// <param name="UserID">UserID</param>
        /// <param name="isMenu">是否是菜单</param>        
        /// <returns></returns>
        public IEnumerable<Menu> GetFunctions(string roleIds, string typeID, string UserID,bool isMenu = false)
        {
            string sql = $"SELECT DISTINCT b.* FROM API_menu as b " +
                $"INNER JOIN API_RoleAuthorize as a On b.Id = a.ItemId  WHERE ObjectId IN (" + roleIds + ")";
            if (roleIds == "")
            {
                sql = $"SELECT DISTINCT b.* FROM API_menu as b where 1=1 ";
            }
            if (isMenu)
            {
                sql = sql + "and menutype in('M','C')";
            }
            if (!string.IsNullOrEmpty(typeID))
            {
                sql = sql + string.Format(" AND SystemTypeId='{0}' ", typeID);
            }

            if (roleIds != "")
            {
                
                string S_SysPath = Directory.GetCurrentDirectory();
                string S_Data4 = S_SysPath + "\\Data4.dll";  //API_RoleAuthorize

                if (File.Exists(S_Data4) == false)
                {
                    string S_IsValidateSecond = "0";
                    try
                    {
                        S_IsValidateSecond = Configs.GetConfigurationValue("AppSetting", "IsValidateSecond");
                        S_IsValidateSecond = S_IsValidateSecond ?? "0";
                    }
                    catch
                    {
                        S_IsValidateSecond = "0";
                    }

                    if (S_IsValidateSecond == "1")
                    {
                        sql = "SELECT DISTINCT b.* FROM API_menu as b where 1=1 and  b.Id IS null";
                    }
                }
                else
                {
                    DataTable DT_Sql_Menu = Data_Table(sql);
                    DataTable dataTable4 = new DataTable();

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

                    string S_NotId = "";
                    for (int i = 0; i < DT_Sql_Menu.Rows.Count; i++) 
                    {
                        string S_ItemId = DT_Sql_Menu.Rows[i]["Id"].ToString();
                        DataRow[] DR_dataTable4 = dataTable4.Select("ItemId='"+ S_ItemId + "'");
                        if (DR_dataTable4.Count() == 0) 
                        {
                            if (S_NotId == "")
                            {
                                S_NotId = "'" + S_ItemId + "'";
                            }
                            else 
                            {
                                S_NotId += ",'" + S_ItemId + "'";
                            }
                        }
                    }

                    if (S_NotId != "") 
                    {
                        sql += " and b.Id not in( "+ S_NotId+")";
                    }
                }
            }

            return DapperConnRead.Query<Menu>(sql);
        }


        /// <summary>
        /// 根据系统类型ID，获取对应的操作功能列表
        /// </summary>
        /// <param name="typeID">系统类型ID</param>
        /// <returns></returns>
        public IEnumerable<Menu> GetFunctions(string typeID)
        {
            string sql = $"SELECT DISTINCT b.* FROM API_menu as b ";
            if (!string.IsNullOrEmpty(typeID))
            {
                sql = sql + string.Format(" Where SystemTypeId='{0}' ", typeID);
            }
            return DapperConnRead.Query<Menu>(sql);
        }


        public async Task<List<API_Menu>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            ConnectionConfig conn = new ConnectionConfig();
            conn.ConnectionString = DapperConnRead.ConnectionString;
            conn.DbType = SqlSugar.DbType.SqlServer;
            SqlSugarClient SuDB = new SqlSugarClient(conn);
            SuDB.Open();

            List<API_Menu> list = new List<API_Menu>();

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

            list = await SuDB.Queryable<API_Menu>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                .WhereIF(!string.IsNullOrEmpty(condition), condition)
                .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
            info.RecordCount = totalCount;

            return list;
        }

    }
}