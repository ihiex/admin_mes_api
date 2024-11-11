using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.Public;
//using SunnyMES.Security._1_Models.Report;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.Services
{
    public class PublicSCService : BaseServiceReport<string>, IPublicSCService
    {
        private readonly IPublicSCRepository _repository;
        private readonly ILogService _logService;

        public PublicSCService(IPublicSCRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<SC_IdDesc>> GetIdDescription(string TableName, string ID) 
        {
            return await _repository.GetIdDescription(TableName, ID);
        }

        public async Task<List<SC_IdDescDescEN>> GetIdDescDescEN(string S_Sql) 
        {
            return await _repository.GetIdDescDescEN(S_Sql);
        }

        public async Task<List<SC_IdDescDescEN>> GetWinPermission(string UserId)   
        {
            return await _repository.GetWinPermission(UserId);
        }

        public async Task<List<SC_IdNameDesc>> GetIdNameDescription(string TableName, string ID)
        {
            return await _repository.GetIdNameDescription(TableName, ID);
        }

        public async Task<List<SC_IdSNDesc>> GetIdSNDescription(string TableName, string ID)
        {
            return await _repository.GetIdSNDescription(TableName, ID);
        }

        public async Task<List<SC_Employee>> GetEmployee()
        {
            return await _repository.GetEmployee();
        }


        public async Task<List<SC_SIdDesc>> GetLineType() 
        {
            return await _repository.GetLineType();
        }

        public async Task<List<SC_mesLine>> GetmesLine(string ID) 
        {
            return await _repository.GetmesLine(ID);
        }

        public async Task<List<SC_IdDescLineType>> GetmesLinePO(string S_LineTypeID, string S_PartFamilyID) 
        {
            return await  _repository.GetmesLinePO(S_LineTypeID, S_PartFamilyID);
        }

        public async Task<List<SC_IdDesc>> GetTTWIPReportType() 
        {
            return await _repository.GetTTWIPReportType();
        }

        public async Task<List<SC_IdDesc>> GetTTWIPTableName(string ReportType) 
        {
            return await _repository.GetTTWIPTableName(ReportType);
        }

        public async Task<List<SC_SIdName>> GetLineGroupName()
        {
            return await _repository.GetLineGroupName();
        }

        public async Task<List<SC_IdPOSNDesc>> GetmesProductionOrder() 
        {
            return await _repository.GetmesProductionOrder();
        }

        public async Task<List<V_LabelType>> GetVLabelType()
        {
            return await _repository.GetVLabelType();
        }
        public async Task<List<V_OutputType>> GetVOutputType()
        {
            return await _repository.GetVOutputType();
        }
        public async Task<List<V_ModuleName>> GetVModuleName()
        {
            return await _repository.GetVModuleName();
        }

        public async Task<List<Q_DBField>> GetDBField(string S_Type)
        {
            return await _repository.GetDBField(S_Type);
        }

        public async Task<List<V_FunctionName>> GetVFunctionName()
        {
            return await _repository.GetVFunctionName();
        }


        public async Task<List<SC_IdName>> GetLabelFormatPosName() 
        {
            return await _repository.GetLabelFormatPosName();
        }

        public async Task<List<SC_IdDesc>> GetSectionType() 
        {
            return await _repository.GetSectionType();
        }

        public async Task<List<SC_IdDesc>> GetSectionStatus() 
        {
            return await _repository.GetSectionStatus();
        }

        public async Task<List<SC_IdName>> GetProcesureUDP() 
        {
            return await _repository.GetProcesureUDP();
        }

        public async Task<List<SC_IdDesc>> GetluProductionOrderDetailDef_PMC() 
        {
            return await _repository.GetluProductionOrderDetailDef_PMC();
        }


        public async Task<List<Com_Value>> GetCom_a() 
        {
            return await _repository.GetCom_a();
        }
        public async Task<List<Com_Value>> GetCom_d()
        {
            return await _repository.GetCom_d();
        }
        public async Task<List<Com_Value>> GetCom_h()
        {
            return await _repository.GetCom_h();
        }
        public async Task<List<Com_Value>> GetCom_n()
        {
            return await _repository.GetCom_n();
        }
        public async Task<List<Com_Value>> GetCom_o()
        {
            return await _repository.GetCom_o();
        }

        public async Task<List<SC_mesPart>> GetmesPart(string ID)
        {
            return await _repository.GetmesPart(ID);
        }

        public async Task<string> InitializeBaseData()
        {
            return await _repository.InitializeBaseData();
        }

        public async Task<List<SC_StationTypeTT>> GetSectionTypeTT()
        {
            List<SC_StationTypeTT> sC_StationTypeTTs = new List<SC_StationTypeTT>();
            string sql = @"SELECT a.ID,a.Description, CASE WHEN b.Content = 'TT' THEN 0 ELSE 1 END  IsSFC
                            FROM dbo.mesStationType a
                            LEFT JOIN dbo.mesStationTypeDetail b ON b.StationTypeID = a.ID AND b.StationTypeDetailDefID = (SELECT ID FROM dbo.luStationTypeDetailDef WHERE Description = 'StationTypeType')";

            sC_StationTypeTTs = await SqlSugarHelper.Db.Ado.SqlQueryAsync<SC_StationTypeTT>(sql);

            return sC_StationTypeTTs;
        }
    }
}
