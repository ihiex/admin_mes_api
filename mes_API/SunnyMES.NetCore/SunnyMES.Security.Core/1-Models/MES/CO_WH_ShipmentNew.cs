using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    /// <summary>
    /// [CO_WH_ShipmentNew]表实体类
    /// 创建时间:2023-04-18 08:57:52
    /// </summary>
    [Serializable]
    public  class CO_WH_ShipmentNew
    {

        private int? _FInterID;
        /// <summary>
        /// 
        /// </summary>
        public int? FInterID
        {
            set { _FInterID = value; }
            get { return _FInterID; }
        }
        private string _FBillNO;
        /// <summary>
        /// 
        /// </summary>
        public string FBillNO
        {
            set { _FBillNO = value; }
            get { return _FBillNO; }
        }
        private DateTime? _FDate;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FDate
        {
            set { _FDate = value; }
            get { return _FDate; }
        }
        private string _HAWB;
        /// <summary>
        /// 
        /// </summary>
        public string HAWB
        {
            set { _HAWB = value; }
            get { return _HAWB; }
        }
        private int? _FPalletSeq;
        /// <summary>
        /// 
        /// </summary>
        public int? FPalletSeq
        {
            set { _FPalletSeq = value; }
            get { return _FPalletSeq; }
        }
        private int? _FPalletCount;
        /// <summary>
        /// 
        /// </summary>
        public int? FPalletCount
        {
            set { _FPalletCount = value; }
            get { return _FPalletCount; }
        }
        private int? _FGrossweight;
        /// <summary>
        /// 
        /// </summary>
        public int? FGrossweight
        {
            set { _FGrossweight = value; }
            get { return _FGrossweight; }
        }
        private int? _FEmptyCarton;
        /// <summary>
        /// 
        /// </summary>
        public int? FEmptyCarton
        {
            set { _FEmptyCarton = value; }
            get { return _FEmptyCarton; }
        }
        private int? _FUserID;
        /// <summary>
        /// 
        /// </summary>
        public int? FUserID
        {
            set { _FUserID = value; }
            get { return _FUserID; }
        }
        private DateTime? _FCreateDatetime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FCreateDatetime
        {
            set { _FCreateDatetime = value; }
            get { return _FCreateDatetime; }
        }
        private int? _FStatus;
        /// <summary>
        /// 
        /// </summary>
        public int? FStatus
        {
            set { _FStatus = value; }
            get { return _FStatus; }
        }
        private string _FShipNO;
        /// <summary>
        /// 
        /// </summary>
        public string FShipNO
        {
            set { _FShipNO = value; }
            get { return _FShipNO; }
        }
        private int? _FCTN;
        /// <summary>
        /// 
        /// </summary>
        public int? FCTN
        {
            set { _FCTN = value; }
            get { return _FCTN; }
        }
        private string _FProjectNO;
        /// <summary>
        /// 
        /// </summary>
        public string FProjectNO
        {
            set { _FProjectNO = value; }
            get { return _FProjectNO; }
        }
        private string _Region;
        /// <summary>
        /// 
        /// </summary>
        public string Region
        {
            set { _Region = value; }
            get { return _Region; }
        }
        private string _Country;
        /// <summary>
        /// 
        /// </summary>
        public string Country
        {
            set { _Country = value; }
            get { return _Country; }
        }
        private string _HubCode;
        /// <summary>
        /// 
        /// </summary>
        public string HubCode
        {
            set { _HubCode = value; }
            get { return _HubCode; }
        }
        private string _TruckNo;
        /// <summary>
        /// 
        /// </summary>
        public string TruckNo
        {
            set { _TruckNo = value; }
            get { return _TruckNo; }
        }
        private string _ReferenceNO;
        /// <summary>
        /// 
        /// </summary>
        public string ReferenceNO
        {
            set { _ReferenceNO = value; }
            get { return _ReferenceNO; }
        }
        private string _Carrier;
        /// <summary>
        /// 
        /// </summary>
        public string Carrier
        {
            set { _Carrier = value; }
            get { return _Carrier; }
        }
        private string _ShipID;
        /// <summary>
        /// 
        /// </summary>
        public string ShipID
        {
            set { _ShipID = value; }
            get { return _ShipID; }
        }
        private string _ReturnAddress;
        /// <summary>
        /// 
        /// </summary>
        public string ReturnAddress
        {
            set { _ReturnAddress = value; }
            get { return _ReturnAddress; }
        }
        private string _DeliveryToName;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryToName
        {
            set { _DeliveryToName = value; }
            get { return _DeliveryToName; }
        }
        private string _AdditionalDeliveryToName;
        /// <summary>
        /// 
        /// </summary>
        public string AdditionalDeliveryToName
        {
            set { _AdditionalDeliveryToName = value; }
            get { return _AdditionalDeliveryToName; }
        }
        private string _DeliveryStreetAddress;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryStreetAddress
        {
            set { _DeliveryStreetAddress = value; }
            get { return _DeliveryStreetAddress; }
        }
        private string _DeliveryCityName;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryCityName
        {
            set { _DeliveryCityName = value; }
            get { return _DeliveryCityName; }
        }
        private string _DeliveryPostalCode;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryPostalCode
        {
            set { _DeliveryPostalCode = value; }
            get { return _DeliveryPostalCode; }
        }
        private string _DeliveryRegion;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryRegion
        {
            set { _DeliveryRegion = value; }
            get { return _DeliveryRegion; }
        }
        private string _DeliveryCountry;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryCountry
        {
            set { _DeliveryCountry = value; }
            get { return _DeliveryCountry; }
        }
        private string _TelNo;
        /// <summary>
        /// 
        /// </summary>
        public string TelNo
        {
            set { _TelNo = value; }
            get { return _TelNo; }
        }
        private string _MAWB_OceanContainerNumber;
        /// <summary>
        /// 
        /// </summary>
        public string MAWB_OceanContainerNumber
        {
            set { _MAWB_OceanContainerNumber = value; }
            get { return _MAWB_OceanContainerNumber; }
        }
        private string _TotalVolume;
        /// <summary>
        /// 
        /// </summary>
        public string TotalVolume
        {
            set { _TotalVolume = value; }
            get { return _TotalVolume; }
        }
        private string _VolumeUnit;
        /// <summary>
        /// 
        /// </summary>
        public string VolumeUnit
        {
            set { _VolumeUnit = value; }
            get { return _VolumeUnit; }
        }
        private string _TransportMethod;
        /// <summary>
        /// 
        /// </summary>
        public string TransportMethod
        {
            set { _TransportMethod = value; }
            get { return _TransportMethod; }
        }
        private string _Origin;
        /// <summary>
        /// 
        /// </summary>
        public string Origin
        {
            set { _Origin = value; }
            get { return _Origin; }
        }
        private string _POE_COC;
        /// <summary>
        /// 
        /// </summary>
        public string POE_COC
        {
            set { _POE_COC = value; }
            get { return _POE_COC; }
        }
        private string _POE;
        /// <summary>
        /// 
        /// </summary>
        public string POE
        {
            set { _POE = value; }
            get { return _POE; }
        }
        private string _PalletSN;
        /// <summary>
        /// 
        /// </summary>
        public string PalletSN
        {
            set { _PalletSN = value; }
            get { return _PalletSN; }
        }
        private string _CTRY;
        /// <summary>
        /// 
        /// </summary>
        public string CTRY
        {
            set { _CTRY = value; }
            get { return _CTRY; }
        }
        private string _SHA;
        /// <summary>
        /// 
        /// </summary>
        public string SHA
        {
            set { _SHA = value; }
            get { return _SHA; }
        }
        private string _Sales;
        /// <summary>
        /// 
        /// </summary>
        public string Sales
        {
            set { _Sales = value; }
            get { return _Sales; }
        }
        private string _WebJ ;
        /// <summary>
        /// 
        /// </summary>
        public string WebJ
        {
            set
            {
                _WebJ = value; }
            get {
                    return _WebJ; }
        }
        private string _UUI;
        /// <summary>
        /// 
        /// </summary>
        public string UUI
        {
            set { _UUI = value; }
            get { return _UUI; }
        }
        private string _DNJ ;
        /// <summary>
        /// 
        /// </summary>
        public string DNJ
        {
            set
            {
                _DNJ = value; }
            get {
                    return _DNJ; }
        }
        private string _DeliveryJ ;
        /// <summary>
        /// 
        /// </summary>
        public string DeliveryJ
        {
            set
            {
                _DeliveryJ = value; }
            get {
                    return _DeliveryJ; }
        }
        private string _Special;
        /// <summary>
        /// 
        /// </summary>
        public string Special
        {
            set { _Special = value; }
            get { return _Special; }
        }
        private string _SCAC;
        /// <summary>
        /// 
        /// </summary>
        public string SCAC
        {
            set { _SCAC = value; }
            get { return _SCAC; }
        }
        private string _OEMSpecificPO1;
        /// <summary>
        /// 
        /// </summary>
        public string OEMSpecificPO1
        {
            set { _OEMSpecificPO1 = value; }
            get { return _OEMSpecificPO1; }
        }
        private string _OEMSpecificPO2;
        /// <summary>
        /// 
        /// </summary>
        public string OEMSpecificPO2
        {
            set { _OEMSpecificPO2 = value; }
            get { return _OEMSpecificPO2; }
        }


        private string _MyStatus;
        /// <summary>
        /// 
        /// </summary>
        public string MyStatus
        {
            set { _MyStatus = value; }
            get { return _MyStatus; }
        }
    }


}
