using Hzdtf.Utility.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Consul.ConfigCenter.AspNet.Core
{
    /// <summary>
    /// 配置中心辅助类
    /// @ 黄振东
    /// </summary>
    public static class ConfigCenterUtil
    {
        /// <summary>
        /// 获取键路径，以/分隔
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="applicationName">应用程序名称，如果为空，则默认取UtilTool.CurrApplicationName</param>
        /// <returns>键路径</returns>
        public static string GetKeyPath(string key, string applicationName = null)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                applicationName = UtilTool.CurrApplicationName;
            }

            return string.IsNullOrWhiteSpace(applicationName) ? key : $"{applicationName}/{key}";
        }
    }
}
