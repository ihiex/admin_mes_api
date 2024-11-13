using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.Public;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.IServices
{
    public interface IPublicSCService : IServiceReport<string>
    {
        Task<List<SC_IdDesc>> GetIdDescription(string TableName, string ID);
        Task<List<SC_IdDescDescEN>> GetIdDescDescEN(string S_Sql);
        Task<List<SC_IdDescDescEN>> GetWinPermission(string UserId);
        Task<List<SC_IdNameDesc>> GetIdNameDescription(string TableName, string ID);
        Task<List<SC_IdSNDesc>> GetIdSNDescription(string TableName, string ID);
        Task<List<SC_mesPart>> GetmesPart(string ID);
        Task<List<SC_Employee>> GetEmployee();
        Task<List<SC_mesLine>> GetmesLine(string ID);
        Task<List<SC_IdDescLineType>> GetmesLinePO(string S_LineTypeID, string S_PartFamilyID);

        Task<List<SC_SIdDesc>> GetLineType();

        Task<List<SC_IdDesc>> GetTTWIPReportType();

        Task<List<SC_IdDesc>> GetTTWIPTableName(string ReportType);

        Task<List<SC_SIdName>> GetLineGroupName();

        Task<List<SC_IdPOSNDesc>> GetmesProductionOrder();

        Task<List<V_LabelType>> GetVLabelType();

        Task<List<V_OutputType>> GetVOutputType();

        Task<List<V_ModuleName>> GetVModuleName();

        Task<List<Q_DBField>> GetDBField(string S_Type);
        Task<List<V_FunctionName>> GetVFunctionName();


        Task<List<SC_IdName>> GetLabelFormatPosName();

        Task<List<SC_IdDesc>> GetSectionType();
        Task<List<SC_StationTypeTT>> GetSectionTypeTT();
        Task<List<SC_IdDesc>> GetSectionStatus();

        Task<List<SC_IdName>> GetProcesureUDP();

        Task<List<SC_IdDesc>> GetluProductionOrderDetailDef_PMC();

        Task<List<Com_Value>> GetCom_a();
        Task<List<Com_Value>> GetCom_d();
        Task<List<Com_Value>> GetCom_h();
        Task<List<Com_Value>> GetCom_n();
        Task<List<Com_Value>> GetCom_o();

        Task<string> InitializeBaseData();
    }
}