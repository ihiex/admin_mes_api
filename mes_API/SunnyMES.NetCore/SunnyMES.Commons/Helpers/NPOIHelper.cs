﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace SunnyMES.Commons.Helpers
{
    /// <summary>
    /// office 导入导出
    /// </summary>
    public class NPOIHelper
    {
        /// <summary>
        /// DataTable 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="dtSource">源 DataTable</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题</param>
        /// <returns></returns>
        public static MemoryStream ExportExcel(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 文件属性
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "SunnyMES.com";
            workbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "SunnyMES.com";
            si.ApplicationName = "SunnyMES.com";
            si.LastAuthor = "SunnyMES.com";
            si.Comments = "";
            si.Title = "";
            si.Subject = "";
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
            #endregion
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            int[] arrColWidth=new int[dtSource.Columns.Count];
            foreach(DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding("gb2312").GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count;i++ )
            {
                for (int j = 0; j < dtSource.Columns.Count;j++ )
                {
                    int intTemp = Encoding.GetEncoding("gb2312").GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            int intTop = 0;
            foreach(DataRow row in dtSource.Rows)
            {
                #region 新建表、填充表头、填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }
                    intTop = 0;
                    #region 表头及样式
                    {
                        if (strHeaderText.Length > 0)
                        {
                            IRow headerRow = sheet.CreateRow(intTop);
                            intTop += 1;
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);
                            ICellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                           
                        }
                    }
                    #endregion
                    #region  列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(intTop);
                        intTop += 1;
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach(DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                       
                        
                    }
                    #endregion
                    rowIndex = intTop;
                }
                #endregion
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach(DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    { 
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV=0;
                            int.TryParse(drValue,out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            double doubV=0;
                            double.TryParse(drValue,out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                           newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            using(MemoryStream ms=new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position=0;
                return ms;
            }
        }
        /// <summary>
        /// DaataTable 导出到 Excel 文件
        /// </summary>
        /// <param name="dtSource">源 DataaTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置(文件名及路径)</param>
        public static void ExportExcel(DataTable dtSource, string strHeaderText,string strFileName)
        {
            using (MemoryStream ms = ExportExcel(dtSource, strHeaderText))
            { 
                 using(FileStream fs=new FileStream(strFileName,FileMode.Create,FileAccess.Write))
                 {
                   byte[] data=ms.ToArray();
                     fs.Write(data,0,data.Length);
                     fs.Flush();
                 }
            }
        }

        /// <summary>
        /// 读取 excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel 文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(string strFileName)
        {
            int ii = strFileName.LastIndexOf(".");
            string filetype = strFileName.Substring(ii + 1, strFileName.Length - ii - 1);
            DataTable dt = new DataTable();
            ISheet sheet;
            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }
                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                sheet = hssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                {
                    DataRow dataRow = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {

                            dataRow[j] = row.GetCell(j).ToString();

                        }
                    }
                    dt.Rows.Add(dataRow);
                }
            }
            return dt;

        }


        /// <summary>
        /// DataSet 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="dsSource">源 DataSet</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题(多个表对应多个表头以英文逗号(,)分开，个数应与表相同)</param>
        /// <returns></returns>
        public static MemoryStream ExportExcel(DataSet dsSource, string strHeaderText)
        {

            HSSFWorkbook workbook = new HSSFWorkbook();
           
            #region 文件属性
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "SunnyMES.com";
            workbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "SunnyMES.com";
            si.ApplicationName = "SunnyMES.com";
            si.LastAuthor = "SunnyMES.com";
            si.Comments = "";
            si.Title = "";
            si.Subject = "";
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
            #endregion

            #region 注释


            //ICellStyle dateStyle = workbook.CreateCellStyle();
            //IDataFormat format = workbook.CreateDataFormat();
            //dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //ISheet sheet = workbook.CreateSheet();
            //int[] arrColWidth = new int[dtSource.Columns.Count];
            //foreach (DataColumn item in dtSource.Columns)
            //{
            //    arrColWidth[item.Ordinal] = Encoding.GetEncoding("gb2312").GetBytes(item.ColumnName.ToString()).Length;
            //}
            //for (int i = 0; i < dtSource.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dtSource.Columns.Count; j++)
            //    {
            //        int intTemp = Encoding.GetEncoding("gb2312").GetBytes(dtSource.Rows[i][j].ToString()).Length;
            //        if (intTemp > arrColWidth[j])
            //        {
            //            arrColWidth[j] = intTemp;
            //        }
            //    }
            //}
            //int rowIndex = 0;
            //int intTop = 0;
            //foreach (DataRow row in dtSource.Rows)
            //{
            //    #region 新建表、填充表头、填充列头，样式
            //    if (rowIndex == 65535 || rowIndex == 0)
            //    {
            //        if (rowIndex != 0)
            //        {
            //            sheet = workbook.CreateSheet();
            //        }
            //        intTop = 0;
            //        #region 表头及样式
            //        {
            //            if (strHeaderText.Length > 0)
            //            {
            //                IRow headerRow = sheet.CreateRow(intTop);
            //                intTop += 1;
            //                headerRow.HeightInPoints = 25;
            //                headerRow.CreateCell(0).SetCellValue(strHeaderText);
            //                ICellStyle headStyle = workbook.CreateCellStyle();
            //                headStyle.Alignment = HorizontalAlignment.CENTER;
            //                IFont font = workbook.CreateFont();
            //                font.FontHeightInPoints = 20;
            //                font.Boldweight = 700;
            //                headStyle.SetFont(font);
            //                headerRow.GetCell(0).CellStyle = headStyle;
            //                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));

            //            }
            //        }
            //        #endregion
            //        #region  列头及样式
            //        {
            //            IRow headerRow = sheet.CreateRow(intTop);
            //            intTop += 1;
            //            ICellStyle headStyle = workbook.CreateCellStyle();
            //            headStyle.Alignment = HorizontalAlignment.CENTER;
            //            IFont font = workbook.CreateFont();
            //            font.Boldweight = 700;
            //            headStyle.SetFont(font);
            //            foreach (DataColumn column in dtSource.Columns)
            //            {
            //                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            //                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
            //                //设置列宽
            //                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
            //            }


            //        }
            //        #endregion
            //        rowIndex = intTop;
            //    }
            //    #endregion
            //    #region 填充内容
            //    IRow dataRow = sheet.CreateRow(rowIndex);
            //    foreach (DataColumn column in dtSource.Columns)
            //    {
            //        ICell newCell = dataRow.CreateCell(column.Ordinal);
            //        string drValue = row[column].ToString();
            //        switch (column.DataType.ToString())
            //        {
            //            case "System.String"://字符串类型
            //                newCell.SetCellValue(drValue);
            //                break;
            //            case "System.DateTime"://日期类型
            //                DateTime dateV;
            //                DateTime.TryParse(drValue, out dateV);
            //                newCell.SetCellValue(dateV);
            //                newCell.CellStyle = dateStyle;//格式化显示
            //                break;
            //            case "System.Boolean"://布尔型
            //                bool boolV = false;
            //                bool.TryParse(drValue, out boolV);
            //                newCell.SetCellValue(boolV);
            //                break;
            //            case "System.Int16":
            //            case "System.Int32":
            //            case "System.Int64":
            //            case "System.Byte":
            //                int intV = 0;
            //                int.TryParse(drValue, out intV);
            //                newCell.SetCellValue(intV);
            //                break;
            //            case "System.Decimal":
            //            case "System.Double":
            //                double doubV = 0;
            //                double.TryParse(drValue, out doubV);
            //                newCell.SetCellValue(doubV);
            //                break;
            //            case "System.DBNull"://空值处理
            //                newCell.SetCellValue("");
            //                break;
            //            default:
            //                newCell.SetCellValue("");
            //                break;
            //        }
            //    }
            //    #endregion
            //    rowIndex++;
            //}
            #endregion

            string[] strNewText = strHeaderText.Split(Convert.ToChar(","));
            if (dsSource.Tables.Count == strNewText.Length) 
            {
                for(int i=0;i<dsSource.Tables.Count;i++)
                {
                    ExportFromDSExcel(workbook, dsSource.Tables[i], strNewText[i]);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
         /// <summary>
        /// DataTable 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="workbook">源 workbook</param>
        /// <param name="dtSource">源 DataTable</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题(多个表对应多个表头以英文逗号(,)分开，个数应与表相同)</param>
        /// <returns></returns>
        public static void ExportFromDSExcel(HSSFWorkbook workbook, DataTable dtSource, string strHeaderText)
        {
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
            ISheet sheet = workbook.CreateSheet(strHeaderText);
            
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding("gb2312").GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding("gb2312").GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            int intTop = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表、填充表头、填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }
                    intTop = 0;
                    #region 表头及样式
                    {
                        if (strHeaderText.Length > 0)
                        {
                            IRow headerRow = sheet.CreateRow(intTop);
                            intTop += 1;
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);
                            ICellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));

                        }
                    }
                    #endregion
                    #region  列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(intTop);
                        intTop += 1;
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            // sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256); // 设置设置列宽 太长会报错 修改2014 年9月22日
                            int  dd=(arrColWidth[column.Ordinal] + 1) * 256;

                            if (dd > 200 * 256)
                            {
                                dd = 100 * 256;
                            }


                            sheet.SetColumnWidth(column.Ordinal, dd);
                        }


                    }
                    #endregion
                    rowIndex = intTop;
                }
                #endregion
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            if (drValue.Length > 0)
                            {
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);
                                newCell.CellStyle = dateStyle;//格式化显示
                            }
                            else { newCell.SetCellValue(drValue); }
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
        }

        /// <summary>
        /// 按指定长度创建列并带入样式
        /// </summary>
        /// <param name="hssfrow"></param>
        /// <param name="len"></param>
        /// <param name="cellstyle"></param>
        /// <returns></returns>
        public static bool CreateCellsWithLength(XSSFRow hssfrow, int len, XSSFCellStyle cellstyle)
        {
            try
            {
                for (int i = 0; i < len; i++)
                {
                    hssfrow.CreateCell(i);
                    hssfrow.Cells[i].CellStyle = cellstyle;
                }
                return true;
            }
            catch (Exception ce)
            {
                throw new Exception("CreateCellsWithLength:" + ce.Message);
            }
        }
        /// <summary>
        /// 流转DataTable  
        /// 带有日期和时间型的栏位，将会被截取，
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static DataTable StreamToTable(Stream stream,string filetype)
        {
            DataTable dt = new DataTable();
            ISheet sheet;
            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                    xssfworkbook = new XSSFWorkbook(stream);
                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                hssfworkbook = new HSSFWorkbook(stream);
                sheet = hssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                {
                    DataRow dataRow = dt.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {  
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }
                    dt.Rows.Add(dataRow);
                }
            }
            return dt;
        }

        /// <summary>
        /// 流转Model
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static List<T> StreamToModel<T>(Stream stream, string filetype) 
            where T : class, new()
        {
            List<T> tableValues = new List<T>();
            ISheet sheet;
            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                xssfworkbook = new XSSFWorkbook(stream);
                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                hssfworkbook = new HSSFWorkbook(stream);
                sheet = hssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;


            List<string> excelColumnsName = new List<string>();
            for (int j = 0; j < cellCount; j++)
            {
                ICell cellCol = headerRow.GetCell(j);
                excelColumnsName.Add(cellCol.ToString());
            }
            var propertys = GetPropertyByType<T>(false);
            var type = typeof(T).GetProperties();
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                
                if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                {
                    T t = Activator.CreateInstance<T>();

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) == null)
                            continue;
                        var cell = row.GetCell(j);
                        var item = type.FirstOrDefault(m => m.Name == excelColumnsName[j]);
                        if (item is null)
                            continue;
                        if (item.PropertyType == typeof(DateTime?) || item.PropertyType == typeof(DateTime?))
                        {
                            try
                            {
                                if (cell.CellType == CellType.String)
                                {
                                    var value = Convert.ToDateTime(cell.ToString());
                                    item.SetValue(t, value);
                                }
                                else
                                {
                                    item.SetValue(t, cell.DateCellValue);
                                }
                            }
                            catch
                            {
                                throw new Exception($"DateTime{cell.ToString()}格式不正确!");
                            }
                        }
                        else if (item.PropertyType == typeof(int))
                        {
                            try
                            {
                                item.SetValue(t, Convert.ToInt32(cell.ToString()));
                            }
                            catch
                            {
                                throw new Exception($"int{cell.ToString()}格式不正确!");
                            }
                        }
                        else if (item.PropertyType == typeof(string))
                        {
                            if (cell.CellType == CellType.String)
                            {
                                item.SetValue(t, cell.ToString());
                            }
                            else
                            {
                                item.SetValue(t, cell.NumericCellValue.ToString());
                            }

                        }
                        else if (item.PropertyType == typeof(decimal?) || item.PropertyType == typeof(decimal))
                        {
                            if (cell != null)
                            {
                                try
                                {
                                    var value = 0m;
                                    if (cell.CellType == CellType.String)
                                    {
                                        value = Convert.ToDecimal(cell.ToString());
                                    }
                                    else
                                    {
                                        value = Convert.ToDecimal(cell.NumericCellValue);
                                    }
                                    item.SetValue(t, value);
                                }
                                catch
                                {
                                    throw new Exception($"decimal{cell.ToString()}格式不正确!");
                                }
                            }
                            else { throw new Exception("excel format exception."); }
                        }
                    }
                    tableValues.Add(t);
                }
            }
            return tableValues;
        }
        /// <summary>
        /// 流转Model
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static dynamic StreamToModel(Stream stream, string filetype, string className)
        {


            List<object> list = new List<object>();
            ISheet sheet;
            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                xssfworkbook = new XSSFWorkbook(stream);
                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                hssfworkbook = new HSSFWorkbook(stream);
                sheet = hssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;


            List<string> excelColumnsName = new List<string>();
            for (int j = 0; j < cellCount; j++)
            {
                ICell cellCol = headerRow.GetCell(j);
                excelColumnsName.Add(cellCol.ToString());
            }

            

            var modelType = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("SunnyMES.Security.Core")).FirstOrDefault()
                .GetTypes().Where(x => x.Name == className).FirstOrDefault();
            var type = modelType.GetProperties();



            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);


                if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                {
                    var t = Activator.CreateInstance(modelType);

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) == null)
                            continue;
                        var cell = row.GetCell(j);
                        var item = type.FirstOrDefault(m => m.Name == excelColumnsName[j]);
                        if (item is null)
                            continue;
                        if (item.PropertyType == typeof(DateTime?) || item.PropertyType == typeof(DateTime?))
                        {
                            try
                            {
                                if (cell.CellType == CellType.String)
                                {
                                    var value = Convert.ToDateTime(cell.ToString());
                                    item.SetValue(t, value);
                                }
                                else
                                {
                                    item.SetValue(t, cell.DateCellValue);
                                }
                            }
                            catch
                            {
                                throw new Exception($"DateTime{cell.ToString()}格式不正确!");
                            }
                        }
                        else if (item.PropertyType == typeof(int))
                        {
                            try
                            {
                                item.SetValue(t, Convert.ToInt32(cell.ToString()));
                            }
                            catch
                            {
                                throw new Exception($"int{cell.ToString()}格式不正确!");
                            }
                        }
                        else if (item.PropertyType == typeof(string))
                        {
                            if (cell.CellType == CellType.String)
                            {
                                item.SetValue(t, cell.ToString());
                            }
                            else
                            {
                                item.SetValue(t, cell.NumericCellValue.ToString());
                            }

                        }
                        else if (item.PropertyType == typeof(decimal?) || item.PropertyType == typeof(decimal))
                        {
                            if (cell != null)
                            {
                                try
                                {
                                    var value = 0m;
                                    if (cell.CellType == CellType.String)
                                    {
                                        value = Convert.ToDecimal(cell.ToString());
                                    }
                                    else
                                    {
                                        value = Convert.ToDecimal(cell.NumericCellValue);
                                    }
                                    item.SetValue(t, value);
                                }
                                catch
                                {
                                    throw new Exception($"decimal{cell.ToString()}格式不正确!");
                                }
                            }
                            else { throw new Exception("excel format exception."); }
                        }
                    }
                    list.Add(t);
                }
            }
            return list;
        }
        /// <summary>
        /// 获得Excel列名
        /// </summary>
        private static Dictionary<string, string> GetPropertyByType<In>(bool isToExcel)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var type = typeof(In);
            try
            {
                foreach (var item in type.GetProperties())
                {
                    var displayName = item.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayName != null)
                    {
                        if (isToExcel)
                        {
                            dict.Add(item.Name, displayName.DisplayName);
                        }
                        else
                        {
                            dict.Add(displayName.DisplayName, item.Name);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dict;
        }

    }
}
