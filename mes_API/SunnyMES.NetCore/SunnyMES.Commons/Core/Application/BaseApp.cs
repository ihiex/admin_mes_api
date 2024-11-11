﻿using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Commons.Application
{

    /// <summary>
    /// 业务层基类，Service用于普通的数据库操作
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TDto">实体类型</typeparam>
    /// <typeparam name="TService">Service类型</typeparam>
    /// <typeparam name="Tkey">主键类型</typeparam>
    public class BaseApp<T, TDto, TService, Tkey>
       where T : Entity
       where TDto : class
        where TService : IService<T, TDto, Tkey>
    {
        /// <summary>
        /// 用于普通的数据库操作
        /// </summary>
        /// <value>The service.</value>
        protected IService<T,TDto, Tkey> service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_service"></param>
        public BaseApp(IService<T,TDto, Tkey> _service)
        {
            service = _service;
        }

        /// <summary>
        /// 同步物理删除实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual bool Delete(T entity)
        {
            return service.Delete(entity, null);
        }
        /// <summary>
        /// 同步物理删除实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual bool Delete(Tkey id)
        {
            return service.Delete(id, null);
        }

        /// <summary>
        /// 异步物理删除实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual Task<bool> DeleteAsync(Tkey id)
        {
            return service.DeleteAsync(id, null);
        }


        /// <summary>
        /// 异步物理删除实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual Task<bool> DeleteAsync(T entity)
        {
            return service.DeleteAsync(entity, null);
        }

        /// <summary>
        /// 按主键批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool DeleteBatch(IList<dynamic> ids)
        {
            return service.DeleteBatch(ids, null);
        }


        /// <summary>
        /// 按条件批量删除
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public virtual bool DeleteBatchWhere(string where)
        {
            return service.DeleteBatchWhere(where, null);
        }
        /// <summary>
        /// 软删除信息，将DeleteMark设置为1-删除，0-恢复删除
        /// </summary>
        /// <param name="bl">true为不删除，false删除</param>
        /// <param name="id">主键ID</param>
        /// <param name="userId">操作用户</param>
        /// <returns></returns>
        public virtual bool DeleteSoft(bool bl, Tkey id, string userId)
        {
            return service.DeleteSoft(bl, id, userId, null);
        }

        /// <summary>
        /// 异步软删除信息，将DeleteMark设置为1-删除，0-恢复删除
        /// </summary>
        /// <param name="bl">true为不删除，false删除</param>
        /// <param name="id">主键ID</param>
        /// <param name="userId">操作用户</param>
        /// <returns></returns>
        public virtual Task<bool> DeleteSoftAsync(bool bl, Tkey id, string userId)
        {
            return service.DeleteSoftAsync(bl, id, userId, null);
        }
        /// <summary>
        /// 同步查询单个实体。
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual T Get(Tkey id)
        {
            return service.Get(id);
        }
        /// <summary>
        /// 同步查询单个实体。
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual T GetWhere(string where)
        {
            return service.GetWhere(where, null);
        }

        /// <summary>
        /// 异步查询单个实体。
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<T> GetWhereAsync(string where)
        {
            return await service.GetWhereAsync(where, null);
        }

        /// <summary>
        /// 根据查询条件查询前多少条数据
        /// </summary>
        /// <param name="top">多少条数据</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetListTopWhere(int top, string where = null)
        {
            return service.GetListTopWhere(top, where);
        }
        /// <summary>
        /// 同步查询所有实体。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return service.GetAll(null);
        }

        /// <summary>
        /// 异步步查询所有实体。
        /// </summary>
        /// <returns></returns>
        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return service.GetAllAsync(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<T> GetAsync(Tkey id)
        {
            return service.GetAsync(id);
        }
        ///<summary>
        /// 根据查询条件查询数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>

        public virtual IEnumerable<T> GetListWhere(string where = null)
        {
            return service.GetListWhere(where, null);
        }
        ///<summary>
        /// 异步根据查询条件查询数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetListWhereAsync(string where = null)
        {
            return await service.GetListWhereAsync(where, null);
        }
        /// <summary>
        /// 同步新增实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual long Insert(T entity)
        {
            return service.Insert(entity, null);
        }

        /// <summary>
        /// 异步步新增实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual Task<long> InsertAsync(T entity)
        {
            return service.InsertAsync(entity, null);
        }
        /// <summary>
        /// 同步更新实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual bool Update(T entity, Tkey id)
        {
            return service.Update(entity, id, null);
        }


        /// <summary>
        /// 异步更新实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public virtual Task<bool> UpdateAsync(T entity, Tkey id)
        {
            return service.UpdateAsync(entity, id, null);
        }



        /// <summary>
        /// 更新某一字段值
        /// </summary>
        /// <param name="strField">字段</param>
        /// <param name="fieldValue">字段值</param>
        /// <param name="where">条件,为空更新所有内容</param>
        /// <returns></returns>
        public virtual bool UpdateTableField(string strField, string fieldValue, string where)
        {

            return service.UpdateTableField(strField, fieldValue, where, null);
        }

        /// <summary>
        /// 查询软删除的数据，如果查询条件为空，即查询所有软删除的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllByIsDeleteMark(string where = null)
        {
            return service.GetAllByIsDeleteMark(where, null);
        }

        /// <summary>
        /// 查询未软删除的数据，如果查询条件为空，即查询所有未软删除的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllByIsNotDeleteMark(string where = null)
        {
            return service.GetAllByIsNotDeleteMark(where, null);
        }

        /// <summary>
        /// 查询有效的数据，如果查询条件为空，即查询所有有效的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllByIsEnabledMark(string where = null)
        {
            return service.GetAllByIsEnabledMark(where, null);
        }

        /// <summary>
        /// 查询无效的数据，如果查询条件为空，即查询所有无效的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllByIsNotEnabledMark(string where = null)
        {
            return service.GetAllByIsNotEnabledMark(where, null);
        }
        /// <summary>
        /// 查询未软删除且有效的数据，如果查询条件为空，即查询所有数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllByIsNotDeleteAndEnabledMark(string where = null)
        {
            return service.GetAllByIsNotDeleteAndEnabledMark(where, null);
        }

        /// <summary>
        /// 查询软删除的数据，如果查询条件为空，即查询所有软删除的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllByIsDeleteMarkAsync(string where = null)
        {
            return await service.GetAllByIsDeleteMarkAsync(where, null);
        }

        /// <summary>
        /// 查询未软删除的数据，如果查询条件为空，即查询所有未软删除的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllByIsNotDeleteMarkAsync(string where = null)
        {
            return await service.GetAllByIsNotDeleteMarkAsync(where, null);
        }

        /// <summary>
        /// 查询有效的数据，如果查询条件为空，即查询所有有效的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual  async Task<IEnumerable<T>> GetAllByIsEnabledMarkAsync(string where = null)
        {
            return await service.GetAllByIsEnabledMarkAsync(where, null);
        }

        /// <summary>
        /// 查询无效的数据，如果查询条件为空，即查询所有无效的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllByIsNotEnabledMarkAsync(string where = null)
        {
            return await service.GetAllByIsNotEnabledMarkAsync(where, null);
        }

        /// <summary>
        /// 查询未软删除且有效的数据，如果查询条件为空，即查询所有数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllByIsNotDeleteAndEnabledMarkAsync(string where = null)
        {
            return await service.GetAllByIsNotDeleteAndEnabledMarkAsync(where, null);
        }


        /// <summary>
        /// 设置数据有效性，将EnabledMark设置为1:有效，0-为无效
        /// </summary>
        /// <param name="bl">true为有效，false无效</param>
        /// <param name="id">主键ID</param>
        /// <param name="userId">操作用户</param>
        /// <returns></returns>
        public virtual bool SetEnabledMark(bool bl, Tkey id, string userId = null)
        {
            return service.SetEnabledMark(bl, id, userId, null);
        }

        /// <summary>
        /// 异步设置数据有效性，将EnabledMark设置为1:有效，0-为无效
        /// </summary>
        /// <param name="bl">true为有效，false无效</param>
        /// <param name="id">主键ID</param>
        /// <param name="userId">操作用户</param>
        /// <returns></returns>
        public virtual async Task<bool> SetEnabledMarkAsync(bool bl, Tkey id, string userId = null)
        {
            return await service.SetEnabledMarkAsync(bl, id, userId, null);
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="condition">查询的条件</param>
        /// <param name="info">分页实体</param>
        /// <returns>指定对象的集合</returns>
        public virtual List<T> FindWithPager(string condition, PagerInfo info)
        {
            return service.FindWithPager(condition, info, null);
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="condition">查询的条件</param>
        /// <param name="info">分页实体</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <returns>指定对象的集合</returns>
        public virtual List<T> FindWithPager(string condition, PagerInfo info, string fieldToSort)
        {
            return service.FindWithPager(condition, info, fieldToSort, null);
        }
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="condition">查询的条件</param>
        /// <param name="info">分页实体</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <param name="desc">是否降序</param>
        /// <returns>指定对象的集合</returns>
        public virtual List<T> FindWithPager(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            return service.FindWithPager(condition, info, fieldToSort, desc, null);
        }

        /// <summary>
        /// 分页查询，自行封装sql语句
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="info">分页信息</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <param name="desc">排序方式 true为desc，false为asc</param>
        /// <returns></returns>
        public virtual List<T> FindWithPagerSql(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            return service.FindWithPagerSql(condition, info, fieldToSort, desc, null);
        }

        /// <summary>
        /// 分页查询包含用户信息
        /// 查询主表别名为t1,用户表别名为t2，在查询字段需要注意使用t1.xxx格式，xx表示主表字段
        /// 用户信息主要有用户账号：Account、昵称：NickName、真实姓名：RealName、头像：HeadIcon、手机号：MobilePhone
        /// 输出对象请在Dtos中进行自行封装，不能是使用实体Model类
        /// </summary>
        /// <param name="condition">查询条件字段需要加表别名</param>
        /// <param name="info">分页信息</param>
        /// <param name="fieldToSort">排序字段，也需要加表别名</param>
        /// <param name="desc">排序方式</param>
        /// <returns></returns>
        public virtual List<object> FindWithPagerRelationUser(string condition, PagerInfo info, string fieldToSort, bool desc)
        {
            return service.FindWithPagerRelationUser(condition, info, fieldToSort, desc, null);
        }
        /// <summary>
        /// 根据条件统计数据
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public virtual int GetCountByWhere(string condition)
        {
            return service.GetCountByWhere(condition);
        }
    }
}
