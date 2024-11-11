using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Security.Models;

namespace API_MSG
{
    /// <summary>
    /// Public 消息代码描述
    /// </summary>
    public class MSG_Public
    {
        /// <summary>
        /// 料号未配置工艺流程路线.
        /// </summary>
        public string MSG_Public_001;
        /// <summary>
        /// 当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
        /// </summary>
        public string MSG_Public_002;
        /// <summary>
        /// 工艺流程校验失败
        /// </summary>
        public string MSG_Public_003;
        /// <summary>
        /// 子料对应的工艺路线未完成或者已过站
        /// </summary>
        public string MSG_Public_004;
        /// <summary>
        /// 此条码已过站.
        /// </summary>
        public string MSG_Public_005;


        /// <summary>
        /// 上一站未扫描.
        /// </summary>
        public string MSG_Public_006;
        /// <summary>
        /// 此条码已NG.
        /// </summary>
        public string MSG_Public_007;
        /// <summary>
        /// 无此工序.
        /// </summary>
        public string MSG_Public_008;

        /// <summary>
        /// 工单不能为空,请确认.
        /// </summary>
        public string MSG_Public_009;
        /// <summary>
        /// 未选择料号类别,请确认.
        /// </summary>
        public string MSG_Public_010;


        /// <summary>
        /// 未选择料号群,请确认.
        /// </summary>
        public string MSG_Public_011;
        /// <summary>
        /// 未选择料号,请确认.
        /// </summary>
        public string MSG_Public_012;
        /// <summary>
        /// 未选择工单，请确认.
        /// </summary>
        public string MSG_Public_013;
        /// <summary>
        /// 未配置校验批次正则表达式.
        /// </summary>
        public string MSG_Public_014;
        /// <summary>
        /// 未配置校验SN正则表达式.
        /// </summary>
        public string MSG_Public_015;
        /// <summary>
        /// 条码不能为空.
        /// </summary>
        public string MSG_Public_016;
        /// <summary>
        /// 确认此物料NG,请设置NG原因.
        /// </summary>
        public string MSG_Public_017;
        /// <summary>
        /// 条码不存在或者状态不符.
        /// </summary>
        public string MSG_Public_018;
        /// <summary>
        /// 此条码和选择的工单不一致.
        /// </summary>
        public string MSG_Public_019;
        /// <summary>
        /// 此条码已NG.
        /// </summary>
        public string MSG_Public_020;
        /// <summary>
        /// 当前工站类型在配置的工艺流程中未配置,请检查工艺流程.
        /// </summary>
        public string MSG_Public_021;
        /// <summary>
        /// 工艺流程校验失败
        /// </summary>
        public string MSG_Public_022;
        /// <summary>
        /// 过程名称不能为空.
        /// </summary>
        public string MSG_Public_023;
        /// <summary>
        /// 料号和线别不匹配
        /// </summary>
        public string MSG_Public_024;
        /// <summary>
        /// 条码类型错误（目前类型只有1,2,3。 4为混合    其他则为异常）
        /// </summary>
        public string MSG_Public_025;
        /// <summary>
        ///  没有找到夹具绑定的条码
        /// </summary>
        public string MSG_Public_026;
        /// <summary>
        /// SN不存在
        /// </summary>
        public string MSG_Public_027;
        /// <summary>
        /// uspTTCheck 检查失败
        /// </summary>
        public string MSG_Public_028;
        /// <summary>
        /// 正则校验未通过，请确认.
        /// </summary>
        public string MSG_Public_029;
        /// <summary>
        /// 工单状态不符,请确认!
        /// </summary>
        public string MSG_Public_030;
        /// <summary>
        /// 工单已经超过配置数量,请确认!
        /// </summary>
        public string MSG_Public_031;
        /// <summary>
        /// 料号未配置参数:
        /// </summary>
        public string MSG_Public_032;
        /// <summary>
        /// 未配置打印模板.
        /// </summary>
        public string MSG_Public_033;
        /// <summary>
        /// CodeSoft 模板配置错误.
        /// </summary>
        public string MSG_Public_034;
        /// <summary>
        /// BarTender 模板配置错误.
        /// </summary>
        public string MSG_Public_035;
        /// <summary>
        /// 此工序没有配置打印标签.
        /// </summary>
        public string MSG_Public_036;
        /// <summary>
        /// 未配置打印文件路径.
        /// </summary>
        public string MSG_Public_037;
        /// <summary>
        /// 料号未关联生成SN的格式.
        /// </summary>
        public string MSG_Public_038;
        /// <summary>
        /// 主条码已经存在,不能重复扫描.
        /// </summary>
        public string MSG_Public_039;
        /// <summary>
        /// 条码生成失败.
        /// </summary>
        public string MSG_Public_040;
        /// <summary>
        /// 不能重复扫描.
        /// </summary>
        public string MSG_Public_041;
        /// <summary>
        /// TT箱子已经扫描结束
        /// </summary>
        public string MSG_Public_042;
        /// <summary>
        /// 这个箱子没有绑定的数据
        /// </summary>
        public string MSG_Public_043;
        /// <summary>
        /// 条码格式没找到
        /// </summary>
        public string MSG_Public_044;
        /// <summary>
        /// 夹具状态不对
        /// </summary>
        public string MSG_Public_045;
        /// <summary>
        /// 未配置ValidFrom参数,请确认.
        /// </summary>
        public string MSG_Public_046;
        /// <summary>
        /// 未配置ValidTo参数,请确认.
        /// </summary>
        public string MSG_Public_047;
        /// <summary>
        /// 未分配当前料号,请确认.
        /// </summary>
        public string MSG_Public_048;
        /// <summary>
        /// 夹具已停用,请更换夹具.
        /// </summary>
        public string MSG_Public_049;
        /// <summary>
        /// 夹具已与其他条码绑定,请更换夹具.
        /// </summary>
        public string MSG_Public_050;
        /// <summary>
        /// 治具参数ValidDistribution未配置当前工序过站次数.
        /// </summary>
        public string MSG_Public_051;
        /// <summary>
        /// 治具扫描次数已超过限制次数
        /// </summary>
        public string MSG_Public_052;
        /// <summary>
        /// 使用次数已超过最大限制,请重置.
        /// </summary>
        public string MSG_Public_053;
        /// <summary>
        /// 解绑夹具校验条码一致性必须先扫描主条码.
        /// </summary>
        public string MSG_Public_054;
        /// <summary>
        /// 绑定工位与解绑工位的主条码不一致，无法解绑.
        /// </summary>
        public string MSG_Public_055;
        /// <summary>
        /// SN 和工单料号不匹配
        /// </summary>
        public string MSG_Public_056;
        /// <summary>
        /// 扫描类型不匹配
        /// </summary>
        public string MSG_Public_057;
        /// <summary>
        /// 产品已经装箱
        /// </summary>
        public string MSG_Public_058;
        /// <summary>
        /// 箱子没有关闭
        /// </summary>
        public string MSG_Public_059;

