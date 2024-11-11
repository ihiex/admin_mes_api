using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWHServices : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL);

        Task<List<IdDescription>> GetWHType();
        Task<List<CO_WH_ShipmentNew>> GetShipmentNew(string S_Start, string S_End, string FStatus);
        Task<List<CO_WH_ShipmentEntryNew>> GetShipmentEntryNew(string S_FInterID);
        Task<List<CO_WH_ShipmentReport>> GetShipmentReport(string S_Start, string S_End, string FStatus);

        Task<string> SetShipmentNew(
                string S_FInterID,

                string S_HAWB, string S_PalletCount, string S_GrossWeight, string S_Project,
                string S_ShipDate, string S_PalletSeq, string S_EmptyCarton,
                string S_PalletSN, string S_ShipNO, string S_ShipID,

                string S_Regin, string S_ReferenceNO, string S_Country, string S_Carrier, string S_HubCode,
                string S_TruckNo, string S_ReturnAddress, string S_DeliveryStreetAddress, string S_DeliveryRegion,
                string S_DeliveryToName, string S_DeliveryCityName, string S_DeliveryCountry, string S_AdditionalDeliveryToName,
                string S_DeliveryPostalCode, string S_TelNo,

                string S_OceanContainerNumber, string S_Origin, string S_TotalVolume, string S_POE_COC, string S_TransportMethod, string S_POE,

                string S_CTRY, string S_SHA, string S_Sales, string S_WebJ, string S_UUI, string S_DNJ, string S_DeliveryJ,
                string S_Special, string S_SCAC, string S_OEMSpecificPO1, string S_OEMSpecificPO2,

                string S_Type
            );




        Task<string> UpdateFStatus(string FInterID_List, string FStatus);
        Task<string> DelShipmentNew(string FInterID);

        Task<string> SetShipmentEntryNew(
                string S_FDetailID, string S_FInterID, string S_FEntryID, string S_FCarrierNo, string S_FCommercialInvoice,
                string S_FCrossWeight, string S_FCTN, string S_FKPONO, string S_FLineItem, string S_FMPNNO, string S_FNetWeight,
                string S_FOutSN, string S_FPartNumberDesc, string S_FQTY, string S_FStatus, string S_FProjectNO,

                string S_Type
            );

        Task<string> DelShipmentEntryNew(string S_FDetailID, string S_FInterID);
    }
}



