using System;
using GrpcText;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FindService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddGrpcReflection();
            services.AddGrpcClient<Text.TextClient>(opt => opt.Address = new Uri("https://localhost:5001"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
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
