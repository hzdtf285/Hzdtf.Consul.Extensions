# Hzdtf.Consul.Extensions
Consul扩展库，支持.NET和.NET Core平台，语言：C#。适用于微服务。主要针对Consul进行功能上的封装，让使用更方便。通过集成配置文件，封装功能有：
1、服务注册与发现，支持与配置中心绑定，动态更新配置信息
2、对服务注册与发现，可选择webapi与grpc通讯方式，支持与配置中心绑定，动态更新配置信息

本框架必须运行在.NET Standard 2.0、.NET Framework 4.6.1和.NET Core 3.1 以上。下载源码用Visual Studio 2019打开。
工程以Standard或Std结尾是标准库，以Framework或Frm结尾为Framework库，以Core结尾为Core库。
初始编译时会耗些时间，因为要从nuget下载包。
本库依赖类库是：
1、Hzdrtf.Utility
2、Hzdrtf.Utility.AspNet.Core
3、Hzdtf.Logger
4、Hzdtf.Platform
类库统一放在src/Library里。（依赖库源码都可在Hzdtf.Foundation.Framework里找到）

一、Consul.Extensions.Comnon包
Consul基本功能的封装，如基本注册；服务发现获取的

二、Consul.Extensions.AspNet包 
为AspNet.Core框架提供功能的封装

三、Consul.ConfigCenter.AspNet.Core包
为AspNet.Core框架提供配置中心的封装

五、样例
默认的配置都会在Config文件夹下，与Consul通讯的配置文件consulConfig.json；服务发现配置的文件：serviceBuilderConfig.json

1、在ASP.NET Core框架里进行服务注册
（1）、在StartUp类里的ConfigureServices方法里执行下面代码：
services.AddRegisterConsul(); // 如果要执行配置中心，则使用services.AddDiscoveryConsulConfigCenter();  如果不需要动态更新配置，则推荐使用普通注册，性能较好
（2）、然后再在Configure方法里执行下面代码：
 app.UseRegisterConsul(); // 如果使用配置中心注册，则不需要执行此句
 
 (3)、如果需要配置中心，则还需要在Program类的CreateHostBuilder方法里执行如下样例代码：
 public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.AddConsulConfigCenter(); // 如果不需要配置中心动态更新配置，则去掉此句。在这里执行，默认会把appsetting.json、appsetting.{当前环境}.json作为key进入到配置中心，key规则：默认是"服务名/appsetting.json"，如服务名为空，则使用当前应用名称
                });
				

webapi注册样例项目在：Hzdtf.Consul.ServiceExample.Core
grpc注册样例项目在：Hzdtf.Consul.GRpc.ServiceExample.Core

2、在普通控制台里进行服务发现：

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

样例项目在：Hzdtf.Consul.ClientExample.Core

3、在ASP.NET Core项目里进行服务发现：
（1）、在StartUp类里ConfigureServices方法里执行下面代码：
services.AddDiscoveryConsul(); // 如果需要将配置进行动态更新，则执行services.AddDiscoveryConsulConfigCenter(); 默认把serviceBuilderConfig.json作为动态配置文件。配置中心的key为：服务名/Config/serviceBuilderConfig.json
（2）、在Config方法里执行下面代码：
app.UseRegisterConsul(); // 如果是执行了配置中心，则不需要执行此句
（3）、由于使用的是依赖注入，则直接在使用方构造器里注入IUnityServicesOptions接口即可，然后代码参考：
// serviceBuilder就是传过来的IUnityServicesOptions类型

using (var httpClient = new HttpClient())
{
	for (var i = 0; i < 100; i++)
	{
		var url = serviceBuilder.BuilderAsync("ServiceExampleA", "/Health").Result; // 如果使用grpc，也是此用法，只不过要把后面的参数["/Health"]去掉
		var content = httpClient.GetStringAsync(url).Result;

		var s = $"第{i + 1}次请求[{url}]:{content} \r\n";

		str.Append(s);
	}
}


consulConfig.json详解：
{
  "ServiceId": "", // 服务ID，本次启动服务的实例ID，不配置默认以GUID
  "ConsulAddress": "http://127.0.0.1:8500", // consul地址
  "CacheExpire": 5, // 获取consul服务地址列表存储到的缓存过期时间，单位秒，过期后再从consul获取。不配置默认为5秒
  "ServiceName": "service1", // 本应用的服务名
  "Datacenter" "dc1", // consul数据中心，不配置默认为dc1
  "ServiceAddress": "http://localhost:5000", // 本应用的服务地址，不配置默认取服务器的IP+监听端口
  "Tags": [],  // 服务的标签
  "ServiceCheck": {  // 服务健康检测对象
	"HealthCheck": "/Health", // 健康检测路径，不配置默认为/Health
	"DeregisterCriticalServiceAfter": 5, // 服务停止多久后注销服务（单位：秒）,不配置默认为5秒
	"Timeout": 15, // 连接consul超时，单位秒，不配置默认为15秒
	"Interval": 10, // consul向服务发起健康检测的间隔时间，单位秒，不配置默认为10秒
  },
  "Keys":[] // 键数组，配置中心需要加入的键，只对需要配置中心才有效
  "AutoLoadCommonKey": true, // 自动加载公共键，如果为是，则会自动加载common.json作为公共key，默认为是。common.json键要放在consul最外层
}


serviceBuilderConfig.json详解：
{
  "Services": [ // 服务数组
    {
      "ServiceName": "ServiceExampleA", // 服务名，必须保证唯一，代码里就是通过此服务名找到对应的配置
	  "LoadBalanceMode": "RANDOM", // 负载均衡模式，不配置默认为随机
	  "Sheme": "http" // 方案，不配置默认为http，有http,https两种
    }
  ],
  "GlobalConfiguration": { // 全局配置
    "LoadBalanceMode": "ROUND_ROBIN", // 负载均衡模式，不配置默认为随机
	"Sheme": "http" // 方案，不配置默认为http，有http,https两种
  }
}

关键属性说明：
GlobalConfiguration：全局配置，如果在Services里没有配置，则全局配置会覆盖Services里的配置，属于公共配置
LoadBalanceMode: 负载均衡模式，目前支持3种：RANDOM（随机），ROUND_ROBIN（轮询），HASH_IP_PORT（哈希IP+端口）


注：如grpc客户端要使用http，默认未开启，需要执行AppContextUtil.SetHttp2UnencryptedSupport();  在Hzdtf.Consul.GRpc.ClientExample.Core命名空间里
