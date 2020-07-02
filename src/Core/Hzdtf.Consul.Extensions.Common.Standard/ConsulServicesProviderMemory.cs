using Hzdtf.Utility.Standard.Attr;
using Hzdtf.Utility.Standard.RemoteService.Provider;
using Hzdtf.Utility.Standard.SystemV2;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul服务提供者内存缓存
    /// @ 黄振东
    /// </summary>
    public class ConsulServicesProviderMemory : ServicesProviderMemory, IDisposable
    {
        /// <summary>
        /// 默认服务提供者
        /// </summary>
        private readonly ConsulServicesProvider defaultServicesProvider;

        /// <summary>
        /// 原生服务提供者
        /// </summary>
        private IServicesProvider protoServicesProvider;

        /// <summary>
        /// 原生服务提供者
        /// </summary>
        protected override IServicesProvider ProtoServicesProvider
        {
            get
            {
                if (protoServicesProvider == null)
                {
                    protoServicesProvider = defaultServicesProvider;
                }

                return protoServicesProvider;
            }
            set => protoServicesProvider = value;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConsulServicesProviderMemory()
            : this(new MemoryCache(new MemoryCacheOptions()
            {
                Clock = new LocalSystemClock()
            }), new ConsulServicesProvider(), Options.Create<ConsulBasicOption>(new ConsulBasicOption()
            {
            }))
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cacheExpire">缓存失效时间（单位：秒），-1为永不过期</param>
        public ConsulServicesProviderMemory(int cacheExpire)
            : this(new MemoryCache(new MemoryCacheOptions()
            {
                Clock = new LocalSystemClock()
            }), new ConsulServicesProvider(), Options.Create<ConsulBasicOption>(new ConsulBasicOption()
            {
                CacheExpire = cacheExpire
            }))
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <param name="defaultServicesProvider">默认服务提供者</param>
        /// <param name="consulOptions">Consul配置选项</param>
        public ConsulServicesProviderMemory(IMemoryCache cache, ConsulServicesProvider defaultServicesProvider, IOptions<ConsulBasicOption> consulOptions)
            : base(cache)
        {
            if (defaultServicesProvider == null)
            {
                defaultServicesProvider = new ConsulServicesProvider();
            }
            this.defaultServicesProvider = defaultServicesProvider;
            if (consulOptions != null)
            {
                this.cacheExpire = consulOptions.Value.CacheExpire;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [ProcTrackLog(ExecProc = false)]
        public override void Dispose()
        {
            if (defaultServicesProvider != null)
            {
                defaultServicesProvider.Dispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~ConsulServicesProviderMemory()
        {
            Dispose();
        }
    }
}
