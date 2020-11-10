using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Nebula.CI.Services.License
{
    public class HostRunningToken : ISingletonDependency
    {
        private CancellationTokenSource TokenSource = new CancellationTokenSource();

        public CancellationToken Get()
        {
            return TokenSource.Token;
        }

        public void Cancel()
        {
            Task.Run(()=>{TokenSource.Cancel();});
        }
    }
}
