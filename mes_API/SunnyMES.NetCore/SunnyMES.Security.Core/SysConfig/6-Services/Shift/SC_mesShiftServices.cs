using Microsoft.AspNetCore.Components.Forms;
using NPOI.SS.Formula.Functions;
using Quartz.Impl.Triggers;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig._2_Dtos.Shift;
using SunnyMES.Security.SysConfig.IRepositories.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.Shift
{
    public class SC_mesShiftServices : BaseCustomService<SC_mesShift, SC_mesShift,int>, ISC_mesShiftServices
    {
        private readonly ISC_mesShiftRepositories shiftRepositories;
        private readonly ISC_mesShiftDetailRepositories shiftDetailRepositories;
        private readonly ISC_mesShiftUPHRepositories shiftUPHRepositories;

        public SC_mesShiftServices(ISC_mesShiftRepositories shiftRepositories, ISC_mesShiftDetailRepositories shiftDetailRepositories, ISC_mesShiftUPHRepositories shiftUPHRepositories) : base(shiftRepositories)         {
            this.shiftRepositories = shiftRepositories;
            this.shiftDetailRepositories = shiftDetailRepositories;
            this.shiftUPHRepositories = shiftUPHRepositories;
        }

        public async Task<CommonResult> CloneDataAsync(SC_mesShift inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var oldShift = await repository.GetAsync(inputDto.ID);
            if (oldShift == null)
            {
                commonResult.ResultMsg = "Clone ID no exists.";
                return commonResult;
            }

            var r = await shiftRepositories.CheckExistAsync(inputDto);
            if (!r)
            {
                commonResult.ResultMsg = " Data already exists.";
                return commonResult;
            }

            var details = await shiftDetailRepositories.GetListWhereAsync($" ShiftCodeID = {inputDto.ID}");
            var cr = await shiftRepositories.CloneDataAsync(inputDto, details);
            if (!cr)
            {
                commonResult.ResultMsg = " Clone Failed.";
            }
            return commonResult;
        }

        public async Task<CommonResult> DeleteDataAsync(SC_mesShift inputDto)
        {
            CommonResult commonResult = new CommonResult();
            var beforeT = await shiftRepositories.GetSingleOrDefaultAsync(x => x.ID == inputDto.ID);
            if (beforeT is null)
            {
                commonResult.ResultMsg = "no found delete data.";
                return commonResult;
            }

            var linkUPHs = await shiftUPHRepositories.GetListWhereAsync($"ShiftID = {inputDto.ID}");
            if (linkUPHs.Any())
            {
                commonResult.ResultMsg = $"please delete the UPH data with shift [{inputDto.ShiftDesc}].";
                return commonResult;
            }

            var details = await shiftDetailRepositories.GetListWhereAsync($" ShiftCodeID = {inputDto.ID}");
            var r = await shiftRepositories.DeleteDataAsync(beforeT, details);
            if (!r)
            {
                commonResult.ResultMsg = " Delete data failed.";
            }
            return commonResult;
        }
        public  async Task<PageResult<SC_mesShift>> FindWithPagerFilterAsync(MesShiftInputDto search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = string.Empty;
            //不需要权限管控
            where = GetDataPrivilege(false);

            if (!string.IsNullOrEmpty(search.Keywords) )
            {
                where += $" and ( ShiftType LIKE '%{search.Keywords}%' OR ShiftCode LIKE '%{search.Keywords}%' OR ShiftDesc LIKE '%{search.Keywords}%' )";
            }
            if(search.ShiftTypeNames.Any())
            {
                where += $" and ShiftType IN ('{string.Join("','",search.ShiftTypeNames.ToArray())}')";
            }
            if (search.ShiftStates.Any())
            {
                where += $" and State IN ({string.Join(",", search.ShiftStates.ToArray())})";
            }
            where += string.IsNullOrEmpty(search.ShiftCode) ? "" : $" and ShiftCode = '{search.ShiftCode}'";
            where += string.IsNullOrEmpty(search.ShiftDesc) ? "" : $" and ShiftDesc = '{search.ShiftDesc}'";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<SC_mesShift> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);

            list.ForEach(async x => x.Details = await shiftDetailRepositories.GetListWhereAsync($"ShiftCodeID = {x.ID}"));

            PageResult<SC_mesShift> pageResult = new PageResult<SC_mesShift>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_mesShift>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }



    }
}