        //Howard

        /// <summary>
        /// 未分配当前料号,请确认.
        /// </summary>
        public string MSG_Public_6001;

        /// <summary>
        /// 工单数量检查错误
        /// </summary>
        public string MSG_Public_6002;
        /// <summary>
        /// 料号未配置BOM信息,无法进行组装.
        /// </summary>
        public string MSG_Public_6003;
        /// <summary>
        /// 存在重复关联数据
        /// </summary>
        public string MSG_Public_6004;

        /// <summary>
        /// 获取缓存数据失败,请重新登录后再进行扫描条码
        /// </summary>
        public string MSG_Public_6005;
        /// <summary>
        /// 不存在条码关联数据
        /// </summary>
        public string MSG_Public_6006;
        /// <summary>
        /// 主条码未扫描通过
        /// </summary>
        public string MSG_Public_6007;
        /// <summary>
        /// 通过key获取缓存为空
        /// </summary>
        public string MSG_Public_6008;
        /// <summary>
        /// 扫描的供应商代码与之前扫描的不一致，请检查
        /// </summary>
        public string MSG_Public_6009;
        /// <summary>
        /// 条码已扫过或者类型不匹配
        /// </summary>
        public string MSG_Public_6010;
        /// <summary>
        /// 子条码信息获取失败
        /// </summary>
        public string MSG_Public_6011;
        /// <summary>
        /// 主条码信息获取失败
        /// </summary>
        public string MSG_Public_6012;
        /// <summary>
        /// 通过料号ID获取料号信息失败，请检查
        /// </summary>
        public string MSG_Public_6013;
        /// <summary>
        /// 逻辑异常
        /// </summary>
        public string MSG_Public_6014;
        /// <summary>
        /// 主条码扫描成功
        /// </summary>
        public string MSG_Public_6015;
        /// <summary>
        /// 当前配置不允许扫描栈板条码
        /// </summary>
        public string MSG_Public_6016;
        /// <summary>
        /// 未获取到当前站点相关信息
        /// </summary>
        public string MSG_Public_6017;
        /// <summary>
        /// 当前箱子绑定的所有产品存在多种料号ID，请检查
        /// </summary>
        public string MSG_Public_6018;
        /// <summary>
        /// 当前箱子绑定的所有产品存在多种工单ID，请检查
        /// </summary>
        public string MSG_Public_6019;
        /// <summary>
        /// 当前条码的料号不匹配，请检查
        /// </summary>
        public string MSG_Public_6020;
        /// <summary>
        /// 栈板条码不允许反入库
        /// </summary>
        public string MSG_Public_6021;
        /// <summary>
        /// 当前条码不是系统栈板条码，请输入正确的栈板条码
        /// </summary>
        public string MSG_Public_6022;
        /// <summary>
        /// 当前栈板绑定的所有产品存在多种料号ID，请检查
        /// </summary>
        public string MSG_Public_6023;
        /// <summary>
        /// 当前栈板绑定的所有产品存在多种工单ID，请检查
        /// </summary>
        public string MSG_Public_6024;
        /// <summary>
        /// 整个栈板的所有产品存在多于两种工序状态
        /// </summary>
        public string MSG_Public_6025;
        /// <summary>
        /// 箱子中的所有产品存在多于一种工序状态
        /// </summary>
        public string MSG_Public_6026;
        /// <summary>
        /// 未查询到工单相关信息
        /// </summary>
        public string MSG_Public_6027;
        /// <summary>
        /// 多个条码存在两两相同
        /// </summary>
        public string MSG_Public_6028;
        /// <summary>
        /// 请检查动态索引
        /// </summary>
        public string MSG_Public_6029;
        /// <summary>
        /// 动态类型中，缓存读取与接收到的状态不符合
        /// </summary>
        public string MSG_Public_6030;
        /// <summary>
        /// FG信息不存在
        /// </summary>
        public string MSG_Public_6031;
        /// <summary>
        /// 当前项的前项没有输入完成无法提交
        /// </summary>
        public string MSG_Public_6032;
        /// <summary>
        /// 请先输入UPC条码
        /// </summary>
        public string MSG_Public_6033;
        /// <summary>
        /// 当检查类型设置为存储过程时，需要配置存储过程名称
        /// </summary>
        public string MSG_Public_6034;
        /// <summary>
        /// 当前站点需要配置PO属性IsScanUPCSN，请检查
        /// </summary>
        public string MSG_Public_6035;
        /// <summary>
        /// 存储过程返回空值
        /// </summary>
        public string MSG_Public_6036;
        /// <summary>
        /// 中箱条码异常，可能存在重复绑定.
        /// </summary>
        public string MSG_Public_6037;
        /// <summary>
        /// 是否产生箱码配置异常
        /// </summary>
        public string MSG_Public_6038;
        /// <summary>
        /// 提交数据失败
        /// </summary>
        public string MSG_Public_6039;
        /// <summary>
        /// 未获取到已扫描完成的UPC SN
        /// </summary>
        public string MSG_Public_6040;
        /// <summary>
        /// 查询到的箱码不存在或者状态不符
        /// </summary>
        public string MSG_Public_6041;
        /// <summary>
        /// 打印机连接地址和端口为空
        /// </summary>
        public string MSG_Public_6042;
        /// <summary>
        /// 请确认当前箱号是否已经关箱，或者当前箱号没有关联产品
        /// </summary>
        public string MSG_Public_6043;
        /// <summary>
        /// 请设置站点必要项为启用状态
        /// </summary>
        public string MSG_Public_6044;
        /// <summary>
        /// 工单未配置卡通箱最大包装数量(BoxQty)
        /// </summary>
        public string MSG_Public_6045;
        /// <summary>
        /// 当前栈板还未装满，不允许补打印
        /// </summary>
        public string MSG_Public_6046;
        /// <summary>
        /// 当前栈板已包装完成，请勿重复提交
        /// </summary>
        public string MSG_Public_6047;
        /// <summary>
        /// 输入的箱码与查询的箱码不一致，请确认
        /// </summary>
        public string MSG_Public_6048;
        /// <summary>
        /// 重量不合格
        /// </summary>
        public string MSG_Public_6049;
        /// <summary>
        /// BillNo存在多个或者一个BillNO关联了多个栈板码
        /// </summary>
        public string MSG_Public_6050;
        /// <summary>
        /// Project NO 不能为空
        /// </summary>
        public string MSG_Public_6051;
        /// <summary>
        /// Project NO 已经存在 
        /// </summary>
        public string MSG_Public_6052;
        /// <summary>
        /// FCountByCase 必须大于0
        /// </summary>
        public string MSG_Public_6053;
        /// <summary>
        /// FTotalWeight  必须大于0
        /// </summary>
        public string MSG_Public_6054;
        /// <summary>
        /// FWeightByPallet 必须大于0
        /// </summary>
        public string MSG_Public_6055;
        /// <summary>
        /// 请选择正确的单元状态
        /// </summary>
        public string MSG_Public_6056;
        /// <summary>
        /// 请检查工艺路线设定，内部自循环线，在相同输入状态下只允许设定一条
        /// </summary>
        public string MSG_Public_6057;
        /// <summary>
        /// 当前状态重测次数超限
        /// </summary>
        public string MSG_Public_6058;
        /// <summary>
        /// 当前日期下，班次和线别已存在信息
        /// </summary>
        public string MSG_Public_6059;
        /// <summary>
        /// 线别或者班次不存在
        /// </summary>
        public string MSG_Public_6060;
        public string MSG_Public_6061;
        public string MSG_Public_6062;

