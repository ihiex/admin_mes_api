using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 数据字典明细
    /// </summary>
    public interface IItemsDetailService:IService<ItemsDetail, ItemsDetailOutputDto, string>
    {
        /// <summary>
        /// 根据数据字典分类编码获取该分类可用内容
        /// </summary>
        /// <param name="itemCode">分类编码</param>
        /// <returns></returns>
        Task<List<ItemsDetailOutputDto>> GetItemDetailsByItemCode(string itemCode);

        /// <summary>
        /// 获取适用于Vue 树形列表
        /// </summary>
        /// <param name="itemId">类别Id</param>
        /// <returns></returns>
       Task<List<ItemsDetailOutputDto>> GetAllItemsDetailTreeTable(string itemId);
    }
}
