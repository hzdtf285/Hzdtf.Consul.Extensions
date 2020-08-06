using System;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul基本选项配置
    /// @ 黄振东
    /// </summary>
    public class ConsulBasicOption
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Consul地址
        /// </summary>
        public string ConsulAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 数据中心，默认是dc1
        /// </summary>
        public string Datacenter
        {
            get;
            set;
        } = "dc1";

        /// <summary>
        /// 缓存失效时间（单位：秒），默认为5秒。-1为永不过期
        /// </summary>
        public int CacheExpire
        {
            get;
            set;
        } = 5;

        /// <summary>
        /// 间隔时间，单位：毫秒，默认为5秒
        /// </summary>
        public int IntervalMillseconds
        {
            get;
            set;
        } = 5000;
    }
}