        /// <summary>
        /// 已入库
        /// </summary>
        public string MSG_Public_60000;
        /// <summary>
        /// 找不到对应的产品信息，请重新扫描!
        /// </summary>
        public string MSG_Public_60001;
        /// <summary>
        /// 已出库
        /// </summary>
        public string MSG_Public_60002;
        /// <summary>
        /// 未入库
        /// </summary>
        public string MSG_Public_60003;
        /// <summary>
        /// 产品和指定MPN不符
        /// </summary>
        public string MSG_Public_60004;
        /// <summary>
        /// MPN已关闭或不正确
        /// </summary>
        public string MSG_Public_60005;
        /// <summary>
        /// 未出库
        /// </summary>
        public string MSG_Public_60006;
        /// <summary>
        /// 无效卡板或未审核
        /// </summary>
        public string MSG_Public_60007;


        /// <summary>
        /// 请在工站中配置 IsChangePN 或 IsChangePO 参数。 
        /// </summary>
        public string MSG_Public_6063;
        /// <summary>
        /// MSG_Public
        /// </summary>
        /// <param name="v_Language">语言 0:ZH_CN 1:EN</param>
        public MSG_Public(int v_Language)
        {
            int I_Language = v_Language;
            MSG_Public_001 = "MSG_Public_001:" + PublicF.GetLangStr("料号未配置工艺流程路线。@" +
                                                        "The material number is not configured with the process route.", I_Language);

            MSG_Public_002 = "MSG_Public_002:" + PublicF.GetLangStr("当前工站类型在配置的工艺流程中未配置,请检查工艺流程。@" +
                                                        "The current station type is not configured in the configured process flow. Please check the process flow.", I_Language);
            MSG_Public_003 = "MSG_Public_003:" + PublicF.GetLangStr("工艺流程校验失败。@" +
                                                        "Process Route failed", I_Language);
            MSG_Public_004 = "MSG_Public_004:" + PublicF.GetLangStr("子料对应的工艺路线未完成或者已过站。@" +
                                                        "The corresponding process route of the sub-material is not completed.", I_Language);
            MSG_Public_005 = "MSG_Public_005:" + PublicF.GetLangStr("此条码已过站.。@" +
                                                        "The bar code has passed the station.", I_Language);

            MSG_Public_006 = "MSG_Public_006:" + PublicF.GetLangStr("上一站未扫描。@" +
                                                        "The last station was not scanned.", I_Language);
            MSG_Public_007 = "MSG_Public_007:" + PublicF.GetLangStr("此条码已NG。@" +
                                                        "The bar code is NG.", I_Language);
            MSG_Public_008 = "MSG_Public_008:" + PublicF.GetLangStr("无此工序。@" +
                                                        "No such process.", I_Language);
            MSG_Public_009 = "MSG_Public_009:" + PublicF.GetLangStr("工单不能为空,请确认。@" +
                                                        "The work order cannot be empty, please confirm.", I_Language);
            MSG_Public_010 = "MSG_Public_010:" + PublicF.GetLangStr("未选择料号类别,请确认.@" +
                                                        "The part number category is not selected, please confirm.", I_Language);

            MSG_Public_011 = "MSG_Public_011:" + PublicF.GetLangStr("未选择料号群,请确认.@" +
                                                        "No material group is selected, please confirm.", I_Language);
            MSG_Public_012 = "MSG_Public_012:" + PublicF.GetLangStr("未选择料号,请确认.@" +
                                                        "The part number is not selected, please confirm.", I_Language);
            MSG_Public_013 = "MSG_Public_013:" + PublicF.GetLangStr("未选择工单，请确认.@" +
                                                        "No work order selected, please confirm.", I_Language);
            MSG_Public_014 = "MSG_Public_014:" + PublicF.GetLangStr("未配置校验批次正则表达式.@" +
                                                        "check batch regular expression not configured.", I_Language);
            MSG_Public_015 = "MSG_Public_015:" + PublicF.GetLangStr("未配置校验SN正则表达式.@" +
                                                        "check SN regular expression not configured.", I_Language);

            MSG_Public_016 = "MSG_Public_016:" + PublicF.GetLangStr("条码不能为空.@" +
                                                        "SN cannot be null.", I_Language);
            MSG_Public_017 = "MSG_Public_017:" + PublicF.GetLangStr("确认此物料NG,请设置NG原因.@" +
                                                        "Confirm this material NG, please set NG reason.", I_Language);
            MSG_Public_018 = "MSG_Public_018:" + PublicF.GetLangStr("条码不存在或者状态不符.@" +
                                                        "The bar code does not exist or does not match the status.", I_Language);
            MSG_Public_019 = "MSG_Public_019:" + PublicF.GetLangStr("此条码和选择的工单不一致.@" +
                                                        "this bar code is inconsistent with the selected work order.", I_Language);
            MSG_Public_020 = "MSG_Public_020:" + PublicF.GetLangStr("此条码已NG.@" +
                                                        "The bar code is NG.", I_Language);

            MSG_Public_021 = "MSG_Public_021:" + PublicF.GetLangStr("当前工站类型在配置的工艺流程中未配置,请检查工艺流程.@" +
                                                        "The current station type is not configured in " +
                                                        "the configured process flow. Please check the process flow.", I_Language);
            MSG_Public_022 = "MSG_Public_022:" + PublicF.GetLangStr("工艺流程校验失败.@" +
                                                        "Process Route failed.", I_Language);
            MSG_Public_023 = "MSG_Public_023:" + PublicF.GetLangStr("过程名称不能为空.@" +
                                                        "The procedure name cannot be null.", I_Language);
            MSG_Public_024 = "MSG_Public_024:" + PublicF.GetLangStr("料号和线别不匹配.@" +
                                                        "Part number and the line don't match.", I_Language);
            MSG_Public_025 = "MSG_Public_025:" + PublicF.GetLangStr("条码类型错误.@" +
                                                        "Barcode type error.", I_Language);

            MSG_Public_026 = "MSG_Public_026:" + PublicF.GetLangStr("没有找到夹具绑定的条码.@" +
                                                        "No bar code was found for tool binding.", I_Language);
            MSG_Public_027 = "MSG_Public_027:" + PublicF.GetLangStr("SN不存在.@" +
                                                        "SN is not exists.", I_Language);
            MSG_Public_028 = "MSG_Public_028:" + PublicF.GetLangStr("uspTTCheck 检查失败.@" +
                                                        "uspTTCheck: Check Fail.", I_Language);
            MSG_Public_029 = "MSG_Public_029:" + PublicF.GetLangStr("正则校验未通过，请确认.@" +
                                                        "regular check failed, please confirm.", I_Language);
            MSG_Public_030 = "MSG_Public_030:" + PublicF.GetLangStr("工单状态不符,请确认!.@" +
                                                        "The status of workorder is inconsistent, please confirm!", I_Language);

            MSG_Public_031 = "MSG_Public_031:" + PublicF.GetLangStr("工单已经超过配置数量,请确认!@" +
                                                        "The workorder has exceeded the configured quantity, please confirm!", I_Language);
            MSG_Public_032 = "MSG_Public_032:" + PublicF.GetLangStr("料号未配置参数:@" +
                                                        "Material no configuration parameter:", I_Language);
            MSG_Public_033 = "MSG_Public_033:" + PublicF.GetLangStr("未配置打印模板.@" +
                                                        "Print templates are not configured.", I_Language);
            MSG_Public_034 = "MSG_Public_034:" + PublicF.GetLangStr("CodeSoft 模板配置错误.@" +
                                                        "CodeSoft template configuration error.", I_Language);
            MSG_Public_035 = "MSG_Public_035:" + PublicF.GetLangStr("BarTender 模板配置错误.@" +
                                                        "BarTender template configuration error.", I_Language);
            MSG_Public_036 = "MSG_Public_036:" + PublicF.GetLangStr("此工序没有配置打印标签.@" +
                                                        "Printing labels are not configured for this process.", I_Language);

            MSG_Public_037 = "MSG_Public_037:" + PublicF.GetLangStr("未配置打印文件路径.@" +
                                                        "The print file path is not configured.", I_Language);
            MSG_Public_038 = "MSG_Public_038:" + PublicF.GetLangStr("料号未关联生成SN的格式.@" +
                                                        "The material number is not correlated to generate the format of SN.", I_Language);
            MSG_Public_039 = "MSG_Public_039:" + PublicF.GetLangStr("主条码已经存在,不能重复扫描.@" +
                                                        "The main bar code already exists and cannot be scanned repeatedly.", I_Language);
            MSG_Public_040 = "MSG_Public_040:" + PublicF.GetLangStr("条码生成失败.@" +
                                                        "Barcode generation failed.", I_Language);
            MSG_Public_041 = "MSG_Public_041:" + PublicF.GetLangStr("不能重复扫描.@" +
                                                        "You cannot repeat the scan.", I_Language);

            MSG_Public_042 = "MSG_Public_042:" + PublicF.GetLangStr("TT箱子已经扫描结束.@" +
                                                        " TT box has been scanned.", I_Language);
            MSG_Public_043 = "MSG_Public_043:" + PublicF.GetLangStr("这个箱子没有绑定的数据.@" +
                                                        " There is no data bound to this box.", I_Language);
            MSG_Public_044 = "MSG_Public_044:" + PublicF.GetLangStr("条码格式没找到.@" +
                                                        " Bar code format not found.", I_Language);
            MSG_Public_045 = "MSG_Public_045:" + PublicF.GetLangStr("夹具状态不对.@" +
                                                        " Tool status is incorrect.", I_Language);


            MSG_Public_046 = "MSG_Public_046:" + PublicF.GetLangStr("未配置ValidFrom参数,请确认.@" +
                                                        " The ValidFrom parameter is not configured, please confirm.", I_Language);
            MSG_Public_047 = "MSG_Public_047:" + PublicF.GetLangStr("未配置ValidTo参数,请确认.@" +
                                                        " The ValidTo parameter is not configured, please confirm.", I_Language);
            MSG_Public_048 = "MSG_Public_048:" + PublicF.GetLangStr("未分配当前料号,请确认.@" +
                                                        " The current material number is not assigned, please confirm.", I_Language);
            MSG_Public_049 = "MSG_Public_049:" + PublicF.GetLangStr("夹具已停用,请更换夹具.@" +
                                                        " The Tooling is out of service. Please replace the Tooling.", I_Language);
            MSG_Public_050 = "MSG_Public_050:" + PublicF.GetLangStr("夹具已与其他条码绑定,请更换夹具.@" +
                                                        " The Tooling has been bound with other bar code, please replace the Tooling.", I_Language);

            MSG_Public_051 = "MSG_Public_051:" + PublicF.GetLangStr("治具参数ValidDistribution未配置当前工序过站次数.@" +
                                                        " Tooling Parameter ValidDistribution has not configured the number of times the current process has missed a stop.", I_Language);
            MSG_Public_052 = "MSG_Public_052:" + PublicF.GetLangStr("治具扫描次数已超过限制次数.@" +
                                                        " The number of Tooling scans has exceeded the limit.", I_Language);
            MSG_Public_053 = "MSG_Public_053:" + PublicF.GetLangStr("使用次数已超过最大限制,请重置.@" +
                                                        " The maximum usage limit has been exceeded. Please reset.", I_Language);
            MSG_Public_054 = "MSG_Public_054:" + PublicF.GetLangStr("解绑夹具校验条码一致性必须先扫描主条码.@" +
                                                        " Untying Tooling to verify the consistency of bar code must first scan the main bar code.", I_Language);
            MSG_Public_055 = "MSG_Public_055:" + PublicF.GetLangStr("绑定工位与解绑工位的主条码不一致，无法解绑.@" +
                                                        " The binding station is inconsistent with the main bar code of the unbinding station, so it cannot be unbound.", I_Language);

            MSG_Public_056 = "MSG_Public_056:" + PublicF.GetLangStr("SN 和工单料号不匹配.@" +
                                                        "SN does not match the material number in the work order", I_Language);
            MSG_Public_057 = "MSG_Public_057:" + PublicF.GetLangStr("扫描类型不匹配.@" +
                                                        "scan type does not match.", I_Language);
            MSG_Public_058 = "MSG_Public_058:" + PublicF.GetLangStr("产品已经装箱.@" +
                                                        "products have been packed into boxes.", I_Language);
            MSG_Public_059 = "MSG_Public_059:" + PublicF.GetLangStr("箱子没有关闭.@" +
                                                        "box is not closed.", I_Language);


            MSG_Public_60000 = "MSG_Public_60000:" + PublicF.GetLangStr("已入库@" +
                                                        "Already entering warehouse", I_Language);
            MSG_Public_60001 = "MSG_Public_60001:" + PublicF.GetLangStr("找不到对应的产品信息，请重新扫描!@" +
                                                        "Can not find the corresponding product information, please scan again!", I_Language);
            MSG_Public_60002 = "MSG_Public_60002:" + PublicF.GetLangStr("已出库.@" +
                                                        "Already EX-warehouse", I_Language);
            MSG_Public_60003 = "MSG_Public_60003:" + PublicF.GetLangStr("未入库.@" +
                                                        "No entering warehouse", I_Language);
            MSG_Public_60004 = "MSG_Public_60004:" + PublicF.GetLangStr("S产品和指定MPN不符.@" +
                                                        "Product does not match the specified MPN", I_Language);
            MSG_Public_60005 = "MSG_Public_60005:" + PublicF.GetLangStr("MPN已关闭或不正确.@" +
                                                        "MPN  Closed or incorrect", I_Language);
            MSG_Public_60006 = "MSG_Public_60006:" + PublicF.GetLangStr("未出库.@" +
                                                        "No EX-warehouse", I_Language);
            MSG_Public_60007 = "MSG_Public_60007:" + PublicF.GetLangStr("无效卡板或未审核.@" +
                                                        "Invalid pallet or no audit", I_Language);


            #region howard add
            ///////////////////////////////////////////////////////////////////////

            MSG_Public_6001 = "MSG_Public_6001:" + PublicF.GetLangStr("未分配当前料号,请确认.@" +
                                                                    "The current material number is not assigned, please confirm.", I_Language);
            MSG_Public_6002 = "MSG_Public_6002:" + PublicF.GetLangStr("工单数量检查错误.@" +
                                                                    "The work order quantity check error.", I_Language);
            MSG_Public_6003 = "MSG_Public_6003:" + PublicF.GetLangStr("料号未配置BOM信息,无法进行组装..@" +
                                                                    "Part number is not configured BOM information, assembly cannot be carried out..", I_Language);
            MSG_Public_6004 = "MSG_Public_6004:" + PublicF.GetLangStr("存在重复关联数据..@" +
                                                                    "exist duplicate associated data...", I_Language);
            MSG_Public_6005 = "MSG_Public_6005:" + PublicF.GetLangStr("获取缓存数据失败,请重新登录后再进行扫描条码@" +
                                                                    "Failed to get cached data,please re-login and scan barcode", I_Language);
            MSG_Public_6006 = "MSG_Public_6006:" + PublicF.GetLangStr("不存在条码关联数据@" +
                                                                    "not exist associated data", I_Language);
            MSG_Public_6007 = "MSG_Public_6007:" + PublicF.GetLangStr("主条码未扫描通过@" +
                                                                    "The main-barcode can't passed by the scanner", I_Language);
            MSG_Public_6008 = "MSG_Public_6008:" + PublicF.GetLangStr("通过key获取缓存为空@" +
                                                                    "the cached data is null by key", I_Language);
            MSG_Public_6009 = "MSG_Public_6009:" + PublicF.GetLangStr("扫描的供应商代码与之前扫描的不一致，请检查@" +
                                                                      "the vendor code no match. please check.", I_Language);
            MSG_Public_6010 = "MSG_Public_6010:" + PublicF.GetLangStr("条码已扫过或者类型不匹配，请检查@" +
                                                                      "the barcode had scanned, or the type does not match. please check.", I_Language);
            MSG_Public_6011 = "MSG_Public_6011:" + PublicF.GetLangStr("子条码信息获取失败，请检查@" +
                                                                      "the child barcode get failed. please check.", I_Language);
            MSG_Public_6012 = "MSG_Public_6012:" + PublicF.GetLangStr("主条码信息获取失败，请检查@" +
                                                                      "the main barcode get failed. please check.", I_Language);
            MSG_Public_6013 = "MSG_Public_6013:" + PublicF.GetLangStr("通过料号ID获取料号信息失败，请检查@" +
                                                                      "Failed to get the part info by part id. please check.", I_Language);
            MSG_Public_6014 = "MSG_Public_6014:" + PublicF.GetLangStr("the program error.....@" +
                                                          "the program error......", I_Language);
            MSG_Public_6015 = "MSG_Public_6015:" + PublicF.GetLangStr("主条码扫描成功@" +
                                              "Main SN scaned success...", I_Language);

            MSG_Public_6016 = "MSG_Public_6016:" + PublicF.GetLangStr("当前配置项'IsScanPalletSNOrCartonBoxSN'不允许扫描栈板条码@" +
                                                                      "Don't allow to scan pallet barcode.please review setting 'IsScanPalletSNOrCartonBoxSN'.", I_Language);
            MSG_Public_6017 = "MSG_Public_6017:" + PublicF.GetLangStr("未获取到当前站点相关信息@" +
                                                                      "Can't get information of the current station", I_Language);
            MSG_Public_6018 = "MSG_Public_6018:" + PublicF.GetLangStr("当前箱子绑定的所有产品存在多种料号ID，请检查@" +
                                                                      "All products of the current carton have multiple part ID, please check", I_Language);
            MSG_Public_6019 = "MSG_Public_6019:" + PublicF.GetLangStr("当前箱子绑定的所有产品存在多种工单ID，请检查@" +
                                                                      "All products of the current carton have multiple PO ID, please check", I_Language);
            MSG_Public_6020 = "MSG_Public_6020:" + PublicF.GetLangStr("当前条码的料号不匹配，请检查@" +
                                                                      "the part number of this barcode no match, please check", I_Language);
            MSG_Public_6021 = "MSG_Public_6021:" + PublicF.GetLangStr("栈板条码不允许反入库@" +
                                                                      "Don't allow cancel in WH by pallet sn.", I_Language);
            MSG_Public_6022 = "MSG_Public_6022:" + PublicF.GetLangStr("当前条码不是系统栈板条码，请输入正确的栈板条码@" +
                                                                      "this SN isn't pallet SN, please input correct SN.", I_Language);
            MSG_Public_6023 = "MSG_Public_6023:" + PublicF.GetLangStr("当前栈板绑定的所有产品存在多种料号ID，请检查@" +
                                                                      "All products of the current pallet have multiple part ID, please check", I_Language);
            MSG_Public_6024 = "MSG_Public_6024:" + PublicF.GetLangStr("当前栈板绑定的所有产品存在多种工单ID，请检查@" +
                                                                      "All products of the current pallet have multiple PO ID, please check", I_Language);

            MSG_Public_6025 = "MSG_Public_6025:" + PublicF.GetLangStr("整个栈板的所有产品存在多于两种工序状态@" +
                                                                      "All products of the pallet have more than two process states", I_Language);
            MSG_Public_6026 = "MSG_Public_6026:" + PublicF.GetLangStr("箱子中的所有产品存在多于一种工序状态@" +
                                                                      "All products of the carton have more than one process states", I_Language);
            MSG_Public_6027 = "MSG_Public_6027:" + PublicF.GetLangStr("未查询到工单相关信息@" +
                                                                      "can't find the production order information", I_Language);


            
            MSG_Public_6028 = "MSG_Public_6028:" + PublicF.GetLangStr("多个条码存在两两相同@" +
                                                                      "Two barcodes cannot be the same", I_Language); 
            MSG_Public_6029 = "MSG_Public_6029:" + PublicF.GetLangStr("请检查动态索引@" +
                                                                          "please verify the dynamic index.", I_Language);
            MSG_Public_6030 = "MSG_Public_6030:" + PublicF.GetLangStr("动态类型中，缓存读取与接收到的状态不符合@" +
                                                                      "In dynamic type, cache read does not match the received state.", I_Language);
            MSG_Public_6031 = "MSG_Public_6031:" + PublicF.GetLangStr("FG信息不存在@" +
                                                                      "FG Info not exists.", I_Language);
            
            MSG_Public_6032 = "MSG_Public_6032:" + PublicF.GetLangStr("当前项的前项没有输入完成，无法提交@" +
                                                                          "The previous item of the current item has not been entered and cannot be submitted.", I_Language);

            MSG_Public_6033 = "MSG_Public_6033:" + PublicF.GetLangStr("请先输入UPC条码@" +
                                                                      "Please enter UPC SN first", I_Language);

            MSG_Public_6034 = "MSG_Public_6034:" + PublicF.GetLangStr("当检查类型设置为存储过程时，需要配置存储过程名称@" +
                                                                      "When the inspection type is set to stored procedure, the stored procedure name needs to be configured", I_Language);
            MSG_Public_6035 = "MSG_Public_6035:" + PublicF.GetLangStr("当前站点需要配置PO属性IsScanUPCSN，请检查@" +
                                                                      "The current station needs to configure the PO attribute 'IsScanUPCSN', please check", I_Language);

            MSG_Public_6063 = "MSG_Public_6063:" + PublicF.GetLangStr("请在工站中配置 IsChangePN 或 IsChangePO 参数。@" +
                                                                      "please setup 'IsChangePN' or 'IsChangePO' in the station config", I_Language);

            MSG_Public_6036 = "MSG_Public_6036:" + PublicF.GetLangStr("条码检查返回异常空值.@" +
                                                                      "Barcode check returned abnormal null value.", I_Language);

            MSG_Public_6037 = "MSG_Public_6037:" + PublicF.GetLangStr("中箱条码异常，可能存在重复绑定.@" +
                                                                      "The barcode of the carton box is abnormal, and there may be duplicate binding.", I_Language);
            
            MSG_Public_6038 = "MSG_Public_6038:" + PublicF.GetLangStr("是否产生箱码配置异常@" +
                                                                          "Whether abnormal box code configuration occurs.", I_Language);
            MSG_Public_6039 = "MSG_Public_6039:" + PublicF.GetLangStr("提交数据失败@" +
                                                                      "Failed to submit data.", I_Language);

            MSG_Public_6040 = "MSG_Public_6040:" + PublicF.GetLangStr("未获取到已扫描完成的UPC条码@" +
                                                                      "Failed to obtain scanned UPC barcode.", I_Language);
            MSG_Public_6041 = "MSG_Public_6041:" + PublicF.GetLangStr("查询到的箱码不存在或者状态不符@" +
                                                                      "The box barcode found does not exist or does not match its status.", I_Language);
            MSG_Public_6042 = "MSG_Public_6042:" + PublicF.GetLangStr("打印机连接地址和端口为空@" +
                                                                      "The printer connection address and port are empty.", I_Language);

            MSG_Public_6043 = "MSG_Public_6043:" + PublicF.GetLangStr("请确认当前箱号是否已经关箱，或者当前箱号没有关联产品@" +
                                                          "Please confirm whether the current container number has been closed, or there is no product associated with the current container number.", I_Language);
            MSG_Public_6044 = "MSG_Public_6044:" + PublicF.GetLangStr("请设置站点必要项为启用状态@" +
                                              "Please to set enabled status of the necessary items.", I_Language);
            MSG_Public_6045 = "MSG_Public_6045:" + PublicF.GetLangStr("工单未配置卡通箱最大包装数量(BoxQty)@" +
                                  "BoxQty parameter is not configured on work order. Please check.", I_Language);
            MSG_Public_6046 = "MSG_Public_6046:" + PublicF.GetLangStr("当前栈板还未装满，不允许补打印@" +
                      "The current pallet is not yet full and cannot be reprinted.", I_Language);
            MSG_Public_6047 = "MSG_Public_6047:" + PublicF.GetLangStr("当前栈板已包装完成，请勿重复提交@" + 
                "The current pallet has been packaged, please do not resubmit.", I_Language);
            MSG_Public_6048 = "MSG_Public_6048:" + PublicF.GetLangStr("输入的箱码与查询的箱码不一致，请确认@" +
    "The entered container code does not match the queried container code. Please confirm.", I_Language);
            MSG_Public_6049 = "MSG_Public_6049:" + PublicF.GetLangStr("重量不合格@" +
"Weight unqualified.", I_Language);
            MSG_Public_6050 = "MSG_Public_6050:" + PublicF.GetLangStr("BillNo存在多个或者一个BillNO关联了多个栈板码@" +
"BillNo has multiple or one BillNO is associated with multiple pallet codes.", I_Language);

            MSG_Public_6051 = "MSG_Public_6051:" + PublicF.GetLangStr("Project NO 不能为空，新增失败@" +
"Project NO is null, insert failed.", I_Language);
            MSG_Public_6052 = "MSG_Public_6052:" + PublicF.GetLangStr("Project NO 已存在，新增失败@" +
"Project NO had existed, insert failed.", I_Language);
            MSG_Public_6053 = "MSG_Public_6053:" + PublicF.GetLangStr("FCountByCase 必须大于0，新增失败@" +
"FCountByCase must be more than 0, insert filed.", I_Language);
            MSG_Public_6054 = "MSG_Public_6054:" + PublicF.GetLangStr("FTotalWeight 必须大于0，新增失败@" +
"FTotalWeight must be more than 0, insert filed.", I_Language);
            MSG_Public_6055 = "MSG_Public_6055:" + PublicF.GetLangStr("FWeightByPallet 必须大于0，新增失败@" +
"FWeightByPallet must be more than 0, insert filed.", I_Language);
            MSG_Public_6056 = "MSG_Public_6056:" + PublicF.GetLangStr("请选择正确的单元状态@" +
"Please select the correct unit state.", I_Language);
            MSG_Public_6057 = "MSG_Public_6057:" + PublicF.GetLangStr("请检查工艺路线设定，内部自循环线，在相同输入状态下只允许设定一条@" +
"please check route setting, one correct state allow set up one line.", I_Language);
            MSG_Public_6058 = "MSG_Public_6058:" + PublicF.GetLangStr("当前状态重测次数超限@" +
"Retest times exceeding the limit.", I_Language);
            MSG_Public_6059 = "MSG_Public_6059:" + PublicF.GetLangStr("当前日期下，班次和线别已存在信息@" +
"data already exists.", I_Language);
            MSG_Public_6060 = "MSG_Public_6060:" + PublicF.GetLangStr("线别或者班次不存在@" +
"Line or shift no exists.", I_Language);

            #endregion
        }


    }
}
