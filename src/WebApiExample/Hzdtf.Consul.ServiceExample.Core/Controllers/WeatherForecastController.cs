using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Standard.RemoteService.Builder;
using Hzdtf.Utility.Standard.RemoteService.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hzdtf.Consul.ServiceExample.Core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IConfiguration config;
        private readonly IUnityServicesBuilder serviceBuilder;

        private readonly IUnityServicesOptions servicesOptions;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, IUnityServicesBuilder serviceBuilder, IUnityServicesOptions servicesOptions)
        {
            _logger = logger;
            this.config = config;
            this.serviceBuilder = serviceBuilder;
            this.servicesOptions = servicesOptions;
        }

        [HttpGet]
        public string Get()
        {
            var str = new StringBuilder();
            using (var httpClient = new HttpClient())
            {
                for (var i = 0; i < 100; i++)
                {
                    var url = serviceBuilder.BuilderAsync("ServiceExampleA", "/Health").Result;
                    var content = httpClient.GetStringAsync(url).Result;

                    var s = $"第{i + 1}次请求[{url}]:{content} \r\n";

                    str.Append(s);
                }
            }
            return str.ToString();
        }

        [HttpGet("GetConfigByKey/{key}")]
        public string GetConfigByKey(string key)
        {
            return config[key];
        }
    }
}
