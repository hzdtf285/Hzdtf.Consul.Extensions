using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Hosting.Server;
using Hzdtf.Consul.Extensions.Common.Standard;
using Microsoft.AspNetCore.Builder;

namespace Hzdtf.Consul.Extensions.AspNet.Core
{
    /// <summary>
    /// Consul服务注册扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulRegisterExtensions
    {
        /// <summary>
        /// 添加服务注册Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="configJsonFilePath">配置JSON文件路径。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddRegisterConsul(this IServiceCollection services, string configJsonFilePath = "Config/consulConfig.json")
        {
            services.AddHealthChecks();

            // 将consul配置文件配置到服务里
            var config = new ConfigurationBuilder().AddJsonFile(configJsonFilePath).Build();
            services.Configure<ConsulOptions>(config);

            // 添加一个能获取本服务地址的服务
            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });

            return services;
        }

        /// <summary>
        /// 使用服务注册Consul
        /// </summary>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseRegisterConsul(this IApplicationBuilder app)
        {
            // 获取consul配置对象
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulOptions>>().Value;

            // 使用健康检测服务
            app.UseHealthChecks(consulConfig.ServiceCheck.HealthCheck);

            // 创建consul客户端对象
            var consulClient = ConsulRegisterUtil.CreateConsulClientRegister(consulConfig, 
                () => app.ApplicationServices.GetService<IServerAddressesFeature>().Addresses.FirstOrDefault());

            return app;
        }
    }
}
