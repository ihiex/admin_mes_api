using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.IRepositories.PO;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Repositories.PO
{
    public class SC_mesProductionOrderRepositories : BaseCustomRepository<SC_mesProductionOrder, string>, ISC_mesProductionOrderRepositories
    {
        public SC_mesProductionOrderRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            
        }

        public async Task<bool> CheckExistAsync(SC_mesProductionOrder mainDto)
        {
            string sql = $@"SELECT * FROM dbo.mesProductionOrder WHERE ProductionOrderNumber = '{mainDto.ProductionOrderNumber}' AND Description = '{mainDto.Description}' AND PartID = {mainDto.PartID} ";
            var r = await DapperConn.QueryFirstOrDefaultAsync<SC_mesProductionOrder>(sql, null, null, I_DBTimeout, null);
            return r is null;
        }

        public async Task<bool> CloneDataAsync(SC_mesProductionOrder mainDto, IEnumerable<SC_mesProductionOrderDetail> childDtos, IEnumerable<SC_mesLineOrder> lineOrders)
        {
            bool result = false;
            //被克隆的主ID
            int MainID = mainDto.ID;
            var beforeT = await GetSingleOrDefaultAsync(x => x.ID == MainID);
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    mainDto.ID = 0;
                    mainDto.CreationTime = DateTime.Now;
                    mainDto.LastUpdate = null;
                    var r1 = _dbContext.Add(mainDto);

                    childDtos.ForEach(d =>
                    {
                        d.ID = 0;
                        d.ProductionOrderID = mainDto.ID;
                        _dbContext.Add<SC_mesProductionOrderDetail>(d);
                    });

                    lineOrders.ForEach(d =>
                    {
                        d.ID = 0;
                        d.ProductionOrderID = mainDto.ID;
                        _dbContext.Add<SC_mesLineOrder>(d);
                    });

                    transaction.Commit();
                    result = true;
                    await base.FormatCloneMsg(beforeT, mainDto);
                }
                catch (Exception e)
                {
                    Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, "", e);
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> DeleteDataAsync(SC_mesProductionOrder inputDto, IEnumerable<SC_mesProductionOrderDetail> childs, IEnumerable<SC_mesLineOrder> lineOrders)
        {
            bool result = false;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    _dbContext.Delete<SC_mesProductionOrder, int>(inputDto.ID);
                    childs.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesProductionOrderDetail, int>(d.ID);
                    });
                    lineOrders.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesLineOrder, int>(d.ID);
                    });
                    await base.FormatDeleteMsg(inputDto, childs.ToList(), lineOrders.ToList());
                    transaction.Commit();
                    result = true;
                }
                catch (Exception e)
                {
                    Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, "", e);
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<CommonResult> InsertPMCAsync(PMCPOInsertInputDto entity)
        {
            //1  插入主表
            //2  检查线别ID为空为选择所有，根据线别ID分别插入分线
            //3  检查料号名称是否包含'(EVT)'字样，  若包含则插入指定属性
            CommonResult commonResult = new CommonResult();
            string S_Sql;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    SC_mesProductionOrder mesProductionOrder = entity.MapTo<SC_mesProductionOrder>();
                    mesProductionOrder.CreationTime = DateTime.Now;
                    DbContext.Add(mesProductionOrder);

                    S_Sql = $@"select a.Description Description,0 LineQuantity,GETDATE() CreationTime,0 StartedQuantity,0 ReadyQuantity,0 AllowOverBuild,a.ID LineID,{mesProductionOrder.ID} ProductionOrderID,0 [Priority]
                                FROM mesLine A 
                                    left join mesLineDetail B on A.ID = B.LineID
                                    left join luLineTypeDef C on B.LineTypeDefID = C.ID
                                where  C.ID=(select ID from luLineTypeDef where Description='LineType') ";

                    S_Sql += (entity.LineID != null && entity.LineID.Any()) ? $@" and  A.ID in ({string.Join(',', entity.LineID)})" : $@" and  B.Content='{entity.LineType}'";

                    var lineOrders = DapperConn.Query<SC_mesLineOrder>(S_Sql, null, null, true, I_DBTimeout, null);
                    lineOrders.ForEach(x => {
                        S_Sql = $@" select b.Content  from mesLine A 
                                            left join mesLineDetail B on A.ID = B.LineID
                                            left join luLineTypeDef C on B.LineTypeDefID = C.ID 
                                        where C.ID=(select ID from luLineTypeDef where Description='PartFamilyID') 
                                            and B.LineID= {x.LineID}";
                        var  content = DapperConn.ExecuteScalar(S_Sql, null, null, I_DBTimeout, null);
                        if (content == null  || (content != null && content.ToInt() == mesProductionOrder.PartFamilyID))
                        {
                            //线别详细中如果配置了料号组属性，则料号组ID必须匹配
                            //         如果没有配置料号组属性，可直接添加
                            x.CreationTime = DateTime.Now;
                            _dbContext.Add<SC_mesLineOrder>(x);
                        }

                        #region tmp
                        //S_Sql = $@" select *  from mesLine A 
                        //                    left join mesLineDetail B on A.ID = B.LineID
                        //                    left join luLineTypeDef C on B.LineTypeDefID = C.ID 
                        //                where C.ID=(select ID from luLineTypeDef where Description='PartFamilyID') 
                        //                    and B.LineID= {x.LineID} ";
                        //var lines = DapperConn.Query<SC_mesLine>(S_Sql, null, null, true, I_DBTimeout, null);
                        //bool isInsert = false;
                        //if (lines != null && lines.Any())
                        //{
                        //    S_Sql = $@" select *  from mesLine A 
                        //                    left join mesLineDetail B on A.ID = B.LineID
                        //                    left join luLineTypeDef C on B.LineTypeDefID = C.ID 
                        //                where C.ID=(select ID from luLineTypeDef where Description='PartFamilyID') 
                        //                    and B.LineID= {x.LineID} and  B.Content='{mesProductionOrder.PartFamilyID}'";
                        //    var flines = DapperConn.Query<SC_mesLine>(S_Sql, null, null, true, I_DBTimeout, null);
                        //    if (flines != null && flines.Any())
                        //        isInsert = true;
                        //}
                        //else
                        //{
                        //    isInsert = true;
                        //}

                        //x.CreationTime = DateTime.Now;
                        //if (isInsert)
                        //    _dbContext.Add<SC_mesLineOrder>(x);
                        #endregion
                    });
                    if (mesProductionOrder.PartNumber?.IndexOf("(EVT)") > 0)
                    {
                        var vDOE = Configs.GetSection("AppSetting:DOE_Parameter1").Value;
                        var detailDef = _dbContext.GetSingleOrDefault<SC_luProductionOrderDetailDef>(x => x.Description == "DOE_Parameter1");
                        int defId;
                        if (detailDef is null)
                        {
                            defId = DapperConn.ExecuteScalar("SELECT MAX(ID)+1 FROM dbo.luProductionOrderDetailDef", null, null, I_DBTimeout, null).ToInt();
                            _dbContext.Add<SC_luProductionOrderDetailDef>(new SC_luProductionOrderDetailDef() { Description = "DOE_Parameter1", ID = defId });
                        }
                        else
                        {
                            defId = detailDef.ID;
                        }
                        SC_mesProductionOrderDetail sC_MesProductionOrderDetail = new SC_mesProductionOrderDetail()
                        {
                            ProductionOrderDetailDefID = defId,
                            Content = string.IsNullOrEmpty(vDOE) ? "PP,SQ,CS,DE,ET,PE,CB" : vDOE,
                            ProductionOrderID = mesProductionOrder.ID,
                        };
                        _dbContext.Add<SC_mesProductionOrderDetail>(sC_MesProductionOrderDetail);
                    }
                    //_dbContext.SaveChanges();
                    transaction.Commit();
                    commonResult.Success = true;
                }
                catch (Exception e)
                {
                    Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, "", e);
                    commonResult.ResultMsg = "Insert PMC exception.";
                    transaction.Rollback();
                }

            }

            return commonResult;
        }
    }
}
