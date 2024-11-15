using System;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IServices;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;
using System.Threading.Tasks;
using SunnyMES.Commons.Models;
using System.Collections.Generic;
using System.Linq;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Log;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 单据编码服务接口实现
    /// </summary>
    public class SequenceService : BaseService<Sequence, SequenceOutputDto, string>, ISequenceService
    {
        private readonly ISequenceRepository _repository;
        private readonly ISequenceRuleRepository _repositoryRule;
        private readonly ILogService _logService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repositoryRule"></param>
        /// <param name="logService"></param>
        public SequenceService(ISequenceRepository repository, ISequenceRuleRepository repositoryRule, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
            _repositoryRule = repositoryRule;
        }

        /// <summary>
        /// 获取最新业务单据编码
        /// </summary>
        /// <param name="sequenceName">业务单据编码名称</param>
        /// <returns></returns>
        public async Task<CommonResult> GetSequenceNextTask(string sequenceName)
        {

            CommonResult result = new CommonResult();
            //生成编号   
            string sequenceNewNo = "";
            #region 获取序号生成器属性
            if (string.IsNullOrWhiteSpace(sequenceName))
            {
                result.ResultMsg = "参数错误：业务编码编号";
                return result;
            }
            //获取序号生成器属性
            Sequence sequence = _repository.GetWhere("SequenceName='" + sequenceName + "'");
            if (sequence != null)
            {
                IEnumerable<SequenceRule> list = _repositoryRule.GetListWhere("SequenceName='" + sequenceName + "' order by RuleOrder asc");
                if (list.Any())
                {
                    int delimiterNum = 0;
                    foreach (SequenceRule item in list)
                    {
                        delimiterNum++;

                        switch (item.RuleType)
                        {
                            case "const"://常量方式
                                sequenceNewNo += item.RuleValue;
                                break;
                            case "shortdate"://短日期 年2位月2位日期2位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd").Substring(2);
                                break;
                            case "date"://日期，年4位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd");
                                break;
                            case "ydate"://年月，年4位月2位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd").Substring(0,6);
                                break;
                            case "timestamp"://日期时间精确到毫秒
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                break;
                            case "number"://计数，流水号
                                int num = CurrentReset(sequence, item);
                                //计数拼接
                                sequenceNewNo += NumberingSeqRule(item, num).ToString();
                                //更新当前序号
                                sequence.CurrentNo =num;
                                break;
                            case "guid"://Guid
                                sequenceNewNo += GuidUtils.NewGuidFormatN();
                                break;
                            case "random"://随机数
                                Random random = new Random();
                                string strMax = "9".ToString().PadLeft(item.RuleValue.Length - 1, '9');
                                string strRandom = random.Next(item.RuleValue.ToInt(), strMax.ToInt()).ToString(); //生成随机编号 
                                sequenceNewNo += strRandom;
                                break;
                        }
                        if (!string.IsNullOrEmpty(sequence.SequenceDelimiter)&& delimiterNum!= list.Count())
                        {
                            sequenceNewNo += sequence.SequenceDelimiter;
                        }

                    }
                    //当前编号
                    sequence.CurrentCode = sequenceNewNo;
                    sequence.CurrentReset = DateTime.Now.ToString("yyyyMMdd");
                    await _repository.UpdateAsync(sequence, sequence.Id);
                    result.ResData = sequenceNewNo;
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ResultMsg = "未查询到业务编码对应的编码规则配置, 请检查编码规则配置";
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.ResultMsg = "请定义" + sequenceName + "的单据编码！";
                return result;
            }
            #endregion
            return result;
        }

        /// <summary>
        /// 获取最新业务单据编码
        /// </summary>
        /// <param name="sequenceName">业务单据编码名称</param>
        /// <returns></returns>
        public CommonResult GetSequenceNext(string sequenceName)
        {

            CommonResult result = new CommonResult();
            //生成编号   
            string sequenceNewNo = "";
            #region 获取序号生成器属性
            if (string.IsNullOrWhiteSpace(sequenceName))
            {
                result.ResultMsg = "参数错误：业务编码编号";
                return result;
            }
            //获取序号生成器属性
            Sequence sequence = _repository.GetWhere("SequenceName='" + sequenceName + "'");
            if (sequence != null)
            {
                IEnumerable<SequenceRule> list = _repositoryRule.GetListWhere("SequenceName='" + sequenceName + "' order by RuleOrder asc");
                if (list.Any())
                {
                    int delimiterNum = 0;
                    foreach (SequenceRule item in list)
                    {
                        delimiterNum++;

                        switch (item.RuleType)
                        {
                            case "const"://常量方式
                                sequenceNewNo += item.RuleValue;
                                break;
                            case "shortdate"://短日期 年2位月2位日期2位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd").Substring(2);
                                break;
                            case "date"://日期，年4位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd");
                                break;
                            case "ydate"://年月，年4位月2位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd").Substring(0, 6);
                                break;
                            case "sydate"://年月，年2位月2位
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMdd").Substring(2, 4);
                                break;
                            case "timestamp"://日期时间精确到毫秒
                                sequenceNewNo += DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                break;
                            case "number"://计数，流水号
                                int num = CurrentReset(sequence, item);
                                //计数拼接
                                sequenceNewNo += NumberingSeqRule(item, num).ToString();
                                //更新当前序号, 
                                sequence.CurrentNo = num;
                                break;
                            case "guid"://Guid
                                sequenceNewNo += GuidUtils.NewGuidFormatN();
                                break;
                            case "random"://随机数
                                Random random = new Random();
                                string strMax="9".ToString().PadLeft(item.RuleValue.Length, '9');
                                string strRandom = random.Next(item.RuleValue.ToInt(), strMax.ToInt()).ToString(); //生成随机编号 
                                sequenceNewNo += strRandom; 
                                break;
                        }
                        if (!string.IsNullOrEmpty(sequence.SequenceDelimiter) && delimiterNum != list.Count())
                        {
                            sequenceNewNo += sequence.SequenceDelimiter;
                        }

                    }
                    //当前编号
                    sequence.CurrentCode = sequenceNewNo;
                    sequence.CurrentReset = DateTime.Now.ToString("yyyyMMdd");
                   _repository.Update(sequence, sequence.Id);
                    result.ResData = sequenceNewNo;
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ResultMsg = "未查询到业务编码对应的编码规则配置, 请检查编码规则配置";
                    return result;
                }
            }
            else
            {
                result.Success = false;
                result.ResultMsg = "请定义" + sequenceName + "的单据编码！";
                return result;
            }
            #endregion
            return result;
        }
        /// <summary>
        /// 计数 方式 重置规则
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="seqRule"></param>
        /// <returns></returns>
        private static int CurrentReset(Sequence seq, SequenceRule seqRule)
        {
            int newNo = 0, ruleNo = 0;
            try
            {
                ruleNo = seqRule.RuleValue.ToInt();
            }
            catch (Exception ex)
            {
                newNo = 1;
                Log4NetHelper.Error(ex.Message, ex);
            }

            switch (seq.SequenceReset)
            {
                case "D"://每天重置
                    if (!string.IsNullOrEmpty(seq.CurrentReset) && seq.CurrentReset != DateTime.Now.ToString("yyyyMMdd"))
                    {
                        newNo = 1;
                    }
                    break;
                case "M"://每月重置
                    if (!string.IsNullOrWhiteSpace(seq.CurrentReset)) {
                        if (!seq.CurrentReset.Contains(DateTime.Now.ToString("yyyyMM")))
                        {
                            newNo = ruleNo;
                        }
                    }
                    else
                    {
                        newNo = 1;
                    }
                    break;
                case "Y"://每年重置
                    if (!string.IsNullOrWhiteSpace(seq.CurrentReset))
                    {
                        if (!seq.CurrentReset.Contains(DateTime.Now.ToString("yyyy")))
                        {
                            newNo = ruleNo;
                        }
                    }
                    else
                    {
                        newNo = 1;
                    }
                    break;
            }
            if (newNo == 0)
            {
                if (seq.CurrentNo == 0)
                {
                    newNo = ruleNo;
                }
                else
                {
                    //当前序号+步长 
                    newNo = seq.CurrentNo + seq.Step;
                }
            }
            return newNo;

        }
        /// <summary>
        /// 计数规则
        /// </summary>
        /// <param name="seqRule"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string NumberingSeqRule(SequenceRule seqRule, int code)
        {
            string str = "";
            if (seqRule.PaddingSide == "Left")
            {
                str += code.ToString().PadLeft(seqRule.PaddingWidth,seqRule.PaddingChar.ToChar());
            }
            if (seqRule.PaddingSide == "Right")
            {
                str += code.ToString().PadRight(seqRule.PaddingWidth, seqRule.PaddingChar.ToChar());
            }
            return str;
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<SequenceOutputDto>> FindWithPagerAsync(SearchInputDto<Sequence> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege();
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and SequenceName like '%{0}%' ", search.Keywords);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<Sequence> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<SequenceOutputDto> pageResult = new PageResult<SequenceOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SequenceOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}