using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resilience;
using Zhengwei.Contact.Api;
using Zhengwei.Contact.Api.Code;
using Zhengwei.Contact.Api.Data;
using Zhengwei.Contact.Api.Dtos;
using Zhengwei.Contact.Api.Service;

namespace Contact
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
            services.AddScoped<IContactApplyRequestRepository, MogoContactApplyRequestRepository>();
            services.AddScoped<IContactRepository, MogoContactRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ContactContext>();
            services.Configure<AppSettings>(Configuration);

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
                return new ResilienceClientFactory(exceptionCountAlloweBeforeBreaking, retryCount, logger, httpcontextAccesser);
            });
            //services.AddSingleton(new HttpClient());
            //注册全局的IHttpClient
            services.AddSingleton<IHttpClient>(p =>
            {
                return p.GetRequiredService<ResilienceClientFactory>().GetResilienceHttpClient();
            }
                );

            services.AddScoped<IUserService, UserService>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(p => {
                        p.RequireHttpsMetadata = false;
                        p.Audience = "cantact_api";
                        p.Authority = "http://localhost";
                        p.SaveToken = true;
                    });


            services.AddMvc();
            services.AddCap(x=> {
                x.UseMySql("server=localhost;port=3306;database=beta_user;userid=zhengwei;password=1230");
                x.UseRabbitMQ("localhost");
                x.UseDashboard();
                x.UseDiscovery(d=> {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 5801;
                    d.NodeId = 2;
                    d.NodeName = "CAP No.2 Node";
                });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
