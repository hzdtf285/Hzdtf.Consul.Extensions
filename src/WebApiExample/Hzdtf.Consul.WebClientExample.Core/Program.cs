using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hzdtf.Consul.ConfigCenter.AspNet.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hzdtf.Consul.WebClientExample.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.AddConsulConfigCenter(); // 如果不需要配置中心动态更新配置，则去掉此句
                });
    }
}
