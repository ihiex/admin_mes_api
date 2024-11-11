using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{

    public interface ISC_mesSNFormatMapRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null);

        Task<string> Clone(SC_mesSNFormatMap v_SC_mesSNFormatMap, IDbTransaction trans = null);

        Task<List<SC_mesSNFormatMapDto>> FindWithPagerMyAsync(SC_mesSNFormatMapSearch search, PagerInfo info);

    }

}
