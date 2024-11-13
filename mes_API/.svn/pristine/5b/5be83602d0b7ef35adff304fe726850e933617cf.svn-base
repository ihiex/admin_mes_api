using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SunnyMES.Commons.Core.Dtos;

[Serializable]
public class StationAttributes
{
    /// <summary>
    /// 是否检查PO
    /// </summary>
    public virtual string IsCheckPO { get; set; } = "1";
    /// <summary>
    /// 是否检查PN
    /// </summary>
    public virtual string IsCheckPN { get; set; } = "1";
    /// <summary>
    /// 程序类型
    /// </summary>
    public virtual string ApplicationType { get; set; } = "";


    /// <summary>
    /// TT 扫描类型
    /// </summary>
    public virtual string TTScanType { get; set; } = "0";


    /// <summary>
    /// SN扫描类型
    /// </summary>
    public virtual string SNScanType { get; set; } = "0";

    /// <summary>
    /// 是否检查供应商
    /// </summary>
    public virtual string IsCheckVendor { get; set; } = "0";

    /// <summary>
    /// 是否允许重测
    /// </summary>
    public virtual string COF { get; set; } = "0";

    public virtual string IsDOEPrint { get; set; } = "0";
    /// <summary>
    /// 
    /// </summary>
    public virtual string PlasmaTimeOut { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string PlasmaGroupNumber { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string PlasmaMaxGroup { get; set; }
    /// <summary>
    /// 新治具名称
    /// </summary>
    public virtual string ToolingFrom { get; set; }
    /// <summary>
    /// 旧治具名称
    /// </summary>
    public virtual string ToolingTo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsTimeCheck_Shell { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckStartStationType_Shell { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMin_Shell { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string PlasmaSetTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMax_Shell { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsTimeCheck_MF { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckStartStationType_MF { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMin_MF { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMax_MF { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string ORTUnitStateID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string ORTFromStationTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string StationTypeType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TTtarryTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string AdhesiveTimeCheck { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsScanAdhesive { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string AdhesiveTimeCheckOut { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsSendYieldToInsight { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsSendModuleToInsight { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsStationTypeName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsTimeCheck_NFCKitting { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckStartStationType_NFCKitting { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMin_NFCKitting { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TimeCheckMax_NFCKitting { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string SiemensProjectType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string SiemensDBConn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual string IsSampleAlarm { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string SampleAlarmTimePoint { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string SampleCount { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsRPTOutput { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsShowTTWIP { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsLastBox { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsNotPrint { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsQC { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TTBoxType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual string TTBoxSN_Pattern { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string IsTTBoxUnpack { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public virtual string TTRegistSN { get; set; }

    /// <summary>
    /// 0:原始模式，只需要扫描箱码，不能扫描栈板条码 ---20230824  老板要求放弃0，和winform保持一致
    /// 1: Carton Box SN ,先扫栈板条码，再扫箱码
    /// 2: Pallet SN， 只扫栈板条码所有箱子一起过站
    /// </summary>
    public virtual string IsScanPalletSNOrCartonBoxSN { get; set; } = "1";

    /// <summary>
    /// 传递给PLC READ 指令
    /// </summary>
    public string READ { get; set; } = "READ";
    /// <summary>
    /// 传递给PLC READY 指令
    /// </summary>
    public string READY { get; set; } = "READY";
    /// <summary>
    /// 传递给PLC Pass指令
    /// </summary>
    public string PASS { get; set; } = "PASS";

    /// <summary>
    /// 传递给PLC Fail指令
    /// </summary>
    public string FAIL { get; set; } = "FAIL";
    /// <summary>
    ///是否是自动站， 1 是自动站，0 非自动站
    /// </summary>
    public string AutoStation { get; set; } = "0";
    /// <summary>
    /// 串口端口名
    /// </summary>
    public string Port { get; set; } = "COM1";

    /// <summary>
    /// 用于发送指令后，需要延迟的时间，单位毫秒
    /// </summary>
    public string Delay { get; set; } = "0";
    /// <summary>
    /// 串口波特率
    /// </summary>
    public string COM_BaudRate { get; set; } = "9600";
    /// <summary>
    /// 串口数据位长度
    /// </summary>
    public string COM_DataBits { get; set; } = "8";
    /// <summary>
    /// 串口校验位
    /// </summary>
    public string COM_Parity { get; set; } = "Odd";
    /// <summary>
    /// 串口停止位
    /// </summary>
    public string COM_StopBits { get; set; } = "1";
    /// <summary>
    /// 打印端口连接
    /// </summary>
    public string PrintIPPort { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Station_IsTimeCheck { get; set; }

    /// <summary>
    /// 是否绑定NFC
    /// </summary>
    public string IsBindNFC { get; set; }
    /// <summary>
    /// 是否绑定胶
    /// </summary>
    public string IsBindAdhesive { get; set; }

    /// <summary>
    /// 退回后新的状态ID,多个状态以逗号隔开
    /// </summary>
    public string OOBANewUnitStateID { get; set; }


    /// <summary>
    /// 当扫描类型为1时，当前项设置为1，则启用解锁按钮，反之则不显示解锁按钮
    /// </summary>
    public string IsShowReleaseBtn { get; set; }

    /// <summary>
    /// 针对OOBA工站，打散退回前工站，是否需要保留UPC条码，参数为1则保留，其他则不保留
    /// </summary>
    public string IsKeepUPCSN { get; set; }

    /// <summary>
    /// 针对OOBAs
    /// </summary>
    public string IsCreateUPCSN { get; set; }

    /// <summary>
    /// OOBA允许的状态ID
    /// </summary>
    public string CheckOOBAUnitStatusID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string CheckQCTimeOut { get; set; }
    /// <summary>
    /// 卡号正则表达式
    /// </summary>
    public string CardIDPattern { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string IsRPTYield { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string YieldCombineStationTypeID { get; set; }
    /// <summary>
    /// 报表站点名字别名
    /// </summary>
    public string RPTOutputName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string CheckReworkTimes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string CheckReworkStationTypeID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SetReworkStatusID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string IsCheckBuckFailCount { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string FailedRate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string LatestRecordCount { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SerialFailOfTimes { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string CheckFailedStationTypeID { get; set; }

    /// <summary>
    /// 跳站前的工序状态
    /// </summary>
    public string JumpFromUnitStateID { get; set; }
    /// <summary>
    /// 跳站后的工序状态
    /// </summary>
    public string JumpToUnitStateID { get; set; }
    /// <summary>
    /// 跳站前的产品状态
    /// JumpFromStatusID
    /// </summary>
    public string JumpStatusID { get; set; }
    /// <summary>
    /// 跳站后的产品状态
    /// JumpToStateID
    /// </summary>
    public string JumpUnitStateID { get; set; }
    /// <summary>
    /// 针对新增的参数放在此处    /// 
    /// </summary>
    public Dictionary<string, string> OtherProperty { get; set; } = new Dictionary<string, string>();
}