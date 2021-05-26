using System;
using Auth.Common.Configurations;
using Auth.Common.Extensions;
using Auth.Common.Handlers;
using GrpcText;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FindService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddAuthorization();
            services.AddJwtAuth(Configuration);
            services.Configure<TokenConfiguration>(Configuration.GetSection(nameof(TokenConfiguration)));
            services.TryAddTransient<AuthHttpClientHandler>();
            services.AddGrpcReflection();
            services.AddGrpcClient<Text.TextClient>(opt => opt.Address = new Uri("https://localhost:5001")).ConfigurePrimaryHttpMessageHandler<AuthHttpClientHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Find service started!");
                });
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
                endpoints.MapGrpcService<Services.FindService>();
            });
        }
    }
}
