﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;


namespace SunnyMES.Security.IRepositories
{
    public interface ISC_mesStationTypeRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesStationType v_SC_mesStationTypeDto, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesStationType v_SC_mesStationTypeDto, IDbTransaction trans = null);

        Task<string> Clone(SC_mesStationType v_SC_mesStationTypeDto, IDbTransaction trans = null);
        Task<List<SC_mesStationTypeALLDto>> FindWithPagerMyAsync(SC_mesStationTypeSearch search, PagerInfo info);


        Task<string> InsertDetail(string ParentId, SC_mesStationTypeDetail v_DetailDto, IDbTransaction trans = null);
        Task<string> DeleteDetail(string Id, IDbTransaction trans = null);
        Task<string> UpdateDetail(SC_mesStationTypeDetail v_DetailDto, IDbTransaction trans = null);

    }
}