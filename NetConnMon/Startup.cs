using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetConnMon.Areas.Identity;
using MudBlazor.Services;
using Microsoft.AspNetCore.Identity;
//using NetConnMon.Api; // coming soon
using NetConnMon.Server; // going soon (from here anyway)
using MediatR;
// TODO: move identity-related features to mudblazor UI
// TODO: Add authentication by putthing it in the root layout
// TODO:? WASM, with API by swaping use of server API on this side (_Imports.razor) with our own here,
//    with the requests/commands moved to domain project (for reuse by both apps)
//    and the client-side handlers implemented via mediator with [refit](https://jonhilton.net/blazor-refit/)

namespace NetConnMon
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration  Configuration   { get; private set; }
        public IdentityBuilder IdentityBuilder { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityBuilder = services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true);
         
            services.ConfigureNetConnMonServer(Configuration, IdentityBuilder);


            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices();

            // may need to move this....
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();

            var assembly = AppDomain.CurrentDomain.Load("NetConnMon.Server");
            var efAssembly = AppDomain.CurrentDomain.Load("NetConnMon.Persistence");
            services.AddMediatR(assembly, efAssembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
