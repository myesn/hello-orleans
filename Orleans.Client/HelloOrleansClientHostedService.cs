using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Grains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Client
{
    public class HelloOrleansClientHostedService : IHostedService
    {
        private readonly IClusterClient _client;
        private readonly ILogger<HelloOrleansClientHostedService> _logger;

        public HelloOrleansClientHostedService(IClusterClient client, ILogger<HelloOrleansClientHostedService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await MockLoginAsync("Hello.Orleans.Console");
            await MockLoginAsync("Hello.Orleans.Web");
        }

        // 模拟指定应用的登录
        private async Task MockLoginAsync(string appName)
        {
            var sessionControl = _client.GetGrain<ISessionControlGrain>(appName);
            ParallelLoopResult result = Parallel.For(0, 10000, (index) =>
            {
                var userId = $"User-{index}";
                sessionControl.Login(userId);
            });

            if (result.IsCompleted)
            {
                // ParallelLoopResult.IsCompleted 只是返回所有循环创建完毕，并不保证循环的内部任务创建并执行完毕
                //所以，此处手动延迟5秒后再去读取活动用户数。
                await Task.Delay(TimeSpan.FromSeconds(5));
                var activeUserCount = await sessionControl.GetActiveUserCount();

                _logger.LogInformation($"The Active Users Count of {appName} is {activeUserCount}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Closed!");

            return Task.CompletedTask; ;
        }

    }
}
