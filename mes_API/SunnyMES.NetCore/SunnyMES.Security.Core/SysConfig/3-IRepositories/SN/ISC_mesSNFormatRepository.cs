﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;


namespace SunnyMES.Security.IRepositories
{
    public interface ISC_mesSNFormatRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesSNFormat v_SC_mesSNFormat,  IDbTransaction trans = null);

        Task<string> Delete(string Id,  IDbTransaction trans = null);

        Task<string> Update(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null);

        Task<string> Clone(SC_mesSNFormat v_SC_mesSNFormat,  IDbTransaction trans = null);

        Task<List<SC_mesSNFormatALLDto>> FindWithPagerMyAsync(SC_mesSNFormatSearch search,  PagerInfo info);



        Task<string> InsertDetail(string ParentId, SC_mesSNSectionEdit v_DetailDto,IDbTransaction trans = null);
        Task<string> DeleteDetail(string Id, IDbTransaction trans = null);
        Task<string> UpdateDetail(SC_mesSNSectionEdit v_DetailDto, IDbTransaction trans = null);
        Task<SC_mesSNFormatDetailList> List_Detail(string ParentId);


        Task<string> InsertSNRange(string ParentId, SC_mesSNRange v_DetailDto, IDbTransaction trans = null);
        Task<string> DeleteSNRange(string Id, IDbTransaction trans = null);
        Task<string> UpdateSNRange(SC_mesSNRange v_DetailDto, IDbTransaction trans = null);
        Task<SC_mesSNRangeList> List_SNRange(string ParentId);


    }
}

