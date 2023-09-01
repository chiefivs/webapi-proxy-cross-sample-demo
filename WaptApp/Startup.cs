using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using StandardProducts;
using WebApiProxy;

namespace WaptApp
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
            services.AddControllers();
            services.AddTransient<HttpClient>(provider =>
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Context", "{\"System\":\"MA1B\"}");
                client.DefaultRequestHeaders.Add("Accept-Language", "de");//Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2));
                return client;
            });

            var uri = new Uri("http://localhost:8000");
            var options = new WebApiEndpointOptions
            {
                TransferHeaders = new []{ "Context", "Accept-Language" }
            };
            services.AddWebApiEndpoints(
                new WebApiEndpoint<IProductCatalogApi>(uri, options)
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
