using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface ISiemensService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);
        Task<List<IdDescription>> GetTemp();
        WH_InDTDto WHIn(string S_MPN, string S_BoxSN, string S_Type);

        //WH_InDTDto WHIn_DT( string S_BoxSN);

        WH_OutDTDto WHOut(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type);

        //WH_OutDTDto WHOut_DT( string S_BoxSN);

        List<WH_BillNo> CheckBillNo(string S_BillNo, out string S_Result);

        List<WH_IPBB> GetIpad_BB();

        List<CO_WH_Shipment> GetShipment(string S_Start, string S_End, string FStatus);
        List<CO_WH_ShipmentEntry> GetShipmentEntry(string S_FInterID);
        List<ShipmentReport> GetShipmentReport(string S_Start, string S_End, string FStatus);

        string Edit_CO_WH_Shipment
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
                    );

        string DeleteShipment(string FInterID);
        string DeleteShipmentEntry(string FDetailID);
        string DeleteMultiSelectShipment(string FInterID_List);
        string UpdateShipment_FStatus(string FInterID_List, string Status);
        List<CO_WH_Shipment> GetShipment_One(string FInterID);
        List<CO_WH_ShipmentEntry> GetShipmentEntry_One(string FDetailID);
        string Edit_CO_WH_ShipmentEntry
            (
                string S_FInterID,
                string S_FEntryID,
                string S_FDetailID,
                string S_FKPONO,
                string S_FMPNNO,
                string S_FCTN,
                string S_FStatus,

                string S_Type
            );

        string DB_ExecSql(string S_Sql);

        WH_ImportDto ImportCheck(List<ExcelDT> v_ExcelDT);

        string ImportEnter(List<ExcelDT> v_ExcelDT);
        //DataSet DB_Data_Set(string S_Sql);
    }
}

