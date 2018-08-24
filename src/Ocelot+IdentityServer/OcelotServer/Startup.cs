﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;

namespace OcelotServer
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
            Action<IdentityServerAuthenticationOptions> isaOptOrder = option =>
            {
                option.Authority = "http://localhost:5000";
                option.ApiName = "OrderService";
                option.RequireHttpsMetadata = false;
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = "ordersecret";
            };

            //定义要保护的Api资源（对应IdentityServer 的ApiResources）
            //具体的能保护的Api资源范围由配置中的"AllowedScopes"决定
            Action<IdentityServerAuthenticationOptions> isaOptProduct = option =>
            {
                option.Authority = "http://localhost:5000";
                option.ApiName = "ProductService";
                option.RequireHttpsMetadata = false;
                option.SupportedTokens = SupportedTokens.Both;
                option.ApiSecret = "productsecret";
            };

            services.AddAuthentication()
                .AddIdentityServerAuthentication("OrderServiceKey", isaOptOrder)
                .AddIdentityServerAuthentication("ProductServiceKey", isaOptProduct);

            // Ocelot
            services.AddOcelot(Configuration);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseAuthentication();
            app.UseOcelot().Wait();
            //app.UseMvc();
        }
    }
}
