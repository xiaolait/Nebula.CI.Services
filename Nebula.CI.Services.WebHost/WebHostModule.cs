using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nebula.CI.Services.Pipeline;
using Nebula.CI.Services.PipelineHistory;
using Nebula.CI.Services.Proxy;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Nebula.CI.Services.WebHost
{
    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    [DependsOn(typeof(AbpAutofacModule))]

    [DependsOn(typeof(PipelineApplicationModule))]
    [DependsOn(typeof(PipelineEFCoreModule))]
    [DependsOn(typeof(PipelineEFCoreDbMigrationsModule))]

    [DependsOn(typeof(PipelineHistoryApplicationModule))]
    [DependsOn(typeof(PipelineHistoryEFCoreModule))]
    [DependsOn(typeof(PipelineHistoryEFCoreDbMigrationsModule))]

    [DependsOn(typeof(ServicesProxyModule))]
    public class WebHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(PipelineApplicationModule).Assembly);
                options.ConventionalControllers.Create(typeof(PipelineHistoryApplicationModule).Assembly);
            });

            ConfigureAuthentication(context);
            configureSwaggerService(context);
        }


        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var pipelineMigrator = app.ApplicationServices.GetRequiredService<EFCorePipelineDbSchemaMigrator>();
            pipelineMigrator.MigrateAsync().Wait();

            var pipelineHistoryMigrator = app.ApplicationServices.GetRequiredService<EFCorePipelineHistoryDbSchemaMigrator>();
            pipelineHistoryMigrator.MigrateAsync().Wait();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(option =>
            {
                option.AllowAnyHeader();
                option.AllowAnyMethod();
                option.AllowAnyOrigin();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseConfiguredEndpoints();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nebula.CI Service API");
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["AuthServer"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api";
                });
        }

        private void configureSwaggerService(ServiceConfigurationContext context)
        {
            context.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Bugdigger Service API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
        }
    }
}
