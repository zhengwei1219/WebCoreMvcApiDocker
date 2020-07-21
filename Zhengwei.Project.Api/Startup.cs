using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Zhengwei.Project.Infrastructure;
using Zhengwei.Project.Api.Applications.Queries;
using Zhengwei.Project.Api.Applications.Service;
using Zhengwei.Project.Api.Dtos;
using Consul;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Zhengwei.Project.Domain.AggregatesModel;
using Zhengwei.Project.Infrastructure.Repositories;

namespace Zhengwei.Project.Api
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
            services.AddDbContext<ProjectContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlProject"));

            });
            services.Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var s = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;
                if (!string.IsNullOrEmpty(s.Consul.HttpEndpoint))
                {
                    cfg.Address = new Uri(s.Consul.HttpEndpoint);
                }
            }));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(p => {
                        p.RequireHttpsMetadata = false;
                        p.Audience = "cantact_api";
                        p.Authority = "http://localhost";
                    });

            services.AddScoped<IRecommendService, RecommendService>()
                .AddScoped<IProjectQueries, ProjectQueries>()
                .AddScoped<IProjectRepository, ProjectRepository>(sp=>{
                    var projectContext = sp.GetRequiredService<ProjectContext>();
                    return new ProjectRepository(projectContext);
            });
            services.AddMvc();
            services.AddMediatR();


            services.AddCap(p => {
                p.UseEntityFramework<ProjectContext>()
                .UseRabbitMQ("localhost")
                .UseDashboard();
                p.UseDiscovery(d => {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 14288;
                    d.NodeId = 1;
                    d.NodeName = "CAP No.3 Node";
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime,
            IOptions<ServiceDisvoveryOptions> serviceOptions,
            IConsulClient consul)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启动时注册服务   安装consul后，通过localhost:8500可以查看服务
            lifetime.ApplicationStarted.Register(() => {
                RegisterService(app, serviceOptions, consul, lifetime);
            });
            //停止时注销服务
            lifetime.ApplicationStopped.Register(() => {
                DeRegisterService(app, serviceOptions, consul, lifetime);
            });
            app.UseCap();
            app.UseAuthentication();
            app.UseMvc();
        }

        private void DeRegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul, IApplicationLifetime lifetime)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                                    .Addresses
                                    .Select(p => new Uri(p));
            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();

            }


        }

        private void RegisterService(IApplicationBuilder app, IOptions<ServiceDisvoveryOptions> serviceOptions, IConsulClient consul, IApplicationLifetime lifetime)
        {
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = $"http://localhost:14288/HealthCheck"
            };

            var anentReg = new AgentServiceRegistration()
            {
                ID = "projectapi:14288",
                Check = httpCheck,
                Address = "localhost",
                Name = "projectapi",
                Port = 14288
            };
            var serviceId = "projectapi:localhost:14288";
            consul.Agent.ServiceRegister(anentReg).GetAwaiter().GetResult();
            lifetime.ApplicationStopping.Register(() => {
                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            });

        }
    }
}
