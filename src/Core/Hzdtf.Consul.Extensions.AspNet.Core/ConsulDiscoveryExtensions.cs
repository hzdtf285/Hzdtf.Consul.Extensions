using Hzdtf.Consul.Extensions.Common.Standard;
using Hzdtf.Utility.AspNet.Core.RemoteService;
using Hzdtf.Utility.Standard.RemoteService.Provider;
using Hzdtf.Utility.Standard.SystemV2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.Extensions.AspNet.Core
{
    /// <summary>
    /// Consul服务发现扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulDiscoveryExtensions
    {
        /// <summary>
        /// 添加服务发现Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="options">配置回调</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddDiscoveryConsul(this IServiceCollection services, Action<UnityConsulOptions> options = null)
        {
            var unityConsulOptions = new UnityConsulOptions();
            if (options != null)
            {
                options(unityConsulOptions);
            }
            if (unityConsulOptions.ConsulBasicOption == null)
            {
                var config = new ConfigurationBuilder().AddJsonFile(unityConsulOptions.ConsulBasicOptionJsonFile).Build();
                services.Configure<ConsulBasicOption>(config);

                unityConsulOptions.ConsulBasicOption = config.Get<ConsulBasicOption>();
            }

            if (unityConsulOptions.ConsulBasicOption != null)
            {
                services.AddBasicConsul(unityConsulOptions.ConsulBasicOption);
            }
            else if (unityConsulOptions.ConsulBasicOptionJsonFile == null)
            {
                services.AddBasicConsul();
            }
            else
            {
                services.AddBasicConsul(unityConsulOptions.ConsulBasicOptionJsonFile);
            }

            services.AddUnityServicesBuilderCache(builder =>
            {
                if (unityConsulOptions.UnityServicesOptionsJsonFile != null)
                {
                    builder.ServiceBuilderConfigJsonFile = unityConsulOptions.UnityServicesOptionsJsonFile;
                }

                switch (unityConsulOptions.CacheType)
                {
                    case ServiceProviderCacheType.TIMER_REFRESH:
                        builder.ServiceProvider = new ConsulServiceProviderAgg(unityConsulOptions.ConsulBasicOption.IntervalMillseconds, unityConsulOptions.ConsulBasicOption);

                        break;

                    case ServiceProviderCacheType.DALAY_REFRESH:
                        services.AddMemoryCache(options =>
                        {
                            options.Clock = new LocalSystemClock();
                        });

                        builder.ServiceProvider = new ConsulServicesProviderMemory(Options.Create<ConsulBasicOption>(unityConsulOptions.ConsulBasicOption));

                        break;

                    default:
                        builder.ServiceProvider = new ConsulServicesProvider(unityConsulOptions.ConsulBasicOption);

                        break;
                }
            });

            return services;
        }
    }
}
