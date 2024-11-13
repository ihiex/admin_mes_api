using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.PublicFun
{
    /// <summary>
    /// PublicF
    /// </summary>
    public static class PublicF
    {
        /// <summary>
        /// Stuff
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="replaceChar"></param>
        /// <returns></returns>
        public static string Stuff(this string str, int startPosition, int length, char replaceChar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            string result = "";
            if (startPosition < 0)
                return "";
            result = str.Substring(0, startPosition) + "".PadLeft(length, replaceChar);
            var indexNew = startPosition + length;
            if (indexNew <= str.Length - 1)
                result += str.Substring(indexNew);
            return result;
        }


        /// <summary>
        /// 字符串转Enum
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="str">字符串</param>
        /// <returns>转换的枚举</returns>
        public static T ToEnum<T>(string str)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), str);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// GetSNRRegDateTime
        /// </summary>
        /// <param name="S_Mask"></param>
        /// <param name="Date_Time"></param>
        /// <returns></returns>
        public static string GetSNRRegDateTime(string S_Mask,DateTime Date_Time) 
        {
            string S_RetVal = "";
            string S_SingleDSet = " 123456789ABCDEFGHIJKLMNOPQRSTUV";
            string S_SingleMSet = " 123456789ABC";
            string S_SingleHSet = " 0123456789ABCDEFGHIJKLMN";
            string S_SingleBSet = " 0123456789ABC123456789AB";

            string S_Temp1 = "";
            string S_MainWeek = null;
            string S_CurChar = "";
            string S_Temp = "";

            int I_Temp = 0;
            int I_Temp2 = 0;
            int I_Temp3 = 0;
            int I_Temp4 = 0;

            S_Mask = " " + S_Mask+" ";
            int I_Count = S_Mask.Length;
            int i = 1;

            while (i< I_Count)
            {
                I_Temp = i;
                S_CurChar = S_Mask.Substring(i, 1);                
                S_Temp = S_CurChar;

                if (S_CurChar.Trim() == "") 
                {
                    return S_RetVal;
                }

                //while (S_CurChar == S_Temp)
                while (string.Equals(S_CurChar, S_Temp, StringComparison.OrdinalIgnoreCase))
                {
                    I_Temp += 1;
                    S_Temp= S_Mask.Substring(I_Temp,1);                    
                }

                string S_Temp2= S_Mask.Substring(i, I_Temp-i);
                i = I_Temp;

                if (S_CurChar.ToUpper() == "Y")
                {
                    I_Temp = S_Temp2.Length;
                    S_Temp1 = Date_Time.ToString("yyyy");
                    S_RetVal += StrRight(S_Temp1, I_Temp);//.PadRight(I_Temp);
                }
                else if (S_CurChar.ToUpper() == "M")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Month;

                    if (I_Temp == 1)
                    {
                        S_RetVal += S_SingleMSet.Substring(I_Temp2, 1).ToUpper();
                    }
                    else if (I_Temp == 2)
                    {
                        S_Temp1 = I_Temp2.ToString();
                        S_Temp1 = StrRight("00" + S_Temp1, 2);//.PadRight(2);
                        S_RetVal += S_Temp1;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "D")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Day;

                    if (I_Temp == 1)
                    {
                        S_Temp = S_SingleDSet.Substring(I_Temp2, 1).ToUpper();
                        S_RetVal += S_Temp;
                    }
                    else if (I_Temp == 2)
                    {
                        S_Temp1 = I_Temp2.ToString();
                        S_Temp1 = StrRight("00" + S_Temp1, 2);//.PadRight(2);
                        S_RetVal += S_Temp1;
                    }
                    else if (I_Temp == 3)
                    {
                        I_Temp2 = Convert.ToInt32(Date_Time.DayOfWeek) + 1;
                        I_Temp3 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));
                        I_Temp4 = Convert.ToInt32(Convert.ToDateTime(Date_Time.Year.ToString() + "-01-01").DayOfWeek) + 1;

                        I_Temp2 = (I_Temp3 - 1) * 7 + I_Temp2 - I_Temp4 + 1;

                        S_Temp1 = I_Temp2.ToString();
                        S_Temp1 = StrRight("000" + S_Temp1, 3);//.PadRight(3);
                        S_RetVal += S_Temp1;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "W")
                {
                    I_Temp = S_Temp2.Length;

                    I_Temp2 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));

                    if (I_Temp == 1)
                    {
                        DateTime Date_Temp1 = Convert.ToDateTime(Date_Time.ToString("yyyy-MM") + "-01");
                        I_Temp3 = Convert.ToInt32(Math.Round((((Double)Date_Temp1.DayOfYear / (Double)7) + 1), 0));

                        I_Temp2 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));
                        I_Temp2 = I_Temp2 - I_Temp3 + 1;

                        S_RetVal += I_Temp2.ToString();
                    }
                    else if (I_Temp == 2)
                    {
                        if (S_MainWeek == null) 
                        {
                            S_MainWeek= GetManWeek(Date_Time);
                        }
                        S_Temp2 = GetManWeek(Date_Time);
                        S_RetVal += StrRight(S_Temp2, 2);//.PadRight(2);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "C")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));

                    if (I_Temp == 1)
                    {
                        S_RetVal += I_Temp2.ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "H")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Hour;

                    if (I_Temp == 1)
                    {
                        I_Temp2 += 1;

                        S_Temp = S_SingleHSet.Substring(I_Temp2, 1);
                        S_RetVal += S_Temp;
                    }
                    else if (I_Temp == 2)
                    {
                        S_Temp1 = I_Temp2.ToString();
                        S_Temp2 = StrRight("00" + S_Temp1, 2); //.PadRight(2);

                        S_RetVal += S_Temp2;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "N")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Minute;

                    if (I_Temp == 2)
                    {
                        S_Temp1 = I_Temp2.ToString();
                        S_Temp2 = StrRight("00" + S_Temp1,2);//.PadRight(2);
                        S_RetVal += S_Temp2;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "S")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Second;

                    if (I_Temp == 2)
                    {
                        S_Temp1 = I_Temp2.ToString();
                        S_Temp2 = StrRight("00" + S_Temp1, 2);//.PadRight(2);
                        S_RetVal += S_Temp2;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "B")
                {
                    I_Temp = S_Temp2.Length;
                    I_Temp2 = Date_Time.Hour;
                    I_Temp2 += 1;

                    if (I_Temp == 1)
                    {
                        S_Temp = S_SingleBSet.Substring(I_Temp2, 1);
                        S_RetVal += S_Temp;
                    }
                    else if (I_Temp == 2)
                    {
                        S_Temp1 = (I_Temp2 % 12).ToString();
                        S_Temp2 = StrRight("00" + S_Temp1, 2);//.PadRight(2);
                        S_RetVal += S_Temp2;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "A")
                {
                    I_Temp = S_Temp2.Length;
                    if (I_Temp == 1)
                    {
                        I_Temp2 = Date_Time.Hour;
                        if (I_Temp2 > 12)
                        {
                            S_RetVal += "P";
                        }
                        else
                        {
                            S_RetVal += "A";
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (S_CurChar.ToUpper() == "V") 
                {
                    if (S_MainWeek == null)
                    {
                        S_MainWeek = GetManWeek(Date_Time);
                    }

                    I_Temp = S_Temp2.Length;
                    S_Temp1 = StrLeft(S_MainWeek, 4); //.PadLeft(4);
                    S_RetVal += S_Temp1.PadRight(I_Temp);
                }

            }
            return S_RetVal;
        }

        /// <summary>
        /// GetManWeek
        /// </summary>
        /// <param name="Date_Curr"></param>
        /// <returns></returns>
        public static string GetManWeek(DateTime Date_Curr)            
        {
            string S_Result = "";

            try
            {
                string S_Year = Date_Curr.Year.ToString();
                string S_Week = Math.Round((((Double)Date_Curr.DayOfYear / (Double)7) + 1), 0).ToString();
                string S_YearLst = (Date_Curr.Year - 1).ToString();
                string S_Jan1 = S_Year + "-01-01";
                string S_Jan1LstYr = S_YearLst + "-01-01";
                string S_Dec31 = S_Year + "-12-31";
                string S_Dec31LstYr = S_YearLst + "-12-31";
                Boolean B_bShift = false;

                string S_1stMon = "";
                DateTime Date_Jan1 = Convert.ToDateTime(S_Jan1);
                switch (Date_Jan1.DayOfWeek)
                {
                    case DayOfWeek.Monday: S_1stMon = S_Year + "01-01"; B_bShift = false; break;
                    case DayOfWeek.Tuesday: S_1stMon = S_Year + "01-07"; B_bShift = false; break;
                    case DayOfWeek.Wednesday: S_1stMon = S_Year + "01-06"; B_bShift = false; break;
                    case DayOfWeek.Thursday: S_1stMon = S_Year + "01-05"; B_bShift = false; break;
                    case DayOfWeek.Friday: S_1stMon = S_Year + "01-04"; B_bShift = true; break;
                    case DayOfWeek.Saturday: S_1stMon = S_Year + "01-03"; B_bShift = true; break;
                    case DayOfWeek.Sunday: S_1stMon = S_Year + "01-02"; B_bShift = true; break;
                }

                string S_LstMon = "";
                DateTime Date_Dec31 = Convert.ToDateTime(S_Dec31);
                switch (Date_Dec31.DayOfWeek)
                {
                    case DayOfWeek.Monday: S_LstMon = S_Year + "12-31"; break;
                    case DayOfWeek.Tuesday: S_LstMon = S_Year + "12-30"; break;
                    case DayOfWeek.Wednesday: S_LstMon = S_Year + "12-29"; break;
                    case DayOfWeek.Thursday: S_LstMon = S_Year + "12-28"; break;
                    case DayOfWeek.Friday: S_LstMon = S_Year + "12-27"; break;
                    case DayOfWeek.Saturday: S_LstMon = S_Year + "12-26"; break;
                    case DayOfWeek.Sunday: S_LstMon = S_Year + "12-25"; break;
                }

                string S_1stMonLtYr = "";
                DateTime Date_Jan1LstYr = Convert.ToDateTime(S_Jan1LstYr);
                switch (Date_Jan1LstYr.DayOfWeek)
                {
                    case DayOfWeek.Monday: S_1stMonLtYr = S_YearLst + "01-01"; break;
                    case DayOfWeek.Tuesday: S_1stMonLtYr = S_YearLst + "01-07"; break;
                    case DayOfWeek.Wednesday: S_1stMonLtYr = S_YearLst + "01-06"; break;
                    case DayOfWeek.Thursday: S_1stMonLtYr = S_YearLst + "01-05"; break;
                    case DayOfWeek.Friday: S_1stMonLtYr = S_YearLst + "01-04"; break;
                    case DayOfWeek.Saturday: S_1stMonLtYr = S_YearLst + "01-03"; break;
                    case DayOfWeek.Sunday: S_1stMonLtYr = S_YearLst + "01-02"; break;
                }

                DateTime Date_1stMon = Convert.ToDateTime(S_1stMon);
                DateTime Date_LstMon = Convert.ToDateTime(S_LstMon);
                DateTime Date_1stMonLtYr = Convert.ToDateTime(S_1stMonLtYr);

                if (Date_Curr < Date_1stMon)
                {
                    if (B_bShift == true)
                    {
                        S_Year = (Convert.ToInt32(S_Year) - 1).ToString();

                        int I_Week_Temp = 0;
                        if (Date_Jan1LstYr.DayOfWeek == DayOfWeek.Friday || Date_Jan1LstYr.DayOfWeek == DayOfWeek.Saturday ||
                            Date_Jan1LstYr.DayOfWeek == DayOfWeek.Sunday)
                        {
                            I_Week_Temp = 1;
                        }

                        TimeSpan TS_Week = new TimeSpan();
                        TS_Week = Date_Dec31 - Date_1stMonLtYr;
                        int I_TS_Week = Convert.ToInt32(Math.Round((((Double)TS_Week.Days / (Double)7) + 1), 0));

                        S_Week = (I_Week_Temp + I_TS_Week).ToString();

                    }
                    else
                    {
                        S_Week = "01";
                    }
                }
                else if ((Date_Curr >= Date_LstMon) &&
                        (
                            Date_Dec31.DayOfWeek == DayOfWeek.Monday || Date_Dec31.DayOfWeek == DayOfWeek.Tuesday ||
                            Date_Dec31.DayOfWeek == DayOfWeek.Wednesday
                        )
                    )
                {
                    S_Year = (Convert.ToInt32(S_Year) + 1).ToString();
                    S_Week = "01";
                }
                else
                {
                    int I_Week_Temp = 0;
                    if (B_bShift == false)
                    {
                        if (Date_Jan1.DayOfWeek == DayOfWeek.Monday)
                        {
                            I_Week_Temp = 0;
                        }
                        else
                        {
                            I_Week_Temp = 1;
                        }
                    }
                    else
                    {
                        I_Week_Temp = 0;
                    }
                    TimeSpan TS_Week = new TimeSpan();
                    TS_Week = Date_Curr - Date_1stMon;
                    int I_TS_Week = Convert.ToInt32(Math.Round((((Double)TS_Week.Days / (Double)7) + 1), 0));

                    S_Week = (I_Week_Temp + I_TS_Week).ToString();
                }

                if (S_Week.Length == 1) { S_Week = "0"; }

                S_Result = S_Year + "WK" + S_Week;
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }

            return S_Result;
        }

        /// <summary>
        /// StrLeft
        /// </summary>
        /// <param name="S_Str"></param>
        /// <param name="I_Width"></param>
        /// <returns></returns>
        public static string StrLeft(string S_Str,int I_Width) 
        {
            string S_Value= S_Str.Substring(0,I_Width);
            return S_Value;
        }
        /// <summary>
        /// StrRight
        /// </summary>
        /// <param name="S_Str"></param>
        /// <param name="I_Width"></param>
        /// <returns></returns>
        public static string StrRight(string S_Str, int I_Width)
        {
            string S_Value = S_Str.Substring(S_Str.Length- I_Width, I_Width);
            return S_Value;
        }

        /// <summary>
        /// GetSNRDateTime
        /// </summary>
        /// <param name="S_Type"></param>
        /// <param name="S_Mask"></param>
        /// <param name="Date_Time"></param>
        /// <param name="S_InvalidChars"></param>
        /// <returns></returns>
        public static string GetSNRDateTime(string S_Type,string S_Mask,DateTime Date_Time,string S_InvalidChars) 
        {
            S_Mask = " " + S_Mask + " ";

            string S_DateTimeMaskCharSet= "TWYMCV", S_RetVal="";
            int I_Count= S_Mask.Length,
                i=1, j, k, I_Temp, I_Temp2, I_Temp3, I_Temp4;
            string S_Temp, S_Temp1, S_Temp2, S_TempInvalid, S_CurChar, 
                S_SingleTSet= " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", S_SingleMSet= " 123456789ABC";

            if (S_Type == "@") 
            {
                S_RetVal = S_DateTimeMaskCharSet.ToUpper();
                return S_RetVal;
            }
            if (S_Type == "#") 
            {
                while (i < I_Count) 
                {
                    I_Temp = i;
                    S_CurChar = S_Mask.Substring(i, 1);
                    S_Temp = S_CurChar;

                    while (string.Equals(S_CurChar, S_Temp, StringComparison.OrdinalIgnoreCase))
                    {
                        I_Temp += 1;
                        S_Temp = S_Mask.Substring(I_Temp, 1);
                    }

                    S_Temp2 = S_Mask.Substring(i, I_Temp - i);
                    i=I_Temp;

                    if (S_CurChar.ToUpper() == "Y")
                    {
                        I_Temp = S_Temp2.Length;
                        S_Temp1 = Date_Time.Year.ToString();
                        S_RetVal += StrRight(S_Temp1, I_Temp);
                    }
                    else if (S_CurChar.ToUpper() == "M")
                    {
                        I_Temp = S_Temp2.Length;
                        I_Temp2 = Date_Time.Month;

                        if (I_Temp == 1)
                        {
                            S_RetVal += S_SingleMSet.Substring(I_Temp2, 1);
                        }
                        else if (I_Temp == 2)
                        {
                            S_Temp1 = I_Temp2.ToString();
                            S_Temp1 = StrRight("00" + S_Temp1, 2);
                            S_RetVal += S_Temp1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (S_CurChar.ToUpper() == "T")
                    {
                        I_Temp = S_Temp2.Length;
                        I_Temp2 = Date_Time.Day;

                        if (I_Temp == 1)
                        {
                            k = 1;
                            if (S_InvalidChars.Trim() != "")
                            {
                                S_InvalidChars = " " + S_InvalidChars + " ";
                                while (k <= S_InvalidChars.Length)
                                {
                                    S_TempInvalid = S_InvalidChars.Substring(k, 1);
                                    j = S_SingleTSet.IndexOf(S_TempInvalid)+1;

                                    while (j != 0)
                                    {
                                        S_SingleTSet = Stuff(S_SingleTSet, j, 1, Convert.ToChar(""));
                                        j = S_SingleTSet.IndexOf(S_TempInvalid)+1;
                                    }
                                    k += 1;
                                }
                            }
                            if (S_SingleTSet.Length < 31)
                            {
                                return null;
                            }
                            S_Temp = S_SingleTSet.Substring(I_Temp2, 1);
                            S_RetVal += S_Temp;
                        }
                        else if (I_Temp == 2)
                        {
                            S_Temp1 = I_Temp2.ToString();
                            S_Temp1 = StrRight("00" + S_Temp1, 2);
                            S_RetVal += S_Temp1;
                        }
                        else if (I_Temp == 3)
                        {
                            I_Temp3 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));
                            I_Temp2 = Convert.ToInt32(Date_Time.DayOfWeek) + 1;

                            DateTime Date_0101 = Convert.ToDateTime(Date_Time.Year.ToString() + "-01-01");
                            I_Temp4 = Convert.ToInt32(Date_0101.DayOfWeek) + 1;

                            I_Temp2 = (I_Temp3 - 1) * 7 + I_Temp2 - I_Temp4 + 1;

                            S_Temp1 = I_Temp2.ToString();
                            S_Temp1 = StrRight("000" + S_Temp1, 3);
                            S_RetVal += S_Temp1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (S_CurChar.ToUpper() == "W")
                    {
                        I_Temp = S_Temp2.Length;
                        I_Temp2 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));

                        if (I_Temp == 1)
                        {
                            DateTime Date_W = Convert.ToDateTime(Date_Time.ToString("yyyy-MM") + "-01");
                            I_Temp3 = Convert.ToInt32(Math.Round((((Double)Date_W.DayOfYear / (Double)7) + 1), 0));
                            I_Temp2 = Convert.ToInt32(Math.Round((((Double)Date_Time.DayOfYear / (Double)7) + 1), 0));
                            I_Temp2 = I_Temp2 - I_Temp3 + 1;

                            S_RetVal += I_Temp2.ToString();
                        }
                        else if (I_Temp == 2)
                        {
                            S_Temp2 = GetManWeek(Date_Time);
                            S_RetVal += StrRight(S_Temp2, 2);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (S_CurChar.ToUpper() == "C")
                    {
                        I_Temp = S_Temp2.Length;
                        I_Temp2 = Convert.ToInt32(Date_Time.DayOfWeek) + 1;

                        if (I_Temp == 1)
                        {
                            S_RetVal += I_Temp2.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else if (S_CurChar.ToUpper() == "V") 
                    {
                        I_Temp = S_Temp2.Length;
                        S_Temp1 = StrLeft(GetManWeek(Date_Time), 4);
                        S_RetVal += StrRight(S_Temp1, I_Temp);
                    }
                }
            }

            return S_RetVal;
        }

        /// <summary>
        /// GetSNRCharSet
        /// </summary>
        /// <param name="S_Type"></param>
        /// <param name="S_CurDigit"></param>
        /// <param name="I_Omesset"></param>
        /// <param name="S_InvalidChars"></param>
        /// <returns></returns>
        public static string GetSNRCharSet(string S_Type, string S_CurDigit, int I_Omesset, string S_InvalidChars) 
        {
            string S_DigitMaskCharSet= "HANDO",
                   S_DecCharSet= "0123456789", 
                   S_OctCharSet= "01234567", 
                   S_HexCharSet= "0123456789ABCDEF", 
                   S_AlpCharset= "ABCDEFGHIJKLMNOPQRSTUVWXYZ", 
                   S_AlnCharSet= "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                   S_CurCharSet;
            int i, I_idx, I_Count=0, I_len_ivc,I_cnt_ivc;
            string S_Propergation="0", S_RetVal = "";

            S_CurDigit = S_CurDigit ?? "";
            S_InvalidChars = S_InvalidChars ?? "";

            if (S_Type == "@") 
            {
                if (S_CurDigit == "")
                {
                    S_RetVal = S_DigitMaskCharSet.ToUpper();
                    return S_RetVal;
                }
                else 
                {
                    S_RetVal = "";
                    switch (S_CurDigit) 
                    {
                        case "H":S_RetVal = S_HexCharSet;break;
                        case "A": S_RetVal = S_AlpCharset; break;
                        case "N": S_RetVal = S_AlnCharSet; break;
                        case "D": S_RetVal = S_DecCharSet; break;
                        case "O": S_RetVal = S_OctCharSet; break;
                    }
                    return S_RetVal;
                }
            }

            S_CurCharSet = "";
            switch (S_Type)
            {
                case "H": S_CurCharSet = S_HexCharSet; break;
                case "A": S_CurCharSet = S_AlpCharset; break;
                case "N": S_CurCharSet = S_AlnCharSet; break;
                case "D": S_CurCharSet = S_DecCharSet; break;
                case "O": S_CurCharSet = S_OctCharSet; break;
            }
            if (S_CurCharSet == "") 
            {
                S_RetVal = "";
                return S_RetVal;
            }

            S_InvalidChars = S_InvalidChars.Replace(",", "");
            I_cnt_ivc = 1;
            I_len_ivc = S_InvalidChars.Length;

            while (I_cnt_ivc <= I_len_ivc) 
            {
                S_CurCharSet = S_CurCharSet.Replace(S_InvalidChars.Substring(I_cnt_ivc-1, 1), "");
                I_cnt_ivc += 1;
            }

            I_Count = S_CurCharSet.Length;
            I_idx = S_CurCharSet.IndexOf(S_CurDigit)+1;
            i = I_idx + I_Omesset;

            if (I_idx == 0) 
            {
                S_RetVal = "0," + S_CurCharSet.Substring(0, 1);
                return S_RetVal;
            }

            if (i > I_Count) 
            {
                S_Propergation=(Math.Round((double)i/(double)I_Count,0)).ToString();
                i = i % I_Count;

                if (i == 0) 
                {
                    S_Propergation = (Convert.ToInt32(S_Propergation) - 1).ToString();
                    i = I_Count;
                }
            }

            S_CurDigit = S_CurCharSet.Substring(i-1, 1);
            S_RetVal= S_Propergation+","+S_CurDigit;

            return S_RetVal.ToUpper();
        }

        /// <summary>
        /// SNRParseParam
        /// </summary>
        /// <param name="S_LastUsed"></param>
        /// <param name="S_Mask"></param>
        /// <returns></returns>
        public static List<Public_SNPar> SNRParseParam(string S_LastUsed,string S_Mask) 
        {
            List <Public_SNPar> List_Result=new List<Public_SNPar>();
            int I_SNPar_ID=0;
            Public_SNPar v_Public_SNPar = new Public_SNPar();

            try
            {
                S_LastUsed = S_LastUsed ?? "";
                S_Mask = S_Mask ?? "";

                int i = 1, I_Count, I_Temp;
                string S_Temp, S_Temp2,
                       S_DateTimeMaskSet = GetSNRDateTime("@", null, DateTime.Now, null),
                       S_DigitMaskSet = GetSNRCharSet("@", null, 0, null),
                       S_DateTimeMask = "", S_DigitMask = "", S_DateTimeLastUsed = "", S_DigitLastUsed = "";
                int I_DigitPos = 0, I_DigitLen = 0, I_DateTimePos = 0, I_DateTimeLen = 0;

                if (S_Mask.Trim() == "") { return null; }

                I_Count = S_Mask.Length;
                S_Mask = " " + S_Mask.Trim().ToUpper();
                S_LastUsed = S_LastUsed.Trim().ToUpper();
                S_Temp2 = S_DateTimeMaskSet + S_DigitMaskSet;

                while (i <= I_Count)
                {
                    S_Temp = S_Mask.Substring(i, 1);
                    if (S_Temp2.IndexOf(S_Temp) == -1)
                    {
                        return List_Result;
                    }
                    i += 1;
                }

                S_Temp = StrRight(S_Mask, 1);
                
                if (S_DateTimeMaskSet.IndexOf(S_Temp) != -1)
                {
                    i = I_Count;
                    while (S_DateTimeMaskSet.IndexOf(S_Temp) != -1 && i > 0)
                    {
                        i = i - 1;
                        S_Temp = S_Mask.Substring(i, 1);
                    }


                    S_DateTimeMask = StrRight(S_Mask, I_Count - i);
                    S_DigitMask = StrLeft(S_Mask, i);
                    I_DigitPos = 1;
                    I_DigitLen = i;
                    I_DateTimePos = i + 1;
                    I_DateTimeLen = I_Count - i;

                    if (S_LastUsed == "")
                    {
                        S_DateTimeLastUsed = "";
                        S_DigitLastUsed = "";
                    }
                    else
                    {
                        S_DateTimeLastUsed = StrRight(S_LastUsed, I_Count - i);
                        S_DigitLastUsed = StrLeft(S_LastUsed, i);
                    }

                    v_Public_SNPar = new Public_SNPar();
                    I_SNPar_ID += 1;
                    v_Public_SNPar.ID = I_SNPar_ID;
                    v_Public_SNPar.mask = S_DigitMask.Trim();
                    v_Public_SNPar.pos = I_DigitPos;
                    v_Public_SNPar.len = I_DigitLen;
                    v_Public_SNPar.type = 2;
                    v_Public_SNPar.luVal = S_DigitLastUsed.Trim();
                    List_Result.Add(v_Public_SNPar);

                    v_Public_SNPar = new Public_SNPar();
                    I_SNPar_ID += 1;
                    v_Public_SNPar.ID = I_SNPar_ID;
                    v_Public_SNPar.mask = S_DateTimeMask.Trim();
                    v_Public_SNPar.pos = I_DateTimePos;
                    v_Public_SNPar.len = I_DateTimeLen;
                    v_Public_SNPar.type = 1;
                    v_Public_SNPar.luVal = S_DateTimeLastUsed.Trim();
                    List_Result.Add(v_Public_SNPar);
                }
                else
                {
                    S_Temp = StrLeft(S_Mask, 2).Trim();

                    if (S_DateTimeMaskSet.IndexOf(S_Temp) != -1)
                    {
                        i = 1;
                        while (S_DateTimeMaskSet.IndexOf(S_Temp) != -1 && i < I_Count)
                        {
                            i += 1;
                            S_Temp = S_Mask.Substring(i, 1);
                        }

                        S_DateTimeMask = StrLeft(S_Mask, i - 1);
                        S_DigitMask = StrRight(S_Mask, I_Count - i + 1);

                        if (S_LastUsed == "")
                        {
                            S_DateTimeLastUsed = "";
                            S_DigitLastUsed = "";
                        }
                        else
                        {
                            S_DateTimeLastUsed = StrLeft(S_LastUsed, i - 1);
                            S_DigitLastUsed = StrRight(S_LastUsed, I_Count - i + 1);
                        }

                        I_DateTimePos = 1;
                        I_DateTimeLen = i - 1;
                        I_DigitPos = i;
                        I_DigitLen = I_Count - i + 1;

                        v_Public_SNPar = new Public_SNPar();
                        I_SNPar_ID += 1;
                        v_Public_SNPar.ID = I_SNPar_ID;
                        v_Public_SNPar.mask = S_DateTimeMask.Trim();
                        v_Public_SNPar.pos = I_DateTimePos;
                        v_Public_SNPar.len = I_DateTimeLen;
                        v_Public_SNPar.type = 1;
                        v_Public_SNPar.luVal = S_DateTimeLastUsed.Trim();
                        List_Result.Add(v_Public_SNPar);

                        v_Public_SNPar = new Public_SNPar();
                        I_SNPar_ID += 1;
                        v_Public_SNPar.ID = I_SNPar_ID;
                        v_Public_SNPar.mask = S_DigitMask.Trim();
                        v_Public_SNPar.pos = I_DigitPos;
                        v_Public_SNPar.len = I_DigitLen;
                        v_Public_SNPar.type = 2;
                        v_Public_SNPar.luVal = S_DigitLastUsed.Trim();
                        List_Result.Add(v_Public_SNPar);
                    }
                    else
                    {
                        i = 1;
                        I_Temp = S_DateTimeMaskSet.Length;

                        while (i < I_Temp)
                        {
                            S_Temp = S_DateTimeMaskSet.Substring(i, 1);
                            if (S_Mask.IndexOf(S_Temp) != -1)
                            {
                                return List_Result;
                            }
                            i += 1;
                        }

                        S_DigitMask = S_Mask;
                        S_DigitLastUsed = S_LastUsed;

                        I_DateTimePos = 0;
                        I_DateTimeLen = 0;
                        I_DigitPos = 1;
                        I_DigitLen = I_Count;

                        v_Public_SNPar = new Public_SNPar();
                        I_SNPar_ID += 1;
                        v_Public_SNPar.ID = I_SNPar_ID;
                        v_Public_SNPar.mask = S_DigitMask.Trim();
                        v_Public_SNPar.pos = I_DigitPos;
                        v_Public_SNPar.len = I_DigitLen;
                        v_Public_SNPar.type = 2;
                        v_Public_SNPar.luVal = S_DigitLastUsed.Trim();
                        List_Result.Add(v_Public_SNPar);
                    }
                }
            }
            catch (Exception ex) 
            {
                v_Public_SNPar = new Public_SNPar();
                
                v_Public_SNPar.ID = 0;
                v_Public_SNPar.mask = "ERROR";
                v_Public_SNPar.pos = 0;
                v_Public_SNPar.len = 0;
                v_Public_SNPar.type = 0;
                v_Public_SNPar.luVal = ex.ToString();
                List_Result.Add(v_Public_SNPar);
            }
            return List_Result;
        }

        /// <summary>
        /// GetSNRPureCounter
        /// </summary>
        /// <param name="S_LastUsed"></param>
        /// <param name="S_Mask"></param>
        /// <param name="I_Offset"></param>
        /// <param name="S_InvalidChars"></param>
        /// <returns></returns>
        public static string GetSNRPureCounter(string S_LastUsed, string S_Mask, int I_Offset, string S_InvalidChars) 
        {
            int I_Count=0,i=0;
            string S_Temp, S_Temp2, S_curMask, S_curDigit;
            int I_Propergation = I_Offset;

            S_LastUsed = S_LastUsed ?? "";
            S_LastUsed = " " + S_LastUsed;
            S_Mask = S_Mask ?? "";

            I_Count = S_Mask.Length;
            S_Mask = " " + S_Mask;
            
            i = I_Count;

            while (i > 0) 
            {
                S_curMask= S_Mask.Substring(i,1);
                S_curDigit = S_LastUsed.Substring(i, 1);
                S_Temp2 = GetSNRCharSet(S_curMask, S_curDigit, I_Propergation, S_InvalidChars);

                if (S_Temp2.Length < 3) 
                {
                    return "";
                }

                S_curDigit = StrRight(S_Temp2, 1);
                S_LastUsed = Stuff(S_LastUsed, i, 1, Convert.ToChar(S_curDigit));
                S_Temp = StrLeft(S_Temp2, S_Temp2.IndexOf(","));

                if (S_Temp == "0")
                {
                    return S_LastUsed.Trim();
                }
                else 
                {
                    I_Propergation = Convert.ToInt32(S_Temp); 
                }
                i -= 1;
            }
            return S_LastUsed.Trim();
        }


        /// <summary>
        /// 语言文字分割类 @
        /// </summary>
        /// <param name="S_Str">语言文字字符串</param>
        /// <param name="I_Lang">语言 0:ZH_CN 1:EN</param>
        /// <returns></returns>
        public static string GetLangStr(string S_Str,int I_Lang) 
        {
            string[] List_Str = S_Str.Split("@");
            return List_Str[I_Lang].Trim();
        }







        /// <summary>
        ///作用：将字符串内容转化为16进制数据编码 
        /// </summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Encode(string strEncode)
        {
            string strReturn = "";//  存储转换后的编码
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X2");
            }
            return strReturn;
        }
        /// <summary>
        /// 作用：将16进制数据编码转化为字符串
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string Decode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 2; i++)
            {
                sResult += (char)short.Parse(strDecode.Substring(i * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
            }
            return sResult;
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="v_Password"></param>
        /// <param name="v_key"></param>
        /// <returns></returns>
        public static string EncryptPassword(string v_Password, string v_key)
        {
            int i, j;
            int a = 0, b = 0, c = 0;
            string hexS = "", hexskey = "", midS = "", tmpstr = "";

            hexS = Encode(v_Password);
            hexskey = Encode(v_key);
            midS = hexS;

            for (i = 1; i <= hexskey.Length / 2; i++)
            {
                if (i != 1)
                {
                    midS = tmpstr;
                }
                tmpstr = "";
                for (j = 1; j <= midS.Length / 2; j++)
                {
                    a = (char)short.Parse(midS.Substring((j - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                    b = (char)short.Parse(hexskey.Substring((i - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);

                    //a = (char)short.Parse(Convert.ToString(midS[2 * j - 2]) + Convert.ToString(midS[2 * j-1]), global::System.Globalization.NumberStyles.HexNumber);
                    //b = (char)short.Parse(Convert.ToString(hexskey[2 * i - 2]) + Convert.ToString(hexskey[2 * i-1]), global::System.Globalization.NumberStyles.HexNumber);

                    c = a ^ b;
                    tmpstr += Encode(Convert.ToString((Convert.ToChar(c))));
                }
            }
            return tmpstr;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="v_Password"></param>
        /// <param name="v_key"></param>
        /// <returns></returns>
        public static string DecryptPassword(string v_Password, string v_key)
        {
            int i, j;
            int a = 0, b = 0, c = 0;
            string hexS = "", hexskey = "", midS = "", tmpstr = "";

            hexS = v_Password;
            if (hexS.Length % 2 == 1)
            {
                //Response.Write("<script>alert(\"密文错误，无法解密字符串\");</script>");
            }
            hexskey = Encode(v_key);
            tmpstr = hexS;
            midS = hexS;
            for (i = hexskey.Length / 2; i >= 1; i--)
            {
                if (i != hexskey.Length / 2)
                {
                    midS = tmpstr;
                }
                tmpstr = "";
                for (j = 1; j <= midS.Length / 2; j++)
                {
                    try
                    {
                        a = (char)short.Parse(midS.Substring((j - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                        b = (char)short.Parse(hexskey.Substring((i - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                        c = a ^ b;
                    }
                    catch 
                    {
                        c = 0;
                    }

                    tmpstr += Encode(Convert.ToString((Convert.ToChar(c))));
                }
            }
            return Decode(tmpstr);
        }


        public static string DynPWd() 
        {
            string S_Date = DateTime.Now.ToString("yyyy-MM-dd_HH");
            string S_DynPWD = EncryptPassword
            (
            "QwAsZxReFdVc" +
            S_Date,
            S_Date
            );

            return S_DynPWD;
        }


        /// <summary>
        /// DataTableToJson
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable table)
        {
            var JsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JsonString.Append("[" + "\r\n");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JsonString.Append("{" + "\r\n");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        string S_Value = table.Rows[i][j].ToString();
                        //int i1 = Regex.Matches(S_Value, @"-").Count;
                        //int i2 = Regex.Matches(S_Value, @":").Count;
                        //if (i1 == 2 && i2 == 2) 
                        //{
                        //    S_Value = S_Value.Insert(10, "T");
                        //    S_Value = S_Value.Substring(0, 11) + S_Value.Substring(12, 8);
                        //}

                        if (j < table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + S_Value + "\"," + "\r\n");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + S_Value + "\"" + "\r\n");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JsonString.Append("}" + "\r\n");
                    }
                    else
                    {
                        JsonString.Append("}," + "\r\n");
                    }
                }
                JsonString.Append("]");
            }
            else
            {
                JsonString.Append("[" + "\r\n");

                JsonString.Append("{" + "\r\n");
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    string S_Value = "";

                    if (j < table.Columns.Count - 1)
                    {
                        JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + S_Value + "\"," + "\r\n");
                    }
                    else if (j == table.Columns.Count - 1)
                    {
                        JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + S_Value + "\"" + "\r\n");
                    }
                }
                JsonString.Append("}" + "\r\n");

                JsonString.Append("]");
            }
            return JsonString.ToString();
        }

    }
}
