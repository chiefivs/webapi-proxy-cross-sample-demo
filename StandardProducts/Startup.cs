using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using StandardProducts.Controllers;
using System;
using System.IO;
using WebApiProxy;

namespace StandardProducts {

    /// <summary>
    /// Startup
    /// </summary>
    public class Startup {
        public Startup(IWebHostEnvironment env) {            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true);
            builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IProductCatalogApi, ProductCatalogControllerImpl>();

            var windsorContainer = new WindsorContainer();
            var container = WindsorRegistrationHelper.CreateServiceProvider(windsorContainer, services);

            return container;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="serviceProvider"></param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider) {
            app.UseWebApiEndpoint<IProductCatalogApi>();
        }
    }

}
