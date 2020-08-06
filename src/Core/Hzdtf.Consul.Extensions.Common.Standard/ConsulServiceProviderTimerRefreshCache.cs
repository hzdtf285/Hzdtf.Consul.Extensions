using Hzdtf.Utility.Standard.Attr;
using Hzdtf.Utility.Standard.Cache.TimerRefresh;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.Extensions.Common.Standard
{
    /// <summary>
    /// Consul服务提供者定时刷新缓存
    /// @ 黄振东
    /// </summary>
    public class ConsulServiceProviderTimerRefreshCache : TimerRefreshCacheBase<string[]>
    {
        /// <summary>
        /// 默认服务提供者
        /// </summary>
        private readonly ConsulServicesProvider defaultServicesProvider;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="intervalMillseconds">间隔时间，单位：毫秒</param>
        /// <param name="state">状态</param>
        /// <param name="options">Consul基本选项</param>
        public ConsulServiceProviderTimerRefreshCache(int intervalMillseconds, object state = null, ConsulBasicOption options = null)
            : base(intervalMillseconds, state, false)
        {
            if (options == null)
            {
                defaultServicesProvider = new ConsulServicesProvider();
            }
            else
            {
                defaultServicesProvider = new ConsulServicesProvider(options);
            }

            InitTimer();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>值</returns>
        protected override string[] Refresh(object state)
        {
            if (state is string[])
            {
                var str = state as string[];
                if (str.Length != 2)
                {
                    throw new ArgumentException("状态[state]数组长度必须是2");
                }

                return defaultServicesProvider.GetAddresses(str[0], str[1]).Result;
            }
            else
            {
                throw new ArgumentException("状态[state]参数不是字符串数组");
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        [ProcTrackLog(ExecProc = false)]
        public override void Dispose()
        {
            if (defaultServicesProvider != null)
            {
                defaultServicesProvider.Dispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// 析构方法
        /// </summary>
        ~ConsulServiceProviderTimerRefreshCache()
        {
            Dispose();
        }
    }
}
