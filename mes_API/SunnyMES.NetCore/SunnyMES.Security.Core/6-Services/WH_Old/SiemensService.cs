using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class SiemensService : BaseServiceReport<string>, ISiemensService
    {
        private readonly ISiemensRepository _repository;
        private readonly ILogService _logService;

        public SiemensService(ISiemensRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            return await _repository.GetConfInfo(I_Language, I_LineID, I_StationID, I_EmployeeID, S_CurrentLoginIP);
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            return await _repository.GetPageInitialize(S_URL);
        }

        public async Task<List<IdDescription>> GetTemp() 
        {
            return await _repository.GetTemp();
        }

        public WH_InDTDto WHIn(string S_MPN, string S_BoxSN, string S_Type) 
        {
            return  _repository.WHIn( S_MPN,  S_BoxSN,  S_Type);
        }

        //public WH_InDTDto WHIn_DT(string S_BoxSN) 
        //{
        //    return _repository.WHIn_DT( S_BoxSN);
        //}

        public WH_OutDTDto WHOut(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type) 
        {
            return _repository.WHOut( S_MPN,  S_BillNo,  S_BoxSN,  S_Type);
        }

        //public WH_OutDTDto WHOut_DT(string S_BoxSN) 
        //{
        //    return _repository.WHOut_DT( S_BoxSN);
        //}


        public List<WH_BillNo> CheckBillNo(string S_BillNo, out string S_Result) 
        {
            return _repository.CheckBillNo( S_BillNo, out S_Result);
        }

        public List<WH_IPBB> GetIpad_BB() 
        {
            return _repository.GetIpad_BB();
        }

        public List<CO_WH_Shipment> GetShipment(string S_Start, string S_End, string FStatus) 
        {
            return _repository.GetShipment( S_Start,  S_End,  FStatus);
        }

        public List<CO_WH_ShipmentEntry> GetShipmentEntry(string S_FInterID) 
        {
            return _repository.GetShipmentEntry( S_FInterID);
        }

        public List<ShipmentReport> GetShipmentReport(string S_Start, string S_End, string FStatus) 
        {
            return _repository.GetShipmentReport( S_Start,  S_End,  FStatus);
        }


        public string Edit_CO_WH_Shipment
                    (
                        string S_FInterID,
                        string S_ShipDate,
                        string S_HAWB,
                        string S_PalletSeq,
                        string S_PalletCount,
                        string S_GrossWeight,
                        string S_EmptyCarton,
                        string S_ShipNO,
                        string S_Project,

                        string S_Type
                    )
        {
            return _repository.Edit_CO_WH_Shipment
                    (
                         S_FInterID,
                         S_ShipDate,
                         S_HAWB,
                         S_PalletSeq,
                         S_PalletCount,
                         S_GrossWeight,
                         S_EmptyCarton,
                         S_ShipNO,
                         S_Project,

                         S_Type
                    );
        }


        public string DeleteShipment(string FInterID) 
        {
            return _repository.DeleteShipment( FInterID);
        }
        public string DeleteShipmentEntry(string FDetailID) 
        {
            return _repository.DeleteShipmentEntry( FDetailID);
        }
        public string DeleteMultiSelectShipment(string FInterID_List) 
        {
            return _repository.DeleteMultiSelectShipment( FInterID_List);
        }

        public string UpdateShipment_FStatus(string FInterID_List, string Status) 
        {
            return _repository.UpdateShipment_FStatus( FInterID_List,  Status);
        }

        public List<CO_WH_Shipment> GetShipment_One(string FInterID) 
        {
            return _repository.GetShipment_One( FInterID);
        }

        public List<CO_WH_ShipmentEntry> GetShipmentEntry_One(string FDetailID) 
        {
            return _repository.GetShipmentEntry_One( FDetailID);
        }

        public string Edit_CO_WH_ShipmentEntry
            (
                string S_FInterID,
                string S_FEntryID,
                string S_FDetailID,
                string S_FKPONO,
                string S_FMPNNO,
                string S_FCTN,
                string S_FStatus,

                string S_Type
            )
        {
            return _repository.Edit_CO_WH_ShipmentEntry
                (
                 S_FInterID,
                 S_FEntryID,
                 S_FDetailID,
                 S_FKPONO,
                 S_FMPNNO,
                 S_FCTN,
                 S_FStatus,

                 S_Type
                );
        }

        public string DB_ExecSql(string S_Sql) 
        {
            return _repository.DB_ExecSql( S_Sql);
        }


        public WH_ImportDto ImportCheck(List<ExcelDT> v_ExcelDT) 
        {
            return _repository.ImportCheck( v_ExcelDT );
        }

        public string ImportEnter(List<ExcelDT> v_ExcelDT) 
        {
            return _repository.ImportEnter(v_ExcelDT);
        }

    }
}
