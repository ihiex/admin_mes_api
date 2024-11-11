using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.IRepositories.Machine;
using SunnyMES.Security.SysConfig.IServices.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Machine
{
    public class SC_mesMachineServices : BaseCustomService<SC_mesMachine, SC_mesMachine, string>, ISC_mesMachineServices
    {
        private readonly IPublicSCRepository publicSCRepository;

        public SC_mesMachineServices(ISC_mesMachineRepositories repositories, IPublicSCRepository publicSCRepository) : base(repositories)
        {
            this.publicSCRepository = publicSCRepository;
        }


        public async Task<PageResult<SC_mesMachine>> FindWithPagerSearchAsync(SearchMachineInputDto search)
        {
            bool order = search.Order.ToUpper().Trim() == "DESC";

            string selectStr = $@"SELECT a.*, b.Description StationTypeName , c.PartNumber PartNumber,d.Name MachineFamilyName, s.Description StatusDesc
                                , ISNULL(m.SN, '') ParentName
                                FROM dbo.mesMachine a
                                JOIN dbo.mesStationType b ON b.ID = a.StationTypeID
                                JOIN dbo.mesPart c ON c.ID = a.PartID
                                JOIN dbo.luMachineFamily d ON d.ID = a.MachineFamilyID
                                JOIN dbo.luMachineStatus s ON s.ID = a.StatusID
                                LEFT JOIN dbo.mesMachine m ON m.ID = a.ParentID
                                WHERE 1 = 1
                                 AND ('{search.Keywords}' = '' 
                                     OR a.SN LIKE '%{search.Keywords}%'
                                     OR	b.Description LIKE '%{search.Keywords}%'
                                     OR	c.PartNumber LIKE '%{search.Keywords}%'
                                     OR	d.Name LIKE '%{search.Keywords}%'
                                        )
                                 AND (                                        
                                      ('{search.SN}' = '' OR	a.SN  = '{search.SN}')
                                     AND ('{search.StationTypeName}' = '' OR	b.Description = '{search.StationTypeName}')
                                    AND ('{search.PartNumber}' = '' OR	c.PartNumber = '{search.PartNumber}')
                                    AND ('{search.MachineFamilyName}' = '' OR d.Name  = '{search.MachineFamilyName}')
                                    {(search.WarningStateIds.Any() ? $" AND a.WarningStatus in ({string.Join(',', search.WarningStateIds)}) " : "")}
                                    {(search.StateIds.Any() ? $" AND a.StatusID in ({string.Join(',', search.StateIds)}) " : "")}
                                 )

            ";

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await repository.FindWithPagerCustomSqlAsync(selectStr, pagerInfo, search.Sort, order);


            var stationTypes = SqlSugarHelper.Db.Ado.SqlQuery<SC_mesStationType>("SELECT * FROM dbo.mesStationType");

            list.ForEach(x => {
                string[] ValidFroms = x.ValidFrom?.Split(';');
                string[] ValidTos = x.ValidTo?.Split(';');
                string[] ValidDistributions = x.ValidDistribution?.Split(";");

                 
                for (int i = 0; ValidFroms is not null && i < ValidFroms.Length; i++)
                {
                    if (string.IsNullOrEmpty(ValidFroms[i]))
                        continue;

                    if (!int.TryParse( ValidFroms[i],out int r))
                        throw new Exception($"{x.SN} ValidFroms 转换数据失败，请检查数据库中值是否正确...");

                    x.ValidFroms.Add(new SC_IdDesc { Description = stationTypes.First(x => x.ID == ValidFroms[i].ToString()).Description, ID = ValidFroms[i].ToInt() });   
                }

                for (int i = 0; ValidTos is not null && i < ValidTos.Length; i++)
                {
                    if (string.IsNullOrEmpty(ValidTos[i]))
                        continue;

                    if (!int.TryParse(ValidTos[i], out int r))
                        throw new Exception($"{x.SN} ValidTos 转换数据失败，请检查数据库中值是否正确...");

                    x.ValidTos.Add(new SC_IdDesc { Description = stationTypes.First(x => x.ID == ValidTos[i].ToString()).Description, ID = ValidTos[i].ToInt() });
                }

                for(int i = 0; ValidDistributions is not null && i < ValidDistributions.Length ; i++)
                {
                    if (string.IsNullOrEmpty(ValidDistributions[i]))
                        continue;
                    string[] strings = ValidDistributions[i].Split(",");
                    if (strings.Length != 2)
                        continue;

                    if (!int.TryParse(strings[0], out int r) || !int.TryParse(strings[1], out int t))
                        throw new Exception($"{x.SN} ValidDistributions 转换数据失败，请检查数据库中值是否正确...");

                    x.ValidDistributions.Add(new SC_IdDescCount { Description = stationTypes.First(x => x.ID == strings[0].ToString()).Description, ID = strings[0].ToInt(), Count = strings[1].ToInt() });
                }
            });


            PageResult<SC_mesMachine> pageResult = new PageResult<SC_mesMachine>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,

            };
            return pageResult;
        }
    }
}
