using Hzdtf.Consul.Extensions.Common.Standard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Standard.Utils;
using System.IO;

namespace Hzdtf.Consul.Extensions.AspNet.Core
{
    /// <summary>
    /// Consul扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulExtensions
    {
        /// <summary>
        /// 添加基本Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="configJsonFilePath">配置JSON文件路径。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddBasicConsul(this IServiceCollection services, string configJsonFilePath = "Config/consulConfig.json")
        {
            // 将consul配置文件配置到服务里
            var config = new ConfigurationBuilder().AddJsonFile(configJsonFilePath).Build();
            services.Configure<ConsulBasicOption>(config);

            return services;
        }

        /// <summary>
        /// 添加基本Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="consulBasic">Consul基本配置</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddBasicConsul(this IServiceCollection services, ConsulBasicOption consulBasic)
        {
            var jsonStr = JsonUtil.SerializeIgnoreNull(consulBasic);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
                services.Configure<ConsulBasicOption>(config);
            }

            return services;
        }
    }
}
