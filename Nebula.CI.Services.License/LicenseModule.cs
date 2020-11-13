using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace Nebula.CI.Services.License
{
    [DependsOn(typeof(AbpBackgroundWorkersModule))]
    public class LicenseModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var isUseLicenseConf = configuration["IsUseLicense"];
            var isUseLicense = true;
            if(!string.IsNullOrEmpty(isUseLicenseConf) && isUseLicenseConf == "false") isUseLicense = false;
            PreConfigure<LicenseOptions>(options =>
            {
                options.IsUseLicense = isUseLicense;
            });

        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<LicenseHandokWorker>();
        }
    }
}
