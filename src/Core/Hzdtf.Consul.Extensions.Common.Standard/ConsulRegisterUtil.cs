using Consul;
using Hzdtf.Utility.Standard.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul注册工具类
    /// @ 黄振东
    /// </summary>
    public static class ConsulRegisterUtil
    {
        /// <summary>
        /// 创建Consul客户端注册并根据选项进行配置
        /// </summary>
        /// <param name="optionsJsonFile">选项配置JSON文件</param>
        /// <param name="getLocalServiceAddress">回调获取本地服务地址</param>
        /// <param name="config">回调选项配置</param>
        /// <returns>Consul客户端</returns>
        public static ConsulClient CreateConsulClientRegister(string optionsJsonFile = "Config/consulConfig.json", Func<string> getLocalServiceAddress = null, Action<ConsulOptions> config = null)
        {
            if (string.IsNullOrWhiteSpace(optionsJsonFile))
            {
                throw new ArgumentNullException("配置JSON文件路径不能为空");
            }

            var options = JsonUtil.DeserializeFromFile<ConsulOptions>(optionsJsonFile);
            if (config != null)
            {
                config(options);
            }

            return CreateConsulClientRegister(options, getLocalServiceAddress);
        }

        /// <summary>
        /// 创建Consul客户端注册并根据选项进行配置
        /// </summary>
        /// <param name="options">选项配置</param>
        /// <param name="getLocalServiceAddress">回调获取本地服务地址</param>
        /// <returns>Consul客户端</returns>
        public static ConsulClient CreateConsulClientRegister(ConsulOptions options, Func<string> getLocalServiceAddress = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException("选项配置不能为null");
            }
            if (string.IsNullOrWhiteSpace(options.ConsulAddress))
            {
                throw new ArgumentNullException("Consul地址不能为空");
            }
            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                throw new ArgumentNullException("服务名不能为空");
            }

            // 服务ID，如果有配置指定，则使用配置，否则由程序生成唯一
            options.ServiceId = options.ServiceId ?? $"{NetworkUtil.LocalIP}_{StringUtil.NewShortGuid()}";

            // 定义consul客户端对象
            var consulClient = new ConsulClient(clientConfig =>
            {
                clientConfig.Address = new Uri(options.ConsulAddress);
                clientConfig.Datacenter = options.Datacenter;
            });

            // 获取本服务的地址，如果不为空，则直接取。否则取配置选项里的服务地址
            if (string.IsNullOrWhiteSpace(options.ServiceAddress) && getLocalServiceAddress != null)
            {
                options.ServiceAddress = NetworkUtil.FilterUrl(getLocalServiceAddress());
            }
            if (string.IsNullOrWhiteSpace(options.ServiceAddress))
            {
                throw new ArgumentNullException("服务地址不能为空");
            }

            var serviceUri = new Uri(options.ServiceAddress);

            // 定义一个代理服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = options.ServiceId,
                Tags = options.Tags,
                Check = new AgentServiceCheck() // 定义健康检测对象
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(options.ServiceCheck.DeregisterCriticalServiceAfter), // 服务停止后多久注销服务
                    HTTP = $"{serviceUri.Scheme}://{serviceUri.Host}:{serviceUri.Port}{options.ServiceCheck.HealthCheck}", // 健康检测URL地址
                    Interval = TimeSpan.FromSeconds(options.ServiceCheck.Interval), // 每隔多少时间检测
                    Timeout = TimeSpan.FromSeconds(options.ServiceCheck.Timeout), // 注册超时时间                   
                },
                Name = options.ServiceName,
                Address = serviceUri.Host,
                Port = serviceUri.Port,
            };

            // 将注册配置信息注册到consul里
            consulClient.Agent.ServiceRegister(registration).Wait();
            // 程序退出时需要反注册服务
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            };

            return consulClient;
        }
    }
}
