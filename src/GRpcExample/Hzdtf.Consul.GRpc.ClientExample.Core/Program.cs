using Grpc.Net.Client;
using Hzdtf.Consul.Extensions.Common.Standard;
using Hzdtf.Consul.GRpc.ServiceExample.Core;
using Hzdtf.Utility.Standard.RemoteService;
using Hzdtf.Utility.Standard.RemoteService.Builder;
using Hzdtf.Utility.Standard.RemoteService.Options;
using Hzdtf.Utility.Standard.Utils;
using System;
using System.Net.Http;
using System.Threading;

namespace Hzdtf.Consul.GRpc.ClientExample.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContextUtil.SetHttp2UnencryptedSupport();

            Console.WriteLine("start grpc test...");

            var serviceProvider = new ConsulServicesProviderMemory();
            var serviceOptions = new UnityServicesOptionsCache();
            var unityServicesBuilder = new UnityServicesBuilder(serviceProvider, serviceOptions);

            for (var i = 0; i < 20; i++)
            {
                var url = unityServicesBuilder.BuilderAsync("GRpcServiceExampleA").Result;
                using (var channel = GrpcChannel.ForAddress(url))
                {
                    var client = new Greeter.GreeterClient(channel);
                    var res = client.SayHello(new HelloRequest()
                    {
                        Name = "张三" + i
                    });
                    Console.WriteLine($"第{i}次请求[{url}]:{JsonUtil.SerializeIgnoreNull(res)}");

                    Thread.Sleep(1000);
                }
            }

            Console.ReadLine();
        }
    }
}
