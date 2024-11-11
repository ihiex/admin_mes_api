using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.DependencyInjection;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Commons.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseServiceReport<TKey> : IServiceReport<TKey>, ITransientDependency
        where TKey : IEquatable<TKey>
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 
        /// </summary>
        protected IRepositoryReport<TKey> repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iRepository"></param>
        protected BaseServiceReport(IRepositoryReport<TKey> iRepository)
        {
            repository = iRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iRepository"></param>
        /// <param name="accessor"></param>
        protected BaseServiceReport(IRepositoryReport<TKey> iRepository, IHttpContextAccessor accessor)
        {
            _accessor = accessor; 
            repository = iRepository;

        }


        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~BaseService() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        /// <summary>
        /// 添加此代码以正确实现可处置模式
        /// </summary>
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }




        #endregion
    }
}
