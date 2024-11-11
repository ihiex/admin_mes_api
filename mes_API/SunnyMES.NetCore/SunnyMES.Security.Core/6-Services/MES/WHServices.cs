using API_MSG;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.Repositories;

namespace SunnyMES.Security.Services
{
    public class WHServices : BaseServiceReport<string>, IWHServices
    {
        private readonly IWHRepository _repository;
        private readonly ILogService _logService;

        public WHServices(IWHRepository repository, ILogService logService) : base(repository)
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


        public async Task<List<IdDescription>> GetWHType()
        {
            return await _repository.GetWHType();
        }

        public async Task<List<CO_WH_ShipmentNew>> GetShipmentNew(string S_Start, string S_End, string FStatus)
        {
            return await _repository.GetShipmentNew(S_Start, S_End, FStatus);
        }

        public async Task<List<CO_WH_ShipmentEntryNew>> GetShipmentEntryNew(string S_FInterID)
        {
            return await _repository.GetShipmentEntryNew(S_FInterID);
        }

        public async Task<List<CO_WH_ShipmentReport>> GetShipmentReport(string S_Start, string S_End, string FStatus)
        {
            return await _repository.GetShipmentReport(S_Start , S_End, FStatus);
        }


        public async Task<string> SetShipmentNew
            (
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
            )
        {
            return await _repository.SetShipmentNew
                (
                 S_FInterID,

                 S_HAWB, S_PalletCount, S_GrossWeight, S_Project,
                 S_ShipDate, S_PalletSeq, S_EmptyCarton,
                 S_PalletSN, S_ShipNO, S_ShipID,

                 S_Regin, S_ReferenceNO, S_Country, S_Carrier, S_HubCode,
                 S_TruckNo, S_ReturnAddress, S_DeliveryStreetAddress, S_DeliveryRegion,
                 S_DeliveryToName, S_DeliveryCityName, S_DeliveryCountry, S_AdditionalDeliveryToName,
                 S_DeliveryPostalCode, S_TelNo,

                 S_OceanContainerNumber, S_Origin, S_TotalVolume, S_POE_COC, S_TransportMethod, S_POE,

                 S_CTRY, S_SHA, S_Sales, S_WebJ, S_UUI, S_DNJ, S_DeliveryJ,
                 S_Special, S_SCAC, S_OEMSpecificPO1, S_OEMSpecificPO2,

                 S_Type
                );
        }

        public async Task<string> UpdateFStatus(string FInterID_List, string FStatus)
        {
            return await _repository.UpdateFStatus(FInterID_List, FStatus);
        }

        public async Task<string> DelShipmentNew(string FInterID)
        {
            return await _repository.DelShipmentNew(FInterID);
        }


        public async Task<string> SetShipmentEntryNew
            (
                string S_FDetailID, string S_FInterID, string S_FEntryID, string S_FCarrierNo, string S_FCommercialInvoice,
                string S_FCrossWeight, string S_FCTN, string S_FKPONO, string S_FLineItem, string S_FMPNNO, string S_FNetWeight,
                string S_FOutSN, string S_FPartNumberDesc, string S_FQTY, string S_FStatus, string S_FProjectNO,

                string S_Type
            )
        {
            return await _repository.SetShipmentEntryNew
                (
                 S_FDetailID, S_FInterID, S_FEntryID, S_FCarrierNo, S_FCommercialInvoice,
                 S_FCrossWeight, S_FCTN, S_FKPONO, S_FLineItem, S_FMPNNO, S_FNetWeight,
                 S_FOutSN, S_FPartNumberDesc, S_FQTY, S_FStatus, S_FProjectNO,

                 S_Type
                );
        }

        public async Task<string> DelShipmentEntryNew(string S_FDetailID, string S_FInterID)
        {
            return await _repository.DelShipmentEntryNew(S_FDetailID,S_FInterID);
        }



    }
}