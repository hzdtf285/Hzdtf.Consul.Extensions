using Hzdtf.Consul.Extensions.Common.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.Extensions.AspNet.Core
{
    /// <summary>
    /// 统一Consul选项配置
    /// @ 黄振东
    /// </summary>
    public class UnityConsulOptions
    {
        /// <summary>
        /// 统一服务配置Json文件
        /// </summary>
        public string UnityServicesOptionsJsonFile
        {
            get;
            set;
        } = "Config/serviceBuilderConfig.json";

        /// <summary>
        /// Consul基本配置
        /// </summary>
        public ConsulBasicOption ConsulBasicOption
        {
            get;
            set;
        }

        /// <summary>
        /// Consul基本配置Json文件
        /// </summary>
        public string ConsulBasicOptionJsonFile
        {
            get;
            set;
        } = "Config/consulConfig.json";
    }
}
