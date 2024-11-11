using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IRepositories.PO
{
    public interface ISC_mesProductionOrderRepositories : ICustomRepository<SC_mesProductionOrder, string>
    {
        Task<bool> CloneDataAsync(SC_mesProductionOrder mainDto, IEnumerable<SC_mesProductionOrderDetail> childDtos, IEnumerable<SC_mesLineOrder> lineOrders);
        Task<bool> CheckExistAsync(SC_mesProductionOrder mainDto);
        Task<bool> DeleteDataAsync(SC_mesProductionOrder inputDto, IEnumerable<SC_mesProductionOrderDetail> childs, IEnumerable<SC_mesLineOrder> lineOrders);
        Task<CommonResult> InsertPMCAsync(PMCPOInsertInputDto entity);
    }
}
