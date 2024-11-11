using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper.Execution;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Services;
//using SunnyMES.Email;
//using SunnyMES.Security.Dtos.Report;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.Services
{
    public class ReportService : BaseServiceReport<string>, IReportService
    {
        private readonly IReportRepository _repository;
        private readonly ILogService _logService;
        public ReportService(IReportRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }


        public async Task<List<dynamic>> GetOutputSum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //GetAPP();
            return await _repository.GetOutputSum(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel
                ,  YieldLevel,  IsCombineYield);
        }


        public async Task<IEnumerable<dynamic>> GetPartFamilyType()
        {
            //GetAPP();
            return await _repository.GetPartFamilyType();
        }

        public async Task<IEnumerable<dynamic>> GetPartFamily(string PartFamilyTypeID)
        {
            //GetAPP();
            return await _repository.GetPartFamily(PartFamilyTypeID);
        }


        public async Task<IEnumerable<dynamic>> GetPart(string PartFamilyID)
        {
            //GetAPP();
            return await _repository.GetPart(PartFamilyID);
        }


        public async Task<IEnumerable<dynamic>> GetProductionOrder(string PartID)
        {
            //GetAPP();
            return await _repository.GetProductionOrder(PartID);
        }


        public async Task<IEnumerable<dynamic>> GetStationType()
        {
            // GetAPP();
            return await _repository.GetStationType();
        }


        public async Task<IEnumerable<dynamic>> GetStation(string StationTypeID)
        {
            //GetAPP();
            return await _repository.GetStation(StationTypeID);
        }

        public async Task<IEnumerable<dynamic>> GetShift()
        {
            // GetAPP();
            return await _repository.GetShift();
        }

        public async Task<IEnumerable<dynamic>> GetLine()
        {
            // GetAPP();
            return await _repository.GetLine();
        }

        public async Task<List<dynamic>> GetUPHYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //GetAPP();
            return await _repository.GetUPHYield(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType,
                 S_DataLevel,  YieldLevel,  IsCombineYield);
        }

        public async Task<List<dynamic>> GetUPH(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {            
            return await _repository.GetUPH(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel
                ,  YieldLevel,  IsCombineYield);
        }

        public async Task<List<dynamic>> GetUPHCum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {            
            return await _repository.GetUPHCum(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel
                , YieldLevel, IsCombineYield);
        }

        public async Task<List<dynamic>> GetDefectYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string S_TopQTY, string YieldLevel, string IsCombineYield)
        {
            //GetAPP();
            return await _repository.GetDefectYield(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel, S_TopQTY
                ,  YieldLevel,  IsCombineYield);
        }

        public async Task<IEnumerable<dynamic>> GetDataLevel()
        {
            //GetAPP();
            return await _repository.GetDataLevel();
        }

        public async Task<List<dynamic>> GetReportGeneral(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
           // string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            return await _repository.GetReportGeneral(S_Type, S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
             //   Shift,  YieldLevel,  IsCombineYield,
                S_SNs, S_PageIndex, S_PageQTY
                );
        }

        public async Task<List<dynamic>> GetSearchCenter(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_Type)
        {
            return await _repository.GetSearchCenter(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                S_SNs, S_Type
                );
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            return await _repository.GetExportExcel(S_Type, S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                S_SNs, S_PageIndex, S_PageQTY);
        }

        public async Task<List<dynamic>> GetWIPComponentList(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            return await _repository.GetWIPComponentList(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,

                S_SNs, S_PageIndex, S_PageQTY
                );
        }

        public async Task<List<dynamic>> GetRawData(  string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
             string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            return await _repository.GetRawData(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                 Shift, YieldLevel, IsCombineYield,
                S_SNs, S_PageIndex, S_PageQTY
                );
        }


        public async Task<IEnumerable<TabVal>> GetExportExcel_RawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            return await _repository.GetExportExcel_RawData( S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                Shift, YieldLevel, IsCombineYield,
                S_SNs, S_PageIndex, S_PageQTY);
        }


        public async Task<IEnumerable<dynamic>> GetPartFamilyAll(string S_PartFamilyTypeID)
        {
            return await _repository.GetPartFamilyAll(S_PartFamilyTypeID);
        }

        public async Task<IEnumerable<dynamic>> GetPartFamilyTypeAll()
        {
            return await _repository.GetPartFamilyTypeAll();
        }

        public async Task<IEnumerable<dynamic>> GetPartNumberAll(string S_PartFamilyID)
        {
            return await _repository.GetPartNumberAll(S_PartFamilyID);
        }

        public async Task<IEnumerable<dynamic>> GetProductionOrderAll(string S_PartID)
        {
            return await _repository.GetProductionOrderAll(S_PartID);
        }

        public async Task<IEnumerable<dynamic>> GetStationAll(string S_StationTypeID)
        {
            return await _repository.GetStationAll(S_StationTypeID);
        }

        public async Task<IEnumerable<dynamic>> GetStationTypeAll()
        {
            return await _repository.GetStationTypeAll();
        }

        public async Task<IEnumerable<dynamic>> GetLineAll()
        {
            return await _repository.GetLineAll();
        }

        public async Task<IEnumerable<dynamic>> GetUnitStatusAll()
        {
            return await _repository.GetUnitStatusAll();
        }

        public async Task<IEnumerable<dynamic>> GetUnitStateAll()
        {
            return await _repository.GetUnitStateAll();
        }

        public async Task<List<dynamic>> GetSAPStock(int plant, string version, string material,  int PageIndex, int PageQTY)
        {
            return await _repository.GetSAPStock(plant, version, material,PageIndex,PageQTY);
        }

        public async Task<List<dynamic>> GetSAPAllPlant()
        {
            return await _repository.GetSAPAllPlant();
        }

        public async Task<List<dynamic>> GetSAPAllVersion(int plant)
        {
            return await _repository.GetSAPAllVersion(plant);
        }

        public async Task<List<dynamic>> GetSAPStockMaterial(int plant)
        {
            return await _repository.GetSAPStockMaterial(plant);
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel_SapStock(int plant, string version, string material)
        {
            return await _repository.GetExportExcel_SapStock(plant,version,material);
        }

        public async Task<List<dynamic>> GetSAPStockPro(int plant, string version, string material, int PageIndex, int PageQTY, SapDateType sapDateType)
        {
            return await _repository.GetSAPStockPro(plant, version, material, PageIndex, PageQTY, sapDateType);
        }

        public async Task<IEnumerable<TabVal>> GetExportExcel_SapStockPro(int plant, string version, string material, SapDateType sapDateType)
        {
            return await _repository.GetExportExcel_SapStockPro(plant, version, material, sapDateType);
        }

        public async Task<List<dynamic>> GetAutoAnalysisAlarmDashboard()
        {
            return await _repository.GetAutoAnalysisAlarmDashboard();
        }

        //public string SendMail(AttachMailInputDto attachMailInputDto)
        public string SendMail()
        {
            string result = string.Empty;
            //try
            //{
            //    for (int i = 0; i < attachMailInputDto.FilePaths.Count; i++)
            //    {
            //        var tmpMBE = attachMailInputDto.FilePaths[i];
            //        if (string.IsNullOrEmpty(tmpMBE) || !File.Exists(tmpMBE))
            //            return result = "请检查文件是否存在 " + tmpMBE;
            //    }
            //    string sql = $@"SET ROWCOUNT 0 
            //                    EXEC msdb.dbo.sp_send_dbmail
            //                    @profile_name = '{attachMailInputDto.ProfileName}',
            //                    @recipients = '{string.Join(';', attachMailInputDto.Recipients)}',
            //                    @body = '{attachMailInputDto.Body}',
            //                    @file_attachments = '{string.Join(';',attachMailInputDto.FilePaths)}',
            //                    @subject = '{attachMailInputDto.Subject}'";
            //    int aff = SqlSugarHelper.Db.Ado.ExecuteCommand(sql);
            //    if (!string.IsNullOrEmpty(attachMailInputDto.MovePath)) {
            //        for (int i = 0; i < attachMailInputDto.FilePaths.Count; i++)
            //        {
            //            var tmpMBE = attachMailInputDto.FilePaths[i];
            //            FileHelper.MoveFile(tmpMBE, attachMailInputDto.MovePath, true);
            //        }
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
            //    result = ex.Message;
            //}
            return result;
        }

        //private IEnumerable<APP> GetAPP()
        //{
        //    IEnumerable<APP> appList = _repository.GetAPP();
        //    MemoryCacheHelper.Set("cacheAppList", appList);

        //    return appList;
        //}



    }
}