using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Nebula.CI.Services.License
{
    public class HostRunningToken : ISingletonDependency
    {
        private readonly LicenseOptions _licenseOptions;

        public HostRunningToken(IOptions<LicenseOptions> licenseOptions)
        {
            _licenseOptions = licenseOptions.Value;
        }

        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        public CancellationToken Get()
        {
            return TokenSource.Token;
        }

        public void Cancel()
        {
            if (!_licenseOptions.IsUseLicense) return;
            Task.Run(()=>{TokenSource.Cancel();});
        }
    }
}
