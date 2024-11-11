using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
//using SunnyMES.Email;
using SunnyMES.Security.Dtos;
//using SunnyMES.Security.Dtos.Report;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReportService : IServiceReport<string>
    {

        Task<List<dynamic>> GetOutputSum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield);


        Task<IEnumerable<dynamic>> GetPartFamilyType();

        Task<IEnumerable<dynamic>> GetPartFamily(string PartFamilyTypeID);

        Task<IEnumerable<dynamic>> GetPart(string PartFamilyID);

        Task<IEnumerable<dynamic>> GetProductionOrder(string PartID);

        Task<IEnumerable<dynamic>> GetStationType();

        Task<IEnumerable<dynamic>> GetStation(string StationTypeID);

        Task<IEnumerable<dynamic>> GetShift();

        Task<IEnumerable<dynamic>> GetLine();

        Task<List<dynamic>> GetUPHYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield);

        Task<List<dynamic>> GetUPH(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield);

        Task<List<dynamic>> GetUPHCum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield);

        Task<List<dynamic>> GetDefectYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string S_TopQTY, string YieldLevel, string IsCombineYield);

        Task<List<dynamic>> GetReportGeneral(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            //string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY);

        Task<List<dynamic>> GetSearchCenter(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_Type);

        Task<IEnumerable<TabVal>> GetExportExcel(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY);

        Task<List<dynamic>> GetWIPComponentList(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY);

        Task<List<dynamic>> GetRawData(string S_StartDateTime, string S_EndDateTime,
                    string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
                    string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
                     string Shift, string YieldLevel, string IsCombineYield,
                    string S_SNs, string S_PageIndex, string S_PageQTY);

        Task<IEnumerable<TabVal>> GetExportExcel_RawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY);

        Task<IEnumerable<dynamic>> GetDataLevel();

        //IEnumerable<APP> GetAPP();


        Task<IEnumerable<dynamic>> GetPartFamilyAll(string S_PartFamilyTypeID);

        Task<IEnumerable<dynamic>> GetPartFamilyTypeAll();

        Task<IEnumerable<dynamic>> GetPartNumberAll(string S_PartFamilyID);

        Task<IEnumerable<dynamic>> GetProductionOrderAll(string S_PartID);

        Task<IEnumerable<dynamic>> GetStationAll(string S_StationTypeID);

        Task<IEnumerable<dynamic>> GetStationTypeAll();

        Task<IEnumerable<dynamic>> GetLineAll();

        Task<IEnumerable<dynamic>> GetUnitStatusAll();

        Task<IEnumerable<dynamic>> GetUnitStateAll();

        Task<List<dynamic>> GetSAPStock(int plant, string version, string material, int PageIndex, int PageQTY);
        Task<List<dynamic>> GetSAPAllPlant();
        Task<List<dynamic>> GetSAPAllVersion(int plant);
        Task<List<dynamic>> GetSAPStockMaterial(int plant);
        Task<IEnumerable<TabVal>> GetExportExcel_SapStock(int plant, string version, string material);
        Task<List<dynamic>> GetSAPStockPro(int plant, string version, string material, int PageIndex, int PageQTY, SapDateType sapDateType);
        Task<IEnumerable<TabVal>> GetExportExcel_SapStockPro(int plant, string version, string material, SapDateType sapDateType);

        Task<List<dynamic>> GetAutoAnalysisAlarmDashboard();
        //string SendMail(AttachMailInputDto attachMailInputDto);
    }
}
