using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nebula.CI.Services.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://+:5000");
                }).UseAutofac();
                /*.ConfigureServices(services => {
                    var containerBuilder = services.GetContainerBuilder();
                    containerBuilder.RegisterType<PipelineProxy>().As<IPipelineProxy>()
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

                    containerBuilder.RegisterType<PipelineHistoryProxy>().As<IPipelineHistoryProxy>()
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
                });*/
    }
}
