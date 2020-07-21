using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zhengwei.Use.Api.Dtos;
using Zhengwei.Use.Api.Filters;
using Zhengwei.UserApi.Data;

namespace Zhengwei.UserApi
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

            services.AddDbContext<UserContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlUser"));

            });

            services.Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var s = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;
                if(!string.IsNullOrEmpty(s.Consul.HttpEndpoint))
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

            services.AddMvc(p=>p.Filters.Add(typeof(GlobalExceptionFilter)));


            services.AddCap(p => {
                p.UseEntityFramework<UserContext>()
                .UseRabbitMQ("localhost")
                .UseDashboard();
                p.UseDiscovery(d=> {
                    d.DiscoveryServerHostName = "localhost";
                    d.DiscoveryServerPort = 8500;
                    d.CurrentNodeHostName = "localhost";
                    d.CurrentNodePort = 33545;
                    d.NodeId = 1;
                    d.NodeName = "CAP No.1 Node";
                });
            });

              //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime lifetime,
            IOptions<ServiceDisvoveryOptions> serviceOptions,
            IConsulClient consul)

        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               // app.UseHsts();
            }

            //启动时注册服务   安装consul后，通过localhost:8500可以查看服务
            lifetime.ApplicationStarted.Register(()=> {
                RegisterService(app, serviceOptions,consul,lifetime);
            });
            //停止时注销服务
            lifetime.ApplicationStopped.Register(() => {
                DeRegisterService(app, serviceOptions, consul, lifetime);
            });
            app.UseCap();
            app.UseAuthentication();
            app.UseMvc();
            //UserContextSeed.SeedAsync(app, loggerFactory).Wait();

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
            //var features = app.Properties["server.Features"] as FeatureCollection;
            //var addresses = features.Get<IServerAddressesFeature>()
            //                        .Addresses
            //                        .Select(p => new Uri(p));
            //foreach (var address in addresses)
            //{
            //    var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";
            //    var httpCheck = new AgentServiceCheck()
            //    {
            //        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
            //        Interval = TimeSpan.FromSeconds(30),
            //        HTTP = new Uri(address,"HealthCheck").OriginalString
            //    };

            //    var registration = new AgentServiceRegistration()
            //    {
            //        Checks = new[] { httpCheck },
            //        Address = address.Host,
            //        ID = serviceId,
            //        Name = serviceOptions.Value.ServiceName,
            //        Port = address.Port
            //    };

            //    consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();
            //    lifetime.ApplicationStopping.Register(()=> {
            //        consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            //    });
            //}
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = $"http://localhost:33545/HealthCheck"
                };

            var anentReg = new AgentServiceRegistration()
            {
                ID = "userapi:33545",
                Check = httpCheck,
                Address = "localhost",
                Name = "userapi",
                Port = 33545
            };
            var serviceId = "userapi:localhost:33545";
            consul.Agent.ServiceRegister(anentReg).GetAwaiter().GetResult();
                lifetime.ApplicationStopping.Register(()=> {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });

        }
    }
}
