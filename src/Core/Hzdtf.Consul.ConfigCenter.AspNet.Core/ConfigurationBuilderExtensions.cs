using Hzdtf.Utility.Standard.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Winton.Extensions.Configuration.Consul;

namespace Hzdtf.Consul.ConfigCenter.AspNet.Core
{
    /// <summary>
    /// 配置生成器扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// 添加Consul配置中心
        /// </summary>
        /// <param name="builder">配置生成器</param>
        /// <param name="consulConfigFile">Consul配置文件。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <param name="options">配置回调</param>
        /// <returns>配置生成器</returns>
        public static IConfigurationBuilder AddConsulConfigCenter(this IConfigurationBuilder builder, string consulConfigFile = "Config/consulConfig.json", Action<ConfigCenterOptions> options = null)
        {
            if (string.IsNullOrWhiteSpace(consulConfigFile))
            {
                throw new ArgumentNullException("Consul配置文件路径不能为空");
            }

            var configOptions = JsonUtil.DeserializeFromFile<ConfigCenterOptions>(consulConfigFile);
            if (options != null)
            {
                options(configOptions);
            }
            builder.AddConsulConfigCenter(o =>
            {
                o.FillFrom(configOptions);
            });

            return builder;
        }

        /// <summary>
        /// 添加Consul配置中心
        /// </summary>
        /// <param name="builder">配置生成器</param>
        /// <param name="options">选项回调</param>
        /// <returns>配置生成器</returns>
        public static IConfigurationBuilder AddConsulConfigCenter(this IConfigurationBuilder builder, Action<ConfigCenterOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("回调配置不能为null");
            }
            var configOptions = new ConfigCenterOptions();
            options(configOptions);
            if (string.IsNullOrWhiteSpace(configOptions.ConsulAddress))
            {
                throw new ArgumentNullException("Consul地址不能为空");
            }
            if (configOptions.Keys.IsNullOrCount0())
            {
                throw new ArgumentNullException("键不能为空");
            }

            Action<IConsulConfigurationSource> fun = delegate (IConsulConfigurationSource source)
            {
                source.ConsulConfigurationOptions = cco =>
                {
                    // 配置Consul
                    cco.Address = new Uri(configOptions.ConsulAddress);
                    cco.Datacenter = configOptions.Datacenter;
                };
                source.Optional = true; // 配置选项
                source.ReloadOnChange = true; // 配置文件更新后重新加载
                source.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 忽略异常
            };

            foreach (var key in configOptions.Keys)
            {
                builder.AddConsul(key, fun);
            }

            return builder;
        }
    }
}
