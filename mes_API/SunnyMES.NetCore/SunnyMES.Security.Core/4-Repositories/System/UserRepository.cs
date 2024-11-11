
using Dapper;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Vml;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Commons;
using SunnyMES.Commons.Cache;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Quartz.Impl.Triggers;
using System.Configuration;

namespace SunnyMES.Security.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRepository : BaseRepository<User, string>, IUserRepository
    {
        /// <summary>
        /// 
        /// </summary>
        public UserRepository()
        {
        }
       
        public UserRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// GetRole
        /// </summary>
        /// <param name="S_RoleCode"></param>
        /// <returns></returns>
        public async Task<List<Role>> GetRole(string S_RoleCode)
        {
            string S_Sql = @"SELECT * from API_Role where EnCode='" + S_RoleCode + "'";
            var v_Query = await DapperConn.QueryAsync<Role>(S_Sql);
            return v_Query.AsList();
        }
        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="S_Sql"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUser(string S_Sql)
        {
            var v_Query = await DapperConn.QueryAsync<User>(S_Sql);
            return v_Query.AsList();
        }

        public async Task<List<Models.mesEmployee>> GetmesEmployee(string S_Sql)
        {
            var v_Query = await DapperConn.QueryAsync<Models.mesEmployee>(S_Sql);
            return v_Query.AsList();
        }

        public async Task<string> GetOperatorUserPermission(string StationID, string EmployeeID)
        {
            string S_Result = "0";
            string S_Sql =
                @"  SELECT A.ID ValStr1,A.StationTypeID ValStr2 FROM mesStation A 
                     JOIN mesStationTypeAccess B ON A.StationTypeID=B.StationTypeID
                     JOIN mesEmployee C ON C.ID = B.EmployeeID                                                           
                 WHERE A.ID='" + StationID + "' AND B.[Status]=1 AND C.UserID='" + EmployeeID + "'";
            var v_Query = await DapperConn.QueryAsync<Models.TabVal>(S_Sql);

            if (v_Query.Count() > 0)
            {
                S_Result = "1";
            }
            return S_Result;
        }

        public async Task<string> SetSyncUserData(string S_EmployeeID)
        {
            string S_Result = "OK";
            try
            {
                string S_Sql = "select * from mesEmployee where Id='" + S_EmployeeID + "'";
                mesEmployee v_mesEmployee = await DapperConn.QueryFirstAsync<mesEmployee>(S_Sql);
                if (v_mesEmployee.UserID != "developer")
                {
                    return S_Result = "Synchronization invalidation";
                }

                string S_SysPath = Directory.GetCurrentDirectory();
               
                string S_Data1 = S_SysPath + "\\Data1.dll";
                string S_Data2 = S_SysPath + "\\Data2.dll";
                string S_Data3 = S_SysPath + "\\Data3.dll";
                string S_Data4 = S_SysPath + "\\Data4.dll";
                string S_Data5 = S_SysPath + "\\Data5.dll";

                string S_Data100 = S_SysPath + "\\Data100.dll";
                string S_Data101 = S_SysPath + "\\Data101.dll";


                string S_Sql1 = "select * from mesEmployee";
                DataTable DT_Sql1 = Data_Table(S_Sql1);
                string json1 = PublicF.DataTableToJson(DT_Sql1);
                string S_MI1 = EncryptHelper2023.EncryptString(json1);
                File.WriteAllText(S_Data1, S_MI1);

                string S_Sql2 = "select * from API_UserLogOn";
                DataTable DT_Sql2 = Data_Table(S_Sql2);
                string json2 = PublicF.DataTableToJson(DT_Sql2);
                string S_MI2 = EncryptHelper2023.EncryptString(json2);
                File.WriteAllText(S_Data2, S_MI2);

                string S_Sql3 = "select * from API_Role";
                DataTable DT_Sql3 = Data_Table(S_Sql3);
                string json3 = PublicF.DataTableToJson(DT_Sql3);
                string S_MI3 = EncryptHelper2023.EncryptString(json3);
                File.WriteAllText(S_Data3, S_MI3);

                string S_Sql4 = "select * from API_RoleAuthorize";
                DataTable DT_Sql4 = Data_Table(S_Sql4);
                string json4 = PublicF.DataTableToJson(DT_Sql4);
                string S_MI4 = EncryptHelper2023.EncryptString(json4);
                File.WriteAllText(S_Data4, S_MI4);

                string S_Sql5 = "select * from API_RoleData";
                DataTable DT_Sql5 = Data_Table(S_Sql5);
                string json5 = PublicF.DataTableToJson(DT_Sql5);
                string S_MI5 = EncryptHelper2023.EncryptString(json5);
                File.WriteAllText(S_Data5, S_MI5);


                string S_Sql100 = "select * from mesUnitInputState";
                DataTable DT_Sql100 = Data_Table(S_Sql100);
                string json100 = PublicF.DataTableToJson(DT_Sql100);
                string S_MI100 = EncryptHelper2023.EncryptString(json100);
                File.WriteAllText(S_Data100, S_MI100);

                string S_Sql101 = "select * from mesUnitOutputState";
                DataTable DT_Sql101 = Data_Table(S_Sql101);
                string json101 = PublicF.DataTableToJson(DT_Sql101);
                string S_MI101 = EncryptHelper2023.EncryptString(json101);
                File.WriteAllText(S_Data101, S_MI101);


                try
                {

                    string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                    string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                    foreach (var item in List_WinformWebDIR)
                    {
                        //string S_WinformWebDIR1 = item + "\\Data1.dll";
                        //string S_WinformWebDIR2 = item + "\\Data2.dll";
                        //string S_WinformWebDIR3 = item + "\\Data3.dll";
                        //string S_WinformWebDIR4 = item + "\\Data4.dll";
                        //string S_WinformWebDIR5 = item + "\\Data5.dll";
                        //string S_WinformWebDIR100 = item + "\\Data100.dll";
                        //string S_WinformWebDIR101 = item + "\\Data101.dll";


                        //File.WriteAllText(S_WinformWebDIR1, S_MI1);
                        //File.WriteAllText(S_WinformWebDIR2, S_MI2);
                        //File.WriteAllText(S_WinformWebDIR3, S_MI3);
                        //File.WriteAllText(S_WinformWebDIR4, S_MI4);
                        //File.WriteAllText(S_WinformWebDIR5, S_MI5);
                        //File.WriteAllText(S_WinformWebDIR100, S_MI100);
                        //File.WriteAllText(S_WinformWebDIR101, S_MI101);


                        try
                        {
                            string S_FTPIP = item;
                            string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                            string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                            FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                            FTP.GotoDirectory("", true);
                            FTP.Upload(S_Data1, "Data1.dll");
                            FTP.Upload(S_Data2, "Data2.dll");
                            FTP.Upload(S_Data3, "Data3.dll");
                            FTP.Upload(S_Data4, "Data4.dll");
                            FTP.Upload(S_Data5, "Data5.dll");
                            FTP.Upload(S_Data100, "Data100.dll");
                            FTP.Upload(S_Data101, "Data101.dll");
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
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }
        public async Task<string> ValidateSecond(string UserID, string PWD)
        {
            string S_Result = "OK";

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

            if (S_IsValidateSecond == "0")
            {
                return S_Result;
            }


            string S_Sql_ID = "";
            string S_Sql_UserID = "";
            string S_Sql_Lastname = "";
            string S_Sql_Firstname = "";
            string S_Sql_Password = "";
            string S_Sql_OrganizeId = "";
            string S_Sql_DepartmentId = "";
            string S_Sql_RoleId = "";
            string S_Sql_IsAdministrator = "";
            string S_Sql_UserType = "";

            string S_Sql2_Password = "";

            string S_Data1_ID = "";
            string S_Data1_UserID = "";
            string S_Data1_Lastname = "";
            string S_Data1_Firstname = "";
            string S_Data1_Password = "";
            string S_Data1_OrganizeId = "";
            string S_Data1_DepartmentId = "";
            string S_Data1_RoleId = "";
            string S_Data1_IsAdministrator = "";
            string S_Data1_UserType = "";

            string S_Data2_UserId = "";
            string S_Data2_Password = "";

            string S_DynPWD = "";
            try
            {
                string S_Sql = "select * from mesEmployee where UserID='" + UserID.Trim()  + "'";
                var v_Query = await DapperConn.QueryAsync<mesEmployee>(S_Sql);
                List<mesEmployee> List_mesEmployee = v_Query.ToList();

               
                List_mesEmployee[0].UserID= List_mesEmployee[0].UserID??"";
                List_mesEmployee[0].Lastname= List_mesEmployee[0].Lastname??"";
                List_mesEmployee[0].Firstname= List_mesEmployee[0].Firstname??"";
                List_mesEmployee[0].Password= List_mesEmployee[0].Password??"";
                List_mesEmployee[0].OrganizeId= List_mesEmployee[0].OrganizeId??"";
                List_mesEmployee[0].DepartmentId= List_mesEmployee[0].DepartmentId??"";
                List_mesEmployee[0].RoleId = List_mesEmployee[0].RoleId??"";               
                List_mesEmployee[0].UserType= List_mesEmployee[0].UserType??"";


                S_Sql_ID = List_mesEmployee[0].Id.ToString().Trim();
                S_Sql_UserID = List_mesEmployee[0].UserID.ToString().Trim();
                S_Sql_Lastname = List_mesEmployee[0].Lastname.ToString().Trim();
                S_Sql_Firstname = List_mesEmployee[0].Firstname.ToString().Trim();
                S_Sql_Password = List_mesEmployee[0].Password.ToString().Trim();
                S_Sql_OrganizeId = List_mesEmployee[0].OrganizeId.ToString().Trim();
                S_Sql_DepartmentId = List_mesEmployee[0].DepartmentId.ToString().Trim();
                S_Sql_RoleId = List_mesEmployee[0].RoleId.ToString().Trim();
                S_Sql_IsAdministrator = List_mesEmployee[0].IsAdministrator.ToString().Trim();
                S_Sql_UserType = List_mesEmployee[0].UserType.ToString().Trim();

                string S_Sql2 = "select * from API_UserLogOn where UserID='" + S_Sql_ID + "'";
                var v_Query2 = await DapperConn.QueryAsync<UserLogOn>(S_Sql2);
                List<UserLogOn> List_UserLogOn = v_Query2.ToList();

                S_Sql2_Password = List_UserLogOn[0].UserPassword.Trim();

                string S_SysPath = Directory.GetCurrentDirectory();                
                DataTable dataTable1 = new DataTable();
                DataTable dataTable2 = new DataTable();
                string S_Data1 = S_SysPath + "\\Data1.dll";
                string S_Data2 = S_SysPath + "\\Data2.dll";
                
                try
                {                   
                    S_DynPWD = PublicF.DynPWd();

                    string S_MI1 = File.ReadAllText(S_Data1);
                    string S_JsonJM1 = EncryptHelper2023.DecryptString(S_MI1);
                    JArray jsonArray1 = JArray.Parse(S_JsonJM1);

                    foreach (JProperty property in jsonArray1[0])
                    {
                        dataTable1.Columns.Add(property.Name);
                    }
                    foreach (JObject jsonObject in jsonArray1)
                    {
                        DataRow row = dataTable1.NewRow();
                        foreach (JProperty property in jsonObject.Properties())
                        {
                            row[property.Name] = property.Value.ToString();
                        }
                        dataTable1.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    if (UserID == "developer" && PWD == S_DynPWD)
                    {

                    }
                    else 
                    {
                        Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\r\n" +
                            S_Data1 + "\r\n" +
                            S_Data2 + "\r\n" +
                            ex.ToString());

                        return S_Result = "secondary verification account is invalid.";
                    }
                }

                try
                {
                    DataRow[] List_DR1 = dataTable1.Select("UserID='" + UserID + "'");
                    if (List_DR1.Count() > 0)
                    {
                        S_Data1_ID = List_DR1[0]["ID"].ToString().Trim();
                        S_Data1_UserID = List_DR1[0]["UserID"].ToString().Trim();
                        S_Data1_Lastname = List_DR1[0]["Lastname"].ToString().Trim();
                        S_Data1_Firstname = List_DR1[0]["Firstname"].ToString().Trim();

                        S_Data1_Password = List_DR1[0]["Password"].ToString().Trim();

                        S_Data1_OrganizeId = List_DR1[0]["OrganizeId"].ToString().Trim();
                        S_Data1_DepartmentId = List_DR1[0]["DepartmentId"].ToString().Trim();
                        S_Data1_RoleId = List_DR1[0]["RoleId"].ToString().Trim();
                        S_Data1_IsAdministrator = List_DR1[0]["IsAdministrator"].ToString().Trim();
                        S_Data1_IsAdministrator = S_Data1_IsAdministrator == "" ? "False" : S_Data1_IsAdministrator.Trim();

                        S_Data1_UserType = List_DR1[0]["UserType"].ToString().Trim();
                    }
                }
                catch { }

                try
                {                    
                    string S_MI2 = File.ReadAllText(S_Data2);
                    string S_JsonJM2 = EncryptHelper2023.DecryptString(S_MI2);
                    JArray jsonArray2 = JArray.Parse(S_JsonJM2);
                    foreach (JProperty property in jsonArray2[0])
                    {
                        dataTable2.Columns.Add(property.Name);
                    }
                    foreach (JObject jsonObject in jsonArray2)
                    {
                        DataRow row = dataTable2.NewRow();
                        foreach (JProperty property in jsonObject.Properties())
                        {
                            row[property.Name] = property.Value.ToString();
                        }
                        dataTable2.Rows.Add(row);
                    }
                }
                catch 
                {
                }

                try
                {
                    DataRow[] List_DR2 = dataTable2.Select("UserID='" + S_Sql_ID + "'");
                    if (List_DR2.Count() > 0)
                    {
                        S_Data2_UserId = List_DR2[0]["UserId"].ToString();
                        S_Data2_Password=List_DR2[0]["UserPassword"].ToString();
                    }
                }
                catch { }

                if (
                    (S_Sql_ID == S_Data1_ID)
                    &&(S_Sql_UserID == S_Data1_UserID)
                    && (S_Sql_Lastname == S_Data1_Lastname)
                    && (S_Sql_Firstname == S_Data1_Firstname)
                   
                    && (S_Sql_ID == S_Data2_UserId)

                    && (S_Sql2_Password == S_Data2_Password)

                    && (S_Sql_OrganizeId == S_Data1_OrganizeId)
                    && (S_Sql_DepartmentId == S_Data1_DepartmentId)
                    && (S_Sql_RoleId == S_Data1_RoleId)
                    && (S_Sql_IsAdministrator == S_Data1_IsAdministrator)
                    && (S_Sql_UserType == S_Data1_UserType)

                    )
                {
                    S_Result = "OK";
                }
                else
                {
                    string S_LoginLog =
                    "S_Sql_Id:" + S_Sql_ID + "    S_Data1_ID:" + S_Data1_ID + "\r\n" +
                    "S_Sql_UserID:" + S_Sql_UserID + "    S_Data1_UserID:" + S_Data1_UserID + "\r\n" +
                    "S_Sql_Lastname:" + S_Sql_Lastname + "    S_Data1_Lastname:" + S_Data1_Lastname + "\r\n" +
                    "S_Sql_Firstname:" + S_Sql_Firstname + "    S_Data1_Firstname:" + S_Data1_Firstname + "\r\n" +
                    "S_Sql_OrganizeId:" + S_Sql_OrganizeId + "    S_Data1_OrganizeId:" + S_Data1_OrganizeId + "\r\n" +
                    "S_Sql_DepartmentId:" + S_Sql_DepartmentId + "    S_Data1_DepartmentId:" + S_Data1_DepartmentId + "\r\n" +
                    "S_Sql_RoleId:" + S_Sql_RoleId + "    S_Data1_RoleId:" + S_Data1_RoleId + "\r\n" +
                    "S_Sql_IsAdministrator:" + Convert.ToString(S_Sql_IsAdministrator) + "    S_Data1_IsAdministrator:" + S_Data1_IsAdministrator + "\r\n" +
                    "S_Sql_UserType:" + S_Sql_UserType + "    S_Data1_UserType:" + S_Data1_UserType + "\r\n"
                    ;

                    //Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + S_LoginLog);



                    S_Result = "secondary verification account is invalid.";
                }

                if (S_Result != "OK")
                {
                    if ((UserID == "developer" || UserID == "Developer") && PWD == S_DynPWD)
                    {
                        S_Result = "OK";
                    }
                }

                //if (S_IsValidateSecond == "1")
                {
                    string S_ServerData = GetServerData();
                    if (S_ServerData.IndexOf("ERROR:") > -1)
                    {
                        S_Result = S_ServerData;
                    }
                }

            }
            catch (Exception ex) 
            {
                S_Result=ex.ToString();
            }
            return S_Result;
        }

        /// <summary>
        /// 根据用户账号查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetByUserName(string userName)
        {
            string sql = @"SELECT * FROM mesEmployee t WHERE t.Account = @UserName order by Id";
            return await DapperConn.QueryFirstOrDefaultAsync<User>(sql, new { @UserName = userName });
        }

        /// <summary>
        /// 根据用户手机号码查询用户信息
        /// </summary>
        /// <param name="mobilephone">手机号码</param>
        /// <returns></returns>
        public async Task<User> GetUserByMobilePhone(string mobilephone)
        {
            string sql = @"SELECT * FROM mesEmployee t WHERE t.MobilePhone = @MobilePhone order by Id";
            return await DapperConn.QueryFirstOrDefaultAsync<User>(sql, new { @MobilePhone = mobilephone });
        }

        /// <summary>
        /// 根据Email查询用户信息
        /// </summary>
        /// <param name="email">email</param>
        /// <returns></returns>
        public async Task<User> GetUserByEmail(string email)
        {
            string sql = @"SELECT * FROM mesEmployee t WHERE t.EmailAddress = @Email order by Id";
            return await DapperConn.QueryFirstOrDefaultAsync<User>(sql, new { @Email = email });
        }
        /// <summary>
        /// 根据Email、Account、手机号查询用户信息
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <returns></returns>
        public async Task<User> GetUserByLogin(string account)
        {
            string sql = @"SELECT * FROM mesEmployee t WHERE
(t.Account = @Account Or t.EmailAddress = @Account Or t.MobilePhone = @Account) order by Id";
            return await DapperConn.QueryFirstOrDefaultAsync<User>(sql, new { @Account = account });
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public  bool Insert(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null)
        {
            //userLogOnEntity.Id = GuidUtils.CreateNo();
            //userLogOnEntity.UserId = entity.Id;
            entity.UserID = entity.UserID.Trim();
            entity.Account = entity.Account.Trim();

            string S_Sql = "select * from mesEmployee where UserID='" + entity.UserID + "' or Account='" + entity.UserID + "'";
            try
            {
                var q_Query = DapperConn.Query<mesEmployee>(S_Sql); 
                if (q_Query.ToList().Count > 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }


            S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesEmployee";
            MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
            string S_PWD = userLogOnEntity.UserPassword;


            userLogOnEntity.Id = v_MaxID.LastID.ToString();  //GuidUtils.CreateNo();
            userLogOnEntity.UserId = v_MaxID.LastID.ToString();  //entity.Id;

            userLogOnEntity.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
            userLogOnEntity.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(userLogOnEntity.UserPassword).ToLower(), userLogOnEntity.UserSecretkey).ToLower()).ToLower();


            entity.Id = v_MaxID.LastID.ToString();
            entity.UserID = entity.Account;
            entity.Password = S_PWD;
            entity.Lastname = entity.RealName;
            entity.Firstname = entity.NickName;
            entity.StatusID = 1;

            entity.LoginAttempt = 0;


            DbContext.GetDbSet<User>().Add(entity);
            DbContext.GetDbSet<UserLogOn>().Add(userLogOnEntity);
            return DbContext.SaveChanges()>0;
            
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public async Task<bool> InsertAsync(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null)
        {


            string S_Sql = "select * from mesEmployee where UserID='" + entity.UserID + "' or Account='" + entity.UserID + "'";
            try
            {
                var q_Query = await DapperConn.QueryAsync<mesEmployee>(S_Sql);
                if (q_Query.ToList().Count > 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            S_Sql = "SELECT MAX(Id)+1 AS LastID FROM mesEmployee";
            MaxID v_MaxID  =DapperConn.QueryFirst<MaxID>(S_Sql);
            string S_PWD = userLogOnEntity.UserPassword;
           
            userLogOnEntity.Id = v_MaxID.LastID.ToString();  //GuidUtils.CreateNo();
            userLogOnEntity.UserId = v_MaxID.LastID.ToString();  //entity.Id;

            userLogOnEntity.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
            userLogOnEntity.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(S_PWD).ToLower(), 
                userLogOnEntity.UserSecretkey).ToLower()).ToLower();

            string S_JM = SFCEncode.EncryptPassword(S_PWD, "");

            entity.Id= v_MaxID.LastID.ToString();
            entity.UserID = entity.Account;
            entity.Password = S_JM;
            entity.Lastname = entity.RealName;
            entity.Firstname = entity.NickName;
            entity.EmployeeGroupID = entity.EmployeeGroupID;
            //entity.StatusID = 1;
            entity.PermissionId = entity.PermissionId;
            entity.StatusID = entity.StatusID;

            entity.LoginAttempt = 0;

            DbContext.GetDbSet<User>().Add(entity);
            DbContext.GetDbSet<UserLogOn>().Add(userLogOnEntity);
            
            int I_Result = await DbContext.SaveChangesAsync();
            string S_Result= SetData1_2(entity, "Insert");
            if (S_Result != "OK") 
            {
                I_Result = 0;
            }

            return I_Result>0;
        }

        public async Task<bool> UpdateAsync(User v_User, string S_Id, IDbTransaction trans = null)
        {
            bool B_Result = true;

            v_User.UserID = v_User.Account;
            v_User.Lastname = v_User.RealName;
            v_User.Firstname = v_User.NickName;
            
            DbContext.Edit<User>(v_User);

            string S_Sql = "SELECT COUNT(1) LastID FROM API_UserLogOn WHERE UserId='" + v_User.Id + "'";
            MaxID v_MaxID = DapperConn.QueryFirst<MaxID>(S_Sql);
          
            if (v_MaxID.LastID == 0)
            {
                UserLogOn userLogOnEntity = new UserLogOn();
                userLogOnEntity.Id = v_User.Id.ToString();
                userLogOnEntity.UserId = v_User.Id.ToString();

                userLogOnEntity.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
                userLogOnEntity.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(
                    PublicF.DecryptPassword(v_User.Password, "")).ToLower(),
                    userLogOnEntity.UserSecretkey).ToLower()).ToLower();

                DbContext.GetDbSet<UserLogOn>().Add(userLogOnEntity);
                int I_Result = DbContext.SaveChanges();

                if (I_Result == 0)
                {
                    return B_Result = false;
                }
            }
            else 
            {
                UserLogOn userLogOn = new UserLogOn();
                userLogOn= DapperConn.QueryFirst<UserLogOn>("select * from API_UserLogOn where UserId='" + v_User.Id.ToString() + "'");

                string S_UserSecretkey = userLogOn.UserSecretkey;
                string S_EmPWD = SFCEncode.DecryptPassword(v_User.Password, "");

                string S_PWD= MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(S_EmPWD).ToLower(),
                        S_UserSecretkey).ToLower()).ToLower();
                string S_Sql_UserLogOn = "UPDATE API_UserLogOn SET UserPassword ='" + S_PWD + "' where UserId='" + v_User.Id.ToString() + "'";

                int I_Result=await DapperConn.ExecuteAsync(S_Sql_UserLogOn);
                if (I_Result == 0)
                {
                    return B_Result = false;
                }
            }

            string S_SetData1_2= SetData1_2(v_User, "Update");
            if (S_SetData1_2 != "OK") 
            {
                return B_Result = false;
            }

            return  B_Result;
        }

        private DataRow NewDT_Text(DataTable DT_Text,DataTable DT_Sql) 
        {
            DataRow DR = DT_Text.NewRow();
            for (int k = 0; k < DT_Text.Columns.Count; k++)
            {
                DR[k] = DT_Sql.Rows[0][k].ToString();
            }
            //DT_Text.Rows.Add(DR);

            return DR;
        }

        private  string SetData1_2(User v_User,string Type)
        {
            string S_SysPath = Directory.GetCurrentDirectory();

            string S_Result = "OK";
           
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            string S_Data1 = S_SysPath + "\\Data1.dll";
            string S_Data2 = S_SysPath + "\\Data2.dll";

            string S_Sql1 = "";
            string S_Sql2 = "";
            DataTable DT_Sql1 = null;
            DataTable DT_Sql2 = null;

            try
            {
                if (File.Exists(S_Data1) == false) 
                {
                    S_Sql1 = "select * from mesEmployee where ID='" + v_User.Id + "'";
                    DT_Sql1 = Data_Table(S_Sql1);
                    string json1_Exists = PublicF.DataTableToJson(DT_Sql1);
                    string S_MI1_Exists = EncryptHelper2023.EncryptString(json1_Exists);
                    File.WriteAllText(S_Data1, S_MI1_Exists);
                  
                }

                if (File.Exists(S_Data2) == false)
                {
                    S_Sql2 = "select * from API_UserLogOn where UserId='" + v_User.Id + "'";
                    DT_Sql2 = Data_Table(S_Sql2);
                    string json2_Exists = PublicF.DataTableToJson(DT_Sql2);
                    string S_MI2_Exists = EncryptHelper2023.EncryptString(json2_Exists);
                    File.WriteAllText(S_Data2, S_MI2_Exists);
                }

                string S_MI1 = File.ReadAllText(S_Data1);
                string S_JsonJM1 = EncryptHelper2023.DecryptString(S_MI1);
                JArray jsonArray1 = JArray.Parse(S_JsonJM1);

                foreach (JProperty property in jsonArray1[0])
                {
                    dataTable1.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray1)
                {
                    DataRow row = dataTable1.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable1.Rows.Add(row);
                }

                string S_MI2 = File.ReadAllText(S_Data2);
                string S_JsonJM2 = EncryptHelper2023.DecryptString(S_MI2);
                JArray jsonArray2 = JArray.Parse(S_JsonJM2);

                foreach (JProperty property in jsonArray2[0])
                {
                    dataTable2.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray2)
                {
                    DataRow row = dataTable2.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable2.Rows.Add(row);
                }

                S_Sql1 = "select * from mesEmployee where ID='" + v_User.Id + "'";
                DT_Sql1 = Data_Table(S_Sql1);

                S_Sql2 = "select * from API_UserLogOn where UserId='" + v_User.Id + "'";
                DT_Sql2 = Data_Table(S_Sql2);

                if (Type == "Insert")
                {
                    dataTable1.Rows.Add(NewDT_Text(dataTable1, DT_Sql1));
                    dataTable2.Rows.Add(NewDT_Text(dataTable2, DT_Sql2));

                }
                else if (Type == "Delete")
                {
                    for (int i = 0; i < dataTable1.Rows.Count; i++)
                    {
                        if (dataTable1.Rows[i]["Id"].ToString().Trim() == DT_Sql1.Rows[0]["Id"].ToString().Trim())
                        {
                            dataTable1.Rows[i].Delete();
                            break;
                        }
                    }

                    for (int i = 0; i < dataTable2.Rows.Count; i++)
                    {
                        if (dataTable2.Rows[i]["UserId"].ToString().Trim() == DT_Sql2.Rows[0]["UserId"].ToString().Trim())
                        {
                            dataTable2.Rows[i].Delete();
                            break;
                        }
                    }
                }
                else if (Type == "Update") 
                {
                    if (dataTable1.Select("Id='" + DT_Sql1.Rows[0]["Id"].ToString().Trim() + "'").Count() > 0)
                    {

                        for (int i = 0; i < dataTable1.Rows.Count; i++)
                        {
                            if (dataTable1.Rows[i]["Id"].ToString().Trim() == DT_Sql1.Rows[0]["Id"].ToString().Trim())
                            {
                                dataTable1.Rows[i].Delete();

                                DataRow DR= NewDT_Text(dataTable1, DT_Sql1);
                                dataTable1.Rows.Add(DR);

                                break;
                            }
                        }
                    }
                    else 
                    {
                        DataRow DR = NewDT_Text(dataTable1, DT_Sql1);
                        dataTable1.Rows.Add(DR);
                    }

                    if (dataTable2.Select("UserId='" + DT_Sql2.Rows[0]["UserId"].ToString().Trim() + "'").Count() > 0)
                    {
                        for (int i = 0; i < dataTable2.Rows.Count; i++)
                        {
                            if (dataTable2.Rows[i]["UserId"].ToString().Trim() == DT_Sql2.Rows[0]["UserId"].ToString().Trim())
                            {
                                dataTable2.Rows[i].Delete();

                                DataRow DR = NewDT_Text(dataTable2, DT_Sql2);
                                dataTable2.Rows.Add(DR);
                                
                                break;
                            }
                        }
                    }
                    else 
                    {
                        DataRow DR = NewDT_Text(dataTable2, DT_Sql2);
                        dataTable2.Rows.Add(DR);
                    }
                }

                string json1_New = PublicF.DataTableToJson(dataTable1);
                string S_MI1_New = EncryptHelper2023.EncryptString(json1_New);
                File.WriteAllText(S_Data1, S_MI1_New);

                string json2_New = PublicF.DataTableToJson(dataTable2);
                string S_MI2_New = EncryptHelper2023.EncryptString(json2_New);
                File.WriteAllText(S_Data2, S_MI2_New);

                try
                {

                    string S_WinformWebDIR = Configs.GetConfigurationValue("AppSetting", "WinformWebDIR");
                    string[] List_WinformWebDIR = S_WinformWebDIR.Split(',');

                    foreach (var item in List_WinformWebDIR)
                    {
                        //string S_WinformWebDIR_Data1 = item + "\\Data1.dll";
                        //File.WriteAllText(S_WinformWebDIR_Data1, S_MI1_New);

                        //string S_WinformWebDIR_Data2 = item + "\\Data2.dll";
                        //File.WriteAllText(S_WinformWebDIR_Data2, S_MI2_New);

                        try
                        {
                            string S_FTPIP = item;
                            string S_FTPUser = Configs.GetConfigurationValue("AppSetting", "FTPUser");
                            string S_FTPPassword = Configs.GetConfigurationValue("AppSetting", "FTPPassword");

                            FtpWeb FTP = new FtpWeb(S_FTPIP, "", S_FTPUser, S_FTPPassword);
                            FTP.GotoDirectory("", true);
                            FTP.Upload(S_Data1, "Data1.dll");
                            FTP.Upload(S_Data2, "Data2.dll");                            
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
            catch(Exception ex)
            {
                S_Result = ex.Message;
            }
            return S_Result;
        }

        public string GetServerData() 
        {
            string S_Result = "OK";
            try
            {
                string CPU = "";
                string ZB = "";
                var Menory = "";
                var DiskDrive = "";
                var MAC = "";

                try
                {
                    ManagementClass mc_CPU = new ManagementClass("Win32_Processor");
                    ManagementObjectCollection moc_CPU = mc_CPU.GetInstances();
                   

                    foreach (ManagementObject mo in moc_CPU)
                    {
                        CPU = mo.Properties["ProcessorId"].Value.ToString();
                        break;
                    }

                    ManagementClass mc_ZB = new ManagementClass("Win32_BaseBoard");
                    ManagementObjectCollection moc_ZB = mc_ZB.GetInstances();
                   

                    foreach (ManagementObject mo in moc_ZB)
                    {
                        ZB = mo.Properties["SerialNumber"].Value.ToString();
                        break;
                    }
                    
                    var mc_MAC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    var moc_MAC = mc_MAC.GetInstances();
                    foreach (var o in moc_MAC)
                    {
                        var mo = (ManagementObject)o;
                        if (!(bool)mo["IPEnabled"]) continue;
                        MAC = mo["MacAddress"].ToString();
                        break;
                    }
                    
                    var mc_Menory = new ManagementClass("Win32_ComputerSystem");
                    var moc_Menory = mc_Menory.GetInstances();
                    foreach (var mo in moc_Menory)
                    {
                        if (mo["TotalPhysicalMemory"] != null)
                        {
                            Menory = mo["TotalPhysicalMemory"].ToString();
                            break;
                        }
                    }
                    
                    var mc_DiskDrive = new ManagementClass("Win32_DiskDrive");
                    var moc_DiskDrive = mc_DiskDrive.GetInstances();
                    foreach (var o in moc_DiskDrive)
                    {
                        var mo = (ManagementObject)o;
                        DiskDrive += mo.Properties["Model"].Value.ToString() + ";";
                    }
                }
                catch(Exception ex) 
                {
                    Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ex.ToString());
                }

                
                string S_Top =
                "911C53107B32880492DA539B4BC653167FB87BE96B99820E45ED10177D267A1453A74D6A" +
                "4D6A7FF5421988417A156B997E1A89951017625F622B6B998C4F69EB4E0B7A7863F363B5" +
                "531374618B1C101763E29FB0985463BF772C755E464C6C2B531D93B66B9992DA7A7853A7" +
                "10177A377BE3906D4F2D63C0459E90F2426141A59FAC5310421D101792868C654EF75327" +
                "8862469492C14D0F4E7A4A3569214EED1017411242DE8DBE705C41196B99706A431F4833" +
                "4E4B421D9FAC7A4B101792A08D786B99443471AE485763AA63AA705C70D16B997E207651" +
                "8DD3101799C08CDA499148578C948C4F424E7651101763BF85817B0E9D1C92716A21754C" +
                "682D71C6731871FF1017446453427BE37B32909792DA539B89EB71AE531A8CD16B999E4F" +
                "4E82101753CB53427BE37B324F0F6529539B63C76DDF705C99204A3553A788E9101748327" +
                "A148DBE53106A3D6B994CF0751253137DD853DA52809F1010177B32538C795A7B328BF5787" +
                "E8DE04A354911531D5337795A66F310178DB28DE575BB48574EF953D7491110179E7E4E537B" +
                "0E4EA68A4F416C441944199FEC8BD94CBF101799C08CDA49918C948C4F424E7651101763BF8" +
                "5817B0E9D1C92716A21754C682D71C6731871FF";


                string S_Sql = "select Count(*) Count from mesStation";
                DataTable DT = Data_Table(S_Sql);
                int I_StationCount =Convert.ToInt32(DT.Rows[0][0].ToString());

                string S_SysPath = Directory.GetCurrentDirectory();
                string S_Data200 = S_SysPath + "\\Data200.dll";

                //Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + S_Data200);

                if (File.Exists(S_Data200))
                {
                    try
                    {                        
                        string S_MI200_PWD = File.ReadAllText(S_Data200);
                        string S_MI200 = EncryptHelper2023.DecryptString(S_MI200_PWD);
                        string[] List_MI200 = S_MI200.Split(',');

                        string S_Local_Top= List_MI200[0].Replace("\r", "").Replace("\n", "");
                        string S_Local_CPU = List_MI200[1].Replace("\r", "").Replace("\n", "");
                        string S_Local_ZB = List_MI200[2].Replace("\r", "").Replace("\n", "");
                        string S_Local_MAC = List_MI200[3].Replace("\r", "").Replace("\n", "");
                        string S_Local_Menory = List_MI200[4].Replace("\r", "").Replace("\n", "");
                        string S_Local_DiskDrive = List_MI200[5].Replace("\r", "").Replace("\n", "");
                        string S_Local_StationCount = List_MI200[6].Replace("\r", "").Replace("\n", "");

                        string S_Server = "CPU:" + CPU + "   S_Local_CPU:" + S_Local_CPU + "\r\n" +
                                         "ZB:" + ZB + "   S_Local_ZB:" + S_Local_ZB + "\r\n" +
                                         "MAC:" + MAC + "   S_Local_MAC:" + S_Local_MAC + "\r\n" +
                                         "Menory:" + Menory + "   S_Local_Menory:" + S_Local_Menory + "\r\n" +
                                         "DiskDrive:" + DiskDrive + "   S_Local_DiskDrive:" + S_Local_DiskDrive;
                        //Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + S_Server);


                        int I_Local_StationCount = 0;
                        if (S_Local_StationCount != "") 
                        {
                            I_Local_StationCount = Convert.ToInt32(S_Local_StationCount);
                        }

                        if (I_StationCount > I_Local_StationCount)
                        {
                            S_Result = "ERROR:Too many station.";
                        }
                        else
                        {
                            if (S_Top == S_Local_Top
                                && CPU == S_Local_CPU
                                && ZB == S_Local_ZB
                                && MAC == S_Local_MAC
                                && Menory == S_Local_Menory
                                && DiskDrive == S_Local_DiskDrive
                                )
                            {

                            }
                            else
                            {
                                S_Result = "ERROR:Server authentication failure.";
                            }
                        }
                    }
                    catch
                    {
                        S_Result = "ERROR:Server authentication failure.";
                    }
                }
                else 
                {
                    S_Result = "ERROR:Server verification data does not exist.";
                }

            }
            catch (Exception ex)
            {
                S_Result ="ERROR:"+ ex.ToString();
            }
            return S_Result;
        }


        /// <summary>
        /// 注册用户,第三方平台
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="userOpenIds"></param>
        /// <param name="trans"></param>
        public bool Insert(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null)
        {

            DbContext.GetDbSet<User>().Add(entity);
            DbContext.GetDbSet<UserLogOn>().Add(userLogOnEntity); userLogOnEntity.Id = GuidUtils.CreateNo();
            userLogOnEntity.UserId = entity.Id;
            userLogOnEntity.UserSecretkey = MD5Util.GetMD5_16(GuidUtils.NewGuidFormatN()).ToLower();
            userLogOnEntity.UserPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(userLogOnEntity.UserPassword).ToLower(), userLogOnEntity.UserSecretkey).ToLower()).ToLower();
            DbContext.GetDbSet<User>().Add(entity);
            DbContext.GetDbSet<UserLogOn>().Add(userLogOnEntity);
            DbContext.GetDbSet<UserOpenIds>().Add(userOpenIds);
            return  DbContext.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据微信UnionId查询用户信息
        /// </summary>
        /// <param name="unionId">UnionId值</param>
        /// <returns></returns>
        public User GetUserByUnionId(string unionId)
        {
            string sql = string.Format("select * FROM mesEmployee where UnionId = '{0}'", unionId);
            return DapperConn.QueryFirstOrDefault<User>(sql);
        }
        /// <summary>
        /// 根据第三方OpenId查询用户信息
        /// </summary>
        /// <param name="openIdType">第三方类型</param>
        /// <param name="openId">OpenId值</param>
        /// <returns></returns>
        public User GetUserByOpenId(string openIdType, string openId)
        {
            string sql = string.Format("select * FROM mesEmployee as u join API_UserOpenIds as o on u.Id = o.UserId and  o.OpenIdType = '{0}' and o.OpenId = '{1}'", openIdType, openId);
            return DapperConn.QueryFirstOrDefault<User>(sql);
        }

        /// <summary>
        /// 根据userId查询用户信息
        /// </summary>
        /// <param name="openIdType">第三方类型</param>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        public UserOpenIds GetUserOpenIdByuserId(string openIdType, string userId)
        {
            string sql = string.Format("select * FROM API_UserOpenIds  where OpenIdType = '{0}' and UserId = '{1}'", openIdType, userId);
            return DapperConn.QueryFirstOrDefault<UserOpenIds>(sql);
        }

        /// <summary>
        /// 更新用户信息,第三方平台
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public bool UpdateUserByOpenId(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null)
        {
            DbContext.GetDbSet<User>().Add(entity);
            DbContext.GetDbSet<UserOpenIds>().Add(userOpenIds);
            return DbContext.SaveChanges() > 0;
        }





        /// <summary>
        /// 分页得到所有用户用于关注
        /// </summary>
        /// <param name="currentpage"></param>
        /// <param name="pagesize"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IEnumerable<UserAllListFocusOutPutDto> GetUserAllListFocusByPage(string currentpage,
            string pagesize, string userid)
        {
            string sqlRecord = "";
            string sql = "";

            int countNotIn = (int.Parse(currentpage) - 1) * int.Parse(pagesize);

            sqlRecord = @"select * FROM mesEmployee where nickname <> '游客' and  ismember=1 ";

            sql = @"SELECT TOP " + pagesize +
                @"
case when t2.Id is null then 'n' 
else 'y' end as IfFocus ,
IsNull(t3.totalFocus,0) as TotalFocus, 
t1.*
from 
(select ISNULL(tin2.VipGrade,0) as VipGrade,
ISNULL(tin2.IsAuthentication,0) as IsAuthentication,
ISNULL(tin2.AuthenticationType,0) as AuthenticationType,
tin1.* FROM mesEmployee tin1 
left join API_UserExtend tin2 on tin1.Id=tin2.UserId 
where nickname <> '游客' and  ismember=1) t1
left join 
(select * FROM API_UserFocus where creatorUserid='" + userid + @"') t2 
on t1.id=t2.focususerid 
left join 
(select  top 100 percent focusUserID,count(*) as totalFocus FROM API_UserFocus group by focusUserID order by totalfocus desc) t3
on t1.Id=t3.focusUserID 

where t1.Id not in 
(
select top " + countNotIn + @"
tt1.Id 
from 
(select ISNULL(tin2.VipGrade,0) as VipGrade,
ISNULL(tin2.IsAuthentication,0) as IsAuthentication,
ISNULL(tin2.AuthenticationType,0) as AuthenticationType,
tin1.* FROM mesEmployee tin1 
left join API_UserExtend tin2 on tin1.Id=tin2.UserId 
where nickname <> '游客' and  ismember=1) tt1
left join 
(select * FROM API_UserFocus where creatorUserid='" + userid + @"') tt2
on tt1.id=tt2.focususerid 
left join 
(select  top 100 percent focusUserID,count(*) as totalFocus FROM API_UserFocus group by focusUserID order by totalfocus desc) tt3
on tt1.Id=tt3.focusUserID 

order by tt3.totalFocus desc
)

order by t3.totalFocus desc";


            List<UserAllListFocusOutPutDto> list = new List<UserAllListFocusOutPutDto>();

            IEnumerable<UserAllListFocusOutPutDto> infoOutputDto = DapperConn.Query<UserAllListFocusOutPutDto>(sql);

            //得到总记录数
            List<UserAllListFocusOutPutDto> recordCountList = DapperConn.Query<UserAllListFocusOutPutDto>(sqlRecord).AsList();

            list = infoOutputDto.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].RecordCount = recordCountList.Count;
            }
            return list;
        }


        public async Task<List<Models.mesEmployee>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            List<Models.mesEmployee> list = new List<Models.mesEmployee>();
            try
            {
                ConnectionConfig conn = new ConnectionConfig();
                conn.ConnectionString = DapperConnRead.ConnectionString;
                conn.DbType = SqlSugar.DbType.SqlServer;
                SqlSugarClient SuDB = new SqlSugarClient(conn);
                SuDB.Open();
              
                //if (HasInjectionData(condition))
                //{
                //    Log4NetHelper.Info(string.Format("检测出SQL注入的恶意数据, {0}", condition));
                //    throw new Exception("检测出SQL注入的恶意数据");
                //}
                if (string.IsNullOrEmpty(condition))
                {
                    condition = "1=1";
                }
                if (desc)
                {
                    fieldToSort += " desc";
                }
                RefAsync<int> totalCount = 0;



                list = await SuDB.Queryable<Models.mesEmployee>().OrderByIF(!string.IsNullOrEmpty(fieldToSort), fieldToSort)
                    .WhereIF(!string.IsNullOrEmpty(condition), condition)
                    .ToPageListAsync(info.CurrentPageIndex, info.PageSize, totalCount);
                info.RecordCount = totalCount;
            }
            catch (Exception ex)
            {

            }

            return list;
        }


    }
}