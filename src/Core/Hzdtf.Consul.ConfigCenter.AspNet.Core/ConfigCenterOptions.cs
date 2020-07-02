using Hzdtf.Consul.Extensions.Common.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.ConfigCenter.AspNet.Core
{
    /// <summary>
    /// 配置中心选项
    /// @ 黄振东
    /// </summary>
    public class ConfigCenterOptions : ConsulBasicOption
    {
        /// <summary>
        /// 键列表
        /// </summary>
        public IList<string> Keys
        {
            get;
            set;
        } = new List<string>();

        /// <summary>
        /// 自动加载公共键，如果为是，则会自动加载common.json作为公共key，默认为是
        /// </summary>
        public bool AutoLoadCommonKey
        {
            get;
            set;
        } = true;

        /// <summary>
        /// 填充来自配置
        /// </summary>
        /// <param name="source">配置</param>
        public void FillFrom(ConfigCenterOptions source)
        {
            FillFrom(source as ConsulBasicOption);
            this.Keys = source.Keys;
        }

        /// <summary>
        /// 填充来自配置
        /// </summary>
        /// <param name="source">配置</param>
        public void FillFrom(ConsulBasicOption source)
        {
            this.ConsulAddress = source.ConsulAddress;
            this.CacheExpire = source.CacheExpire;
            this.Datacenter = source.Datacenter;
            this.ServiceName = source.ServiceName;
        }
    }
}
