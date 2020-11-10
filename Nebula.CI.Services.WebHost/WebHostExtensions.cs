using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nebula.CI.Services.License;

namespace Nebula.CI.Services.WebHost
{
    public static class WebHostExtensions
    {
        public static async Task RunWithTokenAsync(this IHost host)
        {
            HostRunningToken hostRunningToken = null;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                hostRunningToken = services.GetRequiredService<HostRunningToken>();
            }
            await host.RunAsync(hostRunningToken.Get());
        }
    }
}
