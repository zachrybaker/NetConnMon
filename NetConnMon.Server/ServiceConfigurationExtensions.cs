using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetConnMon.Domain.Entities;
using NetConnMon.Persistence;
using NetConnMon.Server.API._Internal.RequestHandlers;
using NetConnMon.Server.API.Requests;
using NetConnMon.Server.Services;
using Hangfire;
using Hangfire.MemoryStorage;

namespace NetConnMon.Server
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection ConfigureNetConnMonServer(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityBuilder identityBuilder)
        {
            services.AddPersistence(configuration, identityBuilder);

            // server-facing tasks
            services.AddScoped<IEmailer, Emailer>();
            services.AddHostedService<TestService>();
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GenericPipelineBehavior<,>));
            //services.AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));
            //services.AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>));

            // retry for the emails, since the connection is possibly down when the email is attempted
            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            return services;
        }
    }
}
