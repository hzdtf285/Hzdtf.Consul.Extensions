using System;
using System.Collections.Generic;
using Hzdtf.Utility.Standard;
using Hzdtf.Utility.Standard.Utils;
using Microsoft.AspNetCore.Hosting;

namespace Hzdtf.Consul.ConfigCenter.AspNet.Core
{
    /// <summary>
    /// Web主机生成器扩展类
    /// @ 黄振东
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// 添加Consul配置中心
        /// 需要在Program程序启动时，在配置web主机时调用，如IHostBuilder.ConfigureWebHostDefaults
        /// </summary>
        /// <param name="webBuilder">web主机生成器</param>
        /// <param name="consulConfigFile">Consul配置文件。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>web主机生成器</returns>
        public static IWebHostBuilder AddConsulConfigCenter(this IWebHostBuilder webBuilder, string consulConfigFile = "Config/consulConfig.json")
        {
            if (string.IsNullOrWhiteSpace(consulConfigFile))
            {
                throw new ArgumentNullException("Consul配置文件路径不能为空");
            }

            var configOptions = JsonUtil.DeserializeFromFile<ConfigCenterOptions>(consulConfigFile);
            webBuilder.AddConsulConfigCenter(o =>
            {
                o.FillFrom(configOptions);
            });

            return webBuilder;
        }

        /// <summary>
        /// 添加Consul配置中心
        /// 需要在Program程序启动时，在配置web主机时调用，如IHostBuilder.ConfigureWebHostDefaults
        /// 关于key的前辍说明优先顺序说明：
        /// 1、从传过来的ServiceName查找
        /// 2、从appsetting里查找ServiceName的key
        /// 3、使用当前应用程序名
        /// </summary>
        /// <param name="webBuilder">web主机生成器</param>
        /// <param name="options">配置回调</param>
        /// <returns>web主机生成器</returns>
        public static IWebHostBuilder AddConsulConfigCenter(this IWebHostBuilder webBuilder, Action<ConfigCenterOptions> options)
        {
            var configOptions = new ConfigCenterOptions();
            if (options != null)
            {
                options(configOptions);
            }

            webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var configuration = config.Build();
                if (string.IsNullOrWhiteSpace(configOptions.ServiceName))
                {
                    configOptions.ServiceName = configuration["ServiceName"];
                }
                if (string.IsNullOrWhiteSpace(configOptions.ConsulAddress))
                {
                    configOptions.ConsulAddress = configuration["ConsulAddress"];
                    if (string.IsNullOrWhiteSpace(configOptions.ConsulAddress))
                    {
                        throw new ArgumentNullException("Consul地址不能为空");
                    }
                }

                var envi = hostingContext.HostingEnvironment;
                UtilTool.CurrApplicationName = envi.ApplicationName;

                var defaultKey = new List<string>();
                if (configOptions.AutoLoadCommonKey)
                {
                    defaultKey.Add("common.json");
                }
                defaultKey.Add(ConfigCenterUtil.GetKeyPath("appsettings.json", configOptions.ServiceName));
                defaultKey.Add(ConfigCenterUtil.GetKeyPath($"appsettings.{envi.EnvironmentName}.json", configOptions.ServiceName));

                foreach (var k in defaultKey)
                {
                    if (configOptions.Keys.Contains(k))
                    {
                        continue;
                    }

                    configOptions.Keys.Add(k);
                }

                hostingContext.Configuration = config.AddConsulConfigCenter((o) =>
                {
                    o.FillFrom(configOptions);
                })
                .Build();
            });

            return webBuilder;
        }
    }
}
