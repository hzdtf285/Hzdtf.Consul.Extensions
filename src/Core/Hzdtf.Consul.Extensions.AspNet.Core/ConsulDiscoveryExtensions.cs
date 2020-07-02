using Hzdtf.Consul.Extensions.Common.Standard;
using Hzdtf.Utility.AspNet.Core.RemoteService;
using Hzdtf.Utility.Standard.SystemV2;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMemoryCache(options =>
            {
                options.Clock = new LocalSystemClock();
            });

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
                builder.ServiceProvider = new ConsulServicesProviderMemory();
            });

            return services;
        }
    }
}
