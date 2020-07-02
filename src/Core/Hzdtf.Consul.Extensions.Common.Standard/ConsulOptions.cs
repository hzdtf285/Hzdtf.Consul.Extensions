using System;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul选项配置
    /// @ 黄振东
    /// </summary>
    public class ConsulOptions : ConsulBasicOption
    {
        /// <summary>
        /// 服务ID（唯一）
        /// </summary>
        public string ServiceId
        {
            get;
            set;
        }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServiceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 标签数组
        /// </summary>
        public string[] Tags
        {
            get;
            set;
        }

        /// <summary>
        /// 服务检测
        /// </summary>
        public ServiceCheckOptions ServiceCheck
        {
            get;
            set;
        } = new ServiceCheckOptions();
    }

    /// <summary>
    /// 服务检测选项配置
    /// </summary>
    public class ServiceCheckOptions
    {
        /// <summary>
        /// 健康检测地址
        /// </summary>
        public string HealthCheck
        {
            get;
            set;
        } = "/Health";

        /// <summary>
        /// 服务停止多久后注销服务（单位：秒）
        /// </summary>
        public int DeregisterCriticalServiceAfter
        {
            get;
            set;
        } = 5;

        /// <summary>
        /// 超时时间（单位：秒）
        /// </summary>
        public int Timeout
        {
            get;
            set;
        } = 15;

        /// <summary>
        /// 间隔时间（单位：秒）
        /// </summary>
        public int Interval
        {
            get;
            set;
        } = 10;
    }
}
