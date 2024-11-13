using Quartz.Impl.Triggers;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.Repositories;
using static NPOI.HSSF.Util.HSSFColor;

namespace SunnyMES.Security.Services
{
    public class SC_mesLineService : BaseServiceReport<string>, ISC_mesLineService
    {
        private readonly ISC_mesLineRepository _repository;
        private readonly ILogService _logService;

        public SC_mesLineService(ISC_mesLineRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP) 
        {
            return await _repository.GetConfInfo( I_Language,  I_LineID,  I_StationID,  I_EmployeeID,  S_CurrentLoginIP);
        }

        public async Task<string> Insert(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            return await _repository.Insert(v_SC_mesLineDto, trans);
        }

        public async Task<string> Delete(string Id, IDbTransaction trans = null)
        {
            return await _repository.Delete(Id, trans);
        }

        public async Task<string> Update(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            return await _repository.Update(v_SC_mesLineDto, trans);
        }

        public async Task<string> Clone(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null)
        {
            return await _repository.Clone(v_SC_mesLineDto, trans);
        }


        //public async Task<List<SC_mesLineDto>> FindWithPagerMyAsync(string condition, PagerInfo info, string fieldToSort, bool desc)
        //{
        //    return await _repository.FindWithPagerMyAsync(condition, info, fieldToSort, desc);
        //}


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public async Task<PageResult<SC_mesLineALLDto>> FindWithPagerSearchAsync(SC_mesLineSearch search)
        {
            //search.ID = search.ID ?? "";
            //search.Description = search.Description ?? "";                        
            //search.StatusValue = search.StatusValue ?? "";
            //search.LineTypeDefID = search.LineTypeDefID ?? "";
            //search.LineTypeDefValue = search.LineTypeDefValue ?? "";
            //search.Content = search.Content ?? "";

            //search.LikeQuery = search.LikeQuery ?? "";

            //string S_QueryPage =
            //@"
            //    SELECT A.ID, A.[Description], A.Location, A.ServerID, A.StatusID,
            //       B.[Description] StatusValue FROM mesLine A
            //         JOIN sysStatus B ON A.StatusID=B.ID
            //    where 1=1 
            //";

            //string S_QueryCount =
            //@"
            //    SELECT Count(*) Valint1 FROM mesLine A
            //         JOIN sysStatus B ON A.StatusID=B.ID
            //    where 1=1 
            //";

            //string S_SqlPage = "";
            //string S_SqlCount = "";

            //if (string.IsNullOrEmpty(search.LikeQuery)==false)
            //{
            //    S_SqlPage =
            //    @" 
            //    SELECT  * INTO #Tab1 FROM 
            //    (
            //  SELECT D1.ID,D1.LineID, D1.LineTypeDefID, D1.[Content],D2.[Description]  FROM mesLineDetail D1 
            //   JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID                
            //  WHERE 1=1 AND
            //        (
            //              D1.[Content] LIKE '%" + search.LikeQuery + @"%' 
            //           OR D2.[Description] LIKE '%" + search.LikeQuery + @"%' 
            //        )
            //    )A
            //    "+ S_QueryPage+
            //    @" AND(
            //           A.Description like '%" + search.LikeQuery + @"%'
            //        OR B.[Description] like '%" + search.LikeQuery + @"%'
            //        OR (A.id IN
            //                ( SELECT LineID  from #Tab1 )
            //            )      
            //      )
            //    "
            //    ;

            //    S_SqlCount =
            //    @" 
            //    SELECT  * INTO #Tab1 FROM 
            //    (
            //  SELECT D1.ID,D1.LineID, D1.LineTypeDefID, D1.[Content],D2.[Description]  FROM mesLineDetail D1 
            //   JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID                
            //  WHERE 1=1 AND
            //        (
            //              D1.[Content] LIKE '%" + search.LikeQuery + @"%' 
            //           OR D2.[Description] LIKE '%" + search.LikeQuery + @"%' 
            //        )
            //    )A
            //    " + S_QueryCount +
            //    @" AND(
            //           A.Description like '%" + search.LikeQuery + @"%'
            //        OR B.[Description] like '%" + search.LikeQuery + @"%'
            //        OR (A.id IN
            //                ( SELECT LineID  from #Tab1 )
            //            )      
            //      )
            //    "
            //    ;

            //}
            //else 
            //{
            //    S_SqlPage =
            //    @"
            //    SELECT  * INTO #Tab1 FROM 
            //    (
            //  SELECT D1.ID,D1.LineID, D1.LineTypeDefID, D1.[Content],D2.[Description]  FROM mesLineDetail D1 
            //   JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID                
            //  WHERE 1=1
            //           AND(
            //                    (D1.[Content]='' OR D1.[Content] Like  '%" + search.Content + @"%' ) 
            //                 AND (D1.LineTypeDefID>0 OR D1.LineTypeDefID in ('" + search.LineTypeDefID + @"'))
            //                 AND (D2.[Description]='' OR D2.[Description] LIKE '%" + search.LineTypeDefValue + @"%' ) 
            //             )
            //    )A
            //    "+ S_QueryPage
            //    ;

            //    S_SqlCount =
            //    @"
            //    SELECT  * INTO #Tab1 FROM 
            //    (
            //  SELECT D1.ID,D1.LineID, D1.LineTypeDefID, D1.[Content],D2.[Description]  FROM mesLineDetail D1 
            //   JOIN luLineTypeDef D2 on D1.LineTypeDefID=D2.ID                
            //  WHERE 1=1
            //           AND(
            //                    (D1.[Content]='' OR D1.[Content] Like  '%" + search.Content + @"%' ) 
            //                 AND (D1.LineTypeDefID>0 OR D1.LineTypeDefID in ('" + search.LineTypeDefID + @"'))
            //                 AND (D2.[Description]='' OR D2.[Description] LIKE '%" + search.LineTypeDefValue + @"%' ) 
            //             )
            //    )A
            //    " + S_QueryCount
            //    ;


            //    if (string.IsNullOrEmpty(search.ID) == false)
            //    {
            //        S_SqlPage += "  and A.ID in (" + search.ID + ") " + "\r\n";

            //        S_SqlCount += "  and A.ID in (" + search.ID + ") " + "\r\n";
            //    }

            //    //if (search.StatusID != null)
            //    //{
            //    //    S_SqlPage += "  and A.StatusID = '" + search.StatusID + "'" + "\r\n";

            //    //    S_SqlCount += "  and A.StatusID = '" + search.StatusID + "'" + "\r\n";
            //    //}

            //    S_SqlPage +=
            //    @"
            //       AND (A.StatusID='0' OR A.StatusID Like '%" + search.StatusID + @"%')
            //       AND (A.Description='' OR A.Description Like '%" + search.Description + @"%')
            //       AND (B.Description='' OR B.Description Like '%" + search.StatusValue + @"%')
            //       OR (A.id 
            //                IN( 
            //                    SELECT LineID  from #Tab1 
            //                 )
            //            )
            //    ";


            //    S_SqlCount +=
            //    @"
            //       AND (A.StatusID='0' OR A.StatusID Like '%" + search.StatusID + @"%')
            //       AND (A.Description='' OR A.Description Like '%" + search.Description + @"%')
            //       AND (B.Description='' OR B.Description Like '%" + search.StatusValue + @"%')
            //       OR (A.id 
            //                IN( 
            //                    SELECT LineID  from #Tab1 
            //                 )
            //            )
            //    ";
            //}


            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            //List<SC_mesLineALLDto> list = await _repository.FindWithPagerMyAsync(S_SqlPage, S_SqlCount, pagerInfo);
            List<SC_mesLineALLDto> list = await _repository.FindWithPagerMyAsync(search, pagerInfo);

            decimal v_PageTotal = 0;
            try
            {
                int I_Mod = pagerInfo.RecordCount % pagerInfo.PageSize;
                decimal I_De = pagerInfo.RecordCount / pagerInfo.PageSize;
                v_PageTotal = I_Mod == 0 ? I_De : I_De + 1;
            }
            catch (Exception ex) 
            { }

            PageResult<SC_mesLineALLDto> pageResult = new PageResult<SC_mesLineALLDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,
                TotalPages=Convert.ToInt32(v_PageTotal)
            };
            return pageResult;
        }


        public async Task<string> InsertDetail(string ParentId, SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null)
        {
            return await _repository.InsertDetail(ParentId, v_DetailDto, trans);
        }

        public async Task<string> DeleteDetail(string Id, IDbTransaction trans = null)
        {
            return await _repository.DeleteDetail(Id, trans);
        }

        public async Task<string> UpdateDetail(SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null)
        {
            return await _repository.UpdateDetail(v_DetailDto, trans);
        }
        public async Task<SC_mesLineDetailList> List_Detail(string ParentId)
        {
            return await _repository.List_Detail(ParentId);
        }


    }
}
