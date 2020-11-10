using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Nebula.CI.Services.License
{
    public class LicenseHandokWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public LicenseHandokWorker(
                AbpTimer timer,
                IServiceScopeFactory serviceScopeFactory
                ) : base(
                timer,
                serviceScopeFactory)
        {
            Timer.Period = 30000; //30 seconds
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var hostRunningToken = workerContext.ServiceProvider
                .GetRequiredService<HostRunningToken>();
            var licenseClient = workerContext.ServiceProvider
                .GetRequiredService<LicenseClient>();
            var isOnline = await licenseClient.IsOnline();
            if (!isOnline) hostRunningToken.Cancel();
        }
    }
}
