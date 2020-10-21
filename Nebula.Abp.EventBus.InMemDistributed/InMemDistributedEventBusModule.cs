using System;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Nebula.Abp.EventBus.InMemDistributed
{
    [DependsOn(typeof(AbpEventBusModule))]
    public class InMemDistributedEventBusModule : AbpModule
    {
    }
}