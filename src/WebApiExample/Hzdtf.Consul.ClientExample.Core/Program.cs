using Hzdtf.Consul.Extensions.Common.Standard;
using System;
using System.Net.Http;
using System.Threading;
using Hzdtf.Utility.Standard.RemoteService.Builder;
using Hzdtf.Utility.Standard.RemoteService.Options;

namespace Hzdtf.Consul.ClientExample.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start test...");

            var serviceProvider = new ConsulServicesProviderMemory();
            var serviceOptions = new UnityServicesOptionsCache();
            var unityServicesBuilder = new UnityServicesBuilder(serviceProvider, serviceOptions);

            using (var httpClient = new HttpClient())
            {
                for (var i = 0; i < 20; i++)
                {
                    var url = unityServicesBuilder.BuilderAsync("ServiceExampleA", "/Health").Result;
                    var content = httpClient.GetStringAsync(url).Result;

                    Console.WriteLine($"第{i}次请求[{url}]:{content}");

                    Thread.Sleep(1000);
                }
            }

            Console.ReadLine();
        }
    }
}
