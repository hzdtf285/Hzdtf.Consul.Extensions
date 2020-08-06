using Hzdtf.Utility.Standard.Data;
using Hzdtf.Utility.Standard.RemoteService.Provider;
using Hzdtf.Utility.Standard.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul服务提供者聚合
    /// @ 黄振东
    /// </summary>
    public class ConsulServiceProviderAgg : ServiceProviderAggReaderBase
    {
        /// <summary>
        /// 间隔时间，单位：毫秒
        /// </summary>
        private readonly int intervalMillseconds;

        /// <summary>
        /// Consul基本选项
        /// </summary>
        private readonly ConsulBasicOption options = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="consulConfigFile">Consul配置文件</param>
        public ConsulServiceProviderAgg(string consulConfigFile = "Config/consulConfig.json")
        {
            var config = JsonUtil.Deserialize<ConsulBasicOption>(File.ReadAllText(consulConfigFile));

            this.intervalMillseconds = config.IntervalMillseconds;
            this.options = config;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="intervalMillseconds">间隔时间，单位：毫秒</param>
        /// <param name="options">Consul基本选项</param>
        public ConsulServiceProviderAgg(int intervalMillseconds, ConsulBasicOption options = null)
        {
            this.intervalMillseconds = intervalMillseconds;
            this.options = options;
        }

        /// <summary>
        /// 创建读取
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>读取</returns>
        protected override IReader<string[]> CreateReader(string serviceName, string tag = null)
            => new ConsulServiceProviderTimerRefreshCache(intervalMillseconds, new string[] { serviceName, tag }, options);
    }
}
