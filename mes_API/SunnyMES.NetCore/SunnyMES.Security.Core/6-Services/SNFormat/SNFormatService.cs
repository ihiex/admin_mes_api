using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    public class SNFormatService : BaseServiceGeneric<TabVal,TabVal,string>, ISNFormatService
    {
        private readonly ISNFormatRepository _repository;
        private readonly ILogService _logService;

        public SNFormatService(ISNFormatRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<string> GetSNRGetNext(string S_SNFormat, string S_ReuseSNByStation,
            string S_ProdOrder, string S_Part, string S_Station, string S_ExtraData)
        { 
            return await _repository.GetSNRGetNext(S_SNFormat, S_ReuseSNByStation, S_ProdOrder, S_Part, S_Station, S_ExtraData);
        }
    }
}
