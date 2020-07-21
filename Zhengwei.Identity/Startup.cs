using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;
using Zhengwei.Identity.Authentication;
using Zhengwei.Identity.Code;
using Zhengwei.Identity.Dtos;
using Zhengwei.Identity.Services;

namespace Zhengwei.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddExtensionGrantValidator<SmsAuthCodeValidator>()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources());

            services.AddTransient<IProfileService, ProfileService>();

            services.Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(p =>
            {
                var s = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;
                return new LookupClient(s.Consul.DnsEndpoint.ToIpEndPoint());
            });
            //注册全局单例ResilienceClientFactory
            services.AddSingleton(typeof(ResilienceClientFactory), p =>
            {
                var logger = p.GetRequiredService<ILogger<ResilienceHttpClient>>();
                var httpcontextAccesser = p.GetRequiredService<IHttpContextAccessor>();
                var retryCount = 5;
                var exceptionCountAlloweBeforeBreaking = 5;
                return new ResilienceClientFactory(exceptionCountAlloweBeforeBreaking,retryCount,logger, httpcontextAccesser);
            });
            //services.AddSingleton(new HttpClient());
            //注册全局的IHttpClient
            services.AddSingleton<IHttpClient>(p=>
            {
                return p.GetRequiredService<ResilienceClientFactory>().GetResilienceHttpClient();
            }
                );

            services.AddScoped<IAuthCodeService, AuthCodeService>()
                .AddScoped<IUserService, UserService>();


            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
