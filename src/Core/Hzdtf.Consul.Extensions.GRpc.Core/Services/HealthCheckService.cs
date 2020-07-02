using Grpc.Core;
using Grpc.Health.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hzdtf.Consul.Extensions.GRpc.Core.Services
{
    /// <summary>
    /// 健康检查服务
    /// @ 黄振东
    /// </summary>
    public class HealthCheckService : Health.HealthBase
    {
        /// <summary>
        /// 检测
        /// </summary>
        /// <param name="request">健康检测请求</param>
        /// <param name="context">服务调用上下文</param>
        /// <returns>健康检测响应任务</returns>
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            //TODO:检查逻辑
            return Task.FromResult(new HealthCheckResponse() { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }

        /// <summary>
        /// 监控
        /// </summary>
        /// <param name="request">健康检测请求</param>
        /// <param name="responseStream">响应流</param>
        /// <param name="context">服务调用上下文</param>
        /// <returns>任务</returns>
        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            //TODO:检查逻辑
            await responseStream.WriteAsync(new HealthCheckResponse()
            { Status = HealthCheckResponse.Types.ServingStatus.Serving });
        }
    }
}
