using System;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Nebula.CI.Services.License
{
    [DependsOn(typeof(AbpBackgroundWorkersModule))]
    public class LicenseModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<LicenseHandokWorker>();
        }
    }
}
