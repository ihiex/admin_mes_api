﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface ISC_mesLabelFieldDefinitionService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesLabelFieldDefinition v_SC_mesLabelFieldDefinition, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesLabelFieldDefinition v_SC_mesLabelFieldDefinition, IDbTransaction trans = null);

        Task<string> Clone(SC_mesLabelFieldDefinition v_SC_mesLabelFieldDefinition, IDbTransaction trans = null);

        Task<PageResult<SC_mesLabelFieldDefinition>> FindWithPagerMyAsync(SC_mesLabelFieldDefinitionSearch search);
    }
}


