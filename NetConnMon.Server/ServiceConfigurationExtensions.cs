using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetConnMon.Domain.Configuration;
using NetConnMon.Domain.Entities;
using NetConnMon.Domain.Logic;
using NetConnMon.Persistence;
using NetConnMon.Server.API._Internal.RequestHandlers;
using NetConnMon.Server.API.Requests;
using NetConnMon.Server.Services;

namespace NetConnMon.Server
{
    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection ConfigureNetConnMonServer(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityBuilder identityBuilder)
        {
            services
                .Configure<EncryptionSettings>(configuration.GetSection("EncryptionSettings"))
                .Configure<OverridableSettings>(configuration.GetSection("OverridableSettings"));
                
            services.AddPersistence(configuration, identityBuilder);

            // server-facing tasks
            services.AddSingleton<IEncryptor, Encryptor>();
            services.AddScoped<IEmailer, Emailer>();
            services.AddHostedService<TestService>();
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(GenericPipelineBehavior<,>));
            //services.AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));
            //services.AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>));

            return services;
        }
    }
}
