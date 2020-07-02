using Hzdtf.Consul.Extensions.Common.Standard;
using Hzdtf.Consul.Extensions.GRpc.Core.Services;
using Hzdtf.Utility.Standard.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NConsul;
using NConsul.AspNetCore;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Hzdtf.Consul.Extensions.GRpc.Core
{
    /// <summary>
    /// ConsulGRpc注册扩展类
    /// @ 黄振东
    /// </summary>
    public static class ConsulGRpcRegisterExtensions
    {
        /// <summary>
        /// Consul生成器
        /// </summary>
        private static NConsulBuilder consulBuilder = null;

        /// <summary>
        /// 添加GRpc注册Consul
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="configJsonFilePath">配置JSON文件路径。如果传入为null，则默认为Config/consulConfig.json</param>
        /// <returns>服务</returns>
        public static IServiceCollection AddGRpcRegisterConsul(this IServiceCollection services, string configJsonFilePath = "Config/consulConfig.json")
        {
            // 将consul配置文件配置到服务里
            var config = new ConfigurationBuilder().AddJsonFile(configJsonFilePath).Build();
            services.Configure<ConsulOptions>(config);

            // 添加一个能获取本服务地址的服务
            services.AddSingleton(serviceProvider =>
            {
                var server = serviceProvider.GetRequiredService<IServer>();
                return server.Features.Get<IServerAddressesFeature>();
            });

            // 从JSON文件里解析对象
            var jsonStr = File.ReadAllText(configJsonFilePath);
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                throw new ArgumentNullException("json文件内容不能为空");
            }
            var consulConfig = JsonUtil.Deserialize<ConsulOptions>(jsonStr);
            if (consulConfig == null)
            {
                throw new ArgumentNullException("json文件内容反序列化ConsulOptions对象为空");
            }
            if (string.IsNullOrWhiteSpace(consulConfig.ConsulAddress))
            {
                throw new ArgumentNullException("Consul地址不能为空");
            }
            if (string.IsNullOrWhiteSpace(consulConfig.ServiceName))
            {
                throw new ArgumentNullException("服务名不能为空");
            }

            // 生成Consul注册所需要的对象
            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(consulConfig.ConsulAddress);
                x.Datacenter = consulConfig.Datacenter;
            });
            services.AddSingleton(consulClient);
            services.Configure(new Action<NConsulOptions>(op =>
            {
                op.Address = consulConfig.ConsulAddress;
            }));

            consulBuilder = new NConsulBuilder(consulClient);

            return services;
        }

        /// <summary>
        /// 使用GRpc注册Consul
        /// </summary>
        /// <param name="app">应用生成器</param>
        /// <returns>应用生成器</returns>
        public static IApplicationBuilder UseGRpcRegisterConsul(this IApplicationBuilder app)
        {
            // 获取consul配置对象
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulOptions>>().Value;

            // 获取本服务的地址，如果不为空，则直接取。否则取配置选项里的服务地址
            if (string.IsNullOrWhiteSpace(consulConfig.ServiceAddress))
            {
                consulConfig.ServiceAddress = NetworkUtil.FilterUrl(app.ApplicationServices.GetService<IServerAddressesFeature>().Addresses.FirstOrDefault());
                if (string.IsNullOrWhiteSpace(consulConfig.ServiceAddress))
                {
                    throw new ArgumentNullException("服务地址不能为空");
                }
            }

            // 注册到Consul
            var serviceUri = new Uri(consulConfig.ServiceAddress);
            var agentCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(consulConfig.ServiceCheck.DeregisterCriticalServiceAfter),
                Interval = TimeSpan.FromSeconds(consulConfig.ServiceCheck.Interval),
                GRPC = $"{serviceUri.Host}:{serviceUri.Port}",
                GRPCUseTLS = string.Compare(serviceUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0,
                Timeout = TimeSpan.FromSeconds(consulConfig.ServiceCheck.Timeout),                
            };
            consulBuilder.AddHealthCheck(agentCheck)
                         .RegisterService(consulConfig.ServiceName, serviceUri.Host, serviceUri.Port, consulConfig.Tags);

            return app;
        }

        /// <summary>
        /// 使用GRpc路由
        /// </summary>
        /// <param name="endpoints">终结点路由生成器</param>
        /// <returns>终结点路由生成器</returns>
        public static IEndpointRouteBuilder UseGRpcRoute(this IEndpointRouteBuilder endpoints)
        {
            // 映射健康检测路由
            endpoints.MapGrpcService<HealthCheckService>();

            return endpoints;
        }
    }
}
