using SunnyMES.Commons.IServices;
using SunnyMES.Security.SysConfig.Dtos.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.Shift
{
    public interface ISC_mesShiftUPHServices : ICustomService<SC_mesShiftUPH, SC_mesShiftUPH,int>
    {
        public Task<string> InsertBulkAsync(MesShiftUPHInputDto tinfo);
        public Task<string> CheckLineShift(List<int> lineIds, List<int> shiftIds);

        public Task<string> DeleteBulkAsync(MesShiftUPHBulkDeleteDto tinfo);
    }
}
