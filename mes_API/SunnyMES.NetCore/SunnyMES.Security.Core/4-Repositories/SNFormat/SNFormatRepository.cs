using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using static Dapper.SqlMapper;
using SunnyMES.Commons.Core.PublicFun;
using Quartz.Impl.Triggers;
using SunnyMES.Commons.Enums;
using API_MSG;

namespace SunnyMES.Security.Repositories
{
    public class SNFormatRepository : BaseRepositoryGeneric<TabVal,string>, ISNFormatRepository
    {
        
        public SNFormatRepository()
        {
        }
        
        public SNFormatRepository(IDbContextCoreGeneric dbContext) : base(dbContext)
        {            
        }

        public async Task<string> GetSNRGetNext(string S_SNFormat, string S_ReuseSNByStation,
            string S_ProdOrder, string S_Part, string S_Station, string S_ExtraData)
        {
            Boolean B_IsFirst = true;
            Boolean B_IsReset = false; 

            string S_CurrDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null; 
            Tuple<bool, string> Tuple_Result = null;

            string S_Start = "";string S_End = "";

     
            string S_Result = "";
            string S_Sql = "SELECT ID as ValStr1 FROM mesSNFormat WHERE NAME='" + S_SNFormat + "'";
            IEnumerable<TabVal> List_SNFormat = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            string S_SNFormatID = List_SNFormat.First().ValStr1;
            if (S_SNFormatID == "")
            {
                S_Result = "ERROR:"+ S_Sql;
                return S_Result;
            }

             S_Sql = @"select s.[ID] SID, 
	                        s.SectionType STypeID, 
	                        s.SectionParam SParam,
	                        s.Increment Increment, 
	                        s.InvalidChar , 
	                        s.LastUsed , 
	                        r.[ID] RID, 
	                        r.Start, 
	                        r.[End],
	                        r.[Order] CurROrder
                        from 	mesSNSection s with (updlock) left outer join
	                        mesSNRange r with (updlock) on r.SNSectionID = s.[ID] and r.StatusID = 1
                        where	s.SNFormatID = '"+ S_SNFormatID + @"'
                        order by s.[order]";

            IEnumerable<SNSectionRange> List_SNSectionRange = await DapperConnRead.QueryAsync<SNSectionRange>(S_Sql, null, null, I_DBTimeout, null);

            if (List_SNSectionRange.Count() == 0) 
            {
               S_Result = "ERROR:SNFormat Setting Not found";
               return S_Result;
            }

            S_Sql = "select * from mesSNSection where SNFormatID='" + S_SNFormatID + "'  AND (LastUsed IS NOT NULL OR LastUsed<>'')";
            IEnumerable<SNSection> List_SNSection = await DapperConnRead.QueryAsync<SNSection>(S_Sql, null, null, I_DBTimeout, null);
            if (List_SNSection.Count() > 0)
            {
                B_IsFirst = false;
            }


            string S_SNbuf = "";
            foreach (var item in List_SNSectionRange) 
            {
                S_Start = item.Start;
                S_End = item.End;

                S_Sql = "select * from mesSNSection where ID='" + item.SID + "'";
                List_SNSection = await DapperConnRead.QueryAsync<SNSection>(S_Sql, null, null, I_DBTimeout, null);
                SNSection v_SNSection = List_SNSection.First();

                SSType Enum_SSType = PublicF.ToEnum<SSType>(item.STypeID);
                if (Enum_SSType == SSType.FixedString)
                {
                    S_SNbuf = S_SNbuf + item.SParam;
                }
                else if (Enum_SSType == SSType.UdfProcedure)
                {
                    string S_SP_Value = "";
                    try
                    {
                        S_Sql = "declare @strOutput varchar(200) " + "\r\n" +
                                "exec " + item.SParam + "  '" + S_SNFormat + "','" + S_ProdOrder + "','" + S_Part + "','" +
                                 S_Station + "','" + S_ExtraData + "','" + S_SNbuf + "'," + "@strOutput OUTPUT" + "\r\n" +
                                 "select @strOutput ValStr1";

                        IEnumerable<TabVal> List_SP = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                        S_SP_Value = List_SP.First().ValStr1;
                        if (S_SP_Value != null)
                        {
                            S_SNbuf = S_SNbuf + S_SP_Value;
                        }
                        else
                        {
                            S_Result = "ERROR:" + item.SParam + " is null value";
                            return S_Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        S_Result = "ERROR:" + ex.Message;
                        return S_Result;
                    }


                    if (v_SNSection.AllowReset == true)
                    {
                        if (B_IsFirst == false)
                        {
                            if (v_SNSection.LastUsed != S_SP_Value)
                            {
                                List_SNSectionRange.Where(a => a.STypeID == "4").ToList().ForEach(x => x.LastUsed = null);

                                S_Sql = "update mesSNSection set LastUsed=null where SNFormatID='" + S_SNFormatID + "'  AND  SectionType=4" + "\r\n" +
                                        " insert into udt_SNRGetNextLog(strSNFormat,xmlProdOrder,xmlPart," +
                                            "xmlStation,xmlExtraData,strOutput,PreLastUsed,SID,STypeID) Values(" + "\r\n" +
                                        "'" + S_SNFormatID + "'," + "\r\n" +
                                        "'" + S_ProdOrder + "'," + "\r\n" +
                                        "'" + S_Part + "'," + "\r\n" +
                                        "'" + S_Station + "'," + "\r\n" +
                                        "'" + S_ExtraData + "'," + "\r\n" +
                                        "'" + S_SP_Value + "'," + "\r\n" +
                                        "'" + v_SNSection.LastUsed + "'," + "\r\n" +
                                        "'" + item.SID + "'," + "\r\n" +
                                        "'" + item.STypeID + "'"+
                                        ")"
                                        ;

                                B_IsReset = true;
                                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                List_Tuple.Add(Tuple_Val);
                                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                if (Tuple_Result.Item1 == false)
                                {
                                    S_Result = "ERROR:" + Tuple_Result.Item2;
                                    return S_Result;
                                }
                            }
                        }
                    }

                    S_Sql = "update mesSNSection set LastUsed ='" + S_SP_Value + "' where ID='" + item.SID + "'";
                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                    List_Tuple.Add(Tuple_Val);
                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                    if (Tuple_Result.Item1 == false)
                    {
                        S_Result = "ERROR:" + Tuple_Result.Item2;
                        return S_Result;
                    }
                }
                else if (Enum_SSType == SSType.DateTime)
                {
                    //S_Sql = "select [dbo].[ufnSNRRegDateTimeIF]('" + item.SParam + "','" + S_CurrDate + "') as ValStr1 ";
                    //IEnumerable<TabVal> List_DateTimeIF = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                    //string S_DateTimeIF = List_DateTimeIF.First().ValStr1 ?? "";

                    string S_DateTimeIF = PublicF.GetSNRRegDateTime(item.SParam, Convert.ToDateTime(S_CurrDate));
                    if (S_DateTimeIF != "")
                    {
                        S_SNbuf = S_SNbuf + S_DateTimeIF;
                    }
                    else
                    {
                        S_Result = "ERROR:" + item.SParam + " is null value";
                        return S_Result;
                    }


                    if (v_SNSection.AllowReset == true)
                    {
                        if (B_IsFirst == false)
                        {
                            if (v_SNSection.LastUsed != S_DateTimeIF)
                            {
                                List_SNSectionRange.Where(a => a.STypeID == "4").ToList().ForEach(x => x.LastUsed = null);

                                S_Sql = "update mesSNSection set LastUsed=null where SNFormatID='" + S_SNFormatID + "'  AND  SectionType=4" + "\r\n" +
                                        " insert into udt_SNRGetNextLog(strSNFormat,xmlProdOrder,xmlPart," +
                                            "xmlStation,xmlExtraData,strOutput,PreLastUsed,SID,STypeID) Values(" + "\r\n" +
                                        "'" + S_SNFormatID + "'," + "\r\n" +
                                        "'" + S_ProdOrder + "'," + "\r\n" +
                                        "'" + S_Part + "'," + "\r\n" +
                                        "'" + S_Station + "'," + "\r\n" +
                                        "'" + S_ExtraData + "'," + "\r\n" +
                                        "'" + v_SNSection.LastUsed + "'," + "\r\n" +
                                        "'" + S_DateTimeIF + "'," + "\r\n" +
                                        "'" + item.SID + "'," + "\r\n" +
                                        "'" + item.STypeID + "'"+
                                        ")"
                                        ;

                                B_IsReset = true;
                                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                List_Tuple.Add(Tuple_Val);
                                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                if (Tuple_Result.Item1 == false)
                                {
                                    S_Result = "ERROR:" + Tuple_Result.Item2;
                                    return S_Result;
                                }
                            }
                        }
                    }
                    S_Sql = "update mesSNSection set LastUsed ='" + S_DateTimeIF + "' where ID='" + item.SID + "'";
                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                    List_Tuple.Add(Tuple_Val);
                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                    if (Tuple_Result.Item1 == false)
                    {
                        S_Result = "ERROR:" + Tuple_Result.Item2;
                        return S_Result;
                    }
                }
                else if (Enum_SSType == SSType.Counter)
                {
                    Boolean B_r_reset = false;
                    Boolean B_dt_reset = false;
                    string S_strOutput = item.SParam;
                    string S_RID = item.RID;

                    B_IsReset = true;
                    if (B_IsReset == false) 
                    {
                        if (B_IsFirst == false) 
                        {
                            List_SNSectionRange.Where(a => a.STypeID == "4").ToList().ForEach(x => x.LastUsed = null);
                            S_Sql = "update mesSNSection set LastUsed=null where SNFormatID='" + S_SNFormatID + "'  AND  SectionType=4" + "\r\n" +
                                    "update mesSNRange set StatusID=0 where SNSectionID='"+ item.SID + "' and StatusID=2	 ";

                            Tuple_Val = new Tuple<string, object>(S_Sql, null);
                            List_Tuple.Add(Tuple_Val);
                            Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                            if (Tuple_Result.Item1 == false)
                            {
                                S_Result = "ERROR:" + Tuple_Result.Item2;
                                return S_Result;
                            }
                        }
                    }

                    S_RID = S_RID ?? "0";
                    IEnumerable<SNRange> List_SNRange=new  List<SNRange>();
                    if (S_RID == "0")
                    {
                        S_Sql = "select top 1 * from mesSNRange where SNSectionID='" + item.SID + "' and StatusID = 0 order by [Order]";
                        List_SNRange = await DapperConnRead.QueryAsync<SNRange>(S_Sql, null, null, I_DBTimeout, null);

                        if (List_SNRange.Count() == 0) 
                        {
                            S_Result = "ERROR:SN Section's Range Not Found";
                            return S_Result;
                        }
                        B_r_reset = true;
                        S_RID= List_SNRange.First().ID.ToString();
                        S_Start = List_SNRange.First().Start;
                        S_End= List_SNRange.First().End;

                        S_Sql = "update mesSNRange set [StatusID] = 1 where [ID]='" + List_SNRange.First().ID + "'";
                        Tuple_Val = new Tuple<string, object>(S_Sql, null);
                        List_Tuple.Add(Tuple_Val);
                        Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                        if (Tuple_Result.Item1 == false)
                        {
                            S_Result = "ERROR:" + Tuple_Result.Item2;
                            return S_Result;
                        }
                    }

                    //S_Sql = "select mask, pos, [len], type, luVal from dbo.ufnSNRParseParamIF('" +
                    //            item.LastUsed + "','" + item.SParam + "' ) order by [type]";
                    //IEnumerable<T_Par> List_T_Par = await DapperConnRead.QueryAsync<T_Par>(S_Sql, null, null, I_DBTimeout, null);

                    List<Public_SNPar> List_T_Par =  PublicF.SNRParseParam(item.LastUsed, item.SParam);

                    if (List_T_Par.Count() == 0) 
                    {
                        S_Result = "ERROR:"+ S_Sql;
                        return S_Result;
                    }

                    foreach (var item_Par in List_T_Par) 
                    {
                        PType Enum_PType = PublicF.ToEnum<PType>(item_Par.type.ToString());

                        string S_ParVal = "";
                        if (Enum_PType == PType.DateTime)
                        {
                            //S_Sql = "select dbo.ufnSNRDateTimeIF('#', '" + item_Par.mask + "', '" + S_CurrDate + "', '" +
                            //            item.InvalidChar + "') as ValStr1";
                            //IEnumerable<TabVal> List_SP = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                            //S_ParVal = List_SP.First().ValStr1;

                            S_ParVal=PublicF.GetSNRDateTime("#", item_Par.mask,Convert.ToDateTime(S_CurrDate), item.InvalidChar);

                            if (S_ParVal == "")
                            {
                                S_Result = "ERROR:" + S_Sql;
                                return S_Result;
                            }

                            if (S_ParVal != item_Par.luVal) { B_dt_reset = true; }

                            if (S_strOutput != item.Start)
                            {
                                S_Sql = "select stuff('" + S_strOutput + "'," + item_Par.pos + "," + item_Par.len + ",'" + S_ParVal + "') as ValStr1";
                                IEnumerable<TabVal> List_stuff = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                                string S_stuff = List_stuff.First().ValStr1;

                                if (S_stuff == "")
                                {
                                    S_Result = "ERROR:" + S_Sql;
                                    return S_Result;
                                }
                                S_strOutput = S_stuff;

                                //S_strOutput = PublicF.Stuff(S_strOutput, Convert.ToInt32(item_Par.pos), Convert.ToInt32(item_Par.len), Convert.ToChar(S_ParVal));
                            }
                        }
                        else if (Enum_PType == PType.Counter) 
                        {
                            item.LastUsed = item.LastUsed ?? "";
                            if (item.LastUsed == "")
                            {
                                S_strOutput = item.Start;
                            }
                            else 
                            {
                                IEnumerable<SNRange> List_SNRange_Par=new  List<SNRange>();
                                if (B_dt_reset == true)
                                {
                                    S_Sql = "update mesSNRange set [StatusID] = 0 where [ID] = '" + S_RID + "'";
                                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                    List_Tuple.Add(Tuple_Val);
                                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                    if (Tuple_Result.Item1 == false)
                                    {
                                        S_Result = "ERROR:" + Tuple_Result.Item2;
                                        return S_Result;
                                    }

                                    //List_SNSectionRange.Where(a => a.SID==item.SID).ToList().ForEach(x => x.RID="0");

                                    S_RID = "0";
                                    S_Sql = "select top 1 *,'1' CurROrder from mesSNRange where SNSectionID='" + item.SID +
                                                "' and StatusID = 0 order by  [Order]";
                                    List_SNRange_Par = await DapperConnRead.QueryAsync<SNRange>(S_Sql, null, null, I_DBTimeout, null);
                                    if (List_SNRange_Par.Count() == 0)
                                    {
                                        S_Result = "ERROR:  Can't get first range id " + S_Sql;
                                        return S_Result;
                                    }
                                    S_Start=List_SNRange_Par.First().Start.ToString(); 
                                    S_End= List_SNRange_Par.First().End.ToString();


                                    S_RID = List_SNRange_Par.First().ID.ToString(); 
                                    S_Sql = "update mesSNRange set [StatusID] = 1  where ID='" + S_RID + "'";
                                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                    List_Tuple.Add(Tuple_Val);
                                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                    if (Tuple_Result.Item1 == false)
                                    {
                                        S_Result = "ERROR:" + Tuple_Result.Item2;
                                        return S_Result;
                                    }
                                    S_ParVal = S_Start;

                                }
                                else 
                                {
                                    if (B_r_reset == true)
                                    {
                                        S_ParVal = S_Start;
                                    }
                                    else 
                                    {
                                        if (item_Par.luVal == S_End)
                                        {
                                            S_Sql = "update mesSNRange set [StatusID] = 2  where ID='" + S_RID + "'";
                                            Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                            List_Tuple.Add(Tuple_Val);
                                            Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                            if (Tuple_Result.Item1 == false)
                                            {
                                                S_Result = "ERROR:" + Tuple_Result.Item2;
                                                return S_Result;
                                            }

                                            S_RID = "0";

                                            S_Sql = "select top 1 *,Order CurROrder from mesSNRange where SNSectionID='" +
                                                        item.SID + "' and	StatusID = 0 order by  [Order]";

                                            IEnumerable<SNRange> List_SNRange_Par_V2 = await DapperConnRead.QueryAsync<SNRange>(S_Sql, null, null, I_DBTimeout, null);
                                            if (List_SNRange_Par_V2.Count() == 0)
                                            {
                                                S_Result = "ERROR:  Can't find range id " + S_Sql;
                                                return S_Result;
                                            }
                                            S_RID = List_SNRange_Par_V2.First().ID.ToString();
                                            S_Start = List_SNRange_Par_V2.First().Start.ToString();
                                            S_End = List_SNRange_Par_V2.First().End.ToString();

                                            S_Sql = "update mesSNRange set [StatusID] = 1  where ID='" + S_RID + "'";
                                            Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                            List_Tuple.Add(Tuple_Val);
                                            Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                            if (Tuple_Result.Item1 == false)
                                            {
                                                S_Result = "ERROR:" + Tuple_Result.Item2;
                                                return S_Result;
                                            }

                                            S_ParVal = S_Start;
                                        }
                                        else 
                                        {
                                            //S_Sql = "select dbo.ufnSNRPureCounterIF('"+item_Par.luVal+"', '"+ item_Par.mask +
                                            //        "','"+ item.Increment + "','"+item.InvalidChar+ "') as ValStr1";
                                            //IEnumerable<TabVal> List_CounterIF = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                                            //S_ParVal = List_CounterIF.First().ValStr1;

                                            S_ParVal = PublicF.GetSNRPureCounter(item_Par.luVal, item_Par.mask, 
                                                Convert.ToInt32(item.Increment), item.InvalidChar);

                                            if (S_ParVal == "")
                                            {
                                                S_Result = "ERROR:" + S_Sql;
                                                return S_Result;
                                            }

                                            if (S_Start.CompareTo(S_ParVal) >0 || S_ParVal.CompareTo(S_End)>0)
                                            {
                                                S_Result = "ERROR: Running Number not inside the range";
                                                return S_Result;
                                            }
                                            
                                            if (S_ParVal.CompareTo(S_End)==0)
                                            {
                                                S_Sql = "update mesSNRange set [StatusID] = 2  where ID='" + S_RID + "'";
                                                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                                                List_Tuple.Add(Tuple_Val);
                                                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                                                if (Tuple_Result.Item1 == false)
                                                {
                                                    S_Result = "ERROR:" + Tuple_Result.Item2;
                                                    return S_Result;
                                                }
                                            }                                            
                                        }
                                    }
                                }
                            }

                            if (S_strOutput != item.Start)
                            {
                                S_Sql = "select stuff('" + S_strOutput + "'," + item_Par.pos + "," + item_Par.len + ",'" + S_ParVal + "') as ValStr1";
                                IEnumerable<TabVal> List_stuff = await DapperConnRead.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                                string S_stuff = List_stuff.First().ValStr1;

                                if (S_stuff == "")
                                {
                                    S_Result = "ERROR:" + S_Sql;
                                    return S_Result;
                                }
                                S_strOutput = S_stuff;

                                //S_strOutput = PublicF.Stuff(S_strOutput, Convert.ToInt32(item_Par.pos), Convert.ToInt32(item_Par.len), Convert.ToChar(S_ParVal));
                            }                            
                        }
                    }

                    S_Sql = "update mesSNSection set LastUsed ='" + S_strOutput+"'  where ID='" + item.SID + "'";
                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                    List_Tuple.Add(Tuple_Val);
                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                    if (Tuple_Result.Item1 == false)
                    {
                        S_Result = "ERROR:" + Tuple_Result.Item2;
                        return S_Result;
                    }

                    S_SNbuf +=S_strOutput;
                }
            }
            S_Result = S_SNbuf.Trim();

            return S_Result;
        }

    }
}