using Hzdtf.Consul.Extensions.AspNet.Core;
using Hzdtf.Utility.Standard.SystemV2;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hzdtf.Utility.AspNet.Core.RemoteService;
using Hzdtf.Consul.Extensions.Common.Standard;
using Hzdtf.Utility.Standard.Utils;

namespace Hzdtf.Consul.ConfigCenter.AspNet.Core
{
    /// <summary>
    /// Consul配置中心服务发现扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulCenterConfigDiscoveryExtensions
    {
        /// <summary>
        /// 添加发现Consul配置中心
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="options">配置回调</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddDiscoveryConsulConfigCenter(this IServiceCollection services, Action<UnityConsulOptions> options = null)
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

            string jsonFile = null;
            services.AddUnityServicesBuilderConfigure(builder =>
            {
                if (unityConsulOptions.UnityServicesOptionsJsonFile != null)
                {
                    builder.ServiceBuilderConfigJsonFile = unityConsulOptions.UnityServicesOptionsJsonFile;
                }
                jsonFile = builder.ServiceBuilderConfigJsonFile;
                builder.ServiceProvider = new ConsulServicesProviderMemory();
            }, builder =>
            {
                if (string.IsNullOrWhiteSpace(jsonFile))
                {
                    return;
                }

                builder.AddConsulConfigCenter(options: op =>
                {
                    if (unityConsulOptions.ConsulBasicOption == null)
                    {
                        if (string.IsNullOrWhiteSpace(unityConsulOptions.ConsulBasicOptionJsonFile))
                        {
                            throw new ArgumentNullException("Consul基本配置Json文件不能为空");
                        }
                        else
                        {
                            var centerOptions = JsonUtil.DeserializeFromFile<ConfigCenterOptions>(unityConsulOptions.ConsulBasicOptionJsonFile);
                            op.FillFrom(centerOptions);
                        }
                    }
                    else
                    {
                        op.FillFrom(unityConsulOptions.ConsulBasicOption);
                    }

                    var key = ConfigCenterUtil.GetKeyPath(jsonFile, op.ServiceName);
                    op.Keys.Add(key);
                });
            });

            return services;
        }
    }
}
